using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Part.MMCPart;
using FALibrary.Sequence;
using FALibrary.Utility;
using FALibrary.Part;
using FALibrary;
using FALibrary.Alarm;

namespace FAFramework.VT3500.ExtendedParts
{
    public class FATapeLoadingServo : FAMMCPart
    {
        #region Actions
        [FAAttribute("Action")]
        public FAPartAction MoveHomePos { get; private set; }
        [FAAttribute("Action")]
        public FAPartAction MoveTapeLoadingPos { get; private set; }
        [FAAttribute("Action")]
        public FAPartAction MoveTapeLoadingSlowPos { get; private set; }
        #endregion

        #region Positions
        [FAAttribute("Position")]
        public FAMMCPosition HomePos { get; set; }
        [FAAttribute("Position")]
        public FAMMCPosition TapeLoadingPos { get; set; }
        [FAAttribute("Position")]
        public FAMMCPosition TapeLoadingSlowPos { get; set; }
        #endregion

        #region Alarms
        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int AlarmFailedMoveHomePos { get; set; }
        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int AlarmFailedMoveTapeLoadingPos { get; set; }
        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int AlarmFailedMoveTapeLoadingSlowPos { get; set; }
        #endregion

        public FATapeLoadingServo(FASequenceManager aSequenceManager)
            : base(aSequenceManager)
        {
            MoveHomePos = CreateAction(aSequenceManager);
            MoveTapeLoadingPos = CreateAction(aSequenceManager);
            MoveTapeLoadingSlowPos = CreateAction(aSequenceManager);
        }

        private FAPartAction CreateAction(FASequenceManager aSequenceManager)
        {
            var action = new FAPartAction();
            action.CreateSequence(aSequenceManager);
            return action;
        }

        private void MakeSequence(FAMMCPosition position, FAPartAction action, FALibrary.Utility.FATime time, string alarmPropertyName)
        {
            PositionUtility obj = new PositionUtility();
            obj.Part = this;
            obj.Position = position;
            obj.PartAction = action;
            obj.Timeout = time;
            obj.AlarmPropertyName = alarmPropertyName;
            obj.Initialize();

        }
        public override void LoadParameters(System.Xml.Linq.XElement xml)
        {
            base.LoadParameters(xml);

            // Old motor configurations do not contain the FRONT slow-search position.
            bool hasSlowPosition = xml?.Descendants("Item").Any(item => (string)item.Element("Name") == "TapeLoadingSlowPos") == true;
            if (!hasSlowPosition && TapeLoadingPos != null)
            {
                if (TapeLoadingSlowPos == null) TapeLoadingSlowPos = new FAMMCPosition();
                TapeLoadingPos.CopyTo(TapeLoadingSlowPos);
                TapeLoadingSlowPos.Name = "TapeLoadingSlowPos";
                TapeLoadingSlowPos.DriveSpeed = 50;
            }

            MakeAllSequences();
        }

        private void MakeAllSequences()
        {
            if (AlarmFailedMoveTapeLoadingSlowPos == 0)
                AlarmFailedMoveTapeLoadingSlowPos = AlarmFailedMoveTapeLoadingPos;
            MakeSequence(HomePos, MoveHomePos, MoveToPosTimeout, nameof(AlarmFailedMoveHomePos));
            MakeSequence(TapeLoadingPos, MoveTapeLoadingPos, MoveToPosTimeout, nameof(AlarmFailedMoveTapeLoadingPos));
            MakeSequence(TapeLoadingSlowPos, MoveTapeLoadingSlowPos, MoveToPosTimeout, nameof(AlarmFailedMoveTapeLoadingSlowPos));
        }
    }
}
