using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FAFramework.Utility.SamsungTPLog.ParameterClass
{
    public class FunctionLogParam
    {
        public string DeviceName { get; set; }
        public string EventName { get; set; }
        public Func<string> GetMaterialName { get; set; }
        public string MaterialType { get; set; }
    }
}
