using FAFramework.Utility;
using FALibrary;
using FALibrary.Part.MemoryBasePart;
using FALibrary.Sequence;
using FALibrary.Utility;
using System;

namespace FAFramework.Module
{
    public class FAStackerModule : FAModule, StackerPart
    {
        #region Sequences
        [FAAttribute("Sequences")]
        public FASequence Initialize { get; set; }
        [FAAttribute("Sequences")]
        public FASequence MoveToTopPos { get; set; }
        [FAAttribute("Sequences")]
        public FASequence MoveToBottomPos { get; set; }
        [FAAttribute("Sequences")]
        /// <summary>
        /// 설비가 정지된 상태에서 호출할 수 있도록 만든 Sequence.
        /// </summary>
        public FASequence ManualMoveToBottomPos { get; set; }
        [FAAttribute("Sequences")]
        public FASequence Process { get; set; }
        #endregion

        #region Field
        private object _lockOwner = null;
        #endregion

        #region Time
        [FAAttribute("Time")]
        public FATime TimeUpDownMotorMoveTimeout { get; set; }
        /// <summary>
        /// Charging 상태에서 이 시간동안 도어를 열지 않으면 알람이 발생한다.
        /// </summary>
        [FAAttribute("Time")]
        public FATime TimeDoorOpenTimeout { get; set; }
        /// <summary>
        /// Charging 상태에서 이 시간동안 도어를 닫지 않으면 알람이 발생한다.
        /// </summary>
        [FAAttribute("Time")]
        public FATime TimeDoorCloseTimeout { get; set; }
        [FAAttribute("Time")]
        public FATime TimeDoorLockTimeout { get; set; }
        #endregion

        #region RetryInfo
        [FAAttribute("RetryInfo")]
        public FARetryInfo RetryInfoStackerUpRetry { get; set; }
        #endregion

