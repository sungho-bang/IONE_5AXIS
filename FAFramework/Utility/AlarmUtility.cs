using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Alarm;

namespace FAFramework.Utility
{
    public class AlarmUtility
    {
        public static FAAlarm GetAlarm(int alarmNo, string defaultAlarmName)
        {
            FAAlarm alarm = new FAAlarm();

            if (FAAlarmManager.Instance.Items.ContainsKey(alarmNo) == true)
            {
                FAAlarmManager.Instance.Items[alarmNo].CopyTo(alarm);
            }
            else
            {
                alarm.AlarmNo = alarmNo;
                alarm.AlarmName = defaultAlarmName;
                alarm.Solution = defaultAlarmName + ". UNDEFINED ALARM. PLEASE DEFINE ALARM NO." + alarmNo.ToString();
            }

            return alarm;
        }
    }
}