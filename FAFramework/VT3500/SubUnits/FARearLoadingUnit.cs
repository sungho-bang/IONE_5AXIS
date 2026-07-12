using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary;
using FALibrary.Part.PrinterPart;
using FALibrary.Part.MemoryBasePart;
using FALibrary.Part.MMCPart;
using FAFramework.VT3500.ExtendedParts;

namespace FAFramework.VT3500.SubUnits
{
    public class FARearLoadingUnit : Equipment.SubUnitBase
    {     
        [FAAttribute("")]        
        public FAPartOneWayACMotor SealingTapeLoadingMotor { get; set; } // 사이드 용지를 감아주는 모터
        [FAAttribute("")]
        public FAPartOnOffSensor SealingTapeTensionUpSensor { get; set; } // 마지막 용지 텐션 유지용 위쪽 센서
        [FAAttribute("")]
        public FAPartOnOffSensor SealingTapeTensionDownSensor { get; set; } // 마지막 용지 텐션 유지용 아래쪽 센서

        [FAAttribute("")]
        public FAPartOnOffSensor BlackMarkCheckSensor { get; set; } // 바닥용지 감지 센서

        [FAAttribute("")]
        public FAPartUpDown SealingTopRoller { get; set; } // 롤러 동작 실린더

        [FAAttribute("")]
        public FAPartUpDown SealingBandCutting { get; set; } // 밴드 컷팅 실린더              

        [FAAttribute("")]
        public FABandRollerServo BandRollerServo { get; set; } // 밴드 이동 서보모터

        [FAAttribute("")]
        public FAFourthPressServo FourthPressServo { get; set; }





    }
}