        #region Alarm
        [DefaultAlarmInfo(1, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        [AlarmDescription(KnownCulture.EnglishUS, "Stacker down timeout", "Check stacker motor and sensor")]
        [AlarmDescription(KnownCulture.Korean, "스태커 하강 시간초과.", "스태커 모터, 센서를 확인하세요.")]
        public int AlarmStackerDownTimeout { get; set; }

        [DefaultAlarmInfo(2, Utility.Alarm.EAlarmType.METHOD, Utility.Alarm.EAlarmStatus.WARNING)]
        [AlarmDescription(KnownCulture.EnglishUS, "Stacker already doing something else", "Check stacker status")]
        [AlarmDescription(KnownCulture.Korean, "이미 다른 동작 중입니다.", "스태커 상태를 확인하세요.")]
        public int AlarmAlreadyDoingSomethingElse { get; set; }

        [DefaultAlarmInfo(3, Utility.Alarm.EAlarmType.METHOD, Utility.Alarm.EAlarmStatus.WARNING)]
        [AlarmDescription(KnownCulture.EnglishUS, "Can not move stacker", "Check stacker status")]
        [AlarmDescription(KnownCulture.Korean, "스태커를 동작할 수 없습니다.", "스태커 상태를 확인하세요.")]
        public int AlarmCannotMoveStacker { get; set; }

        [DefaultAlarmInfo(4, Utility.Alarm.EAlarmType.MATERIAL, Utility.Alarm.EAlarmStatus.WARNING)]
        [AlarmDescription(KnownCulture.EnglishUS, "Shortage stacker", "Check stacker status")]
        [AlarmDescription(KnownCulture.Korean, "스태커 자재 부족", "스태커 상태를 확인하세요.")]
        public int AlarmStackerShortageChargingPlease { get; set; }

        [DefaultAlarmInfo(5, Utility.Alarm.EAlarmType.METHOD, Utility.Alarm.EAlarmStatus.WARNING)]
        [AlarmDescription(KnownCulture.EnglishUS, "Stacker is not closed", "Close stacker")]
        [AlarmDescription(KnownCulture.Korean, "스태커가 닫혀 있지 않습니다.", "스태커를 닫아주세요.")]
        public int AlarmStackerIsNotClosed { get; set; }

        [DefaultAlarmInfo(6, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.WARNING)]
        [AlarmDescription(KnownCulture.EnglishUS, "Stacker lock fail", "Check stacker status")]
        [AlarmDescription(KnownCulture.Korean, "스태커 잠금 실패.", "스태커 상태를 확인하세요.")]
        public int AlarmStackerLockFail { get; set; }
        #endregion

        #region Status
        [FAAttribute("Status")]
        [DefaultValue(false)]
        public bool ExistProductAtTopPos { get; set; }

        [FAAttribute("Status")]
        [DefaultValue(false)]
        public bool ChargingReqeust { get; set; }

        [FAAttribute("Status")]
        [DefaultValue(false)]
        public bool ChargingStandby { get; set; }

        [FAAttribute("Status")]
        [DefaultValue(false)]
        public bool ChargingCompleted { get; set; }

        [FAAttribute("Status")]
        [DefaultValue(false)]
        public bool FeedingRequest { get; set; }

        [FAAttribute("Status")]
        [DefaultValue(false)]
        public bool FeedingStandby { get; set; }

        [FAAttribute("Status")]
        [DefaultValue(false)]
        public bool FeedingCompleted { get; set; }

        [FAAttribute("Status")]
        [DefaultValue(0)]
        public int TopPosStackerNo { get; set; }
        #endregion

        #region Parameters
        public int StackersCount { get; set; }
        #endregion

        #region Parts
        public FAPartOnOffSensor[] TopSensors { get; private set; }
        public FAPartUpDownACMotor UpDownMotor { get; set; }
        public FAPartOnOffSensor MotorUpLimit { get; set; }
        public FAPartOnOffSensor MotorDownLimit { get; set; }
        public FAPartOnOffSensor MotorEmptyWarning { get; set; }

        public FAPartLockRelease StackerLock { get; set; }
        public FAPartOnOffSensor StackerCloseCheck { get; set; }
        #endregion

        #region Interface Method
        public bool Lockable(object owner)
        {
            if (owner == null) return false;
            if (_lockOwner != null) return false;

            return true;
        }

        public bool Lock(object owner)
        {
            if (owner == null) return false;
            if (_lockOwner != null)
            {
                if (_lockOwner == owner) return true;
                else return false;
            }

            _lockOwner = owner;
            return true;
        }

        public bool Release(object owner)
        {
            if (owner == null) return false;
            if (owner == _lockOwner)
            {
                _lockOwner = null;
                return true;
            }
            else
                return false;
        }

        public bool IsFeedingStandby()
        {
            return !FeedingRequest &&
                FeedingStandby &&
                !FeedingCompleted;
        }

        public bool ExistObjectOnTopPos()
        {
            return ExistProductAtTopPos;
        }

        public void RequestFeeding()
        {
            FeedingRequest = true;
        }

        public bool IsChargingStandby()
        {
            return ChargingStandby &&
                !ChargingReqeust &&
                !ChargingCompleted;
        }

        public void RequestCharging()
        {
            if (!ChargingReqeust)
                ChargingReqeust = true;
        }

        public void SetTopSensor(params FAPartOnOffSensor[] sensors)
        {
            TopSensors = new FAPartOnOffSensor[StackersCount];
            for (int i = 0; i < sensors.Length; i++)
            {
                if (i >= StackersCount)
                    return;

                TopSensors[i] = sensors[i];
            }
        }
        #endregion

        #region Override Method
        public override void InitializeSequence()
        {
            MakeInitialize();
            MakeMoveToTopPos();
            MakeMoveToBottomPos(MoveToBottomPos);
            MakeMoveToBottomPos(ManualMoveToBottomPos);
            MakeProcess();
        }

        public override void ClearProductInfo()
        {
            base.ClearProductInfo();
            _lockOwner = null;
        }

        public override void SetInterlock()
        {
            base.SetInterlock();

            AddPartInterlock(UpDownMotor.UpAction,
                () => StackerCloseCheck.IsOff,
                "Stacker is open");

            AddPartInterlock(UpDownMotor.UpAction,
                () => StackerLock.Status != StackerLock.StatusList.Lock,
                "Stacker is unlock");

            AddPartInterlock(UpDownMotor.DownAction,
                () => StackerCloseCheck.IsOff,
                "Stacker is open");

            AddPartInterlock(UpDownMotor.DownAction,
                () => StackerLock.Status != StackerLock.StatusList.Lock,
                "Stacker is unlock");

            bool upDownMotorInterlock(ref string msg)
            {
                if (StackerCloseCheck.IsOff)
                    ShowMessage("CanNotMoveStacker", AlarmCannotMoveStacker, $"Can not move stakcer. Stacker is open");
                else if (StackerLock.Status != StackerLock.StatusList.Lock)
                    ShowMessage("CanNotMoveStacker", AlarmCannotMoveStacker, $"Can not move stakcer. Stacker is unlock");
                else
                    return false;

                MotorStop();
                return true;
            }

            UpDownMotor.UpAction.Sequence.AddRunningInterlock(
                (seq) =>
                {
                    string interlockMsg = string.Empty;
                    return upDownMotorInterlock(ref interlockMsg);
                });

            UpDownMotor.DownAction.Sequence.AddRunningInterlock(
                (seq) =>
                {
                    string interlockMsg = string.Empty;
                    return upDownMotorInterlock(ref interlockMsg);
                });
        }
        #endregion

        #region General Method
        private void ClearInterfaceFlag()
        {
            this.SetDefaultValueAtProperty();
        }

        private bool IsTopSensorOn(out int firstDetectedSensorNo)
        {
            firstDetectedSensorNo = 0;

            for (int i = 0; i < StackersCount; i++)
            {
                if (TopSensors[i].IsOn)
                {
                    firstDetectedSensorNo = i;
                    return true;
                }
            }

            return false;
        }

        private void MotorUp()
        {
            UpDownMotor.UpAction.Execute(this);
        }

        private void MotorDown()
        {
            UpDownMotor.DownAction.Execute(this);
        }

        private void MotorStop()
        {
            UpDownMotor.StopAction.Execute(this);
        }
        #endregion

        #region Make Sequence
        private void MakeInitialize()
        {
            var seq = Initialize;

            seq.AddItem(
                (actor, time) =>
                {
                    if (StackerCloseCheck.IsOn)
                    {
                        StackerLock.Lock.Execute(actor);
                        actor.NextStep();
                    }
                    else
                    {
                        RaiseAlarm(actor, AlarmStackerIsNotClosed);
                        actor.NextTerminate();
                    }
                });
            seq.AddItem(
                (actor, time) =>
                {
                    if (StackerLock.Status == StackerLock.StatusList.Lock)
                    {
                        actor.NextStep();
                    }
                    else if (StackerLock.LockTimeout.Time < time)
                    {
                        StackerLock.Release.Execute(actor);
                        RaiseAlarm(actor, AlarmStackerLockFail);
                        actor.NextTerminate();
                    }
                });
            seq.AddItem(MoveToBottomPos);
        }

        private void MakeMoveToTopPos()
        {
            var seq = MoveToTopPos;

            #region Event Handler
            seq.AddWatcher(
                delegate
                {
                    if (MotorEmptyWarning.IsOff)
                        CloseMessage("Stacker shortage");
                });
            seq.OnStart +=
                delegate
                {
                    RetryInfoStackerUpRetry.ClearCount();
                    MotorStop();
                };
            seq.OnSuspended +=
                delegate
                {
                    MotorStop();
                };
            seq.OnStop +=
                delegate
                {
                    MotorStop();
                };
            #endregion

            seq.AddItem(
                (actor, time) =>
                {
                    if (IsTopSensorOn(out int topSensorNo))
                        actor.NextStep();
                    else
                        actor.NextStep("ConfirmUpOk");
                });
            seq.AddStep("EscapeFromTopPos").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (MotorDownLimit.IsOff &&
                        !IsTopSensorOn(out int topSensorNo))
                    {
                        MotorStop();
                        WriteTraceLog($"UpDownMotor is off the top pos. stacker no={TopPosStackerNo}");
                        actor.NextStep();
                    }
                    else if (MotorDownLimit.IsOn)
                    {
                        seq.Result = false;
                        MotorStop();
                        actor.NextStep("Terminate");
                    }
                    else if (TimeUpDownMotorMoveTimeout.Time < time)
                    {
                        WriteTraceLog($"Failed escape from the top pos");
                        seq.Result = false;
                        MotorStop();
                        actor.NextStep("Terminate");
                    }
                    else if (!UpDownMotor.IsDownOutputOn())
                    {
                        WriteTraceLog("Start stacker move below the top pos");
                        MotorDown();
                    }
                });
            seq.AddStep("ConfirmUpOk").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (MotorUpLimit.IsOn)
                    {
                        WriteTraceLog("Stacker UpDownMotor up limit checked.");
                        seq.Result = false;
                        actor.NextStep("Terminate");
                    }
                    else if (MotorDownLimit.IsOff &&
                        MotorUpLimit.IsOff &&
                        IsTopSensorOn(out int topSensorNo))
                    {
                        if (MotorEmptyWarning.IsOn)
                            ShowMessage("Stacker shortage", AlarmStackerShortageChargingPlease, "Stacker shortage charging please");
                        else
                            CloseMessage("Stacker shortage");

                        TopPosStackerNo = topSensorNo;
                        MotorStop();
                        WriteTraceLog($"UpDownMotor complete move to the top pos. stacker no={topSensorNo}");
                        actor.NextStep();
                    }
                    else if (TimeUpDownMotorMoveTimeout.Time < time)
                    {
                        MotorStop();
                        if (RetryInfoStackerUpRetry.IncreaseCount())
                        {
                            WriteTraceLog($"Timeout stacker move to the top pos. Stacker move to the top pos retry. {RetryInfoStackerUpRetry.RetryCount}/{RetryInfoStackerUpRetry.RetryLimit}");
                            actor.NextStep("DownStacker");
                        }
                        else
                        {
                            WriteTraceLog("Timeout stacker move to the top pos. UpDownMotor move to the top pos fail.");
                            seq.Result = false;
                            actor.NextStep("Terminate");
                        }
                    }
                    else if (!UpDownMotor.IsUpOutputOn())
                    {
                        WriteTraceLog("Start stacker move to the top pos");
                        MotorUp();
                    }
                });
            seq.AddStep("Terminate").StepIndex = seq.AddTerminate();

