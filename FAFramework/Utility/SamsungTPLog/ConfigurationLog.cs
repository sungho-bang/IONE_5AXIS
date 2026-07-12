using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FAFramework.Utility.SamsungTPLog
{
    public class ConfigurationLog : LogBase
    {
        public string DeviceID { get; set; }
        public string LogType { get; private set; }
        public string CfgID { get; set; }

        public ConfigurationLog()
        {
            LogType = "CFG";
        }

        public override string ToString()
        {
            AppendElement(DeviceID);
            AppendElement(LogType);
            AppendElement(CfgID);

            return ToLog();
        }
    }
}
