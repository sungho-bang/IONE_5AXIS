using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Sequence;
using FALibrary;
using static FAFramework.Utility.Assign;
using System.Diagnostics;  // 파일 상단 using 에 추가

namespace FAFramework.VT3500
{
    public class SubEquipment : Equipment.StandardEquipment
    {
        /// <summary>
        /// 인버터 디버깅용 간단 로그 (VS Output 창에만 출력)
        /// </summary>
        private void InvTrace(string message)
        {
            Debug.WriteLine($"[INV] {DateTime.Now:HH:mm:ss.fff}  {message}");
        }

        public JobInfo.JobManager JobManagerInstance { get; set; } = new JobInfo.JobManager();

        #region SequenceManager
        #endregion

        #region Status

        private Equipment.UserInfo _lastLoginOperator;
        [FAAttribute("Status")]
        public Equipment.UserInfo LastLoginOperator
        {
            get { return _lastLoginOperator; }
            set
            {
                if (_lastLoginOperator == value) return;
                _lastLoginOperator = value;
                NotifyPropertyChanged("LastLoginOperator");
            }
        }

        #endregion

        #region SubUnits

        [FAAttribute("SubUnits")]
        public SubUnits.FADoorUnit DoorUnit { get; set; }

        [FAAttribute("SubUnits")]
        public SubUnits.FAFrontLoadingUnit FrontLoadingUnit { get; set; }
        [FAAttribute("SubUnits")]
        public SubUnits.FARearLoadingUnit RearLoadingUnit { get; set; }

        [FAAttribute("SubUnits")]
        public SubUnits.FAPressUnit FirstPressUnit { get; set; }
        [FAAttribute("SubUnits")]
        public SubUnits.FAPressUnit SecondPressUnit { get; set; }
        [FAAttribute("SubUnits")]
        public SubUnits.FAPressUnit OptionPressUnit { get; set; }

        [FAAttribute("SubUnits")]
        public SubUnits.FAPressUnit ThirdPressUnit { get; set; }
        [FAAttribute("SubUnits")]
        public SubUnits.FAPressUnit FourthPressUnit { get; set; }
        [FAAttribute("SubUnits")]
        public SubUnits.FASystemUnit SystemUnit { get; set; }
        
        [FAAttribute("SubUnits")]
        public SubUnits.FAHeaterUnit HeaterFirstTopUnit { get; set; }
        [FAAttribute("SubUnits")]
        public SubUnits.FAHeaterUnit HeaterFirstBottomUnit { get; set; }
        [FAAttribute("SubUnits")]
        public SubUnits.FAHeaterUnit HeaterFourthBottomUnit { get; set; }

        #endregion

        #region Modules
        [FAIncludeSequenceAttribute("Modules", "SubSequenceManager")]
        public Modules.ExtensionStateSignalModule ModuleStateSignal { get; set; }

        [FAIncludeSequenceAttribute("Modules.Generals", "MainSequenceManager")]
        public Modules.ExtensionOperationModule ModuleOperation { get; set; }

        [FAIncludeSequenceAttribute("Modules.Generals", "SubSequenceManager")]
        public Modules.FAMainLoopModule MainLoopModule { get; set; }

        [FAIncludeSequenceAttribute("Modules.Generals", "SubSequenceManager")]
        public Modules.FAFrontLoadingModule FrontModule { get; set; }
        [FAIncludeSequenceAttribute("Modules.Generals", "SubSequenceManager")]
        public Modules.FARearLoadingModule RearModule { get; set; }

        [FAIncludeSequenceAttribute("Modules.Generals", "SubSequenceManager")]
        public Modules.FAPressModule FirstPressModule { get; set; }
        [FAIncludeSequenceAttribute("Modules.Generals", "SubSequenceManager")]
        public Modules.FAPressModule SecondPressModule { get; set; }

        [FAIncludeSequenceAttribute("Modules.Generals", "SubSequenceManager")]
        public Modules.FAPressModule OptionPressModule { get; set; }

