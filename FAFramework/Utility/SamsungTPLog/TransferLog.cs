using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FAFramework.Utility.SamsungTPLog
{
    public class TransferLog : LogBase
    {
        public string DeviceID { get; set; }
        public string LogType { get; private set; }
        public string EventID { get; set; }
        public string Status { get; set; }
        public string MaterialID { get; set; }
        public string MaterialType { get; set; }
        public string FromDevice { get; set; }
        public string ToDevice { get; set; }

        public TransferLog()
        {
            LogType = "XFR";
        }

        public override string ToString()
        {
            AppendElement(DeviceID);
            AppendElement(LogType);
            AppendElement(EventID);
            AppendElement(Status);
            AppendElement(MaterialID);
            AppendElement(MaterialType);
            AppendElement(FromDevice);
            AppendElement(ToDevice);

            return ToLog();
        }
    }
}
