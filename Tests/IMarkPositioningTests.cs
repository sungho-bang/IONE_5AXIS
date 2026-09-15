using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using FAFramework.VT3500;
using FAFramework.VT3500.GUI;
using FAFramework.VT3500.JobInfo;
using FAFramework.VT3500.Modules;
using FAFramework.VT3500.ExtendedParts;
using FALibrary.Alarm;
using FALibrary.Part.MemoryBasePart;
using FALibrary.Part.MMCPart;
using FALibrary.Sequence;
using FALibrary.Utility;

internal static class IMarkPositioningTests
{
    internal sealed class Input : FAPartInputIOInfo
    {
        public Input(bool value = true, string name = "X1135")
            : base(null, name, Convert.ToInt32(name.Substring(1), 16), "TEST_ONLY") { Value = value; }
        public override bool Value { get; set; }
    }

    internal sealed class Fixture : IDisposable
    {
        public readonly FASequenceManager Scheduler = new FASequenceManager();
        public readonly Input Sensor = new Input(false);
        public readonly FAFrontLoadingModule Module;
        public readonly FATapeLoadingServo Servo;
        public int FastCalls, SlowCalls, OffsetCalls, StopCalls, AlarmId, CompletedCalls;
        public bool CompleteOffset = true, DetectDuringSlow = true;
        public bool CompleteSlowWithoutMark;
        public double Target;
        private readonly EventHandler<FAAlarmEventArgs> _alarm;

        public Fixture(SubEquipment equipment, string name)
        {
            Module = (FAFrontLoadingModule)FormatterServices.GetUninitializedObject(typeof(FAFrontLoadingModule));
            Module.Name = "FrontPositioningTest";
            Module.Equipment = equipment;
            Module.UseFrontIMark = true;
            Module.FrontIMarkSensorOffset = 2.5;
            Module.TimeFrontIMarkTimeout = new FATime(FATimeType.millisecond, 200);
            Module.AlarmFrontIMarkCheckTimeOut = 2005002;
            Module.FrontIMarkCheckSensor = new FAPartOnOffSensor();
            Module.FrontIMarkCheckSensor.InputIO.Add(Sensor);
            Servo = new FATapeLoadingServo(Scheduler)
            {
                SimulationMode = true,
                AxisNo = 1,
                Tolerance = 0.01,
                MoveToPosTimeout = new FATime(FATimeType.second, 10),
                TapeLoadingPos = new FAMMCPosition { Position = 100 },
                TapeLoadingSlowPos = new FAMMCPosition { Position = 150, DriveSpeed = 17, AccelTime = 30, DecelTime = 40 },
                TargetPosition = new FAMMCPosition()
            };
            Servo.Stop.SetActionMethod(_ => StopCalls++);
            Servo.MoveTapeLoadingPos.SetActionMethod(_ => { FastCalls++; SetFeedback("ActualPos", 100.0); });
            Servo.MoveTapeLoadingSlowPos.SetActionMethod(_ =>
            {
                SlowCalls++;
                SetFeedback("ActualPos", 120.0);
                if (DetectDuringSlow) Sensor.Value = true;
                else if (CompleteSlowWithoutMark)
                {
                    SetFeedback("ActualPos", Servo.TapeLoadingSlowPos.Position);
                    SetFeedback("CommandPos", Servo.TapeLoadingSlowPos.Position);
                    SetFeedback("MotionDone", true);
                }
            });
            Servo.MoveToPos.SetActionMethod(_ =>
            {
                OffsetCalls++;
                Target = Servo.TargetPosition.Position;
                SetFeedback("CommandPos", Target);
                SetFeedback("MotionDone", CompleteOffset);
                if (CompleteOffset) SetFeedback("ActualPos", Target);
            });
            Servo.MoveTapeLoadingPos.Sequence.AddItem(Servo.MoveTapeLoadingPos.ExecuteForSequence);
            Servo.MoveTapeLoadingSlowPos.Sequence.AddItem(Servo.MoveTapeLoadingSlowPos.ExecuteForSequence);
            Module.TapeLoadingServo = Servo;
            Module.TapeLoadGrip = new FAPartGripRelease(Scheduler);
            Module.TapeLoadGrip.InputIO.Add(new Input(true, "X0230"));
            Module.TapeHoldGrip = new FAPartGripRelease(Scheduler);
            Module.TapeHoldGrip.InputIO.Add(new Input(false, "X0232"));
            Module.TapeLoadingMoveWithIMark = new FASequence(Scheduler) { Name = name };
            Module.TapeLoadingMoveWithIMark.OnTerminate += delegate { CompletedCalls++; };
            typeof(FAFrontLoadingModule).GetMethod("MakeTapeLoadingMoveWithIMark", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(Module, null);
            _alarm = (sender, args) => AlarmId = args.Alarm.AlarmNo;
            FAAlarmManager.Instance.OnRaiseAlarm += _alarm;
        }

        public void SetFeedback(string property, object value)
        {
            typeof(FAMMCPart).GetProperty(property).SetValue(Servo, value, null);
        }

        public void Pump(Func<bool> until)
        {
            for (int i = 0; i < 4000 && !until(); i++) { Scheduler.Run(); Thread.Sleep(1); }
        }
        public void Run()
        {
            Module.TapeLoadingMoveWithIMark.Start();
            Pump(() => Module.TapeLoadingMoveWithIMark.State == SequenceState.Terminated || Module.TapeLoadingMoveWithIMark.State == SequenceState.Suspended);
        }
        public void Tick(int count = 10)
        {
            for (int i = 0; i < count; i++) { Scheduler.Run(); Thread.Sleep(1); }
        }
        public void Dispose()
        {
            Module.TapeLoadingMoveWithIMark.Stop();
            FAAlarmManager.Instance.OnRaiseAlarm -= _alarm;
        }
    }

