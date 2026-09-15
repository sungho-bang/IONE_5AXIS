using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using FAFramework.VT3500;
using FAFramework.VT3500.JobInfo;
using FAFramework.VT3500.Modules;
using FAFramework.VT3500.ExtendedParts;
using FALibrary.Part.MemoryBasePart;
using FALibrary.Part.MMCPart;
using FALibrary.Sequence;
using FALibrary.Utility;

internal static class IMarkFrontIntegrationTests
{
    private static void Make(FAFrontLoadingModule module, string method)
    {
        typeof(FAFrontLoadingModule).GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(module, null);
    }

    public static void Run(SubEquipment equipment, Action<bool, string> check)
    {
        CheckParentRoutes(equipment, check);
        using (var f = new IMarkPositioningTests.Fixture(equipment, "FrontCylinderHandoff"))
        {
            var calls = new List<string>();
            var loadInput = (IMarkPositioningTests.Input)f.Module.TapeLoadGrip.InputIO[0];
            var holdInput = (IMarkPositioningTests.Input)f.Module.TapeHoldGrip.InputIO[0];
            f.Module.TapeHoldGrip.Grip.SetActionMethod(_ => { calls.Add("HOLD_GRIP"); holdInput.Value = true; });
            f.Module.TapeLoadGrip.Release.SetActionMethod(_ => { calls.Add("LOAD_RELEASE"); loadInput.Value = false; });
            f.Module.TapeLoadGrip.Grip.SetActionMethod(_ => { calls.Add("LOAD_GRIP"); loadInput.Value = true; });
            f.Module.TapeHoldGrip.Release.SetActionMethod(_ => { calls.Add("HOLD_RELEASE"); holdInput.Value = false; });
            f.Module.TapeMovePickCylinder = new FASequence(f.Scheduler) { Name = "FrontCylinderParent" };
            Make(f.Module, "MakeTapeMovePickCylinder");
            f.CompleteOffset = false;
            f.Module.TapeMovePickCylinder.Start();
            f.Pump(() => f.OffsetCalls == 1);
            f.Pump(() => false);
            check(calls.Count == 0 && f.Module.TapeMovePickCylinder.State == SequenceState.Running,
                "Production FRONT parent cannot hand off cylinders while offset is unfinished");
            f.SetFeedback("ActualPos", f.Target);
            f.SetFeedback("MotionDone", true);
            f.Pump(() => f.Module.TapeMovePickCylinder.State == SequenceState.Terminated);
            check(calls.SequenceEqual(new[] { "HOLD_GRIP", "LOAD_RELEASE" }) && f.Servo.ActualPos == 122.5,
                "Production FRONT parent: offset completion -> HOLD grip -> LOAD release, without coordinate reset");
            check(f.Module.LastFrontIMarkFeedDistance == 122.5,
                "FRONT material-use distance includes slow search and offset, not only the shortened fast target");
            f.Servo.MoveHomePos.SetActionMethod(_ => { calls.Add("HOME_POSITION"); f.SetFeedback("ActualPos", 0.0); });
            f.Servo.MoveHomePos.Sequence.AddItem(f.Servo.MoveHomePos.ExecuteForSequence);
            f.Module.TapeMovePlaceCylinder = new FASequence(f.Scheduler) { Name = "FrontReturnParent" };
            Make(f.Module, "MakeTapeMovePlaceCylinder");
            f.Module.TapeMovePlaceCylinder.Start();
            f.Pump(() => f.Module.TapeMovePlaceCylinder.State == SequenceState.Terminated);
            check(calls.SequenceEqual(new[] { "HOLD_GRIP", "LOAD_RELEASE", "HOME_POSITION", "LOAD_GRIP", "HOLD_RELEASE" }),
                "Production return path preserves 4-axis cylinder handoff and gripper home movement");
            check(f.Servo.AxisNo == 1 && typeof(FARearLoadingModule).GetProperty("IMarkCheckSensor") == null,
                "X1135 positioning uses FRONT AxisNo 1; REAR no longer exposes the added X1135 sensor");
        }
        using (var f = new IMarkPositioningTests.Fixture(equipment, "GripperInterlock"))
        {
            ((IMarkPositioningTests.Input)f.Module.TapeLoadGrip.InputIO[0]).Value = false;
            f.Run();
            check(f.FastCalls == 0 && f.SlowCalls == 0 && f.AlarmId != 0,
                "Released loading gripper blocks FRONT I-Mark movement");
        }
        using (var f = new IMarkPositioningTests.Fixture(equipment, "InitializeRearm"))
        {
            f.Sensor.Value = true;
            f.DetectDuringSlow = false;
            f.CompleteSlowWithoutMark = true;
            Make(f.Module, "ResetFrontIMarkAfterInitialize");
            f.Run();
            check(f.OffsetCalls == 1 && f.SlowCalls == 1 && f.CompletedCalls == 1 && f.AlarmId == 0 && f.Target == 152.5,
                "Initialization ON still executes the slow command and is accepted before handoff");
        }
        using (var f = new IMarkPositioningTests.Fixture(equipment, "FrontJobTargets"))
        {
            var job = new MoveJobInfo { UseFrontIMark = true, UseIMark = true,
                FeedingPitch = 150, FeedingSpeed = 500, FrontIMarkSlowDistance = 10, FrontIMarkSensorOffset = 3 };
            f.Module.ApplyFrontIMarkJob(job);
            check(f.Servo.TapeLoadingPos.Position == 140 && f.Servo.TapeLoadingSlowPos.Position == 150 &&
                f.Servo.TapeLoadingSlowPos.DriveSpeed == 17 && f.Module.FrontIMarkSensorOffset == 3 && job.UseIMark,
                "FRONT JOB: fast = pitch - slow distance, slow = pitch; REAR mode and FRONT slow speed preserved");
            job.UseFrontIMark = false;
            f.Module.ApplyFrontIMarkJob(job);
            check(f.Servo.TapeLoadingPos.Position == 150 && !f.Module.UseFrontIMark && job.UseIMark,
                "FRONT OFF restores normal full pitch without changing REAR mode");
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(MoveJobInfo));
            var legacy = (MoveJobInfo)serializer.Deserialize(new System.IO.StringReader(
                "<MoveJobInfo><UseIMark>true</UseIMark><IMarkSensorOffset>99</IMarkSensorOffset></MoveJobInfo>"));
            check(legacy.UseIMark && !legacy.UseFrontIMark && legacy.FrontIMarkSensorOffset == 0 && legacy.FrontIMarkSlowDistance == 10,
                "Legacy REAR usage/offset cannot implicitly enable or offset the FRONT axis");
            var clone = (MoveJobInfo)job.Clone();
            check(clone.FrontIMarkSlowDistance == 10 && clone.FrontIMarkSensorOffset == 3 && clone.UseIMark,
                "JOB clone preserves distinct FRONT settings and legacy REAR setting");
        }
        foreach (bool useRearMark in new[] { false, true })
        {
            var scheduler = new FASequenceManager();
            var rear = (FARearLoadingModule)FormatterServices.GetUninitializedObject(typeof(FARearLoadingModule));
            rear.Equipment = equipment;
            rear.ProductInfo = new FAFramework.Utility.FAProductInfo();
            rear.Name = "RestoredRearTest";
            rear.UseIMark = useRearMark;
            rear.RetryInfoBlackMarkRetry = new FARetryInfo(3);
            rear.TimeLoadingTimeout = new FATime(FATimeType.second, 1);
            rear.TimeRollerDelay = new FATime(FATimeType.millisecond, 1);
            rear.BlackMarkCheckSensor = new FAPartOnOffSensor();
            rear.BlackMarkCheckSensor.InputIO.Add(new IMarkPositioningTests.Input(true, "X1140"));
            var roller = new FABandRollerServo(scheduler) { SimulationMode = true, AxisNo = 4 };
            int fast = 0, slow = 0;
            roller.Stop.SetActionMethod(_ => { });
            roller.MoveTapeLoadingPos.SetActionMethod(_ => fast++);
            roller.MoveTapeLoadingSlowPos.SetActionMethod(_ => slow++);
            roller.MoveTapeLoadingPos.Sequence.AddItem(roller.MoveTapeLoadingPos.ExecuteForSequence);
            roller.MoveTapeLoadingSlowPos.Sequence.AddItem(roller.MoveTapeLoadingSlowPos.ExecuteForSequence);
            rear.BandRollerServo = roller;
            rear.MovingRoller = new FASequence(scheduler);
            typeof(FARearLoadingModule).GetMethod("MakeMovingRoller", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(rear, null);
            rear.MovingRoller.Start();
            for (int i = 0; i < 300 && rear.MovingRoller.State != SequenceState.Terminated; i++)
            {
                scheduler.Run();
                System.Threading.Thread.Sleep(1);
            }
            check(rear.MovingRoller.State == SequenceState.Terminated && fast == 1 && slow == 1 &&
                rear.CurrentCount == 1 && rear.UICuttingCount == 2,
                "Restored REAR legacy mark mode " + useRearMark + " retains feed/count behavior");
        }
    }

