using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FALibrary.Alarm
{
    public class FAAlarmEventArgs : EventArgs
    {
        public FAAlarm Alarm { get; set; }
        public string Message { get; set; }
        public string DebugInfo { get; set; }
    }
}