    public static void Run(SubEquipment equipment, Action<bool, string> check)
    {
        CheckSlowOnlyWindow(equipment, check);
        CheckArrivalWait(equipment, check);
        using (var f = new Fixture(equipment, "OffsetAndSameOn"))
        {
            f.CompleteOffset = false;
            f.Module.TapeLoadingMoveWithIMark.Start();
            f.Pump(() => f.OffsetCalls == 1);
            check(f.Target == 122.5 && f.FastCalls == 1 && f.SlowCalls == 1 && f.StopCalls >= 2,
                "Slow-window ON: stop confirmed then target = detection 120 + offset 2.5");
            check(f.Servo.TargetPosition.DriveSpeed == 17 && f.Servo.TargetPosition.AccelTime == 30 && f.Servo.TargetPosition.DecelTime == 40,
                "Offset inherits five-axis slow speed and acceleration settings");
            f.Pump(() => false);
            check(f.CompletedCalls == 0 && f.Servo.ActualPos == 120,
                "Unfinished FRONT offset cannot complete or reset coordinates");
            f.SetFeedback("ActualPos", f.Target);
            f.SetFeedback("MotionDone", true);
            f.Pump(() => f.Module.TapeLoadingMoveWithIMark.State == SequenceState.Terminated);
            check(f.CompletedCalls == 1 && f.Servo.ActualPos == 122.5,
                "FRONT offset completion preserves absolute position for cylinder handoff");
            f.Sensor.Value = false;
            f.DetectDuringSlow = true;
            f.CompleteOffset = true;
            f.Run();
            check(f.OffsetCalls == 2 && f.SlowCalls == 2 && f.FastCalls == 2 && f.Target == 122.5 && f.CompletedCalls == 2,
                "Every feed opens its own slow window and applies one new slow detection and offset");
        }
        using (var f = new Fixture(equipment, "OffsetZero"))
        {
            f.Module.FrontIMarkSensorOffset = 0;
            f.Run();
            check(f.OffsetCalls == 0 && f.CompletedCalls == 1, "Zero offset stops without additional motion");
        }
        using (var f = new Fixture(equipment, "OffsetLimit"))
        {
            f.Module.FrontIMarkSensorOffset = 2000;
            f.Run();
            check(f.AlarmId == 2005002 && f.OffsetCalls == 0 && f.CompletedCalls == 0,
                "Offset target outside existing slow-position limits stops and alarms");
            f.Module.TapeLoadingMoveWithIMark.Resume();
            f.Pump(() => f.Module.TapeLoadingMoveWithIMark.State == SequenceState.Suspended);
            check(f.OffsetCalls == 0 && f.CompletedCalls == 0, "Fault acknowledgement cannot silently retry or advance");
        }
        using (var f = new Fixture(equipment, "OffsetUnknown"))
        {
            f.Module.FrontIMarkCheckSensor.InputIO.Clear();
            f.Run();
            check(f.AlarmId == 2005002 && f.FastCalls == 0 && f.OffsetCalls == 0 && f.CompletedCalls == 0,
                "Unknown/unmapped sensor is rejected before movement");
        }
        using (var f = new Fixture(equipment, "OffsetAbort"))
        {
            f.CompleteOffset = false;
            f.Module.TapeLoadingMoveWithIMark.Start();
            f.Pump(() => f.OffsetCalls == 1);
            f.Module.TapeLoadingMoveWithIMark.Stop();
            f.Run();
            check(f.AlarmId == 2005002 && f.FastCalls == 1 && f.OffsetCalls == 1 && f.CompletedCalls == 0,
                "Aborted offset requires initialization; a fresh start cannot mistake the same ON for completion");
        }
        using (var f = new Fixture(equipment, "StopOverrun"))
        {
            f.Servo.Stop.SetActionMethod(_ => { f.StopCalls++; f.SetFeedback("ActualPos", f.Servo.ActualPos + 5); });
            f.Run();
            check(f.AlarmId == 2005002 && f.OffsetCalls == 0 && f.CompletedCalls == 0,
                "Deceleration overrun beyond offset target alarms instead of commanding reverse correction");
        }
        using (var f = new Fixture(equipment, "OffsetPause"))
        {
            f.CompleteOffset = false;
            f.Module.TapeLoadingMoveWithIMark.Start();
            f.Pump(() => f.OffsetCalls == 1);
            f.Module.TapeLoadingMoveWithIMark.PreSuspend();
            f.Module.TapeLoadingMoveWithIMark.Suspend();
            f.Pump(() => f.Module.TapeLoadingMoveWithIMark.State == SequenceState.Suspended);
            f.Module.FrontIMarkSensorOffset = 20;
            f.Module.TapeLoadingMoveWithIMark.Resume();
            f.SetFeedback("ActualPos", 122.5);
            f.SetFeedback("MotionDone", true);
            f.Pump(() => f.Module.TapeLoadingMoveWithIMark.State == SequenceState.Terminated);
            check(f.Target == 122.5 && f.OffsetCalls == 1 && f.CompletedCalls == 1,
                "Pause/resume during offset preserves the original absolute target despite a later setting change");
        }
        foreach (bool applyOffset in new[] { false, true })
        {
            using (var f = new Fixture(equipment, "SharedSearch_" + applyOffset))
            {
                var seq = new FASequence(f.Scheduler) { Name = "SearchHelper_" + applyOffset };
                var flags = BindingFlags.Instance | BindingFlags.NonPublic;
                var log = typeof(FAFrontLoadingModule).GetMethod("CreateIMarkDiagnostics", flags).Invoke(f.Module, new object[] { seq });
                typeof(FAFrontLoadingModule).GetMethod("AddIMarkPositioning", flags).Invoke(f.Module,
                    new object[] { seq, log, false, applyOffset, f.Module.TimeFrontIMarkTimeout });
                seq.Start();
                f.Pump(() => seq.State == SequenceState.Terminated);
                check(seq.State == SequenceState.Terminated && f.FastCalls == 0 && f.SlowCalls == 1 && f.OffsetCalls == (applyOffset ? 1 : 0),
                    applyOffset ? "Search-only shared path applies offset without fast feed" : "Shared search supports no-offset path");
            }
        }
        using (var f = new Fixture(equipment, "SearchResume"))
        {
            f.Sensor.Value = false;
            f.DetectDuringSlow = false;
            f.Module.TimeFrontIMarkTimeout = new FATime(FATimeType.second, 5);
            f.Module.TapeLoadingMoveWithIMark.Start();
            f.Pump(() => f.SlowCalls == 1);
            f.Module.TapeLoadingMoveWithIMark.PreSuspend();
            f.Module.TapeLoadingMoveWithIMark.Suspend();
            f.Pump(() => f.Module.TapeLoadingMoveWithIMark.State == SequenceState.Suspended);
            check(f.StopCalls > 0 && f.CompletedCalls == 0, "Pause during slow search requests a stop without counting");
            f.Module.TapeLoadingMoveWithIMark.Resume();
            f.DetectDuringSlow = true;
            f.Pump(() => f.Module.TapeLoadingMoveWithIMark.State == SequenceState.Terminated);
            check(f.SlowCalls == 2 && f.OffsetCalls == 1 && f.CompletedCalls == 1,
                "Explicit resume restarts interrupted search once and applies offset once");
        }
        CheckJobsAndUi(check);
    }