    private static FASequence EmptySequence(IMarkPositioningTests.Fixture f, string name)
    {
        var sequence = new FASequence(f.Scheduler) { Name = name };
        sequence.AddTerminate();
        return sequence;
    }

    private static List<string> ConfigureParents(IMarkPositioningTests.Fixture f, bool keepSensorOn)
    {
        var m = f.Module;
        m.ProductInfo = new FAFramework.Utility.FAProductInfo();
        var calls = new List<string>();
        var load = (IMarkPositioningTests.Input)m.TapeLoadGrip.InputIO[0];
        var hold = (IMarkPositioningTests.Input)m.TapeHoldGrip.InputIO[0];
        m.TapeHoldGrip.Grip.SetActionMethod(_ => { calls.Add("HOLD_GRIP"); hold.Value = true; });
        m.TapeLoadGrip.Release.SetActionMethod(_ => { calls.Add("LOAD_RELEASE"); load.Value = false; });
        m.TapeLoadGrip.Grip.SetActionMethod(_ => { calls.Add("LOAD_GRIP"); load.Value = true; });
        m.TapeHoldGrip.Release.SetActionMethod(_ => { calls.Add("HOLD_RELEASE"); hold.Value = false; });
        f.Servo.MoveHomePos.SetActionMethod(_ =>
        {
            calls.Add("HOME");
            f.SetFeedback("ActualPos", 0.0);
            if (!keepSensorOn) f.Sensor.Value = false;
        });
        f.Servo.MoveHomePos.Sequence.AddItem(f.Servo.MoveHomePos.ExecuteForSequence);
        m.TapeLoadingMoveWithIMark.OnStart += delegate { calls.Add("FEED"); };
        m.TapeLoadingServoSettingLength = 1000;
        m.RModule = (FARearLoadingModule)FormatterServices.GetUninitializedObject(typeof(FARearLoadingModule));
        m.ThirdPressModule = (FAPressModule)FormatterServices.GetUninitializedObject(typeof(FAPressModule));
        m.BandPickServo = new FABandPickServo(f.Scheduler) { SimulationMode = true };
        m.BandTransferServo = new FABandTransferServo(f.Scheduler) { SimulationMode = true };
        m.BandPitchChangeCylinder = new FAPartPushHome(f.Scheduler) { SimulationMode = true };
        m.BypassCoveyorExistCheck = new FAPartOnOffSensor();
        m.BypassCoveyorExistCheck.InputIO.Add(new IMarkPositioningTests.Input(false));
        m.WorkBandPickMove = EmptySequence(f, "MockBandPick");
        m.WorkTapeMovePick = EmptySequence(f, "MockTransferPick");
        m.WorkFirstPressServo = EmptySequence(f, "MockFirstPress");
        m.WorkSecondPressServo = EmptySequence(f, "MockSecondPress");
        m.WorkOptionPressServo = EmptySequence(f, "MockOptionPress");
        m.WorkThridPressServo = EmptySequence(f, "MockThirdPress");
        m.WorkTomsonPress = EmptySequence(f, "MockTomsonPress");
        m.TapeMovePickCylinder = new FASequence(f.Scheduler) { Name = "TapeMovePickCylinder" };
        m.TapeMovePlaceCylinder = new FASequence(f.Scheduler) { Name = "TapeMovePlaceCylinder" };
        Make(m, "MakeTapeMovePickCylinder");
        Make(m, "MakeTapeMovePlaceCylinder");
        m.WorkTapeMovePickCylinder = new FASequence(f.Scheduler) { Name = "WorkTapeMovePickCylinder" };
        Make(m, "MakeWorkTapeMovePickCylinder");
        // Substitute unrelated transfer hardware, but execute the production gripper return path.
        m.WorkBandPlaceMoveCylinder = new FASequence(f.Scheduler) { Name = "MockTransferWithGripperReturn" };
        m.WorkBandPlaceMoveCylinder.AddItem(m.TapeMovePlaceCylinder);
        m.WorkFirstPress = new FASequence(f.Scheduler) { Name = "WorkFirstPress" };
        m.WorkPress = new FASequence(f.Scheduler) { Name = "WorkPress" };
        Make(m, "MakeWorkFirstPress");
        Make(m, "MakeWorkPress");
        m.WorkFirstBandMoveLoading = new FASequence(f.Scheduler) { Name = "WorkFirstBandMoveLoading" };
        m.WorkLoopBandMoveLoading = new FASequence(f.Scheduler) { Name = "WorkLoopBandMoveLoading" };
        Make(m, "MakeWorkFirstBandMoveLoading");
        Make(m, "MakeWorkLoopBandMoveLoading");
        return calls;
    }

