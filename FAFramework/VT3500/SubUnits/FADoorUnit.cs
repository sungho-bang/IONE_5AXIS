using FALibrary;
using FALibrary.Part.MemoryBasePart;


namespace FAFramework.VT3500.SubUnits
{
    public class FADoorUnit : Equipment.SubUnitBase
    {
        // Shape Door
        [FAAttribute("")]
        public FAPartOnOffSensor ShapeDoorStateCloseCheck { get; set; }
        [FAAttribute("")]
        public FAPartDoor ShapeFLeftDoorCloseCheck { get; set; }
        [FAAttribute("")]
        public FAPartDoor ShapeFRightDoorCloseCheck { get; set; }
        [FAAttribute("")]
        public FAPartDoor ShapeRLeftDoorCloseCheck { get; set; }
        [FAAttribute("")]
        public FAPartDoor ShapeRRightDoorCloseCheck { get; set; }
        [FAAttribute("")]
        public FAPartOnOffSensor ShapeFAreaCheck { get; set; }
        [FAAttribute("")]
        public FAPartOnOffSensor ShapeRAreaCheck { get; set; }

        //Option Door 0610
        [FAAttribute("")]
        public FAPartOnOffSensor OptionDoorStateCheck { get; set; }
        [FAAttribute("")]
        public FAPartDoor OptionFLeftDoorStateCheck { get; set; }
        [FAAttribute("")]
        public FAPartDoor OptionFRightDoorStateCheck { get; set; }
        [FAAttribute("")]
        public FAPartDoor OptionRLeftDoorStateCheck { get; set; }
        [FAAttribute("")]
        public FAPartDoor OptionRRightDoorStateCheck { get; set; }

        [FAAttribute("")]
        public FAPartOnOffSensor OptionFAreaCheck { get; set; }
        [FAAttribute("")]
        public FAPartOnOffSensor OptionRAreaCheck { get; set; }

        // Step Door
        [FAAttribute("")]
        public FAPartOnOffSensor FeedingDoorStateCloseCheck { get; set; }
        [FAAttribute("")]
        public FAPartDoor FeedingFLeftDoorCloseCheck { get; set; }
        [FAAttribute("")]
        public FAPartDoor FeedingFRightDoorCloseCheck { get; set; }
        [FAAttribute("")]
        public FAPartDoor FeedingRLeftDoorCloseCheck { get; set; }
        [FAAttribute("")]
        public FAPartDoor FeedingRRightDoorCloseCheck { get; set; }

        // Packing Door
        [FAAttribute("")]
        public FAPartOnOffSensor PackingDoorStateCloseCheck { get; set; }
        [FAAttribute("")]
        public FAPartDoor PackingFLeftDoorCloseCheck { get; set; }
        [FAAttribute("")]
        public FAPartDoor PackingFRightDoorCloseCheck { get; set; }
        [FAAttribute("")]
        public FAPartDoor PackingRLeftDoorCloseCheck { get; set; }
        [FAAttribute("")]
        public FAPartDoor PackingRRightDoorCloseCheck { get; set; }

        [FAAttribute("")]
        public FAPartOnOffSensor PackingFAreaCheck { get; set; }
        [FAAttribute("")]
        public FAPartOnOffSensor PackingRAreaCheck { get; set; }

        // Sealing Door
        [FAAttribute("")]
        public FAPartOnOffSensor SealingDoorStateCloseCheck { get; set; }
        [FAAttribute("")]
        public FAPartDoor SealingFLeftDoorCloseCheck { get; set; }
        [FAAttribute("")]
        public FAPartDoor SealingFRightDoorCloseCheck { get; set; }
        [FAAttribute("")]
        public FAPartDoor SealingRLeftDoorCloseCheck { get; set; }
        [FAAttribute("")]
        public FAPartDoor SealingRRightDoorCloseCheck { get; set; }

        [FAAttribute("")]
        public FAPartOnOffSensor SealingFAreaCheck { get; set; }
        [FAAttribute("")]
        public FAPartOnOffSensor SealingRAreaCheck { get; set; }
    }
}
