using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FAFramework.Utility.SamsungTPLog
{
    public class AlarmLog : LogBase
    {
        public string DeviceID { get; set; }
        public string LogType { get; private set; }
        public string AlarmCode { get; set; }
        public string EventID { get; set; }
        public string Status { get; set; }

        public AlarmLog()
        {
            LogType = "ALM";
        }

        public override string ToString()
        {
            AppendElement(DeviceID);
            AppendElement(LogType);
            AppendElement(AlarmCode);
            AppendElement(EventID);
            AppendElement(Status);

            return ToLog();
        }
    }
}