        [FAIncludeSequenceAttribute("Modules.Generals", "SubSequenceManager")]
        public Modules.FAPressModule ThirdPressModule { get; set; }
        [FAIncludeSequenceAttribute("Modules.Generals", "SubSequenceManager")]
        public Modules.FAPressModule FourthPressModule { get; set; }

        [FAIncludeSequenceAttribute("Modules.Generals", "SubSequenceManager")]
        public Modules.FASafetyModule SafetyFirstTopModule { get; set; }
        [FAIncludeSequenceAttribute("Modules.Generals", "SubSequenceManager")]
        public Modules.FASafetyModule SafetyFirstBottomModule { get; set; }
        [FAIncludeSequenceAttribute("Modules.Generals", "SubSequenceManager")]
        public Modules.FASafetyModule SafetyFourthBottomModule { get; set; }
        #endregion

        #region Fields & Properties


        #endregion

        #region Constructor

        public SubEquipment()
        {

            //PackingLogManager = new Manager.PackingLogManager();

            //OnChangedUser +=
            //    delegate (object sender, FAGenericEventArgs<Equipment.UserInfo> e)
            //    {
            //        if (e.Value.Permission == Equipment.UserPermissionTypes.OPERATOR)
            //            LastLoginOperator = e.Value;
            //    };

            AlarmRaisingStatusManager.OnRaiseAlarm +=
                    (sender, e) =>
                    {
                    };

        }

        #endregion

        #region Assign

        public override void AssignModule()
        {
            base.AssignModule();

            LinkModule();

            AssingExtensionOperationModule();
            AssingModuleOperationModule();
            AssignFrontModule(); 
            AssignRearModule();  
            AssignFirstPressModule();
            AssignSecondPressModule();

            AssignOptionPressModule();

            AssignThirdPressModule();
            AssignFourthPressModule();
            AssignAreaSensorModule();
            AssignSignalTower();
            AssignMainLoopModule();
            AssignSafetyFirstTopModule();
            AssignSafetyFirstBottomModule();
            AssignSafetyFourthTopModule();

            FirstPressModule.FModule = FrontModule;
            SecondPressModule.FModule = FrontModule;
            OptionPressModule.FModule = FrontModule;

            ThirdPressModule.FModule = FrontModule;
            FourthPressModule.RModule = RearModule;
            FrontModule.ModuleOperation = ModuleOperation;

            FrontModule.FirstPressUnit = FirstPressUnit;
            FrontModule.SecondPressUnit = SecondPressUnit;
            FrontModule.OptionPressUnit = OptionPressUnit;

            FrontModule.ThirdPressUnit = ThirdPressUnit;
            RearModule.FourthPressUnit = FourthPressUnit;

            SafetyFirstTopModule.PressHeaterUnit = HeaterFirstTopUnit;
            SafetyFirstBottomModule.PressHeaterUnit = HeaterFirstBottomUnit;
            SafetyFourthBottomModule.PressHeaterUnit = HeaterFourthBottomUnit;

            FirstPressModule.HeaterFirstTopUnit = HeaterFirstTopUnit;
            FirstPressModule.HeaterFourthBottomUnit = HeaterFirstBottomUnit;
            FourthPressModule.HeaterFourthBottomUnit = HeaterFourthBottomUnit;
        }
        private void AssignMainLoopModule()
        {
            MainLoopModule.OperationModule = ModuleOperation;
        }
        private void AssignFrontModule() 
        {
            var Fmodule = FrontModule;
            var Fsubunit = FrontLoadingUnit;

            AssignPartToModule(Fmodule, Fsubunit);

            Fmodule.ShapeTapeTensionUpSensor = Fsubunit.ShapeTapeTensionUpSensor;
            Fmodule.ShapeTapeTensionSlowSensor = FrontLoadingUnit.ShapeTapeTensionSlowSensor;
            Fmodule.ShapeTapeTensionDownSensor = FrontLoadingUnit.ShapeTapeTensionDownSensor;


            FrontModule.FirstPressModule = FirstPressModule;
            FrontModule.SecondPressModule = SecondPressModule;

            FrontModule.OptionPressModule = OptionPressModule;

            FrontModule.ThirdPressModule = ThirdPressModule;
            FrontModule.FirstPressUnit = FirstPressUnit;
            FrontModule.SecondPressUnit = SecondPressUnit;
            FrontModule.ThirdPressUnit = ThirdPressUnit;
            FrontModule.RModule = RearModule;
        }
        private void AssignSafetyFirstTopModule()
        {
            SafetyFirstTopModule.HeaterOverHeatingCheck = HeaterFirstTopUnit.FirstPressTopHeaterOverHeatingCheck;
            SafetyFirstTopModule.HeaterPowerOn = HeaterFirstTopUnit.FirstPressTopHeaterPowerOn;
            SafetyFirstTopModule.HeaterPowerOnCheck = HeaterFirstTopUnit.FirstPressTopHeaterPowerOnCheck;
            SafetyFirstTopModule.SSR60HeatAlertCheck = HeaterFirstTopUnit.SSR60HeatAlertCheck;
            SafetyFirstTopModule.SSR80HeatAlarmCheck = HeaterFirstTopUnit.SSR80HeatAlarmCheck;
        }

