using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using FAFramework.Utility;
using FAFramework.VT3500.JobInfo;
using FALibrary;
using FALibrary.Part.MemoryBasePart;
using FALibrary.Sequence;
using FALibrary.Utility;

namespace FAFramework.VT3500.Modules
{
    public partial class FAFrontLoadingModule
    {
        private bool _iMarkSensorOnHandled;
        private bool _iMarkPositioningRecoveryRequired;
        private string _frontIMarkCycleId;
        public double LastFrontIMarkFeedDistance { get; private set; }
        public string AppliedFrontIMarkJobName { get; private set; }
        private object _iMarkSettingsSync;
        private object IMarkSettingsSync { get { return LazyInitializer.EnsureInitialized(ref _iMarkSettingsSync); } }
        private PendingIMarkUsage _pendingIMarkUsage;

        private sealed class PendingIMarkUsage
        {
            public string RequestId;
            public string JobName;
            public MoveJobInfo Settings;
            public string Error;
        }

        public bool? PendingFrontIMarkUse
        {
            get { lock (IMarkSettingsSync) return _pendingIMarkUsage?.Settings.UseFrontIMark; }
        }

        public string PendingFrontIMarkError
        {
            get { lock (IMarkSettingsSync) return _pendingIMarkUsage?.Error; }
        }

        private static MoveJobInfo CopyFrontSettings(MoveJobInfo job)
        {
            return new MoveJobInfo
            {
                UseFrontIMark = job.UseFrontIMark, FeedingPitch = job.FeedingPitch,
                FeedingSpeed = job.FeedingSpeed, FrontIMarkSlowDistance = job.FrontIMarkSlowDistance,
                FrontIMarkSensorOffset = job.FrontIMarkSensorOffset
            };
        }

        private static bool SameFrontSettings(MoveJobInfo first, MoveJobInfo second)
        {
            return first.UseFrontIMark == second.UseFrontIMark && first.FeedingPitch == second.FeedingPitch &&
                first.FeedingSpeed == second.FeedingSpeed && first.FrontIMarkSlowDistance == second.FrontIMarkSlowDistance &&
                first.FrontIMarkSensorOffset == second.FrontIMarkSensorOffset;
        }

        public void QueueFrontIMarkUsage(MoveJobInfo job, string jobName)
        {
            lock (IMarkSettingsSync)
            {
                var snapshot = CopyFrontSettings(job);
                ValidateFrontIMarkJob(snapshot);
                var previousRequest = _pendingIMarkUsage?.RequestId;
                _pendingIMarkUsage = new PendingIMarkUsage
                {
                    RequestId = Guid.NewGuid().ToString("N"), JobName = jobName, Settings = snapshot
                };
                WriteIMarkLog(string.Format(CultureInfo.InvariantCulture,
                    "Event=MAIN_USAGE_REQUESTED;RequestId={0};Replaces={1};Job={2};RequestedUse={3};AppliedUse={4};Pitch={5};FeedingSpeed={6};SlowDistance={7};Offset={8};EquipmentState={9};Axis=FRONT.TapeLoadingServo;Input=X1135;Persistence=PendingConfigSave;MotionCommand=None",
                    _pendingIMarkUsage.RequestId, previousRequest, jobName, snapshot.UseFrontIMark, UseFrontIMark,
                    snapshot.FeedingPitch, snapshot.FeedingSpeed, snapshot.FrontIMarkSlowDistance,
                    snapshot.FrontIMarkSensorOffset, Equipment?.State?.GetType().Name));
            }
        }

        private bool IsFrontAxisBusy()
        {
            var servo = TapeLoadingServo;
            return servo == null || servo.RunFlag || new[]
            {
                servo.MoveHomePos?.Sequence, servo.MoveHome?.Sequence, servo.MoveToPos?.Sequence,
                servo.MoveVelocity?.Sequence, servo.MoveTapeLoadingPos?.Sequence,
                servo.MoveTapeLoadingSlowPos?.Sequence, servo.Stop?.Sequence
            }.Any(sequence => sequence != null && sequence.State != SequenceState.Available &&
                sequence.State != SequenceState.Terminated && sequence.State != SequenceState.Aborted);
        }

