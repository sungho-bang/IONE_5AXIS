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
    public class FAHeaterUnit : Equipment.SubUnitBase
    {
        [FAAttribute("")]
        public FAPartOnOffSensor FirstPressTopHeaterPowerOnCheck { get; set; }
        [FAAttribute("")]
        public FAPartOnOffSensor FirstPressTopHeaterOverHeatingCheck { get; set; }

        [FAAttribute("")]
        public FAPartOnOffSensor FirstPressBottomHeaterPowerOnCheck { get; set; }
        [FAAttribute("")]
        public FAPartOnOffSensor FirstPressBottomHeaterOverHeatingCheck { get; set; }
        //210609
        [FAAttribute("")]
        public FAPartOnOffSensor SSR80HeatAlarmCheck { get; set; } //Temperature Over 80 Degrees Machine Status Alarm
        [FAAttribute("")]
        public FAPartOnOffSensor SSR60HeatAlertCheck { get; set; } //Temperature Over 60 Degrees Machine Status Alert
        //210609
        [FAAttribute("")]
        public FAPartOnOff FirstPressTopHeaterPowerOn { get; set; }
        [FAAttribute("")]
        public FAPartOnOff FirstPressBottomHeaterPowerOn { get; set; }
        [FAAttribute("")]
        public FAPartOnOff FourthPressBottomHeaterPowerOn { get; set; }


        [FAAttribute("")]
        public FAPartOnOffSensor FourthPressBottomHeaterPowerOnCheck { get; set; }
        [FAAttribute("")]
        public FAPartOnOffSensor FourthPressBottomHeaterOverHeatingCheck { get; set; }

    }
}