        private void AssignSafetyFirstBottomModule()
        {
            SafetyFirstBottomModule.HeaterOverHeatingCheck = HeaterFirstBottomUnit.FirstPressBottomHeaterOverHeatingCheck;
            SafetyFirstBottomModule.HeaterPowerOn = HeaterFirstBottomUnit.FirstPressBottomHeaterPowerOn;
            SafetyFirstBottomModule.HeaterPowerOnCheck = HeaterFirstBottomUnit.FirstPressBottomHeaterPowerOnCheck;
            SafetyFirstBottomModule.SSR60HeatAlertCheck = HeaterFirstBottomUnit.SSR60HeatAlertCheck;
            SafetyFirstBottomModule.SSR80HeatAlarmCheck = HeaterFirstBottomUnit.SSR80HeatAlarmCheck;
        }

        private void AssignSafetyFourthTopModule()
        {
            SafetyFourthBottomModule.HeaterOverHeatingCheck = HeaterFourthBottomUnit.FourthPressBottomHeaterOverHeatingCheck;
            SafetyFourthBottomModule.HeaterPowerOn = HeaterFourthBottomUnit.FourthPressBottomHeaterPowerOn;
            SafetyFourthBottomModule.HeaterPowerOnCheck = HeaterFourthBottomUnit.FourthPressBottomHeaterPowerOnCheck;
            SafetyFourthBottomModule.SSR60HeatAlertCheck = HeaterFourthBottomUnit.SSR60HeatAlertCheck;
            SafetyFourthBottomModule.SSR80HeatAlarmCheck = HeaterFourthBottomUnit.SSR80HeatAlarmCheck;
        }

