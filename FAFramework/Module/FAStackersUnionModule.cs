using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FALibrary.Sequence;
using FALibrary;
using FALibrary.Utility;
using FAFramework.Utility;
using FALibrary.Part.MemoryBasePart;

namespace FAFramework.Module
{
    public class FAStackersUnionModule : FAModule
    {
        public StackerPart StackerModule1 { get; set; }
        public StackerPart StackerModule2 { get; set; }

        #region Sequences
        [FAAttribute("Sequences")]
        public FASequence Initialize { get; set; }
        [FAAttribute("Sequences")]
        public FASequence Process { get; set; }
        [FAAttribute("Sequences")]
        public FASequence ObserveStackersStatus { get; set; }
        [FAAttribute("Sequences")]
        public FASequence Charging { get; set; }
        [FAAttribute("Sequences")]
        public FASequence ManualCharging { get; set; }
        [FAAttribute("Sequences")]
        public FASequence FlickLamp { get; set; }
        #endregion

        #region Field
        #endregion

        #region Time
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
        [FAAttribute("Time")]
        public FATime TimeLampFlickInterval { get; set; }

        /// <summary>
        /// Door Open/Close 상태가 이 시간동안 유지되어야 인정된다.
        /// </summary>
        [FAAttribute("Time")]
        public FATime TimeDoorOpenSignalStabilization { get; set; }
        #endregion

        #region RetryInfo
        #endregion

        #region Alarm
        [DefaultAlarmInfo(1, Utility.Alarm.EAlarmType.METHOD, Utility.Alarm.EAlarmStatus.WARNING)]
        [AlarmDescription(KnownCulture.EnglishUS, "Stacker already doing something else", "Check stacker status")]
        [AlarmDescription(KnownCulture.Korean, "이미 다른 동작 중입니다.", "스태커 상태를 확인하세요.")]
        public int AlarmAlreadyDoingSomethingElse { get; set; }

        [DefaultAlarmInfo(2, Utility.Alarm.EAlarmType.METHOD, Utility.Alarm.EAlarmStatus.WARNING)]
        [AlarmDescription(KnownCulture.EnglishUS, "Stacker already charging", "Check stacker status")]
        [AlarmDescription(KnownCulture.Korean, "이미 보충 동작 중입니다.", "스태커 상태를 확인하세요.")]
        public int AlarmAlreadyCharging { get; set; }

        [DefaultAlarmInfo(3, Utility.Alarm.EAlarmType.METHOD, Utility.Alarm.EAlarmStatus.WARNING)]
        [AlarmDescription(KnownCulture.EnglishUS, "Stacker charging please", "Supply metarial in stacker")]
        [AlarmDescription(KnownCulture.Korean, "이미 다른 동작 중입니다.", "스태커에 자재를 보충하세요.")]
        public int AlarmChargingPlease { get; set; }

        [DefaultAlarmInfo(4, Utility.Alarm.EAlarmType.METHOD, Utility.Alarm.EAlarmStatus.WARNING)]
        [AlarmDescription(KnownCulture.EnglishUS, "Stacker door is not closed", "Close stacker door")]
        [AlarmDescription(KnownCulture.Korean, "스태커 도어가 닫혀 있지 않습니다.", "스태를 도어를 닫아 주세요.")]
        public int AlarmCloseDoorPlease { get; set; }

        [DefaultAlarmInfo(5, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.WARNING)]
        [AlarmDescription(KnownCulture.EnglishUS, "Stacker door lock fail", "Check stacker status")]
        [AlarmDescription(KnownCulture.Korean, "스태커 도어 잠금 실패.", "스태커 상태를 확인하세요.")]
        public int AlarmDoorLockFail { get; set; }

        [DefaultAlarmInfo(6, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.WARNING)]
        [AlarmDescription(KnownCulture.EnglishUS, "Stacker lock fail", "Check stacker status")]
        [AlarmDescription(KnownCulture.Korean, "스태커 잠금 실패.", "스태커 상태를 확인하세요.")]
        public int AlarmStackerLockFail { get; set; }

        [DefaultAlarmInfo(7, Utility.Alarm.EAlarmType.METHOD, Utility.Alarm.EAlarmStatus.WARNING)]
        [AlarmDescription(KnownCulture.EnglishUS, "Stacker is not closed", "Close stacker")]
        [AlarmDescription(KnownCulture.Korean, "스태커가 닫혀 있지 않습니다.", "스태를 닫아 주세요.")]
        public int AlarmStackerClosePlease { get; set; }
        #endregion

        #region Status
        [FAAttribute("Status")]
        [DefaultValue(false)]
        public bool DoorUnlockRequest { get; set; }
        #endregion

        #region Parameters
        #endregion

