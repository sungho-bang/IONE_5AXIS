using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FAFramework.Utility.SamsungTPLog
{
    public class LotEventLog : LogBase
    {
        public string DeviceID { get; set; }
        public string LogType { get; private set; }
        public string EventID { get; set; }
        public string LotID { get; set; }
        public string FlowRecipeID { get; set; }
        public string CarrierID { get; set; }

        public LotEventLog()
        {
            LogType = "LEH";
        }

        public override string ToString()
        {
            AppendElement(DeviceID);
            AppendElement(LogType);
            AppendElement(EventID);
            AppendElement(LotID);
            AppendElement(FlowRecipeID);
            AppendElement(CarrierID);

            return ToLog();
        }
    }
}