        private void AssingModuleOperationModule() 
        {
        }
            private void AssignRearModule() 
        {
            var Rmodule = RearModule;
            var Rsubunit = RearLoadingUnit;

            AssignPartToModule(Rmodule, Rsubunit);
            RearModule.FourthPressModule = FourthPressModule;
            RearModule.FourthPressUnit = FourthPressUnit;
            RearModule.FModule = FrontModule;
        }
        private void AssignAreaSensorModule()
        {
            FirstPressModule.FAreaCheck = DoorUnit.ShapeFAreaCheck;
            FirstPressModule.RAreaCheck = DoorUnit.ShapeRAreaCheck;
            SecondPressModule.FAreaCheck = DoorUnit.ShapeFAreaCheck;
            SecondPressModule.RAreaCheck = DoorUnit.ShapeRAreaCheck;
            ThirdPressModule.FAreaCheck = DoorUnit.PackingFAreaCheck;
            ThirdPressModule.RAreaCheck = DoorUnit.PackingRAreaCheck;
            FourthPressModule.FAreaCheck = DoorUnit.SealingFAreaCheck;
            FourthPressModule.RAreaCheck = DoorUnit.SealingRAreaCheck;

            OptionPressModule.FAreaCheck = DoorUnit.OptionFAreaCheck;
            OptionPressModule.RAreaCheck = DoorUnit.OptionRAreaCheck;
        }
        private void AssignFirstPressModule()
        {
            FirstPressModule.MotorRunCheck = FirstPressUnit.MotorRunCheck;
            FirstPressModule.OpenCheck = FirstPressUnit.OpenCheck;
            FirstPressModule.CloseCheck = FirstPressUnit.CloseCheck;
            FirstPressModule.PressCheck = FirstPressUnit.PressCheck;
            FirstPressModule.MotorRun = FirstPressUnit.MotorRun;
            FirstPressModule.PressFanMotor = FirstPressUnit.PressFanMotor;
            FirstPressModule.Opening = FirstPressUnit.Opening;
            FirstPressModule.Closing = FirstPressUnit.Closing;
            FirstPressModule.PressOilTempCheck = FirstPressUnit.PressOilTempCheck;
        }

        private void AssignSecondPressModule()
        {
            SecondPressModule.MotorRunCheck = SecondPressUnit.MotorRunCheck;
            SecondPressModule.OpenCheck = SecondPressUnit.OpenCheck;
            SecondPressModule.CloseCheck = SecondPressUnit.CloseCheck;
            SecondPressModule.PressCheck = SecondPressUnit.PressCheck;
            SecondPressModule.MotorRun = SecondPressUnit.MotorRun;
            SecondPressModule.PressFanMotor = SecondPressUnit.PressFanMotor;
            SecondPressModule.Opening = SecondPressUnit.Opening;
            SecondPressModule.Closing = SecondPressUnit.Closing;
            SecondPressModule.PressOilTempCheck = SecondPressUnit.PressOilTempCheck;
        }
        private void AssignOptionPressModule()
        {
            OptionPressModule.MotorRunCheck = OptionPressUnit.MotorRunCheck;
            OptionPressModule.OpenCheck = OptionPressUnit.OpenCheck;
            OptionPressModule.CloseCheck = OptionPressUnit.CloseCheck;
            OptionPressModule.PressCheck = OptionPressUnit.PressCheck;
            OptionPressModule.MotorRun = OptionPressUnit.MotorRun;
            OptionPressModule.PressFanMotor = OptionPressUnit.PressFanMotor;
            OptionPressModule.Opening = OptionPressUnit.Opening;
            OptionPressModule.Closing = OptionPressUnit.Closing;
            OptionPressModule.PressOilTempCheck = OptionPressUnit.PressOilTempCheck;
        }
        private void AssignThirdPressModule()
        {
            ThirdPressModule.MotorRunCheck = ThirdPressUnit.MotorRunCheck;
            ThirdPressModule.OpenCheck = ThirdPressUnit.OpenCheck;
            ThirdPressModule.CloseCheck = ThirdPressUnit.CloseCheck;
            ThirdPressModule.PressCheck = ThirdPressUnit.PressCheck;
            ThirdPressModule.MotorRun = ThirdPressUnit.MotorRun;
            ThirdPressModule.PressFanMotor = ThirdPressUnit.PressFanMotor;
            ThirdPressModule.Opening = ThirdPressUnit.Opening;
            ThirdPressModule.Closing = ThirdPressUnit.Closing;
            ThirdPressModule.PressOilTempCheck = ThirdPressUnit.PressOilTempCheck;
        }