        #region Parts
        public FAPartLockRelease StackerLock { get; set; }
        public FAPartOnOffSensor StackerCloseCheck { get; set; }
        public FAPartDoor Door { get; set; }
        public FAPartOnOffSensor DoorOpenSwitch { get; set; }
        public FAPartOnOff DoorOpenSwitchLamp { get; set; }
        #endregion

        #region Interface Method
        #endregion

        #region Override Method
        public override void InitializeSequence()
        {
            Equipment.StateStop.OnChangedState +=
                delegate
                {
                    if (DoorUnlockRequest)
                    {
                        DoorUnlockRequest = false;
                        Door.Unlock();
                        StackerLock.Release.Execute(this);
                    }
                };

            MakeInitialize();
            MakeProcess();
            MakeObserveStackersStatus();
            MakeCharging(Charging, null, false);
            MakeCharging(ManualCharging,
                () =>
                {
                    if (Equipment.State == Equipment.StateStop &&
                        ChargingRequest() &&
                        ManualCharging.IsStartable())
                    {
                        if (Charging.IsStartable())
                        {
                            if (StackerModule1.Lockable(this) &&
                                StackerModule2.Lockable(this))
                            {
                                ManualCharging.Start();
                                CloseMessage("AlreadyDoingSomethingElse");
                            }
                            else
                            {
                                ShowMessage("AlreadyDoingSomethingElse", AlarmAlreadyDoingSomethingElse, "Alarm Already Doing Something Else");
                            }
                        }
                        else
                        {
                            ShowMessage("AlreadyDoingSomethingElse", AlarmAlreadyCharging, "Already charging");
                        }
                    }
                },
                true);
            MakeFlickLamp();
        }
        #endregion

        #region General Method
        private bool ChargingRequest()
        {
            return DoorOpenSwitch.IsOn;
        }
        #endregion

        #region Make Sequence
        private void MakeInitialize()
        {
            var seq = Initialize;

            seq.AddItem(StackerModule1.Initialize,
                StackerModule2.Initialize);
        }

        private void MakeProcess()
        {
            var seq = Process;

            seq.AddStep("Process").StepIndex = seq.AddItem(ObserveStackersStatus,
                StackerModule1.Process,
                StackerModule2.Process);
        }

