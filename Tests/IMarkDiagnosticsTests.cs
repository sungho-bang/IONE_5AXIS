using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using FAFramework.Manager;
using FAFramework.VT3500;
using FAFramework.VT3500.Modules;
using FAFramework.VT3500.ExtendedParts;
using FALibrary.Alarm;
using FALibrary.Part.MemoryBasePart;
using FALibrary.Utility;
using FALibrary.Sequence;

internal static class IMarkDiagnosticsTests
{
    private static int _passed;

    private static void Check(bool result, string description)
    {
        if (!result) throw new Exception("FAIL: " + description);
        Console.WriteLine("PASS: " + description);
        _passed++;
    }

    private static void Pump(FASequence sequence, int count = 12)
    {
        for (int i = 0; i < count; i++) sequence.Execute();
    }

    private static void CheckDiagnostics()
    {
        var manager = new FASequenceManager();
        var sequence = new FASequence(manager) { Name = "TestIMark" };
        var rows = new List<string>();
        string input = "Off";
        string motion = "Run:True,Done:False";
        bool finish = false;
        int length = sequence.Length;
        var diagnostic = new IMarkSequenceDiagnostics(sequence, () => "Sensor=" + input + ";ActualPos=12.5",
            () => "AutoTimeoutMs=1000;RetryLimit=3", () => input, () => motion, rows.Add);
        Check(sequence.Length == length, "Attaching diagnostics adds no control steps");
        diagnostic.TrackStep(sequence.AddItem((actor, time) => { if (finish) actor.NextStep(); }), "WAIT_IMARK");
        sequence.Start();
        Pump(sequence);
        Check(rows.Any(r => r.Contains("Event=START;") && r.Contains("AutoTimeoutMs=1000")), "Start records runtime configuration");
        Check(rows.Any(r => r.Contains("Event=INPUT_INITIAL;") && r.Contains("Observed=Off")), "Initial OFF input recorded");
        input = "On";
        motion = "Run:False,Done:True";
        Pump(sequence);
        Check(rows.Any(r => r.Contains("Event=INPUT_CHANGED;") && r.Contains("Previous=Off;Observed=On")), "OFF to ON input transition recorded");
        Check(rows.Any(r => r.Contains("Event=MOTION_CHANGED;")), "Motor feedback change recorded");
        input = "NULL";
        Pump(sequence);
        Check(rows.Any(r => r.Contains("Observed=NULL")), "Missing sensor remains distinguishable from OFF");
        for (int i = 0; i < 10000; i++) diagnostic.Repeat("SEARCH_REPEAT", "SearchOnly");
        Check(rows.Count(r => r.Contains("Event=SEARCH_REPEAT;")) == 1, "10000 tight retries produce one immediate record");
        Thread.Sleep(5050);
        Pump(sequence);
        diagnostic.Repeat("SEARCH_REPEAT", "SearchOnly");
        Check(rows.Any(r => r.Contains("Event=SEARCH_REPEAT;") && r.Contains("Count=10001")), "Periodic retry summary retains exact cumulative count");
        Check(rows.Any(r => r.Contains("Event=HEARTBEAT;")), "Long waiting step produces heartbeat");
        sequence.PreSuspend();
        sequence.Suspend();
        Pump(sequence);
        sequence.Resume();
        Pump(sequence);
        Check(rows.Any(r => r.Contains("Event=SUSPENDED;")) && rows.Any(r => r.Contains("Event=RESUMED;")), "Suspend and resume recorded");
        string firstRunId = rows.First(r => r.Contains("Event=START;")).Split(';').First(r => r.StartsWith("RunId="));
        finish = true;
        Pump(sequence);
        Check(rows.Any(r => r.Contains("Event=TERMINATED;")) && rows.Any(r => r.Contains("Event=REPEAT_TOTAL;") && r.Contains("Count=10001")), "Termination flushes repetition totals");
        int endedRows = rows.Count;
        Pump(sequence);
        Check(rows.Count == endedRows, "Idle sequence emits no diagnostic records");
        finish = false;
        sequence.Start();
        Pump(sequence);
        string secondRunId = rows.Last(r => r.Contains("Event=START;")).Split(';').First(r => r.StartsWith("RunId="));
        Check(firstRunId != secondRunId, "Every run has a new correlation ID");
        sequence.Stop();
        Check(rows.Any(r => r.Contains("Event=ABORTED;")), "External stop distinguished from normal termination");
        sequence.Start();
        Pump(sequence);
        sequence.ClearState();
        Pump(sequence);
        Check(rows.Any(r => r.Contains("Event=STATE_RESET_OR_ENDED;")), "ClearState does not leave a phantom active run");
        File.WriteAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "diagnostic-examples.log"), rows);

        var failingSequence = new FASequence(manager) { Name = "FailingLogSink" };
        var failingDiagnostic = new IMarkSequenceDiagnostics(failingSequence, () => "Sensor=Off", () => "",
            () => "Off", () => "Stopped", _ => { throw new IOException("Injected writer failure"); });
        bool controlRan = false;
        failingSequence.AddItem(_ => { failingDiagnostic.Record("TEST"); controlRan = true; });
        failingSequence.Start();
        Pump(failingSequence);
        Check(controlRan && failingSequence.State == SequenceState.Terminated, "Writer failure cannot block sequence completion");

        var snapshotSequence = new FASequence(manager) { Name = "FailingSnapshot" };
        new IMarkSequenceDiagnostics(snapshotSequence, () => { throw new InvalidOperationException("Injected snapshot failure"); },
            () => "", () => "Off", () => "Stopped", _ => { });
        snapshotSequence.AddItem(_ => { });
        snapshotSequence.Start();
        Pump(snapshotSequence);
        Check(snapshotSequence.State == SequenceState.Terminated, "Snapshot failure cannot block sequence completion");
        var retry = new FARetryInfo(3);
        Check(retry.IncreaseCount() && retry.IncreaseCount() && !retry.IncreaseCount() && retry.RetryCount == 0,
            "Existing retry limit 3 means two retries and reset on third timeout");
    }

    private static bool WaitForText(string path, string text)
    {
        for (int i = 0; i < 200; i++)
        {
            try { if (File.Exists(path) && ReadShared(path).Contains(text)) return true; }
            catch (IOException) { }
            Thread.Sleep(25);
        }
        return false;
    }

    private static string ReadShared(string path)
    {
        using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (var reader = new StreamReader(stream))
            return reader.ReadToEnd();
    }

    private sealed class TestInput : FAPartInputIOInfo
    {
        public TestInput(bool value, string name = "X1135")
            : base(null, name, Convert.ToInt32(name.Substring(1), 16), "TEST_ONLY") { Value = value; }
        public override bool Value { get; set; }
    }

    private static void CheckAutomaticSequence(SubEquipment equipment, string logPath, string name, bool useIMark, bool inputOn, bool missingSensor)
    {
        var scheduler = new FASequenceManager();
        var module = (FAFrontLoadingModule)FormatterServices.GetUninitializedObject(typeof(FAFrontLoadingModule));
        module.Name = "FrontModuleTest";
        module.Equipment = equipment;
        module.ProductInfo = null;
        module.UseFrontIMark = useIMark;
        module.TimeFrontIMarkTimeout = new FATime(FATimeType.millisecond, 1);
        module.AlarmFrontIMarkCheckTimeOut = 2005002;
        if (!missingSensor)
        {
            module.FrontIMarkCheckSensor = new FAPartOnOffSensor();
            module.FrontIMarkCheckSensor.InputIO.Add(new TestInput(false));
        }
        // All actions below are counters. The servo has no device and home marking uses simulation only.
        var servo = new FATapeLoadingServo(scheduler) { SimulationMode = true };
        servo.TapeLoadingPos = new FALibrary.Part.MMCPart.FAMMCPosition();
        servo.TapeLoadingSlowPos = new FALibrary.Part.MMCPart.FAMMCPosition();
        servo.TargetPosition = new FALibrary.Part.MMCPart.FAMMCPosition();
        int stopCalls = 0;
        int slowCalls = 0;
        servo.Stop.SetActionMethod(_ => { stopCalls++; });
        servo.MoveTapeLoadingPos.SetActionMethod(_ => { });
        servo.MoveTapeLoadingSlowPos.SetActionMethod(_ =>
        {
            slowCalls++;
            typeof(FALibrary.Part.MMCPart.FAMMCPart).GetProperty("MotionDone").SetValue(servo, true, null);
            if (inputOn && !missingSensor) ((TestInput)module.FrontIMarkCheckSensor.InputIO[0]).Value = true;
        });
        servo.MoveTapeLoadingPos.Sequence.AddItem(servo.MoveTapeLoadingPos.ExecuteForSequence);
        servo.MoveTapeLoadingSlowPos.Sequence.AddItem(servo.MoveTapeLoadingSlowPos.ExecuteForSequence);
        module.TapeLoadingServo = servo;
        module.TapeLoadGrip = new FAPartGripRelease(scheduler);
        module.TapeLoadGrip.InputIO.Add(new TestInput(true, "X0230"));
        module.TapeHoldGrip = new FAPartGripRelease(scheduler);
        module.TapeHoldGrip.InputIO.Add(new TestInput(false, "X0232"));
        module.TapeLoadingMoveWithIMark = new FASequence(scheduler) { Name = name };
        typeof(FAFrontLoadingModule).GetMethod("MakeTapeLoadingMoveWithIMark", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(module, null);
        int alarmId = 0;
        EventHandler<FAAlarmEventArgs> alarmHandler = (sender, args) => { alarmId = args.Alarm.AlarmNo; };
        FAAlarmManager.Instance.OnRaiseAlarm += alarmHandler;
        try
        {
            module.TapeLoadingMoveWithIMark.Start();
            for (int i = 0; i < 4000; i++)
            {
                scheduler.Run();
                if (module.TapeLoadingMoveWithIMark.State == SequenceState.Terminated || module.TapeLoadingMoveWithIMark.State == SequenceState.Suspended) break;
                Thread.Sleep(1);
            }
            if (useIMark && (!inputOn || missingSensor))
            {
                Check(alarmId == 2005002 && slowCalls == (missingSensor ? 0 : 1) && stopCalls > 0 && module.TapeLoadingMoveWithIMark.State != SequenceState.Terminated,
                    name + ": actual module stops and alarms without automatic retry or feed count");
                module.TapeLoadingMoveWithIMark.Stop();
            }
            else
            {
                Check(module.TapeLoadingMoveWithIMark.State == SequenceState.Terminated && (useIMark ? stopCalls > 0 : slowCalls == 0),
                    name + ": FRONT module completes without REAR counter changes");
            }
            LogManager.Instance.WriteIMarkLog(equipment, "TEST_ONLY;CompletedCase=" + name);
            Check(WaitForText(logPath, "CompletedCase=" + name), name + ": production module logs saved");
            string rows = string.Join("\n", ReadShared(logPath).Split('\n').Where(r => r.Contains("Seq=" + name + ";")));
            if (!useIMark)
                Check(rows.Contains("Event=BYPASS;") && !rows.Contains("Event=DETECTED_STOP_REQUESTED;"), name + ": bypass is not reported as detection");
            else if (inputOn && !missingSensor)
                Check(rows.Contains("Event=DETECTED_STOP_REQUESTED;") && rows.Contains("InputName=X1135") && rows.Contains("Event=TERMINATED;"), name + ": detection, IO mapping and end linked to run");
            else
                Check(rows.Contains("Event=POSITIONING_ALARM;") && !rows.Contains("Event=TIMEOUT_RETRY;") &&
                    rows.Contains(missingSensor ? "Reason=InvalidOrMissingInput" : "Reason=SearchTimeout"),
                    name + ": exact fault reason and no automatic retry recorded");
        }
        finally { FAAlarmManager.Instance.OnRaiseAlarm -= alarmHandler; }
    }

    private static void CheckFileWriter()
    {
        var root = AppDomain.CurrentDomain.BaseDirectory;
        var manager = LogManager.Instance;
        try
        {
            // No equipment constructor, device connection, UI or motion command is run.
            var equipment = (SubEquipment)FormatterServices.GetUninitializedObject(typeof(SubEquipment));
            equipment.Name = "IMarkTestEquipment";
            manager.IsEnabledTraceLog = false;
            manager.WriteIMarkLog(equipment, "TEST_ONLY;Event=FILE_WRITE_CHECK");
            string relativeFile = Path.Combine(DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"), DateTime.Now.ToString("yyyy-MM-dd") + ".log");
            string path = Path.Combine(root, "Log", equipment.Name, "IMarkLog", relativeFile);
            Check(WaitForText(path, "Event=FILE_WRITE_CHECK"), "Dedicated IMarkLog writes while TraceLog is disabled");
            Check(ReadShared(path).Contains(DateTime.Now.ToString("yyyy-MM-dd") + " "), "Daily file contains event timestamps");
            Check(LogRetentionSetting.GetRetentionDays(LogRetentionSetting.KEY_IMARK_LOG) == 7, "Isolated default IMarkLog retention is seven days");
            for (int i = 0; i < 100; i++) manager.WriteIMarkLog(equipment, "TEST_ONLY;BurstOrdinal=" + i);
            Check(WaitForText(path, "BurstOrdinal=99"), "Queued burst drains to its last entry");
            var ordinals = ReadShared(path).Split('\n').Where(r => r.Contains("BurstOrdinal=")).Select(r => int.Parse(r.Substring(r.IndexOf("BurstOrdinal=") + 13))).ToArray();
            Check(ordinals.SequenceEqual(Enumerable.Range(0, 100)), "Burst preserves all 100 entries in queue order");
            using (var lockedReader = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                manager.WriteIMarkLog(equipment, "TEST_ONLY;Event=SHORT_LOCK_RECOVERED");
                Thread.Sleep(60);
            }
            Check(WaitForText(path, "Event=SHORT_LOCK_RECOVERED"), "Short reader lock is retried without losing the event");
            CheckAutomaticSequence(equipment, path, "AutoDetection", true, true, false);
            CheckAutomaticSequence(equipment, path, "AutoBypass", false, false, false);
            CheckAutomaticSequence(equipment, path, "AutoTimeout", true, false, false);
            CheckAutomaticSequence(equipment, path, "AutoMissingSensor", true, false, true);
            IMarkPositioningTests.Run(equipment, Check);
            IMarkUsageSelectionTests.Run(Check);
            IMarkFrontIntegrationTests.Run(equipment, Check);

            equipment.Name = "BlockedEquipment";
            string blockedParent = Path.Combine(root, "Log", equipment.Name);
            Directory.CreateDirectory(blockedParent);
            File.WriteAllText(Path.Combine(blockedParent, "IMarkLog"), "TEST_ONLY: this file blocks directory creation");
            manager.WriteIMarkLog(equipment, "TEST_ONLY;Event=EXPECTED_WRITE_FAILURE");
            string systemPath = Path.Combine(root, "Log", "SystemLog", relativeFile);
            Check(WaitForText(systemPath, "Log write failed."), "IMarkLog directory failure is reported in SystemLog");
            equipment.Name = "IMarkTestEquipment";
            manager.WriteIMarkLog(equipment, "TEST_ONLY;Event=WRITER_SURVIVED");
            Check(WaitForText(path, "Event=WRITER_SURVIVED"), "Log worker survives a directory creation failure");
            var logLines = ReadShared(path).Split('\n');
            string slowWindow = string.Join("\n", logLines.Where(r => r.Contains("Seq=IgnoreFastCarryOn;")));
            Check(slowWindow.Contains("Event=FAST_INPUT_IGNORED;") &&
                slowWindow.Contains("Event=SLOW_DETECTION_ENABLED;") &&
                slowWindow.Contains("RequiresNewEdge=false") &&
                slowWindow.IndexOf("Event=SLOW_SEARCH_COMMAND;") < slowWindow.IndexOf("Event=DETECTED_STOP_REQUESTED;") &&
                slowWindow.Contains("DetectedPos=120;") && slowWindow.Contains("DetectionPolicy=SlowLevelOn;"),
                "Production log distinguishes ignored fast input from immediate slow-level detection");
            string heldOn = string.Join("\n", logLines.Where(r => r.Contains("Seq=HeldOnDuringSlow;")));
            Check(heldOn.Contains("Event=SLOW_SEARCH_COMMAND;") && heldOn.Contains("DetectionBasis=OnInSlowWindow") &&
                !heldOn.Contains("Event=SLOW_TARGET_REACHED;") &&
                heldOn.IndexOf("Event=SLOW_SEARCH_COMMAND;") < heldOn.IndexOf("Event=DETECTED_STOP_REQUESTED;") &&
                !heldOn.Contains("Event=POSITIONING_ALARM;"),
                "Held-ON log proves detection stops slow travel without waiting for its endpoint");
            string afterArrival = string.Join("\n", logLines.Where(r => r.Contains("Seq=SlowArrivalDelayedMark;")));
            Check(afterArrival.Contains("Event=SLOW_TARGET_REACHED;") &&
                afterArrival.Contains("DetectionBasis=OnAfterSlowArrival") &&
                !afterArrival.Contains("Event=SLOW_WAIT_INPUT_OFF;") &&
                !afterArrival.Contains("Event=POSITIONING_ALARM;"),
                "Arrival-wait log confirms ON is accepted without an OFF-rearming condition");
            Console.WriteLine("Test-only IMarkLog: " + path);
        }
        finally
        {
            manager.Run = false;
            var thread = (Thread)typeof(LogManager).GetField("_thread", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(manager);
            thread.Join(5000);
        }
    }

    [STAThread]
    private static int Main()
    {
        try
        {
            Console.WriteLine("Hardware-free I-Mark diagnostic verification " + DateTime.Now.ToString("O"));
            CheckDiagnostics();
            CheckFileWriter();
            Console.WriteLine("PASS TOTAL: " + _passed);
            return 0;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return 1;
        }
    }
}
