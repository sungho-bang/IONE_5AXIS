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
    public class FABandPickServo : FAMMCPart
    {

        #region Positions
        [FAAttribute("Position")]
        public FAMMCPosition HomePos { get; set; }

        [FAAttribute("Position")]
        public FAMMCPosition PickPos { get; set; }
        [FAAttribute("Position")]
        public FAMMCPosition PlacePos { get; set; }
        #endregion

        #region Actions
        [FAAttribute("Action")]
        public FAPartAction MoveHomePos { get; private set; }
        [FAAttribute("Action")]
        public FAPartAction MovePickPos { get; private set; }
        [FAAttribute("Action")]
        public FAPartAction MovePlacePos { get; private set; }
        #endregion

        #region Alarms
        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int AlarmFailedMoveHomePos { get; set; }
        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int AlarmFailedMovePickPos { get; set; }
        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int AlarmFailedMovePlacePos { get; set; }
        #endregion

        public FABandPickServo(FASequenceManager aSequenceManager)
            : base(aSequenceManager)
        {
            MoveHomePos = CreateAction(aSequenceManager);
            MovePickPos = CreateAction(aSequenceManager);
            MovePlacePos = CreateAction(aSequenceManager);
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

        // 밴드를 픽하는 실린더는 3번째 실린더와 동시에 Up, Down을 함
        // 밴드를 픽할때 3번째 실린더는 픽하고 옮겨 놓을때 까지 정지
        private void MakeAllSequences()
        {
            MakeSequence(HomePos, MoveHomePos, MoveToPosTimeout, nameof(AlarmFailedMoveHomePos));
            MakeSequence(PickPos, MovePickPos, MoveToPosTimeout, nameof(AlarmFailedMovePickPos));
            MakeSequence(PlacePos, MovePlacePos, MoveToPosTimeout, nameof(AlarmFailedMovePlacePos));
        }
    }
}