        private void AssignFourthPressModule()
        {
            FourthPressModule.MotorRunCheck = FourthPressUnit.MotorRunCheck;
            FourthPressModule.OpenCheck = FourthPressUnit.OpenCheck;
            FourthPressModule.CloseCheck = FourthPressUnit.CloseCheck;
            FourthPressModule.PressCheck = FourthPressUnit.PressCheck;
            FourthPressModule.MotorRun = FourthPressUnit.MotorRun;
            FourthPressModule.PressFanMotor = FourthPressUnit.PressFanMotor;
            FourthPressModule.Opening = FourthPressUnit.Opening;
            FourthPressModule.Closing = FourthPressUnit.Closing;
            FourthPressModule.PressOilTempCheck = FourthPressUnit.PressOilTempCheck;
        }

        private void LinkModule()
        {
        }
     
        private void AssingExtensionOperationModule()
        {
            var moduleOperaion = ModuleOperation;
            var doorUnit = DoorUnit;
           

            AssignPartToModule(moduleOperaion, doorUnit);
            moduleOperaion.SystemUnit = SystemUnit;
            ModuleOperation.FirstPressModule = FirstPressModule;
            ModuleOperation.FourthPressModule = FourthPressModule;
        }

        private void AssignSignalTower()
        {
            ModuleStateSignal.SignalTowerRed = CommonUnit.SignalTowerRed;
            ModuleStateSignal.SignalTowerYellow = CommonUnit.SignalTowerYellow;
            ModuleStateSignal.SignalTowerGreen = CommonUnit.SignalTowerGreen;
            ModuleStateSignal.SignalTowerBuzzer = CommonUnit.SignalTowerBuzzer;
            ModuleStateSignal.SignalPhoneMelodie1 = CommonUnit.SignalTowerBuzzer;
            ModuleStateSignal.SignalPhoneMelodie2 = CommonUnit.SignalTowerBuzzer;
            ModuleStateSignal.SignalPhoneMelodie3 = CommonUnit.SignalTowerBuzzer;
            ModuleStateSignal.SignalPhoneMelodie4 = CommonUnit.SignalTowerBuzzer;
        }
        #endregion

        #region Intelock
        public override void SetInterlock()
        {
            base.SetInterlock();

        }
        #endregion
        
        public override void TurnOnSound()
        {
        }

        protected override void PreLoadConfig()
        {
            base.PreLoadConfig();

            JobManagerInstance.JobFolderPath = System.IO.Path.Combine(ConfigClasses.GlobalConst.CONFIG_PATH, Name);
            JobManagerInstance.Load();
        }

        protected override void PreSaveConfig()
        {
            base.PreSaveConfig();

            JobManagerInstance.Save();
        }

        protected override void AllwaysExecute()
        {
            base.AllwaysExecute();

            #region TapeCover Servo

            if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.State == State.Equipment.StateStop &&
                RearModule.RearSeqTerminated && ModuleOperation.OnceCycleStartSignal && ThirdPressModule.UsePress && !OptionPressModule.UsePress && IsInitializedOk && MainLoopModule.InitializeSelect)
            {
                FrontModule.OnceCycleEnd.Start();
            }

            if (FrontLoadingUnit.TapeCoverServoPowerSignal.IsOn)
            {
                FrontLoadingUnit.TapeCoverServo.ServoOffAction.Execute(this);
            }
            else
            {
                FrontLoadingUnit.TapeCoverServo.ServoOnAction.Execute(this);
            }

            if (FrontLoadingUnit.OptionServoPowerSignal.IsOn)
            {
                FrontLoadingUnit.OptionServo.ServoOffAction.Execute(this);
            }
            else
            {
                FrontLoadingUnit.OptionServo.ServoOnAction.Execute(this);
            }

