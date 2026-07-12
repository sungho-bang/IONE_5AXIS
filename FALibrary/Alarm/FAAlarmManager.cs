using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FALibrary.Alarm
{
    public class FAAlarmManager
    {
        private static volatile FAAlarmManager _instance = null;
        private static object syncRoot = new Object();
        private SortedDictionary<int, FAAlarm> _items = new SortedDictionary<int, FAAlarm>();

        public event EventHandler<FAAlarmEventArgs> OnRaiseAlarm;

        public SortedDictionary<int, FAAlarm> Items { get { return _items; } }
        
        private FAAlarmManager()
        {
        }

        public static FAAlarmManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                            _instance = new FAAlarmManager();
                    }
                }

                return _instance;
            }
        }

        public void RaiseAlarm(object sender, int alarmID, string message = "")
        {
            RaiseAlarm(sender, alarmID, message, null);
        }

        public void RaiseAlarm(object sender, int alarmID, string message, object tag)
        {
            FAAlarmEventArgs args = new FAAlarmEventArgs();

            if (sender is Sequence.FASequence)
                ((Sequence.FASequence)sender).SetAlarm();

            if (Items.Count == 0 ||
                Items.ContainsKey(alarmID) == false)
            {
                args.Alarm = new FAAlarm();
                args.Alarm.AlarmNo = alarmID;
                args.Alarm.AlarmName = "";
                args.Alarm.Description = "";
                args.Alarm.ImagePath = "";
                args.Alarm.Solution = "";
            }
            else
                args.Alarm = Items[alarmID];

            string debugInfo = "";

            try
            {
                StackTrace st = new StackTrace(true);
                int frameCount = st.FrameCount;
                if (frameCount > 7)
                    frameCount = 7;

                for (int i = 0; i < frameCount; i++)
                {
                    string filename = st.GetFrame(i).GetFileName();
                    string lineNo = st.GetFrame(i).GetFileLineNumber().ToString();
                    string methodName = st.GetFrame(i).GetMethod().Name + "()";

                    string temp = filename + " - " +
                        methodName +
                        " Line(" + lineNo + ")";

                    debugInfo += temp + '\n';
                }
            }
            catch
            {
            }

            args.Alarm.SetMetaPropertyValue("Tag", tag);

            args.Message = message;
            args.DebugInfo = debugInfo;
            OnRaiseAlarm(sender, args);
        }
    }
}
