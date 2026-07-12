using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary;

namespace FAFramework.Utility
{
    public class EnablePropertyChangeLog : Attribute
    {
    }

    public class SubSequenceManagerName : Attribute
    {
        public string Name { get; set; }

        public SubSequenceManagerName(string name)
        {
            Name = name;
        }
    }

    [AttributeUsageAttribute(AttributeTargets.Property, AllowMultiple = true)]
    public class AlarmDescription : Attribute
    {
        public string Culture { get; set; }
        public string Name { get; set; }
        public string Solution { get; set; }

        public AlarmDescription(string culture, string name, string solution)
        {
            Culture = culture;
            Name = name;
            Solution = solution;
        }
    }

    public class DefaultAlarmInfo : FAAttribute
    {
        public int AlarmNo { get; set; }
        public int AlarmType { get; set; }
        public int AlarmStatus { get; set; }

        public DefaultAlarmInfo(int alarmNo, Alarm.EAlarmType type, Alarm.EAlarmStatus status)
            : base("Alarm")
        {
            AlarmNo = alarmNo;
            AlarmType = (int)type;
            AlarmStatus = (int)status;
        }
    }

    public class AlarmInfo : FAPropertyAttribute
    {
        public int AlarmType { get; set; }
        public int AlarmStatus { get; set; }
        public string AlarmSolution { get; set; }

        public AlarmInfo(int type, int status, string solution)
        {
            AlarmType = type;
            AlarmStatus = status;
            AlarmSolution = solution;
        }
    }

    public class KukaRobotAlarmInfo : Attribute
    {
        public int AlarmNo { get; set; }
        public int AlarmType { get; set; }
        public int AlarmStatus { get; set; }

        public KukaRobotAlarmInfo(int alarmNo, Alarm.EAlarmType type, Alarm.EAlarmStatus status)
        {
            AlarmNo = alarmNo;
            AlarmType = (int)type;
            AlarmStatus = (int)status;
        }
    }

    public class NotAutoCreateModule : Attribute
    {
    }

    public class ExtensionSaveProperty : FAPropertyAttribute
    {
        public bool UseAutoSave { get; set; }
        public bool UseWriteChangeLog { get; set; }

        public ExtensionSaveProperty()
        {
            UseAutoSave = true;
            UseWriteChangeLog = true;
        }

        public ExtensionSaveProperty(bool autoSave, bool writeChangeLog)
        {
            UseAutoSave = autoSave;
            UseWriteChangeLog = writeChangeLog;
        }
    }

    public class HideOnScreenProperty : Attribute
    {
    }

    public class DefaultValueAttribute : Attribute
    {
        public object DefaultValue { get; private set; }

        public DefaultValueAttribute(object defaultValue)
        {
            DefaultValue = defaultValue;
        }
    }
}