            seq.AddStep("DownStacker").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (MotorUpLimit.IsOff &&
                        !IsTopSensorOn(out int firstDetectedSensorNo))
                    {
                        WriteTraceLog("UpDownMotor moved below the tpo pos");
                        MotorStop();
                        actor.NextStep("ConfirmUpOk");
                    }
                    else if (TimeUpDownMotorMoveTimeout.Time < time)
                    {
                        WriteTraceLog("UpDownMotor move below the top pos fail");
                        MotorStop();
                        actor.NextStep("ConfirmUpOk");
                    }
                    else if (!UpDownMotor.IsDownOutputOn())
                    {
                        WriteTraceLog("Start stacker move below the top pos");
                        MotorDown();
                    }
                });
        }

        private void MakeMoveToBottomPos(FASequence seq)
        {
            #region Event Handler
            seq.OnStart +=
                delegate
                {
                    FeedingStandby = false;
                    MotorStop();
                };
            seq.OnSuspended +=
                delegate
                {
                    MotorStop();
                };
            seq.OnStop +=
                delegate
                {
                    MotorStop();
                };
            #endregion

            seq.AddStep("ConfirmDownOk").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (MotorDownLimit.IsOn &&
                        MotorUpLimit.IsOff)
                    {
                        MotorStop();
                        WriteTraceLog("UpDownMotor Down Completed");
                        actor.NextStep();
                    }
                    else if (TimeUpDownMotorMoveTimeout.Time < time)
                    {
                        RaiseAlarm(actor, AlarmStackerDownTimeout,
                            MotorDownLimit.GetInputIOStatus(), MotorUpLimit.GetInputIOStatus());
                    }
                    else if (!UpDownMotor.IsDownOutputOn())
                    {
                        WriteTraceLog("Start stacker move to the bottom pos");
                        MotorDown();
                    }
                });
        }

        private void MakeProcess()
        {
            var seq = Process;

            seq.AddStep("Start").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (Lock(this))
                        actor.NextStep();
                });
            seq.AddStep("MoveToTopPos").StepIndex = seq.AddItem(MoveToTopPos);
            seq.AddStep("ConfirmMoveToTopPosResult").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (MoveToTopPos.Result)
                    {
                        Release(this);
                        ExistProductAtTopPos = true;
                        actor.NextStep();
                    }
                    else
                    {
                        if (Lock(this))
                        {
                            actor.NextStep("MoveToBottomPos");
                        }
                    }
                });
            seq.AddStep("ConfirmRequestSignals").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (ChargingReqeust)
                    {
                        if (Lock(this))
                        {
                            WriteTraceLog("Charging request");
                            actor.NextStep("MoveToBottomPos");
                        }
                    }
                    else if (FeedingRequest)
                    {
                        WriteTraceLog("Feeding Request On");
                        FeedingRequest = false;
                        FeedingStandby = true;
                        actor.NextStep();
                    }
                });
            seq.AddStep("ConfirmFeedingComplete").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (FeedingRequest)
                    {
                        // MoveToTopPos을 재 요청한다.
                        ExistProductAtTopPos = false;
                        WriteTraceLog("Feeding Re-Request On");
                        actor.NextStep();
                    }
                    else if (FeedingCompleted)
                    {
                        Release(this);
                        ExistProductAtTopPos = false;
                        WriteTraceLog("Feeding completed");
                        ClearInterfaceFlag();
                        actor.NextStep();
                    }
                });
            seq.AddItem("MoveToTopPos");

            seq.AddStep("MoveToBottomPos").StepIndex = seq.AddItem(MoveToBottomPos);
            seq.AddStep("SetChargingStandby").StepIndex = seq.AddItem(
                (o) =>
                {
                    WriteTraceLog("Charging Standby");
                    ChargingStandby = true;
                });
            seq.AddStep("ConfirmChargingCompleted").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (ChargingCompleted)
                    {
                        WriteTraceLog("Charging completed");
                        ChargingStandby = false;
                        ChargingCompleted = false;
                        Release(this);
                        actor.NextStep();
                    }
                });
            seq.AddItem("MoveToTopPos");
        }
        #endregion
    }
}
