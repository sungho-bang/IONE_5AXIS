using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAFramework.Utility.SamsungTPLog.ParameterClass
{
    public class LogParamData
    {
        public string Name { get; set; }
        public Func<object> GetData { get; set; }
        public LogParamData(string name, Func<object> getData)
        {
            Name = name;
            GetData = getData;
        }
    }
}
