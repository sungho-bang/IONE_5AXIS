using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FAFramework.ConfigClasses
{
    public class GlobalConst
    {
        public static string ROOT_PATH = AppDomain.CurrentDomain.BaseDirectory;
        public static string CONFIG_PATH = System.IO.Path.Combine(ROOT_PATH, "config\\");
        public static string PREVIOUS_BAKCUP_CONFIG_PATH = System.IO.Path.Combine(ROOT_PATH, "config\\");
        public static string CONFIG_BACKUP_PATH = @"c:\backup\config\";
        public static bool CHECK_EXIST_DEVICE_IN_DEVICELIST = true;

        public const int ALARM = 0;
        public const int WARNING = 1;
        public const int ALARM_TYPE_MACHINE = 0;
        public const int ALARM_TYPE_MATERIAL = 1;
        public const int ALARM_TYPE_HUMAN = 2;
        public const int ALARM_TYPE_METHOD = 3;
        public const int LAMP_NONE = 0;
        public const int LAMP_OFF = 1;
        public const int LAMP_ON = 2;
        public const int LAMP_SWITCH = 3;
        public const int BUZZER_NONE = 1;
        public const int BUZZER_OFF = 1;
        public const int BUZZER_ON = 2;
        public const int BUZZER_SWITCH = 3;
        public const int BUZZER_MELODY_1 = 4;
        public const int BUZZER_MELODY_2 = 5;
        public const int BUZZER_MELODY_3 = 6;
        public const int BUZZER_MELODY_4 = 7;
    }
}