        public bool TryApplyPendingFrontIMarkUsage(bool beforeFeed, out string error)
        {
            lock (IMarkSettingsSync)
            {
                error = null;
                var pending = _pendingIMarkUsage;
                if (pending == null) return true;
                var equipment = Equipment as SubEquipment;
                var selected = equipment?.JobManagerInstance?.LotJobInstance?.LotJobInfoList
                    .FirstOrDefault(job => job.Name == pending.JobName);
                if (selected == null || equipment.MainLoopModule?.SelectJob != pending.JobName)
                {
                    WriteIMarkLog("Event=MAIN_USAGE_REQUEST_CANCELLED;RequestId=" + pending.RequestId + ";Reason=RecipeChangedOrRemoved");
                    _pendingIMarkUsage = null;
                    return true;
                }
                if (beforeFeed ? IsFrontAxisBusy() : IMarkUsageSelection.GetRecipeBlockReason(equipment, selected) != null)
                    return false;
                try
                {
                    ApplyFrontIMarkJobCore(pending.Settings, pending.JobName);
                    var runtime = equipment.MainLoopModule.MoveJobInfo;
                    runtime.UseFrontIMark = pending.Settings.UseFrontIMark;
                    runtime.FeedingPitch = pending.Settings.FeedingPitch;
                    runtime.FeedingSpeed = pending.Settings.FeedingSpeed;
                    runtime.FrontIMarkSlowDistance = pending.Settings.FrontIMarkSlowDistance;
                    runtime.FrontIMarkSensorOffset = pending.Settings.FrontIMarkSensorOffset;
                    _pendingIMarkUsage = null;
                    WriteIMarkLog("Event=MAIN_USAGE_REQUEST_APPLIED;RequestId=" + pending.RequestId +
                        ";Job=" + pending.JobName + ";UseFrontIMark=" + UseFrontIMark +
                        ";Boundary=" + (beforeFeed ? "BeforeFrontFeed" : "StoppedIdle") + ";MotionCommand=None");
                    return true;
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    if (pending.Error != error)
                        WriteIMarkLog("Event=MAIN_USAGE_REQUEST_FAILED;RequestId=" + pending.RequestId + ";Reason=" + error);
                    pending.Error = error;
                    return false;
                }
            }
        }

        [FA("Sequences")]
        public FASequence TapeLoadingMoveWithIMark { get; set; }
        public FAPartOnOffSensor FrontIMarkCheckSensor { get; set; }

        [FA("Time")]
        public FATime TimeFrontIMarkTimeout { get; set; } = new FATime(FATimeType.second, 1);

