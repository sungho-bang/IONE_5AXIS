
using FALibrary;
using FALibrary.Part.MemoryBasePart;
using FAFramework.VT3500.ExtendedParts;
using FALibrary.Part.Inverter;

namespace FAFramework.VT3500.SubUnits
{
    public class FAFrontLoadingUnit : Equipment.SubUnitBase
    {
        [FAAttribute("")]
        public FAPartOnOffSensor ShapeTapeTensionUpSensor { get; set; } // 첫번째 용지 텐션 유지용 위쪽 센서
        [FAAttribute("")]
        public FAPartOnOffSensor ShapeTapeTensionDownSensor { get; set; } // 첫번째 용지 텐션 유지용 아래쪽 센서
        [FAAttribute("")]
        public FAPartOnOffSensor ShapeTapeTensionSlowSensor { get; set; } // 첫번째 용지 텐션 유지용 슬로우 센서

        [FAAttribute("")]
        public FAPartOnOffSensor PackingTapeTensionUpSensor { get; set; } // 사이드 껍질 용지 텐션 유지용 위쪽 센서
        [FAAttribute("")]
        public FAPartOnOffSensor PackingTapeTensionDownSensor { get; set; } // 사이드 껍질 용지 텐션 유지용 아래쪽 센서
        [FAAttribute("")]
        public FAPartOnOffSensor TapeCoverServoOffSignal { get; set; } 
        [FAAttribute("")]
        public FAPartOnOffSensor PackingMotorRunCheck { get; set; } // 모터 런 체크
        [FAAttribute("")]
        public FAPartOneWayACMotor PackingTapeLoadingMotor { get; set; } // 사이드 껍질 용지 감아주는 AC 모터

        [FAAttribute("")]
        public FAPartGripRelease TapeHoldGrip { get; set; } // 밴드를 잡아주는 행위
        [FAAttribute("")]
        public FAPartGripRelease TapeLoadGrip { get; set; } // 밴드를 이동하는 그립
                
        [FAAttribute("")]
        public FAPartOnOff BandVaccum { get; set; } // 4등분된 밴드를 흡착하는 행위
        [FAAttribute("")]
        public FAPartOnOff BandVaccumEject { get; set; } // 4등분된 밴드를 파기하는 행위
        [FAAttribute("")]
        public FAPartOnOffSensor VacuumCheck_Front { get; set; } // Front쪽 흡착 감지 센서
        [FAAttribute("")]
        public FAPartOnOffSensor VacuumCheck_Rear { get; set; } // Rear쪽 흡착 감지 센서

        [FAAttribute("")]
        public FAPartPushHome BandPitchChangeCylinder { get; set; } // 밴드를 좌우로 벌려주는 행위

        [FAAttribute("")]
        public FATapeCoverServo TapeCoverServo { get; set; }
        [FAAttribute("")]
        public FAPartOnOffSensor TapeCoverServoPowerSignal { get; set; }


        [FAAttribute("")]
        public FAOptionServo OptionServo { get; set; }


        [FAAttribute("")]
        public FAOptinPressServo OptionPressServo { get; set; }

        [FAAttribute("")]
        public FAFirstPressServo FirstPressServo { get; set; }
        [FAAttribute("")]
        public FASecondPressServo SecondPressServo { get; set; }
        [FAAttribute("")]
        public FAThirdPressServo ThirdPressServo { get; set; }
  




        [FAAttribute("")]
        public FAPartOnOffSensor OptionServoPowerSignal { get; set; }
        [FAAttribute("")]
        public FATapeLoadingServo TapeLoadingServo { get; set; } // 중간에 밴드를 끌고가는 서보모터
        [FAAttribute("")]
        public FABandTransferServo BandTransferServo { get; set; } // 4등분된 밴드를 잡으러 이동하는 서보모터
        [FAAttribute("")]
        public FABandPickServo BandPickServo { get; set; } // 4등분된 밴드를 내리고 올리는 서보모터

        [FAAttribute("")]
        public FAInverter InverterMotor { get; set; }

        //5P 추가 0610
        [FAAttribute("")]
        public FAPartOneWayACMotor BypassCoveyorMotor { get; set; }
        [FAAttribute("")]
        public FAPartOnOffSensor BypassCoveyorExistCheck { get; set; } // 모터 런 체크
        [FAAttribute("")]
        public FAPartOnOffSensor ReelPowerBrakeOnCheck { get; set; }
        [FAAttribute("")]
        public FAPartOnOffSensor ReelPowerServoOnCheck { get; set; }
    }
}
