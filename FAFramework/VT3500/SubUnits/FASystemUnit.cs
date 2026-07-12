using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FALibrary;
using FALibrary.Part.MemoryBasePart;
using FAFramework.Utility;

namespace FAFramework.VT3500.SubUnits
{
    public class FASystemUnit : Equipment.SubUnitBase
    {
        [FAAttribute("Emergency")]
        public FAPartOnOffSensor EmergencyStateCheck { get; set; }
        [FAAttribute("Emergency")]
        public FAPartOnOffSensor EmergencyReset { get; set; }


        [FAAttribute("Emergency")]
        public FAPartOnOffSensor ShapeFrontEmergencyCheck { get; set; }
        [FAAttribute("Emergency")]
        public FAPartOnOffSensor ShapeRearEmergencyCheck { get; set; }

        [FAAttribute("Emergency")]
        public FAPartOnOffSensor StepFrontEmergencyCheck { get; set; }

        [FAAttribute("Emergency")]
        public FAPartOnOffSensor PackingRearEmergencyCheck { get; set; }

        [FAAttribute("Emergency")]
        public FAPartOnOffSensor SealingFrontEmergencyCheck { get; set; }
        [FAAttribute("Emergency")]
        public FAPartOnOffSensor SealingRearEmergencyCheck { get; set; }



       


    }
}
