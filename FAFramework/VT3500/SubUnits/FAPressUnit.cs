
using FALibrary;
using FALibrary.Part.MemoryBasePart;

namespace FAFramework.VT3500.SubUnits
{
    public class FAPressUnit : Equipment.SubUnitBase
    {
        // Input
        // Press 동작
        [FAAttribute("")]
        public FAPartOnOffSensor MotorRunCheck { get; set; }
        [FAAttribute("")]
        public FAPartOnOffSensor OpenCheck { get; set; }
        [FAAttribute("")]
        public FAPartOnOffSensor CloseCheck { get; set; }
        [FAAttribute("")]
        public FAPartOnOffSensor PressCheck { get; set; }
        [FAAttribute("")]
        public FAPartOnOffSensor PressOilTempCheck { get; set; }

        // Output 
        // Press 동작
        [FAAttribute("")]
        public FAPartOnOff MotorRun { get; set; }
        [FAAttribute("")]
        public FAPartOnOff Opening { get; set; }
        [FAAttribute("")]
        public FAPartOnOff Closing { get; set; }
        [FAAttribute("")]
        public FAPartOnOff PressFanMotor { get; set; } // 상시 On 
    }
}