    private static void CheckParentRoutes(SubEquipment equipment, Action<bool, string> check)
    {
        foreach (bool useMark in new[] { true, false })
        foreach (bool usePress in new[] { false, true })
        foreach (bool existingOn in new[] { true, false })
        using (var f = new IMarkPositioningTests.Fixture(equipment, "FirstHandoff_" + useMark + "_" + usePress + "_" + existingOn))
        {
            var calls = ConfigureParents(f, existingOn);
            f.Module.UseFrontIMark = useMark;
            f.Module.ThirdPressModule.UsePress = usePress;
            f.Sensor.Value = existingOn;
            f.DetectDuringSlow = !existingOn;
            int observedSlowCalls = 0;
            bool pendingSlowOn = false;
            f.Module.TapeLoadingMoveWithIMark.AddWatcher(() =>
            {
                if (!useMark || !existingOn) return;
                if (observedSlowCalls != f.SlowCalls)
                {
                    observedSlowCalls = f.SlowCalls;
                    f.Sensor.Value = false;
                    pendingSlowOn = true;
                }
                else if (pendingSlowOn)
                {
                    f.Sensor.Value = true;
                    pendingSlowOn = false;
                }
            });
            if (existingOn) Make(f.Module, "ResetFrontIMarkAfterInitialize");
            int feeds = usePress ? 2 : 1;
            double distance = useMark ? 122.5 : 100;
            string scenario = "mark=" + useMark + ", press=" + usePress + ", existingOn=" + existingOn;
            f.Module.WorkFirstBandMoveLoading.Start();
            f.Pump(() => f.Module.WorkFirstBandMoveLoading.State == SequenceState.Terminated || f.AlarmId != 0);
            Console.WriteLine("FirstParent " + scenario + ";State=" + f.Module.WorkFirstBandMoveLoading.State +
                ";FastCalls=" + f.FastCalls + ";SlowCalls=" + f.SlowCalls + ";Completed=" + f.CompletedCalls +
                ";Alarm=" + f.AlarmId + ";Calls=" + string.Join(",", calls));
            check(f.AlarmId == 0 && f.Module.WorkFirstBandMoveLoading.State == SequenceState.Terminated &&
                f.FastCalls == feeds && f.CompletedCalls == feeds && f.Module.RModule.ReSet,
                "First production parent completes without duplicate post-handoff feed: " + scenario);
            check(calls.Count(x => x == "FEED") == feeds && calls.Count(x => x == "HOLD_GRIP") == feeds &&
                calls.Count(x => x == "LOAD_RELEASE") == feeds &&
                Math.Abs(f.Module.TapeLoadingServoUsedLength - feeds * distance / 1000) < 0.000001,
                "First parent hands off and counts material once per real feed: " + scenario);
            check(f.SlowCalls == (useMark ? feeds : 0) && f.OffsetCalls == (useMark ? feeds : 0),
                "Every enabled feed detects only in its slow window: " + scenario);

            int fastBefore = f.FastCalls;
            calls.Clear();
            f.Module.WorkLoopBandMoveLoading.Start();
            f.Pump(() => f.Module.WorkLoopBandMoveLoading.State == SequenceState.Terminated || f.AlarmId != 0);
            check(f.AlarmId == 0 && f.Module.WorkLoopBandMoveLoading.State == SequenceState.Terminated &&
                f.FastCalls == fastBefore + 1 && calls.IndexOf("HOME") >= 0 &&
                calls.IndexOf("HOME") < calls.IndexOf("FEED") && calls.Count(x => x == "FEED") == 1,
                "Next production loop returns home then performs one required feed: " + scenario);
        }
        foreach (bool bypass in new[] { false, true })
        using (var f = new IMarkPositioningTests.Fixture(equipment, "LoopMaterial_" + bypass))
        {
            var calls = ConfigureParents(f, false);
            f.Module.ThirdPressModule.ExistMaterial = true;
            ((IMarkPositioningTests.Input)f.Module.BypassCoveyorExistCheck.InputIO[0]).Value = bypass;
            f.Sensor.Value = false;
            f.DetectDuringSlow = true;
            f.Module.WorkLoopBandMoveLoading.Start();
            f.Pump(() => f.Module.WorkLoopBandMoveLoading.State == SequenceState.Terminated || f.AlarmId != 0);
            check(f.AlarmId == 0 && f.Module.WorkLoopBandMoveLoading.State == SequenceState.Terminated &&
                f.FastCalls == 1 && f.SlowCalls == 1 && f.OffsetCalls == 1 && calls.IndexOf("HOME") < calls.IndexOf("FEED"),
                "Material-present loop preserves required feed for bypass=" + bypass);
        }
    }
}