    private static void CheckArrivalWait(SubEquipment equipment, Action<bool, string> check)
    {
        using (var f = new Fixture(equipment, "SlowArrivalDelayedMark"))
        {
            f.DetectDuringSlow = false;
            f.Servo.MoveTapeLoadingSlowPos.SetActionMethod(_ =>
            {
                f.SlowCalls++;
                f.SetFeedback("ActualPos", 140.0);
                f.SetFeedback("CommandPos", 150.0);
                f.SetFeedback("MotionDone", false);
                f.SetFeedback("RunFlag", true);
            });
            f.Module.TapeLoadingMoveWithIMark.Start();
            f.Pump(() => f.SlowCalls == 1);
            var watch = System.Diagnostics.Stopwatch.StartNew();
            while (watch.ElapsedMilliseconds < 1200) f.Tick(1);
            check(f.AlarmId == 0 && f.CompletedCalls == 0,
                "Slow travel beyond legacy search timeout does not raise a mark timeout");
            f.SetFeedback("ActualPos", 150.0);
            f.SetFeedback("RunFlag", false);
            f.SetFeedback("MotionDone", true);
            f.Tick();
            watch.Restart();
            while (watch.ElapsedMilliseconds < 1500) f.Tick(1);
            check(f.AlarmId == 0 && f.OffsetCalls == 0, "Arrival keeps detection open for three seconds without extra motion");
            f.Sensor.Value = true;
            f.Pump(() => f.CompletedCalls == 1);
            check(f.CompletedCalls == 1 && f.Target == 152.5 && f.SlowCalls == 1,
                "Fresh mark after slow arrival is accepted and offset is applied once");
        }
        using (var f = new Fixture(equipment, "SlowArrivalNoMark"))
        {
            f.DetectDuringSlow = false;
            f.CompleteSlowWithoutMark = true;
            f.Module.TapeLoadingMoveWithIMark.Start();
            f.Pump(() => f.SlowCalls == 1);
            var watch = System.Diagnostics.Stopwatch.StartNew();
            while (watch.ElapsedMilliseconds < 2700) f.Tick(1);
            check(f.AlarmId == 0, "No missing-mark alarm before three seconds after arrival");
            f.Pump(() => f.AlarmId != 0);
            check(watch.ElapsedMilliseconds >= 3000 && f.AlarmId != 0 && f.CompletedCalls == 0 && f.OffsetCalls == 0,
                "Missing mark alarms only after the full three-second arrival wait");
        }
        using (var f = new Fixture(equipment, "SlowTravelStalled"))
        {
            f.Servo.MoveToPosTimeout = new FATime(FATimeType.millisecond, 100);
            f.Servo.MoveTapeLoadingSlowPos.SetActionMethod(_ => { f.SlowCalls++; f.SetFeedback("MotionDone", false); });
            f.Run();
            check(f.AlarmId != 0 && f.CompletedCalls == 0 && f.StopCalls > 0,
                "Motor travel watchdog still stops a slow move that never reaches its target");
        }
    }