        private void MakeObserveStackersStatus()
        {
            var seq = ObserveStackersStatus;

            #region Event Handler
            seq.OnStart +=
                delegate
                {
                    //seq.Atomic = false;
                };

            Equipment.StatePreStop.CustomActions.Add(
                delegate
                {
                    if (//!seq.Atomic &&
                        seq.State != SequenceState.Suspended)
                    {
                        seq.Suspend();
                    }
                });
            #endregion

            seq.AddStep("ConfirmChargingReqeust").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (ChargingRequest())
                    {
                        if (StackerModule1.Lockable(this) &&
                            StackerModule2.Lockable(this))
                        {
                            StackerModule1.RequestCharging();
                            StackerModule2.RequestCharging();
                            //seq.Atomic = true;
                            actor.NextStep();
                        }
                        else
                        {
                            ShowMessage("AlreadyDoingSomethingElse",
                                AlarmAlreadyDoingSomethingElse,
                                "Alarm Already Doing Something Else");
                        }
                    }
                    else
                    {
                        if (StackerModule1.IsChargingStandby() &&
                            StackerModule2.IsChargingStandby())
                        {
                            //seq.Atomic = true;
                            WriteTraceLog("Stackers All empty");
                            actor.NextStep("Charging");
                        }
                    }
                });
            seq.AddStep("ConfirmChargingStandby").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (StackerModule1.IsChargingStandby() &&
                        StackerModule2.IsChargingStandby())
                    {
                        actor.NextStep();
                    }
                });
            seq.AddStep("Charging").StepIndex = seq.AddItem(Charging);
            //seq.AddItem((o) => seq.Atomic = false);
            seq.AddItem(
                (o) =>
                {
                    StackerModule1.Release(this);
                    StackerModule2.Release(this);
                });
            seq.AddItem("ConfirmChargingReqeust");
        }

        private void MakeCharging(FASequence seq, Action watcher, bool doorOpenCheck)
        {
            #region Event Handler
            if (watcher != null)
                seq.AddWatcher(watcher);
            seq.OnStart +=
                delegate
                {
                    DoorUnlockRequest = false;
                };
            #endregion

            seq.AddStep("ConfirmPreDoorClose").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (!Door.Closed)
                    {
                        ShowMessage("CloseDoorPlease", AlarmCloseDoorPlease, "Door Close Please");
                    }
                    else if (StackerCloseCheck.IsOff)
                    {
                        ShowMessage("CloseDoorPlease", AlarmStackerClosePlease, "Stacker Close Please");
                    }
                    else
                    {
                        WriteTraceLog("Pre Door Close Check OK");
                        Door.Lock();
                        StackerLock.Lock.Execute(this);
                        actor.NextStep();
                    }
                });
            seq.AddStep("ConfirmPreDoorLock").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (Door.Locked == DoorLockStatus.Lock &&
                        StackerLock.Status == StackerLock.StatusList.Lock)
                    {
                        WriteTraceLog("Pre Door Lock OK");
                        actor.NextStep();
                    }
                    else if (TimeDoorLockTimeout.Time < time)
                    {
                        Door.Unlock();
                        StackerLock.Release.Execute(this);

                        if (Door.Locked != DoorLockStatus.Lock)
                            ShowMessage("LockFail", AlarmDoorLockFail, "Door lock fail");
                        else if (StackerLock.Status != StackerLock.StatusList.Lock)
                            ShowMessage("LockFail", AlarmStackerLockFail, "Stacker lock fail");

                        actor.NextStep("ConfirmPreDoorClose");
                    }
                });
            seq.AddStep("StackersMoveToBottomPos").StepIndex = seq.AddItem(StackerModule1.MoveToBottomPos,
                StackerModule2.MoveToBottomPos);
            seq.AddStep("UnlockDoor").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (Equipment.State == Equipment.StateStop)
                    {
                        Door.Unlock();
                        StackerLock.Release.Execute(this);
                    }
                    else
                    {
                        DoorUnlockRequest = true;
                        Equipment.RequestStop();
                    }

                    if (doorOpenCheck)
                        actor.NextStep();
                    else
                        actor.NextTerminate();
                });
            seq.AddStep("ConfirmDoorOpen").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (!Door.Closed)
                    {
                        if (TimeDoorOpenSignalStabilization.Time < time)
                        {
                            WriteTraceLog("Door open check ok");
                            CloseMessage("ChargingPlease");
                            actor.NextStep();
                        }
                    }
                    else if (TimeDoorOpenTimeout.Time < time)
                    {
                        ShowMessage("ChargingPlease", AlarmChargingPlease, "Charging Please");
                        actor.NextStep("ConfirmDoorOpen");
                    }
                    else
                        actor.NextStep("ConfirmDoorOpen");
                });
            seq.AddStep("ConfirmDoorClose").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (Door.Closed)
                    {
                        if (TimeDoorOpenSignalStabilization.Time < time)
                        {
                            WriteTraceLog("Door close check ok");
                            CloseMessage("CloseDoorPlease");
                            actor.NextStep();
                        }
                    }
                    else if (TimeDoorCloseTimeout.Time < time)
                    {
                        ShowMessage("CloseDoorPlease", AlarmCloseDoorPlease, "Charging Please");
                        actor.NextStep("ConfirmDoorClose");
                    }
                    else
                        actor.NextStep("ConfirmDoorClose");
                });

            seq.AddStep("LockDoor").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (Door.Closed)
                    {
                        WriteTraceLog("Lock Door and Stacker");
                        Door.Lock();
                        StackerLock.Lock.Execute(actor);
                        actor.NextStep();
                    }
                    else
                        actor.NextStep("ConfirmDoorClose");
                });
            seq.AddStep("ConfirmLockDoor").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (Door.Locked == DoorLockStatus.Lock &&
                        StackerLock.Status == StackerLock.StatusList.Lock)
                    {
                        WriteTraceLog("Lock Door and Stacker completed");
                        actor.NextStep();
                    }
                    else if (TimeDoorLockTimeout.Time < time)
                    {
                        if (Door.Locked != DoorLockStatus.Lock)
                            ShowMessage("LockFail", AlarmDoorLockFail, "Door lock fail");
                        else if (StackerLock.Status != StackerLock.StatusList.Lock)
                            ShowMessage("LockFail", AlarmStackerLockFail, "Stacker lock fail");

                        Door.Unlock();
                        StackerLock.Release.Execute(actor);

                        actor.NextStep("ConfirmDoorClose");
                    }
                });
        }

        private void MakeFlickLamp()
        {
            var seq = FlickLamp;
            seq.IsBackground = true;
            seq.AddWatcher(
                () =>
                {
                    if (DoorUnlockRequest && seq.State != SequenceState.Running)
                        seq.Start();
                    else if (!DoorUnlockRequest && !seq.IsStartable())
                    {
                        DoorOpenSwitchLamp.Off.Execute(this);
                        seq.Stop();
                    }
                });

            seq.AddStep("Start").StepIndex = seq.AddItem(DoorOpenSwitchLamp.On.Execute);
            seq.AddItem(TimeLampFlickInterval);
            seq.AddItem(DoorOpenSwitchLamp.Off.Execute);
            seq.AddItem(TimeLampFlickInterval);
            seq.AddItem("Start");
        }
        #endregion Make Sequence
    }
}
