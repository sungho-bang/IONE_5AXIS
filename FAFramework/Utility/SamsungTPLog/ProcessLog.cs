using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FAFramework.Utility.SamsungTPLog
{
    public class ProcessLog : LogBase
    {
        public string DeviceID { get; set; }
        public string LogType { get; private set; }
        public string EventID { get; set; }
        public string Status { get; set; }
        public string MaterialID { get; set; }
        public string LotID { get; set; }
        public string RecipeID { get; set; }

        public ProcessLog()
        {
            LogType = "PRC";
        }

        public override string ToString()
        {
            AppendElement(DeviceID);
            AppendElement(LogType);
            AppendElement(EventID);
            AppendElement(Status);
            AppendElement(MaterialID);
            AppendElement(LotID);
            AppendElement(RecipeID);

            return ToLog();
        }
    }
}