    private static void CheckSlowOnlyWindow(SubEquipment equipment, Action<bool, string> check)
    {
        using (var f = new Fixture(equipment, "IgnoreFastCarryOn"))
        {
            bool fastDone = false;
            f.DetectDuringSlow = false;
            f.Servo.MoveTapeLoadingPos.Sequence.AddItem((actor, time) => { if (fastDone) actor.NextStep(); });
            f.Module.TapeLoadingMoveWithIMark.Start();
            f.Pump(() => f.FastCalls == 1);
            f.Sensor.Value = true;
            f.SetFeedback("ActualPos", 20.0);
            f.Tick();
            f.Sensor.Value = false;
            f.SetFeedback("ActualPos", 30.0);
            f.Tick();
            f.Sensor.Value = true;
            f.SetFeedback("ActualPos", 40.0);
            f.Tick();
            check(f.SlowCalls == 0 && f.StopCalls == 0 && f.OffsetCalls == 0 && f.CompletedCalls == 0,
                "High-speed ON/OFF/ON transitions never stop, offset or finish the feed");
            f.SetFeedback("ActualPos", 100.0);
            fastDone = true;
            f.Pump(() => f.SlowCalls == 1 || f.OffsetCalls != 0);
            check(f.SlowCalls == 1 && f.OffsetCalls == 0 && f.CompletedCalls == 0,
                "High-speed ON carried into slow start still issues a slow command, without detection");
            f.Pump(() => f.CompletedCalls == 1);
            check(f.CompletedCalls == 1 && f.StopCalls > 0 && f.SlowCalls == 1 && f.OffsetCalls == 1 && f.Target == 122.5,
                "Current ON stops at slow-window position 120 without waiting for a new edge or target 150");
        }
        using (var f = new Fixture(equipment, "FastPulseOnly"))
        {
            bool fastDone = false;
            f.DetectDuringSlow = false;
            f.CompleteSlowWithoutMark = true;
            f.Servo.MoveTapeLoadingPos.Sequence.AddItem((actor, time) => { if (fastDone) actor.NextStep(); });
            f.Module.TapeLoadingMoveWithIMark.Start();
            f.Pump(() => f.FastCalls == 1);
            f.Sensor.Value = true;
            f.Tick();
            f.Sensor.Value = false;
            f.Tick();
            fastDone = true;
            f.Pump(() => f.AlarmId != 0);
            check(f.SlowCalls == 1 && f.OffsetCalls == 0 && f.CompletedCalls == 0 && f.AlarmId != 0,
                "A pulse entirely within high-speed movement cannot satisfy the later slow search");
        }
        using (var f = new Fixture(equipment, "HeldOnDuringSlow"))
        {
            bool stopped = false;
            f.Sensor.Value = true;
            f.DetectDuringSlow = false;
            f.Servo.Stop.Sequence.AddItem((actor, time) => { if (stopped) actor.NextStep(); });
            typeof(FAFrontLoadingModule).GetMethod("ResetFrontIMarkAfterInitialize", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(f.Module, null);
            f.Module.TapeLoadingMoveWithIMark.Start();
            f.Pump(() => f.SlowCalls == 1);
            f.SetFeedback("RunFlag", true);
            f.SetFeedback("MotionDone", false);
            f.Pump(() => f.StopCalls > 0);
            check(f.FastCalls == 1 && f.SlowCalls == 1 && f.StopCalls > 0 && f.Servo.ActualPos == 120,
                "Held ON issues a stop while slow motion is running before target arrival");
            f.Tick();
            check(f.OffsetCalls == 0 && f.CompletedCalls == 0, "Detection cannot advance to offset or handoff before stop confirmation");
            stopped = true;
            f.SetFeedback("RunFlag", false);
            f.Pump(() => f.CompletedCalls == 1 || f.AlarmId != 0);
            check(f.CompletedCalls == 1 && f.OffsetCalls == 1 && f.Target == 122.5 && f.AlarmId == 0 && f.SlowCalls == 1,
                "Confirmed stop applies offset from the slow detection position once before completion");
        }
        using (var f = new Fixture(equipment, "ResumeIgnoresStoppedOn"))
        {
            f.DetectDuringSlow = false;
            f.Module.TimeFrontIMarkTimeout = new FATime(FATimeType.second, 5);
            f.Module.TapeLoadingMoveWithIMark.Start();
            f.Pump(() => f.SlowCalls == 1);
            f.Module.TapeLoadingMoveWithIMark.PreSuspend();
            f.Module.TapeLoadingMoveWithIMark.Suspend();
            f.Sensor.Value = true;
            f.Tick();
            check(f.OffsetCalls == 0 && f.CompletedCalls == 0, "Paused search does not accept ON until explicitly resumed");
            f.Module.TapeLoadingMoveWithIMark.Resume();
            f.Pump(() => f.SlowCalls == 2 || f.OffsetCalls != 0);
            f.Pump(() => f.CompletedCalls == 1);
            check(f.CompletedCalls == 1 && f.OffsetCalls == 1 && f.SlowCalls == 2,
                "Resumed slow command accepts current ON without waiting for another edge");
        }
        using (var f = new Fixture(equipment, "SameOnAtSlowTarget"))
        {
            f.Sensor.Value = true;
            f.DetectDuringSlow = false;
            f.CompleteSlowWithoutMark = true;
            f.Module.TapeLoadingMoveWithIMark.Start();
            f.Pump(() => f.SlowCalls == 1);
            f.Pump(() => f.CompletedCalls == 1);
            check(f.CompletedCalls == 1 && f.StopCalls > 0 && f.OffsetCalls == 1 && f.Target == 152.5 && f.SlowCalls == 1 && f.AlarmId == 0,
                "ON at the slow endpoint stops and completes without requiring an OFF sample");
        }
    }

    private static void CheckJobsAndUi(Action<bool, string> check)
    {
        var job = new MoveJobInfo { UseFrontIMark = true, FrontIMarkSensorOffset = 3.25 };
        check(((MoveJobInfo)job.Clone()).FrontIMarkSensorOffset == 3.25 &&
              job.ToKeyValueArray("").Any(x => x.StartsWith("FrontIMarkSensorOffset=")), "JOB clone and diagnostic export include offset");
        foreach (double bad in new[] { -1, double.NaN, double.PositiveInfinity })
        {
            bool rejected = false;
            try { job.FrontIMarkSensorOffset = bad; } catch (ArgumentOutOfRangeException) { rejected = true; }
            check(rejected && job.FrontIMarkSensorOffset == 3.25, "Invalid offset rejected: " + bad);
        }
        var root = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JobRoundTrip");
        Directory.CreateDirectory(root);
        var manager = new JobManager { JobFolderPath = root };
        var lot = new FALotJobInfo { Name = "OffsetTest", MoveJobInfo = job };
        manager.LotJobInstance.LotJobInfoList.Add(lot);
        manager.Save();
        var loaded = new JobManager { JobFolderPath = root };
        loaded.Load();
        check(loaded.LotJobInstance.LotJobInfoList[0].MoveJobInfo.FrontIMarkSensorOffset == 3.25, "Actual jobfile.xml Save/Load preserves offset");
        var xml = System.Xml.Linq.XDocument.Load(Path.Combine(root, "jobfile.xml"));
        xml.Descendants("FrontIMarkSensorOffset").Remove();
        xml.Save(Path.Combine(root, "jobfile.xml"));
        loaded.Load();
        check(loaded.LotJobInstance.LotJobInfoList[0].MoveJobInfo.FrontIMarkSensorOffset == 0, "Legacy jobfile without offset loads default zero");

        if (Application.Current == null) new Application();
        Application.Current.Resources["DetailFATimeProvider"] = Enum.GetValues(typeof(FATimeType));
        var control = new PositionControl { JobInstance = job };
        var box = (TextBox)control.FindName("IMarkOffsetTextBox");
        control.Measure(new Size(1760, 760));
        control.Arrange(new Rect(0, 0, 1760, 760));
        control.UpdateLayout();
        Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ApplicationIdle, new Action(() => { }));
        box.Text = "4.75";
        check(control.ApplyIMarkEdits() && job.FrontIMarkSensorOffset == 4.75, "Actual WPF offset TextBox updates JOB binding");
        box.Text = "-2";
        check(!control.ApplyIMarkEdits() && job.FrontIMarkSensorOffset == 4.75, "Invalid WPF input blocks save validation");
        box.Text = "4.75";
        control.ApplyIMarkEdits();
        foreach (int width in new[] { 1760, 1024, 600 })
        {
            control.Measure(new Size(width, 760));
            control.Arrange(new Rect(0, 0, width, 760));
            control.UpdateLayout();
            var panel = (WrapPanel)control.FindName("IMarkSettingsPanel");
            var slow = (TextBox)control.FindName("IMarkSlowDistanceTextBox");
            var use = (CheckBox)control.FindName("IMarkUseCheckBox");
            check(((FrameworkElement)box.Parent).Visibility == Visibility.Collapsed &&
                ((FrameworkElement)slow.Parent).Visibility == Visibility.Collapsed &&
                use.Visibility == Visibility.Visible && panel.ActualHeight > 0,
                "JOB hides slow distance and offset while preserving the I-Mark usage checkbox at width " + width);
        }
    }
}
