using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FALibrary;
using FALibrary.Sequence;
using FALibrary.Utility;
using FAFramework.Utility;
using FALibrary.Part.MemoryBasePart;
using FAFramework.VT3500.JobInfo;

namespace FAFramework.VT3500.Modules
{
    public class ExtensionOperationModule : Module.OperationModule
    {
        private VT3500.SubEquipment VT3500Equipment
        {
            get { return Equipment as VT3500.SubEquipment; }
        }

        #region Sequences
        [FAAttribute("Sequences")]
        public FASequence EmergencyReset { get; set; }
        [FAAttribute("Sequences")]
        public FASequence CheckProductStatus { get; set; }
        [FAAttribute("Sequences")]
        public FASequence CheckDoorStatus { get; set; }
        [FAAttribute("Sequences")]
        public FASequence CheckAreaStatus { get; set; }
        #endregion

        #region Status
        [FAAttribute("Status")]
        public bool CheckedDoorClosed { get; set; }

        [FAAttribute("Status")]
        public bool CheckedAreaSensor { get; set; }
        #endregion

        #region Alarm
        [DefaultAlarmInfo(101, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        [AlarmDescription(KnownCulture.EnglishUS, "Door Opened", "Close Door")]
        [AlarmDescription(KnownCulture.Korean, "문이 열려있습니다.", "문을 닫아주세요.")]
        public int AlarmDoorOpen { get; set; }

        [DefaultAlarmInfo(102, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        [AlarmDescription(KnownCulture.EnglishUS, "Door Lock Fail", "Please Check Door Status")]
        [AlarmDescription(KnownCulture.Korean, "문이 잠겨있지 않습니다.", "문을 닫아주세요.")]
        public int AlarmDoorLockFail { get; set; }

        [DefaultAlarmInfo(103, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        [AlarmDescription(KnownCulture.EnglishUS, "Manual Door Opened", "Please Check Door Status")]
        [AlarmDescription(KnownCulture.Korean, "문이 열려있습니다.", "문을 닫아주세요.")]
        public int AlarmManualDoorOpen { get; set; }

        [DefaultAlarmInfo(104, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        [AlarmDescription(KnownCulture.EnglishUS, "Manual Door Unlocked", "Please Check Manual Door Unlock Key Switch.")]
        [AlarmDescription(KnownCulture.Korean, "메뉴얼 열쇠가 열려있습니다.", "메뉴얼 열쇠를 닫아주세요.")]
        public int AlarmManualDoorLockCheckFail { get; set; }

        [DefaultAlarmInfo(105, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        [AlarmDescription(KnownCulture.EnglishUS, "Shape Press FAreaSensor Is Off", "Please Check Front Shape Press AreaSensor.")]
        [AlarmDescription(KnownCulture.Korean, "성형,피넛클 프레스 앞쪽 안전센서가 감지되었습니다.", "성형프레스 앞쪽의 안전센서를 확인해주세요")]
        public int AlarmShapePressFAreaSensorFail { get; set; }
        [DefaultAlarmInfo(106, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        [AlarmDescription(KnownCulture.EnglishUS, "Shape Press RAreaSensor Is Off", "Please Check Rear Shape Press AreaSensor.")]
        [AlarmDescription(KnownCulture.Korean, "성형,피넛클 프레스 뒤쪽 안전센서가 감지되었습니다.", "성형프레스 뒤쪽의 안전센서를 확인해주세요")]
        public int AlarmShapePressRAreaSensorFail { get; set; }
        [DefaultAlarmInfo(107, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        [AlarmDescription(KnownCulture.EnglishUS, "Packing Press FAreaSensor Is Off", "Please Check Front Packing Press AreaSensor.")]
        [AlarmDescription(KnownCulture.Korean, "톰슨 프레스 앞쪽 안전센서가 감지되었습니다.", "톰슨 앞쪽의 안전센서를 확인해주세요")]
        public int AlarmPackingPressFAreaSensorFail { get; set; }
        [DefaultAlarmInfo(108, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        [AlarmDescription(KnownCulture.EnglishUS, "Packing Press RAreaSensor Is Off", "Please Check Rear Packing Press AreaSensor.")]
        [AlarmDescription(KnownCulture.Korean, "톰슨 프레스 앞쪽 안전센서가 감지되었습니다.", "톰슨 앞쪽의 안전센서를 확인해주세요")]
        public int AlarmPackingPressRAreaSensorFail { get; set; }
        [DefaultAlarmInfo(109, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        [AlarmDescription(KnownCulture.EnglishUS, "Sealing Press FAreaSensor Is Off", "Please Check Front Sealing Press AreaSensor.")]
        [AlarmDescription(KnownCulture.Korean, "실링 프레스 앞쪽 안전센서가 감지되었습니다.", "실링 앞쪽의 안전센서를 확인해주세요")]
        public int AlarmSealingPressFAreaSensorFail { get; set; }
        [DefaultAlarmInfo(110, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        [AlarmDescription(KnownCulture.EnglishUS, "Sealing Press RAreaSensor Is Off", "Please Check Rear Sealing Press AreaSensor.")]
        [AlarmDescription(KnownCulture.Korean, "실링 프레스 앞쪽 안전센서가 감지되었습니다.", "실링 앞쪽의 안전센서를 확인해주세요")]
        public int AlarmSealingPressRAreaSensorFail { get; set; }

        [DefaultAlarmInfo(111, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.WARNING)]
        [AlarmDescription(KnownCulture.EnglishUS, "Shape Front Emergency Is On", "Please Check Shape Front Emergency Button.")]
        [AlarmDescription(KnownCulture.Korean, "샤프 전면부 비상이 감지되었습니다.", "샤프 전면부 비상 버튼을 확인해주세요")]
        public int AlarmShapeFrontEmergency { get; set; }
        [DefaultAlarmInfo(112, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.WARNING)]
        [AlarmDescription(KnownCulture.EnglishUS, "Shape Rear Emergency Is On", "Please Check Shape Rear Emergency Button.")]
        [AlarmDescription(KnownCulture.Korean, "샤프 후면부 비상이 감지되었습니다.", "샤프 후면부 비상 버튼을 확인해주세요")]
        public int AlarmShapeRearEmergency { get; set; }
        [DefaultAlarmInfo(113, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.WARNING)]
        [AlarmDescription(KnownCulture.EnglishUS, "Step Front Emergency Is On", "Please Step Check Front Emergency Button.")]
        [AlarmDescription(KnownCulture.Korean, "스텝 전면부 비상이 감지되었습니다.", "스텝 전면부 비상 버튼을 확인해주세요")]
        public int AlarmStepFrontEmergency { get; set; }
        [DefaultAlarmInfo(114, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.WARNING)]
        [AlarmDescription(KnownCulture.EnglishUS, "Packing Rear Emergency Is On", "Please Packing Check Rear Emergency Button.")]
        [AlarmDescription(KnownCulture.Korean, "팩킹 후면부 비상이 감지되었습니다.", "팩킹 후면부 비상 버튼을 확인해주세요")]
        public int AlarmPackingRearEmergency { get; set; }
        [DefaultAlarmInfo(115, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.WARNING)]
        [AlarmDescription(KnownCulture.EnglishUS, "Sealing Front Emergency Is On", "Please Sealing Check Front Emergency Button.")]
        [AlarmDescription(KnownCulture.Korean, "실링 전면부 비상이 감지되었습니다.", "실링 전면부 비상 버튼을 확인해주세요")]
        public int AlarmSealingFrontEmergency { get; set; }
        [DefaultAlarmInfo(116, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.WARNING)]
        [AlarmDescription(KnownCulture.EnglishUS, "Sealing Rear Emergency Is On", "Please Sealing Check Rear Emergency Button.")]
        [AlarmDescription(KnownCulture.Korean, "실링 후면부 비상이 감지되었습니다.", "실링 후면부 비상 버튼을 확인해주세요")]
        public int AlarmSealingRearEmergency { get; set; }
        #endregion

        #region Time
        [FAAttribute("Time")]
        public FATime TimeDoorLockTimeout { get; set; }

        [FAAttribute("Time")]
        public FATime TimeCheckRobotStatus { get; set; }
        #endregion

        #region Parameters
        [FAAttribute("Parameters")]
        [FAPropertyAttribute]
        public bool IgnoreDoorStatus { get; set; }
        [FAAttribute("Parameters")]
        [FAPropertyAttribute]
        public bool IgnoreAreaSensor { get; set; }
        [FAAttribute("Parameters")]
        [FAPropertyAttribute]
        public bool OnceCycleStartSignal { get; set; }
        #endregion

        #region Module
        public FAPressModule FirstPressModule { get; set; } // FirstPressModule 참조
        public FAPressModule FourthPressModule { get; set; } // ThirdPressModule 참조
        #endregion

        #region Override Methods
        public override void InitializeSequence()
        {
            base.InitializeSequence();

            MakeEmergencyReset();
            MakeCheckProductStatus();
            MakeCheckDoorStatus();
            MakeCheckAreaStatus();
            PreStartSequence.Add(CheckDoorStatus);
            PreStartSequence.Add(CheckProductStatus);

            PreInitializeSequence.Add(CheckDoorStatus);
            PreInitializeSequence.Add(CheckProductStatus);
                
        }
        public void StartBackgroundSequences()
        {
            CheckAreaStatus.Start();
        }

        protected override bool IsMainAirOn()
        {
            //return VT3500Equipment.CommonUnit.MainAirPressureOn.IsOn;
            return true;
        }

        protected override void SetEventHandler()
        {
            base.SetEventHandler();

            Equipment.StateEmergency.OnChangedState +=
                delegate
                {
                    //PressSafetyUp();
                    WriteTraceLog("EMERGENCY MACHINE");
                };

            Equipment.StateEmergencyReset.OnChangedState +=
                delegate (object sender, FALibrary.FAGenericEventArgs<Equipment.EquipmentState> e)
                {
                    //PressSafetyUp();
                    EmergencyReset.ClearState();
                    EmergencyReset.Start();

                    VT3500Equipment.OperationUnit.AlarmClearButtonLamp.DoTurnOn(this);
                    VT3500Equipment.OperationUnit.StartButtonLamp.DoTurnOff(this);
                    VT3500Equipment.OperationUnit.InitializeButtonLamp.DoTurnOff(this);
                    VT3500Equipment.OperationUnit.StopButtonLamp.DoTurnOff(this);
                    VT3500Equipment.OperationUnit.SoundClearButtonLamp.DoTurnOff(this);
                    WriteTraceLog("RESET EMERGENCY MACHINE");
                };

            Equipment.StateInitialize.OnChangedState +=
                delegate (object sender, FALibrary.FAGenericEventArgs<Equipment.EquipmentState> e)
                {
                    VT3500Equipment.MainLoopModule.MainLoop.Stop();
                    VT3500Equipment.MainLoopModule.MainLoop.ClearState();
                    Equipment.ClearSubSequencesState();
                    VT3500Equipment.MainLoopModule.InitializeMachine.Stop();
                    VT3500Equipment.MainLoopModule.InitializeMachine.Start();

                    VT3500Equipment.OperationUnit.AlarmClearButtonLamp.DoTurnOff(this);
                    VT3500Equipment.OperationUnit.StartButtonLamp.DoTurnOff(this);
                    VT3500Equipment.OperationUnit.InitializeButtonLamp.DoTurnOn(this);
                    VT3500Equipment.OperationUnit.StopButtonLamp.DoTurnOff(this);
                    VT3500Equipment.OperationUnit.SoundClearButtonLamp.DoTurnOff(this);
                };

            Equipment.StateRun.OnChangedState +=
                delegate (object sender, FALibrary.FAGenericEventArgs<Equipment.EquipmentState> e)
                {
                    Manager.LogManager.Instance.WriteStateLog(Equipment,
                        new Manager.MachineStateLog
                        {
                            Date = DateTime.Now,
                            State = Manager.MachineStateLog.EState.START
                        });

                    VT3500Equipment.OperationUnit.AlarmClearButtonLamp.DoTurnOff(this);
                    VT3500Equipment.OperationUnit.StartButtonLamp.DoTurnOn(this);
                    VT3500Equipment.OperationUnit.InitializeButtonLamp.DoTurnOff(this);
                    VT3500Equipment.OperationUnit.StopButtonLamp.DoTurnOff(this);

                    if (VT3500Equipment.MainLoopModule.MainLoop.IsStartable())
                    {
                        WriteTraceLog("START MACHINE");
                        VT3500Equipment.MainLoopModule.MainLoop.Start();

                        if (FirstPressModule.UsePress)
                        {
                            //SafetyAlertHeater
                            VT3500Equipment.SafetyFirstTopModule.SafetyAlertHeater.Start();
                            VT3500Equipment.SafetyFirstBottomModule.SafetyAlertHeater.Start();
                            //VT3500Equipment.SafetyFirstTopModule.SafetyOverHeater.Start(); //20210106
                            //VT3500Equipment.SafetyFirstBottomModule.SafetyHeater.Start();
                            //VT3500Equipment.SafetyFirstBottomModule.SafetyOverHeater.Start(); //20210106
                        }
                        if (FourthPressModule.UsePress)
                        {
                            VT3500Equipment.SafetyFourthBottomModule.SafetyAlertHeater.Start();
                            //VT3500Equipment.SafetyFourthBottomModule.SafetyHeater.Start();
                            //VT3500Equipment.SafetyFourthBottomModule.SafetyOverHeater.Start(); //20210106
                        }
                        CheckAreaStatus.Start();
                    }
                    else
                    {
                        if (FirstPressModule.UsePress)
                        {
                            VT3500Equipment.SafetyFirstTopModule.SafetyAlertHeater.Start();
                            VT3500Equipment.SafetyFirstBottomModule.SafetyAlertHeater.Start();
                            //VT3500Equipment.SafetyFirstTopModule.SafetyHeater.Stop();
                            //VT3500Equipment.SafetyFirstTopModule.SafetyHeater.Start();
                            //VT3500Equipment.SafetyFirstBottomModule.SafetyHeater.Stop();
                            //VT3500Equipment.SafetyFirstBottomModule.SafetyHeater.Start();
                        }
                        if (FourthPressModule.UsePress)
                        {
                            VT3500Equipment.SafetyFourthBottomModule.SafetyAlertHeater.Start();
                            //VT3500Equipment.SafetyFourthBottomModule.SafetyHeater.Stop();
                            //VT3500Equipment.SafetyFourthBottomModule.SafetyHeater.Start();
                        }

                        WriteTraceLog("RESUME MACHINE");
                        VT3500Equipment.ResumeSubSequences();
                        CheckAreaStatus.Start();
                    }
                };

            Equipment.StatePreInitialize.OnChangedState +=
                delegate
                {
                    CheckedDoorClosed = false;
                    VT3500Equipment.AlarmRaisingStatusManager.AllClear();
                };

            Equipment.StatePreRun.OnChangedState +=
                delegate
                {
                    VT3500Equipment.RearLoadingUnit.SealingTopRoller.Down.Execute(this);
                    CheckedDoorClosed = false;
                    VT3500Equipment.AlarmRaisingStatusManager.AllClear();

                    if (VT3500Equipment.FrontModule.FirstStart == false)
                    {
                        VT3500Equipment.FrontModule.RearStop = true;
                    }

                };

            Equipment.StatePreStop.OnChangedState +=
                delegate
                {
                    VT3500Equipment.FrontModule.BandLoadingInitLock = false;

                    if (VT3500Equipment.FrontModule.FirstStart)
                    {
                        VT3500Equipment.FrontModule.FeedingBeforeLock = false;
                        VT3500Equipment.FrontModule.FeedingAfterLock = false;
                    }

                    //if (VT3500Equipment.FrontModule.TapeMovePickCylinder.State == SequenceState.Running)
                    //{
                    //    VT3500Equipment.FrontModule.FeedingAfterLock = true;
                    //}
                    //else
                    //{
                    //    VT3500Equipment.FrontModule.FeedingBeforeLock = true;
                    //}

                    if (VT3500Equipment.FrontModule.TapeLoadingServo.MoveTapeLoadingPos.Sequence.State == SequenceState.Running)
                    //if(VT3500Equipment.FrontModule.TapeLoadingServo.IsInPosition(VT3500Equipment.FrontModule.TapeLoadingServo.TapeLoadingPos))
                    {
                        VT3500Equipment.FrontModule.FeedingAfterLock = true;
                    }
                    else
                    {
                        VT3500Equipment.FrontModule.FeedingBeforeLock = true;
                    }


                    //PressSafetyUp();
                    OnceCycleStartSignal = true;
                    VT3500Equipment.RearLoadingUnit.SealingTopRoller.Up.Execute(this);
                    WriteTraceLog("PRE STOP MACHINE");
                };

            Equipment.StateStop.OnChangedState +=
                delegate (object sender, FALibrary.FAGenericEventArgs<Equipment.EquipmentState> e)
                {
                    //PressSafetyUp();

                    if (e.Value == Equipment.StateAlarm)
                    {
                        Manager.LogManager.Instance.WriteStateLog(Equipment,
                            new Manager.MachineStateLog
                            {
                                Date = DateTime.Now,
                                State = Manager.MachineStateLog.EState.RELEASE_ALARM
                            });
                        VT3500Equipment.OperationUnit.AlarmClearButtonLamp.DoTurnOn(this);
                        VT3500Equipment.OperationUnit.StartButtonLamp.DoTurnOff(this);
                        VT3500Equipment.OperationUnit.InitializeButtonLamp.DoTurnOff(this);
                        VT3500Equipment.OperationUnit.StopButtonLamp.DoTurnOff(this);
                    }
                    else
                    {
                        VT3500Equipment.OperationUnit.AlarmClearButtonLamp.DoTurnOff(this);
                        VT3500Equipment.OperationUnit.StartButtonLamp.DoTurnOff(this);
                        VT3500Equipment.OperationUnit.InitializeButtonLamp.DoTurnOff(this);
                        VT3500Equipment.OperationUnit.StopButtonLamp.DoTurnOn(this);
                    }

                    Manager.LogManager.Instance.WriteStateLog(Equipment,
                        new Manager.MachineStateLog
                        {
                            Date = DateTime.Now,
                            State = Manager.MachineStateLog.EState.STOP
                        });

                    if (!VT3500Equipment.FrontModule.FirstStart)
                    {
                        VT3500Equipment.FrontModule.BandPlaceInitLock = true;
                    }
                    
                    VT3500Equipment.FrontModule.BandLoadingInitLock = false;
                    VT3500Equipment.FrontModule.RearStop = false;
                    VT3500Equipment.RearModule.MachineResume = true;
                    WriteTraceLog("STOP MACHINE");
                };

            Equipment.StateAlarm.OnChangedState +=
                delegate
                {
                    Manager.LogManager.Instance.WriteStateLog(Equipment,
                        new Manager.MachineStateLog
                        {
                            Date = DateTime.Now,
                            State = Manager.MachineStateLog.EState.ALARM
                        });

                    //PressSafetyUp();
                    VT3500Equipment.OperationUnit.AlarmClearButtonLamp.DoTurnOff(this);
                    VT3500Equipment.OperationUnit.StartButtonLamp.DoTurnOff(this);
                    VT3500Equipment.OperationUnit.InitializeButtonLamp.DoTurnOff(this);
                    VT3500Equipment.OperationUnit.StopButtonLamp.DoTurnOn(this);
                };

            Equipment.StateRundown.OnChangedState +=
                delegate
                {
                };
        }

        protected override void SetProcessAlarmHandler()
        {
            base.SetProcessAlarmHandler();
            Equipment.StateStop.ProcessAlarm +=
                delegate (FALibrary.Alarm.FAAlarmEventArgs args)
                {
                    ShowMessage(args.Alarm.AlarmName,
                        args.Alarm.AlarmNo,
                        $"Alarm = {args.Alarm.AlarmNo}, AlarmName = {args.Alarm.AlarmName}",
                        args.Message);

                    Equipment.SuspendSubSequences();
                };
        }

        protected override void ConfirmDoorClosed(FASequence actor, TimeSpan time)
        {
            if (IgnoreDoorStatus)
                actor.NextStep();
            else
            {
                var alarm = GetDoorOpenAlarm(out var alarmMoreInfo);

                if (alarm >= 0)
                {
                    RaiseAlarm(actor, alarm, alarmMoreInfo.ObjectName + " Open", alarmMoreInfo);
                }
                else
                    actor.NextStep();
            }
        }

        protected override void ConfirmDoorLockOk(FASequence actor, TimeSpan time)
        {
            if (IgnoreDoorStatus)
                actor.NextStep();
            else
            {
                var alarm = GetDoorLockAlarm(out var alarmMoreInfo);

                if (alarm >= 0)
                {
                    RaiseAlarm(actor, alarm, alarmMoreInfo.ObjectName + " Lock Fail", alarmMoreInfo);
                }
                else
                    actor.NextStep();
            }
        }

        protected override void LockDoor(object sender)
        {
            base.LockDoor(sender);
            Equipment.CloseDoor();
        }

        protected override void UnlockDoor(object sender)
        {
            base.UnlockDoor(sender);
            Equipment.OpenDoor();
        }
        #endregion

        #region SubUnits

        public SubUnits.FASystemUnit SystemUnit { get; set; }
        public SubUnits.FADoorUnit DoorUnit { get; set; }
        #endregion


        private void ProcMonitorActiveSignal()
        {
        }
        public void PressSafetyUp()
        {
            if (VT3500Equipment.FirstPressModule.UsePress)
            {
                VT3500Equipment.FirstPressModule.MotorRun.On.Execute(this);
            }
            else
            {
                VT3500Equipment.FirstPressModule.MotorRun.Off.Execute(this);
            }

            if (VT3500Equipment.SecondPressModule.UsePress)
            {
                VT3500Equipment.SecondPressModule.MotorRun.On.Execute(this);
            }
            else
            {
                VT3500Equipment.SecondPressModule.MotorRun.Off.Execute(this);
            }

            if (VT3500Equipment.ThirdPressModule.UsePress)
            {
                VT3500Equipment.ThirdPressModule.MotorRun.On.Execute(this);
            }
            else
            {
                VT3500Equipment.ThirdPressModule.MotorRun.Off.Execute(this);
            }

            if (VT3500Equipment.FourthPressModule.UsePress)
            {
                VT3500Equipment.FourthPressModule.MotorRun.On.Execute(this);
            }
            else
            {
                VT3500Equipment.FourthPressModule.MotorRun.Off.Execute(this);
            }

            if (VT3500Equipment.OptionPressModule.UsePress)
            {
                VT3500Equipment.OptionPressModule.MotorRun.On.Execute(this);
            }
            else
            {
                VT3500Equipment.OptionPressModule.MotorRun.Off.Execute(this);
            }
        }

        public void CheckDoorInterlock()
        {
            if (!IgnoreDoorStatus)
            {
                int alarm = GetDoorOpenAlarm(out var alarmMoreInfo);

                if (alarm >= 0)
                {
                    Equipment.EmergencyAlarmStop(Equipment, alarm, "DOOR OPEN INTERLOCK!!!");
                }
            }
        }

        public void CheckDoorLockInterlock()
        {
            int alarm = GetDoorLockAlarm(out var alarmMoreInfo);

            if (alarm >= 0)
            {
                ShowMessage("DOOR LOCK WARNING", alarm, "DOOR LOCK WARNING");
                Equipment.RequestStop();
            }
        }

        public int GetDoorOpenAlarm(out Utility.Alarm.AlarmMoreInfo alarmMoreInfo)
        {
            alarmMoreInfo = null;

            int alarm = -1;
            var doors = UtilityClass.GetAllPropertiesValue<FAPartDoor>(
                   VT3500Equipment.DoorUnit);
            alarmMoreInfo = GetDoorOpenAlarmMoreInfo(doors);

            if (alarmMoreInfo != null)
            {
                alarm = AlarmDoorOpen;
            }

            return alarm;
        }

        public int GetDoorLockAlarm(out Utility.Alarm.AlarmMoreInfo alarmMoreInfo)
        {
            alarmMoreInfo = null;
            int alarm = -1;

            var doors = UtilityClass.GetAllPropertiesValue<FAPartDoor>(
                    VT3500Equipment.DoorUnit);

            alarmMoreInfo = GetDoorUnlockAlarmMoreInfo(doors);

            if (alarmMoreInfo != null)
            {
                alarm = AlarmDoorLockFail;
            }

            return alarm;
        }

        private Utility.Alarm.AlarmMoreInfo GetDoorOpenAlarmMoreInfo(IEnumerable<FALibrary.Part.MemoryBasePart.FAPartDoor> doors)
        {
            var openDoors = doors.Where(x => !x.Closed);
            if (openDoors != null)
            {
                if (openDoors.Count() > 0 && openDoors.First() != null)
                {
                    return new Utility.Alarm.AlarmMoreInfo
                    {
                        ObjectName = openDoors.First().Name,
                        AutoImageName = openDoors.First().Name
                    };
                }
            }

            return null;
        }

        private Utility.Alarm.AlarmMoreInfo GetDoorUnlockAlarmMoreInfo(IEnumerable<FALibrary.Part.MemoryBasePart.FAPartDoor> doors)
        {
            var openDoors = doors.Where(x => x.Locked == FALibrary.Part.MemoryBasePart.DoorLockStatus.Unlock);

            if (openDoors != null)
            {
                if (openDoors.Count() > 0 && openDoors.First() != null)
                {
                    return new Utility.Alarm.AlarmMoreInfo
                    {
                        ObjectName = openDoors.First().Name,
                        AutoImageName = openDoors.First().Name
                    };
                }
            }

            return null;
        }


        #region Make Sequences

        private void MakeEmergencyReset()
        {
            var seq = EmergencyReset;
            #region Event Handler
            seq.OnStart +=
                delegate
                {
                    if (SystemUnit.EmergencyStateCheck.IsOn)
                    {
                        if (SystemUnit.ShapeFrontEmergencyCheck.IsOn)
                        {
                            ShowMessage("ShapeFrontEmergency", AlarmShapeFrontEmergency, "Shape 전면부 비상 스위치 감지");
                        }
                        else if (SystemUnit.ShapeRearEmergencyCheck.IsOn)
                        {
                            ShowMessage("ShapeRearEmergency", AlarmShapeRearEmergency, "Shape 후면부 비상 스위치 감지");
                        }
                        else if (SystemUnit.StepFrontEmergencyCheck.IsOn)
                        {
                            ShowMessage("StepFrontEmergency", AlarmStepFrontEmergency, "Step 전면부 비상 스위치 감지");
                        }
                        else if (SystemUnit.PackingRearEmergencyCheck.IsOn)
                        {
                            ShowMessage("PackingRearEmergency", AlarmPackingRearEmergency, "Packing 후면부 비상 스위치 감지");
                        }
                        else if (SystemUnit.SealingFrontEmergencyCheck.IsOn)
                        {
                            ShowMessage("SealingFrontEmergency", AlarmSealingFrontEmergency, "Sealing 전면부 비상 스위치 감지");
                        }
                        else if (SystemUnit.SealingRearEmergencyCheck.IsOn)
                        {
                            ShowMessage("SealingRearEmergency", AlarmSealingRearEmergency, "Sealing 후면부 비상 스위치 감지");
                        }
                    }
                };
            seq.OnStop +=
                delegate
                {
                    if(SystemUnit.EmergencyStateCheck.IsOff)
                    {
                        CloseMessage("ShapeFrontEmergency");
                        CloseMessage("ShapeRearEmergency");
                        CloseMessage("StepFrontEmergency");
                        CloseMessage("PackingRearEmergency");
                        CloseMessage("SealingFrontEmergency");
                        CloseMessage("SealingRearEmergency");
                    }
                };
            #endregion
            seq.AddItem(VT3500Equipment.CommonUnit.EmergencyReset.DoTurnOn);
            seq.AddItem(new FATime(FATimeType.millisecond, 500));
            seq.AddItem((object obj) => Equipment.State = Equipment.StateStop);
        }


        private void MakeCheckProductStatus()
        {
            var seq = CheckProductStatus;

            #region Event Handler
            seq.OnSuspended +=
                delegate
                {
                };

            seq.OnStop +=
                delegate
                {
                };
            #endregion

            seq.AddItem(
                delegate (object obj)
                {
                    VT3500Equipment.MainLoopModule.StatusCheckOk = true;

                    if (VT3500Equipment.MainLoopModule.StatusCheckOk == false)
                    {
                    }
                });
            seq.AddItem(
                delegate (FASequence actor, TimeSpan time)
                {
                    if (VT3500Equipment.MainLoopModule.StatusCheckOk)
                    {
                        actor.NextStep();
                    }
                });
        }

        private void MakeCheckDoorStatus()
        {
            var seq = CheckDoorStatus;

            seq.AddItem(ConfirmDoorClosed);
            seq.AddItem(LockDoor);
            seq.AddItem(new FATime(FATimeType.millisecond, 1000));
            seq.AddItem(ConfirmDoorLockOk);
            seq.AddItem((o) => CheckedDoorClosed = true);
        }
        
        private void MakeCheckAreaStatus()
        {
            var seq = CheckAreaStatus;

            #region Event
            seq.OnSuspended += delegate
            {
                //CheckAreaStatus.Stop();
            };
            #endregion

            seq.AddStep("Loop").StepIndex = seq.AddItem(
                (actor, time)=>
                {
                    if (VT3500Equipment.State == VT3500Equipment.StateRun ||
                        VT3500Equipment.State == VT3500Equipment.StateInitialize)
                    {
                        if (!IgnoreAreaSensor)
                        {
                            actor.NextStep();
                        }
                    }
                });
           seq.AddItem(
                (actor, time) => 
                {
                    if (VT3500Equipment.DoorUnit.ShapeFAreaCheck.IsOff)
                    {
                        RaiseAlarm(actor, AlarmShapePressFAreaSensorFail);
                    }
                    else if (VT3500Equipment.DoorUnit.ShapeRAreaCheck.IsOff)
                    {
                        RaiseAlarm(actor, AlarmShapePressRAreaSensorFail);
                    }
                    else if (VT3500Equipment.DoorUnit.PackingFAreaCheck.IsOff)
                    {
                        RaiseAlarm(actor, AlarmPackingPressFAreaSensorFail);
                    }
                    else if (VT3500Equipment.DoorUnit.PackingRAreaCheck.IsOff)
                    {
                        RaiseAlarm(actor, AlarmPackingPressRAreaSensorFail);
                    }
                    else if (VT3500Equipment.DoorUnit.SealingFAreaCheck.IsOff)
                    {
                        RaiseAlarm(actor, AlarmSealingPressFAreaSensorFail);
                    }
                    else if (VT3500Equipment.DoorUnit.SealingRAreaCheck.IsOff)
                    {
                        RaiseAlarm(actor, AlarmSealingPressRAreaSensorFail);
                    }
                    else
                    {
                        actor.NextStep();
                    }
                });
            seq.AddItem("Loop");
        }
        #endregion
    }
}