            //if (FirstPressModule.PressSafeArea &&
            //  SecondPressModule.PressSafeArea &&
            //  FrontModule.TapeLoadGrip.Status == FrontModule.TapeLoadGrip.StatusList.Grip &&
            //  FrontModule.TapeLoadingServo.RunFlag)
            //{
            //    if (FrontModule.TapeCoverServo.IsMotionDone() == true)
            //    {
            //        FrontModule.TapeCoverServo.Velocity = Convert.ToUInt32(FrontModule.HighSpeedVelocity);
            //        FrontModule.TapeCoverServo.MoveVelocity.Execute(this);
            //    }
            //    else
            //    {
            //        FrontModule.TapeCoverServo.OverrideVelocity = Convert.ToUInt32(FrontModule.HighSpeedVelocity);
            //        FrontModule.TapeCoverServo.MoveSpeedOverrideVelocity.Execute(this);
            //    }
            //}
            //else
            //{
            //    if (FrontModule.TapeCoverServo.IsMotionDone() == true)
            //    {
            //        FrontModule.TapeCoverServo.Velocity = Convert.ToUInt32(FrontModule.HighSpeedVelocity);
            //        FrontModule.TapeCoverServo.MoveVelocity.Execute(this);
            //    }
            //    else
            //    {
            //        FrontModule.TapeCoverServo.OverrideVelocity = Convert.ToUInt32(FrontModule.LowSpeedVelocity);
            //        FrontModule.TapeCoverServo.MoveSpeedOverrideVelocity.Execute(this);
            //    }
            //}
            #endregion

            #region FrontModule
            var shapeUp = FrontLoadingUnit.ShapeTapeTensionUpSensor;

            if (FrontModule.ShapeTapeTensionSlowSensor.IsOn)
            {
                //if (FrontModule.ShapeTapeTensionUpSensor.IsOn)
                if(shapeUp.IsOn)
                {
                    InvTrace("INV: Slow+Up ON → Inverter Run 요청"); // 임시 로그
                    FrontModule.InverterMotor.Write_SetSpeed = FrontModule.Inverter_WriteSpeed_Slow;
                    FrontModule.InverterMotor.Run();
                }
                else
                {
                    InvTrace("INV: Slow+Down ON → Inverter Stop 요청"); // 임시 로그
                    if (FrontModule.ShapeTapeTensionDownSensor.IsOn)
                    {
                        FrontModule.InverterMotor.Stop();
                    }
                }
            }

            if (FrontModule.PackingTapeTensionUpSensor.IsOn)
            {
                FrontModule.PackingTapeLoadingMotor.Stop.Execute(this);
            }
            else
            {
                if (FrontModule.PackingTapeTensionDownSensor.IsOn)
                {
                    FrontModule.PackingTapeLoadingMotor.Run.Execute(this);
                }
            }
            #endregion

            //if (FrontModule.TapeLoadGrip.Status == FrontModule.TapeLoadGrip.StatusList.Grip &&
            //    FrontModule.TapeLoadingServo.RunFlag || FrontModule.WorkManualOnceOneCycle.State == SequenceState.Running ||
            //    FrontModule.WorkManualLoading.State == SequenceState.Running)
            //{
            //    FrontModule.FrontACMotorLoop.Start();
            //}

            //상시 On
            if (RearModule.SealingTapeTensionDownSensor.IsOn)
            {
                RearModule.SealingTapeLoadingMotor.Run.Execute(this);
            }
            else
            {
                if (RearModule.SealingTapeTensionUpSensor.IsOn)
                {
                    RearModule.SealingTapeLoadingMotor.Stop.Execute(this);
                }
            }

            if (FrontModule.PackingTapeTensionUpSensor.IsOn)
            {
                FrontModule.PackingTapeLoadingMotor.Stop.Execute(this);
            }
            else
            {
                if (FrontModule.PackingTapeTensionDownSensor.IsOn)
                {
                    FrontModule.PackingTapeLoadingMotor.Run.Execute(this);
                }
            }
            if (RearModule.SealingTapeTensionDownSensor.IsOn)
            {
                RearModule.SealingTapeLoadingMotor.Run.Execute(this);
            }
            else
            {
                if (RearModule.SealingTapeTensionUpSensor.IsOn)
                {
                    RearModule.SealingTapeLoadingMotor.Stop.Execute(this);
                }
            }