        [FAProperty]
        [FA("Alarm")]
        [DefaultAlarmInfo(30, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        [AlarmDescription(KnownCulture.Korean, "FRONT I-Mark 위치 결정 실패", "X1135 입력, 그리퍼 상태 및 이동 범위를 확인하고 초기화하십시오.")]
        [AlarmInfo(ConfigClasses.GlobalConst.ALARM_TYPE_METHOD, ConfigClasses.GlobalConst.WARNING, "FRONT I-Mark X1135 positioning failed")]
        public int AlarmFrontIMarkCheckTimeOut { get; set; }

        public override void LoadParameters(System.Xml.Linq.XElement xml)
        {
            base.LoadParameters(xml);
            if (xml?.Element("Time")?.Elements("Item").Any(item => (string)item.Element("Name") == "TimeFrontIMarkTimeout") != true)
            {
                TimeFrontIMarkTimeout.Type = FATimeType.second;
                TimeFrontIMarkTimeout.TypeValue = 1;
            }
        }

        private bool _useFrontIMark;
        [FA("Jobs")]
        public bool UseFrontIMark
        {
            get { return _useFrontIMark; }
            set
            {
                if (_useFrontIMark == value) return;
                _useFrontIMark = value;
                NotifyPropertyChanged("UseFrontIMark");
                WriteIMarkLog("Event=USE_FRONT_IMARK_CHANGED;UseFrontIMark=" + value);
            }
        }

        private double _frontIMarkSensorOffset;
        [FA("Jobs")]
        public double FrontIMarkSensorOffset
        {
            get { return _frontIMarkSensorOffset; }
            set
            {
                if (!IsFinite(value) || value < 0) throw new ArgumentOutOfRangeException("value");
                if (_frontIMarkSensorOffset == value) return;
                _frontIMarkSensorOffset = value;
                NotifyPropertyChanged("FrontIMarkSensorOffset");
                WriteIMarkLog("Event=OFFSET_CHANGED;Offset=" + value.ToString(CultureInfo.InvariantCulture));
            }
        }

        private static bool IsFinite(double value) { return !double.IsNaN(value) && !double.IsInfinity(value); }

        // JOB targets belong to the reciprocating FRONT gripper, never to the REAR roller.
        public void ApplyFrontIMarkJob(MoveJobInfo job, string jobName = null)
        {
            lock (IMarkSettingsSync)
            {
                ApplyFrontIMarkJobCore(job, jobName);
                var pending = _pendingIMarkUsage;
                if (pending != null && (pending.JobName != jobName || SameFrontSettings(pending.Settings, job)))
                {
                    WriteIMarkLog("Event=" + (pending.JobName == jobName ? "MAIN_USAGE_REQUEST_APPLIED" : "MAIN_USAGE_REQUEST_CANCELLED") +
                        ";RequestId=" + pending.RequestId + ";Job=" + jobName + ";UseFrontIMark=" + UseFrontIMark + ";Boundary=RecipeLoad");
                    _pendingIMarkUsage = null;
                }
            }
        }

        public void ValidateFrontIMarkJob(MoveJobInfo job)
        {
            if (job == null || TapeLoadingServo?.TapeLoadingPos == null || TapeLoadingServo.TapeLoadingSlowPos == null)
                throw new InvalidOperationException("FRONT I-Mark motor positions are not configured.");
            double pitch = job.FeedingPitch;
            double slowDistance = job.FrontIMarkSlowDistance;
            if (!IsFinite(pitch) || pitch < 0 || !IsFinite(slowDistance) || slowDistance < 0 ||
                !IsFinite(job.FeedingSpeed) || job.FeedingSpeed < 0 ||
                !IsFinite(job.FrontIMarkSensorOffset) || job.FrontIMarkSensorOffset < 0 ||
                (job.UseFrontIMark && (pitch <= 0 || job.FeedingSpeed <= 0)))
                throw new ArgumentOutOfRangeException("job", "Invalid FRONT feeding pitch, speed, slow distance or offset.");
            double fastTarget = job.UseFrontIMark ? Math.Max(0, pitch - slowDistance) : pitch;
            if (job.UseFrontIMark &&
                (fastTarget < TapeLoadingServo.TapeLoadingPos.LwLimit || fastTarget > TapeLoadingServo.TapeLoadingPos.UpLimit ||
                 pitch < TapeLoadingServo.TapeLoadingSlowPos.LwLimit || pitch > TapeLoadingServo.TapeLoadingSlowPos.UpLimit ||
                 !IsFinite(TapeLoadingServo.TapeLoadingSlowPos.DriveSpeed) || TapeLoadingServo.TapeLoadingSlowPos.DriveSpeed <= 0))
                throw new ArgumentOutOfRangeException("job", "FRONT I-Mark position limits or slow speed are invalid.");
            var feedingSpeed = Convert.ToUInt32(job.FeedingSpeed);
            if (job.UseFrontIMark && feedingSpeed == 0)
                throw new ArgumentOutOfRangeException("job", "FRONT feeding speed rounds to zero.");
        }

        private void ApplyFrontIMarkJobCore(MoveJobInfo job, string jobName)
        {
            ValidateFrontIMarkJob(job);
            double pitch = job.FeedingPitch;
            double slowDistance = job.FrontIMarkSlowDistance;
            double fastTarget = job.UseFrontIMark ? Math.Max(0, pitch - slowDistance) : pitch;
            var feedingSpeed = Convert.ToUInt32(job.FeedingSpeed);
            TapeLoadingServo.TapeLoadingPos.Position = fastTarget;
            TapeLoadingServo.TapeLoadingPos.DriveSpeed = feedingSpeed;
            TapeLoadingServo.TapeLoadingSlowPos.Position = pitch;
            FrontIMarkSensorOffset = job.FrontIMarkSensorOffset;
            UseFrontIMark = job.UseFrontIMark;
            AppliedFrontIMarkJobName = jobName;
            WriteIMarkLog(string.Format(CultureInfo.InvariantCulture,
                "Event=FRONT_JOB_APPLIED;Pitch={0};SlowDistance={1};FastTarget={2};SlowTarget={0};Offset={3};UseFrontIMark={4};Axis={5};Job={6};FeedingSpeed={7}",
                pitch, slowDistance, fastTarget, FrontIMarkSensorOffset, UseFrontIMark, TapeLoadingServo.AxisNo, jobName, feedingSpeed));
        }

        private bool IsIMarkInputOn() { return FrontIMarkCheckSensor != null && FrontIMarkCheckSensor.IsOn; }
        private bool IsIMarkInputOff() { return FrontIMarkCheckSensor != null && FrontIMarkCheckSensor.IsOff; }
        private string GetIMarkInputStatus() { return FrontIMarkCheckSensor == null ? "NULL" : FrontIMarkCheckSensor.Status.ToString(); }
        private bool AreIMarkGrippersReady()
        {
            return TapeLoadGrip != null && TapeHoldGrip != null &&
                TapeLoadGrip.Status == TapeLoadGrip.StatusList.Grip &&
                TapeHoldGrip.Status == TapeHoldGrip.StatusList.Release;
        }

        private void WriteIMarkLog(string message)
        {
            if (Equipment == null) return;
            try
            {
                Manager.LogManager.Instance.WriteIMarkLog(Equipment,
                    "[" + Name + "]\tFlow=FrontGripperX1135V1;CycleId=" + _frontIMarkCycleId + ";" + message);
            }
            catch (Exception e) { System.Diagnostics.Trace.WriteLine(e); }
        }

        private IMarkSequenceDiagnostics CreateIMarkDiagnostics(FASequence seq)
        {
            return new IMarkSequenceDiagnostics(seq, GetIMarkSnapshot, GetIMarkConfiguration,
                GetIMarkInputStatus, GetIMarkMotionStatus, WriteIMarkLog);
        }

        private string GetIMarkMotionStatus()
        {
            var servo = TapeLoadingServo;
            return string.Format("Run:{0},Done:{1},Alarm:{2},LoadGrip:{3},HoldGrip:{4}",
                servo?.RunFlag, servo?.MotionDone, servo?.ServoAlarm, TapeLoadGrip?.Status, TapeHoldGrip?.Status)
                + ";LoadIO=" + GetGripIO(TapeLoadGrip) + ";HoldIO=" + GetGripIO(TapeHoldGrip);
        }

        private static string GetGripIO(FAPartGripRelease grip)
        {
            if (grip == null) return "NULL";
            return "In[" + string.Join(",", grip.InputIO.Select(io => io.Name + ":" + io.Value + "/" + io.CorrectionValue)) +
                "]Out[" + string.Join(",", grip.OutputIO.Select(io => io.Name + ":" + io.Value + "/" + io.CorrectionValue)) + "]";
        }

        private string GetIMarkSnapshot()
        {
            var servo = TapeLoadingServo;
            var input = FrontIMarkCheckSensor?.InputIO;
            var io = input != null && input.Count > 0 ? input[0] : null;
            return string.Format(CultureInfo.InvariantCulture,
                "ExpectedIO=X1135;Sensor={0};UseFrontIMark={1};Axis={2};ActualPos={3};CommandPos={4};RunFlag={5};MotionDone={6};ServoOn={7};ServoAlarm={8};InputName={9};RawInput={10};CorrectedInput={11};IOInverse={12};InputInverse={13};Offset={14};OnHandled={15};RecoveryRequired={16};LoadGrip={17};HoldGrip={18};Job={19};EquipmentState={20};OffsetTarget={21};FastState={22};SlowState={23};OffsetState={24};OffsetError={25};LoadGripState={26};HoldGripState={27}",
                GetIMarkInputStatus(), UseFrontIMark, servo?.AxisNo, servo?.ActualPos, servo?.CommandPos,
                servo?.RunFlag, servo?.MotionDone, servo?.ServoOn, servo?.ServoAlarm,
                io?.Name, io?.Value, io?.CorrectionValue, io?.IsInverse, FrontIMarkCheckSensor?.InputInverse,
                FrontIMarkSensorOffset, _iMarkSensorOnHandled, _iMarkPositioningRecoveryRequired,
                TapeLoadGrip?.Status, TapeHoldGrip?.Status, (Equipment as SubEquipment)?.MainLoopModule?.SelectJob,
                Equipment?.State?.GetType().Name, servo?.TargetPosition?.Position,
                servo?.MoveTapeLoadingPos?.Sequence?.State, servo?.MoveTapeLoadingSlowPos?.Sequence?.State,
                servo?.MoveToPos?.Sequence?.State, servo?.MoveToPos?.Sequence?.LastErrorMessage,
                TapeLoadGrip?.Grip?.Sequence?.State, TapeHoldGrip?.Grip?.Sequence?.State)
                + ";LoadGripIO=" + GetGripIO(TapeLoadGrip) + ";HoldGripIO=" + GetGripIO(TapeHoldGrip)
                + ";LoadReleaseState=" + TapeLoadGrip?.Release?.Sequence?.State
                + ";HoldReleaseState=" + TapeHoldGrip?.Release?.Sequence?.State
                + ";HomeState=" + servo?.MoveHomePos?.Sequence?.State
                + ";PendingUse=" + PendingFrontIMarkUse;
        }

        private string GetIMarkConfiguration()
        {
            var servo = TapeLoadingServo;
            return string.Format(CultureInfo.InvariantCulture,
                "TimeoutMs={0};FastPosition={1};FastSpeed={2};SlowPosition={3};SlowSpeed={4};FastAccel={5};FastDecel={6};SlowAccel={7};SlowDecel={8};SpeedRate={9};ServoSimulation={10};SensorSimulation={11}",
                TimeFrontIMarkTimeout?.Time.TotalMilliseconds, servo?.TapeLoadingPos?.Position,
                servo?.TapeLoadingPos?.DriveSpeed, servo?.TapeLoadingSlowPos?.Position, servo?.TapeLoadingSlowPos?.DriveSpeed,
                servo?.TapeLoadingPos?.AccelTime, servo?.TapeLoadingPos?.DecelTime,
                servo?.TapeLoadingSlowPos?.AccelTime, servo?.TapeLoadingSlowPos?.DecelTime,
                servo?.SpeedRate, servo?.SimulationMode, FrontIMarkCheckSensor?.SimulationMode);
        }

        private void ResetFrontIMarkAfterInitialize()
        {
            _iMarkSensorOnHandled = IsIMarkInputOn();
            _iMarkPositioningRecoveryRequired = false;
            WriteIMarkLog("Event=INITIAL_POSITION_ESTABLISHED;" + GetIMarkSnapshot());
        }

        private void MakeTapeLoadingMoveWithIMark()
        {
            var seq = TapeLoadingMoveWithIMark;
            double feedStartPosition = 0;
            seq.OnStart += delegate
            {
                _frontIMarkCycleId = Guid.NewGuid().ToString("N");
                LastFrontIMarkFeedDistance = 0;
                feedStartPosition = TapeLoadingServo.ActualPos;
            };
            seq.OnTerminate += delegate
            {
                LastFrontIMarkFeedDistance = Math.Max(0, TapeLoadingServo.ActualPos - feedStartPosition);
                WriteIMarkLog(string.Format(CultureInfo.InvariantCulture,
                    "Event=FRONT_FEED_DISTANCE;StartPosition={0};EndPosition={1};TravelMm={2}",
                    feedStartPosition, TapeLoadingServo.ActualPos, LastFrontIMarkFeedDistance));
            };
            var log = CreateIMarkDiagnostics(seq);
            seq.AddItem((actor, time) =>
            {
                if (_iMarkPositioningRecoveryRequired)
                {
                    TapeLoadingServo.Stop.Execute(actor);
                    log.Record("POSITIONING_ALARM", "Reason=InitializeRequiredAfterInterruptedPositioning");
                    RaiseAlarm(actor, AlarmFrontIMarkCheckTimeOut, "FRONT I-Mark initialization required.");
                }
                else
                {
                    string error;
                    if (!TryApplyPendingFrontIMarkUsage(true, out error))
                    {
                        if (error != null || time > (TimeFrontIMarkTimeout?.Time ?? TimeSpan.FromSeconds(1)))
                        {
                            log.Record("POSITIONING_ALARM", "Reason=PendingUsageApplyFailed;Detail=" + (error ?? "FrontAxisBusy"));
                            RaiseAlarm(actor, AlarmFrontIMarkCheckTimeOut, "FRONT I-Mark setting application failed: " + (error ?? "Front axis is busy."));
                        }
                        return;
                    }
                    if (UseFrontIMark) actor.NextStep();
                    else
                    {
                        log.Record("BYPASS", "Reason=UseFrontIMarkFalse;Mode=NormalPitch");
                        actor.NextStep("UnUseFrontIMark");
                    }
                }
            });
            AddIMarkPositioning(seq, log, true);
            seq.AddItem("Terminate");
            seq.AddStep("UnUseFrontIMark").StepIndex =
                log.TrackStep(seq.AddItem(TapeLoadingServo.MoveTapeLoadingPos.Sequence), "NORMAL_PITCH_WAIT_COMPLETE");
            seq.AddStep("Terminate").StepIndex = seq.AddTerminate();
        }

        // Parent traces cover cylinder handoff and home return after the positioning child ends.
        private void AttachFrontIMarkCycleDiagnostics(FASequence seq)
        {
            CreateIMarkDiagnostics(seq);
        }

        private void AddIMarkPositioning(FASequence seq, IMarkSequenceDiagnostics log,
            bool includeFastMove, bool applyOffset = true, FATime searchTimeout = null)
        {
            bool active = false;
            bool searching = false;
            bool resumeSearch = false;
            bool fastPhase = false;
            TimeSpan? slowArrivedAt = null;
            TimeSpan slowStartedAt = TimeSpan.Zero;
            var arrivalWait = TimeSpan.FromSeconds(3);
            string lastFastInput = null;
            double detectedPosition = 0;
            double offset = 0;
            string fault = null;

            Action<FASequence, string> fail = (actor, reason) =>
            {
                fault = reason;
                _iMarkPositioningRecoveryRequired = true;
                searching = false;
                fastPhase = false;
                TapeLoadingServo.Stop.Execute(actor);
                log.Record("POSITIONING_FAULT_STOP_REQUESTED", "Reason=" + reason +
                    ";AutomaticRetry=false;DetectionPolicy=SlowLevelOn");
                actor.NextStep("IMarkFault");
            };
            Action<FASequence, string> detect = (actor, basis) =>
            {
                searching = false;
                _iMarkSensorOnHandled = true;
                detectedPosition = TapeLoadingServo.ActualPos;
                TapeLoadingServo.Stop.Execute(actor);
                log.Record("DETECTED_STOP_REQUESTED", string.Format(CultureInfo.InvariantCulture,
                    "DecisionInput=On;DetectedPos={0};AppliedOffset={1};Target={2};DetectionWindow=Slow;DetectionPolicy=SlowLevelOn;DetectionBasis={3}",
                    detectedPosition, offset, detectedPosition + offset, basis));
                actor.NextStep("IMarkStopWait");
            };
            seq.OnPreSuspend += delegate
            {
                if (!active) return;
                TapeLoadingServo.Stop.Execute(seq);
                log.Record("POSITIONING_SUSPEND_STOP_REQUESTED");
            };
            seq.OnResume += delegate { resumeSearch = searching; };
            seq.OnStop += delegate
            {
                if (!active) return;
                TapeLoadingServo.Stop.Execute(seq);
                _iMarkPositioningRecoveryRequired = true;
                active = false;
                log.Record("POSITIONING_ABORT_STOP_REQUESTED", "Recovery=InitializeRequired");
            };
            seq.AddWatcher(() =>
            {
                if (active && fastPhase)
                {
                    var input = GetIMarkInputStatus();
                    if (input != lastFastInput)
                    {
                        log.Record("FAST_INPUT_IGNORED", "Previous=" + (lastFastInput ?? "UNOBSERVED") +
                            ";Observed=" + input + ";DetectionEnabled=false");
                        lastFastInput = input;
                    }
                }
            });

            log.TrackStep(seq.AddItem((actor, time) =>
            {
                active = true;
                searching = false;
                resumeSearch = false;
                fastPhase = includeFastMove;
                slowArrivedAt = null;
                slowStartedAt = TimeSpan.Zero;
                lastFastInput = null;
                fault = null;
                detectedPosition = 0;
                offset = applyOffset ? FrontIMarkSensorOffset : 0;
                var timeout = searchTimeout ?? TimeFrontIMarkTimeout;
                if (_iMarkPositioningRecoveryRequired)
                    fail(actor, "InitializeRequiredAfterInterruptedPositioning");
                else if (!AreIMarkGrippersReady())
                    fail(actor, "GripperInterlock");
                else if (FrontIMarkCheckSensor == null || (!IsIMarkInputOn() && !IsIMarkInputOff()))
                    fail(actor, "InvalidOrMissingInput");
                else if (TapeLoadingServo.TapeLoadingSlowPos == null ||
                    (includeFastMove && TapeLoadingServo.TapeLoadingPos == null) ||
                    (offset > 0 && TapeLoadingServo.TargetPosition == null) ||
                    double.IsNaN(offset) || double.IsInfinity(offset) || offset < 0 ||
                    timeout == null || timeout.Time <= TimeSpan.Zero)
                    fail(actor, "InvalidPositioningConfiguration");
                else
                {
                    _iMarkSensorOnHandled = false;
                    log.Record("POSITIONING_BEGIN", string.Format(CultureInfo.InvariantCulture,
                        "FastMove={0};ApplyOffset={1};AppliedOffset={2};LegacyTimeoutMs={3};AutomaticRetry=false;DetectionPolicy=SlowLevelOn;ArrivalWaitMs=3000;TimeoutOrigin=SlowTargetReached",
                        includeFastMove, applyOffset, offset, timeout.Time.TotalMilliseconds));
                    actor.NextStep();
                }
            }), "IMARK_VALIDATE");
            if (includeFastMove)
                log.TrackStep(seq.AddItem(TapeLoadingServo.MoveTapeLoadingPos.Sequence), "FAST_MOVE_WAIT_COMPLETE");

            log.TrackStep(seq.AddItem((actor, time) =>
            {
                fastPhase = false;
                if (!IsIMarkInputOn() && !IsIMarkInputOff())
                {
                    fail(actor, "InvalidOrMissingInput");
                    return;
                }
                // Enable level detection only after issuing the slow command, never during fast travel.
                var startInput = GetIMarkInputStatus();
                searching = true;
                TapeLoadingServo.MoveTapeLoadingSlowPos.Execute(actor);
                log.Record("SLOW_SEARCH_COMMAND", "StartInput=" + startInput +
                    ";DetectionPolicy=SlowLevelOn;Target=" + TapeLoadingServo.TapeLoadingSlowPos.Position.ToString(CultureInfo.InvariantCulture));
                log.Record("SLOW_DETECTION_ENABLED", "Reason=SlowStart;HighSpeedInputIgnored=true;RequiresNewEdge=false");
                actor.NextStep();
            }), "SLOW_SEARCH_ARM");
            log.TrackStep(seq.AddItem((actor, time) =>
            {
                if (!IsIMarkInputOn() && !IsIMarkInputOff())
                {
                    fail(actor, "InvalidOrMissingInput");
                    return;
                }
                if (resumeSearch)
                {
                    // A signal received while suspended is not a moving-search detection.
                    resumeSearch = false;
                    slowArrivedAt = null;
                    slowStartedAt = time;
                    var startInput = GetIMarkInputStatus();
                    TapeLoadingServo.MoveTapeLoadingSlowPos.Execute(actor);
                    log.Record("SLOW_SEARCH_RESUMED", "StartInput=" + startInput + ";DetectionPolicy=SlowLevelOn");
                    log.Record("SLOW_DETECTION_ENABLED", "Reason=Resume;RequiresNewEdge=false");
                    return;
                }
                if (IsIMarkInputOn())
                {
                    detect(actor, slowArrivedAt.HasValue ? "OnAfterSlowArrival" : "OnInSlowWindow");
                    return;
                }
                // The mark wait starts at confirmed arrival, not at the slow command.
                bool atSlowTarget = TapeLoadingServo.MotionDone && !TapeLoadingServo.RunFlag &&
                    TapeLoadingServo.IsInPosition(TapeLoadingServo.TapeLoadingSlowPos);
                if (atSlowTarget && !slowArrivedAt.HasValue)
                {
                    slowArrivedAt = time;
                    log.Record("SLOW_TARGET_REACHED", "ArrivalWaitMs=3000;DetectionContinues=true;AdditionalMotion=false");
                }
                if (slowArrivedAt.HasValue)
                {
                    if (time - slowArrivedAt.Value >= arrivalWait)
                        fail(actor, "SearchTimeout");
                }
                else if (time - slowStartedAt > (TapeLoadingServo.MoveToPosTimeout?.Time ?? TimeSpan.FromSeconds(30)))
                    fail(actor, "SlowMoveTimeout");
            }), "WAIT_IMARK");

            seq.AddStep("IMarkStopWait").StepIndex = log.TrackStep(
                seq.AddItem(TapeLoadingServo.Stop.Sequence), "WAIT_STOP_COMPLETE");
            log.TrackStep(seq.AddItem((actor, time) =>
            {
                log.Record("STOP_CONFIRMED", "DetectedPos=" + detectedPosition.ToString(CultureInfo.InvariantCulture));
                if (offset == 0)
                {
                    log.Record("OFFSET_SKIPPED", "Reason=ZeroOffset");
                    actor.NextStep("IMarkDone");
                    return;
                }
                double target = detectedPosition + offset;
                if (double.IsNaN(target) || double.IsInfinity(target))
                    fail(actor, "InvalidOffsetTarget");
                else if (target < TapeLoadingServo.TapeLoadingSlowPos.LwLimit ||
                    target > TapeLoadingServo.TapeLoadingSlowPos.UpLimit)
                    fail(actor, "OffsetTargetOutsidePositionLimits");
                else if (target < TapeLoadingServo.ActualPos - Math.Abs(TapeLoadingServo.Tolerance))
                    fail(actor, "StopOverrunBeyondOffsetTarget");
                else
                {
                    TapeLoadingServo.TapeLoadingSlowPos.CopyTo(TapeLoadingServo.TargetPosition);
                    TapeLoadingServo.TargetPosition.Position = target;
                    log.Record("OFFSET_MOVE_PREPARED", string.Format(CultureInfo.InvariantCulture,
                        "DetectedPos={0};AppliedOffset={1};Target={2};Speed={3};Accel={4};Decel={5}",
                        detectedPosition, offset, target, TapeLoadingServo.TargetPosition.DriveSpeed,
                        TapeLoadingServo.TargetPosition.AccelTime, TapeLoadingServo.TargetPosition.DecelTime));
                    actor.NextStep();
                }
            }), "PREPARE_OFFSET");
            log.TrackStep(seq.AddItem(TapeLoadingServo.MoveToPos.Sequence), "OFFSET_MOVE_WAIT_COMPLETE");
            seq.AddItem((object o) => log.Record("OFFSET_MOVE_COMPLETED", string.Format(CultureInfo.InvariantCulture,
                "DetectedPos={0};AppliedOffset={1};Target={2};ActualPos={3}",
                detectedPosition, offset, detectedPosition + offset, TapeLoadingServo.ActualPos)));
            seq.AddItem("IMarkDone");

            seq.AddStep("IMarkFault").StepIndex = log.TrackStep(seq.AddItem((actor, time) =>
            {
                TapeLoadingServo.Stop.Execute(actor);
                log.Record("POSITIONING_ALARM", "Reason=" + fault + ";AlarmId=" + AlarmFrontIMarkCheckTimeOut + ";Recovery=InitializeRequired");
                RaiseAlarm(actor, AlarmFrontIMarkCheckTimeOut, "I-Mark X1135: " + fault);
            }), "IMARK_FAULT_REQUIRES_RESTART");
            seq.AddStep("IMarkDone").StepIndex = seq.AddItem((object o) =>
            {
                active = false;
                searching = false;
                log.Record("POSITIONING_COMPLETED");
            });
        }
    }
}
