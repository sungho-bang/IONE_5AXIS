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
    public class FATapeCoverServo : FAMMCPart
    {

        #region Positions

        #endregion
        
        #region Actions
      
        #endregion

        #region Alarms
    
        #endregion

        public FATapeCoverServo(FASequenceManager aSequenceManager)
            : base(aSequenceManager)
        {
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
            
        }
    }
}