            if (FrontModule.PackingTapeTensionUpSensor.IsOn)
            {
                FrontModule.PackingTapeLoadingMotor.Stop.Execute(this);
            }
            else
            {
                if (FrontModule.PackingTapeTensionDownSensor.IsOn)
                {
                    FrontModule.PackingTapeLoadingMotor.Run.Execute(this);
                }
            }

            //if (FirstPressModule.MotorRun.IsOn)
            //{
            //    FirstPressModule.PressFanMotor.DoTurnOn(this);
            //}
            //else
            //{
            //    FirstPressModule.PressFanMotor.DoTurnOff(this);
            //}

            //if (SecondPressModule.MotorRun.IsOn)
            //{
            //    SecondPressModule.PressFanMotor.DoTurnOn(this);
            //}
            //else
            //{
            //    SecondPressModule.PressFanMotor.DoTurnOff(this);
            //}

            //if (OptionPressModule.MotorRun.IsOn)
            //{
            //    OptionPressModule.PressFanMotor.DoTurnOn(this);
            //}
            //else
            //{
            //    OptionPressModule.PressFanMotor.DoTurnOff(this);
            //}

            //if (ThirdPressModule.MotorRun.IsOn)
            //{
            //    ThirdPressModule.PressFanMotor.DoTurnOn(this);
            //}
            //else
            //{
            //    ThirdPressModule.PressFanMotor.DoTurnOff(this);
            //}

            //if (FourthPressModule.MotorRun.IsOn)
            //{
            //    FourthPressModule.PressFanMotor.DoTurnOn(this);
            //}
            //else
            //{
            //    FourthPressModule.PressFanMotor.DoTurnOff(this);
            //}
            //if (FrontModule.ShapeTapeTensionUpSensor.IsOn)
            //{
            //    FrontModule.InverterMotor.Write_SetSpeed = FrontModule.Inverter_WriteSpeed_Fast;
            //    FrontModule.InverterMotor.Run();
            //}
            //else
            //{
            //    if (FrontModule.ShapeTapeTensionDownSensor.IsOn)
            //    {
            //        FrontModule.InverterMotor.Stop();
            //    }
            //}
            //if (FrontModule.ShapeTapeTensionUpSensor.IsOn)
            //{
            //    FrontModule.InverterMotor.Write_SetSpeed = FrontModule.Inverter_WriteSpeed_Fast;
            //    FrontModule.InverterMotor.Run();
            //}
            //else
            //{
            //    if (FrontModule.ShapeTapeTensionDownSensor.IsOn)
            //    {
            //        FrontModule.InverterMotor.Stop();
            //    }
            //}

            // 20200904 고객사 요청사항
            // FrontLoadingUnit.TapeCoverServo.SpeedRate = FrontModule.SpeedScale;
            FrontLoadingUnit.TapeLoadingServo.SpeedRate = FrontModule.SpeedScale;
            FrontLoadingUnit.BandTransferServo.SpeedRate = FrontModule.SpeedScale;
            FrontLoadingUnit.BandPickServo.SpeedRate = FrontModule.SpeedScale;
            RearLoadingUnit.BandRollerServo.SpeedRate = RearModule.SpeedScale;
        }

        public override void OpenDoor()
        {
        }

        public override void CloseDoor()
        {
        }

        protected override void AfterLoadConfig()
        {
            base.AfterLoadConfig();

        }

        protected override void ActionAfterInitialize()
        {
            base.ActionAfterInitialize();
            CommonUnit.EmergencyReset.DoTurnOn(this);
            ModuleOperation.StartBackgroundSequences();

            SafetyFirstTopModule.HeaterPowerOn.DoTurnOn(this);
            SafetyFirstBottomModule.HeaterPowerOn.DoTurnOn(this);
            SafetyFourthBottomModule.HeaterPowerOn.DoTurnOn(this);
        }

        private static volatile SubEquipment _instance = null;
        private static object syncRoot = new Object();

        public static SubEquipment Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                            _instance = new SubEquipment();
                    }
                }

                return _instance;
            }
        }

        public override void DisposeEquipment()
        {
            base.DisposeEquipment();

        }
    }
}
