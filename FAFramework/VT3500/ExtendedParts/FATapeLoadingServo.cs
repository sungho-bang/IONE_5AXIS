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
        #endregion

        #region Positions
        [FAAttribute("Position")]
        public FAMMCPosition HomePos { get; set; }
        [FAAttribute("Position")]
        public FAMMCPosition TapeLoadingPos { get; set; }
        #endregion

        #region Alarms
        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int AlarmFailedMoveHomePos { get; set; }
        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int AlarmFailedMoveTapeLoadingPos { get; set; }
        #endregion

        public FATapeLoadingServo(FASequenceManager aSequenceManager)
            : base(aSequenceManager)
        {
            MoveHomePos = CreateAction(aSequenceManager);
            MoveTapeLoadingPos = CreateAction(aSequenceManager);
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

            MakeAllSequences();
        }

        private void MakeAllSequences()
        {
            MakeSequence(HomePos, MoveHomePos, MoveToPosTimeout, nameof(AlarmFailedMoveHomePos));
            MakeSequence(TapeLoadingPos, MoveTapeLoadingPos, MoveToPosTimeout, nameof(AlarmFailedMoveTapeLoadingPos));
        }
    }
}
