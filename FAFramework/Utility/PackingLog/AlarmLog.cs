using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FAFramework.Utility.PackingLog
{
    public class AlarmLog : LogBase
    {
        public int AlarmCode { get; set; }
        public string UnitName { get; set; }
        public string AlarmDescription { get; set; }

        public override string ToString()
        {
            AppendElement(AlarmCode.ToString());
            AppendElement(UnitName);
            AppendElement(AlarmDescription);

            return ToLog();
        }
    }
}
