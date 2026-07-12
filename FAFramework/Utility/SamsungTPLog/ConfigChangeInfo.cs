using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FAFramework.Utility.SamsungTPLog
{
    public class ConfigChangeInfo
    {
        public string Name { get; private set; }
        public string DeviceID { get; private set; }
        public object OldValue { get; private set; }

        public ConfigChangeInfo(string name, string deviceID, object oldValue)
        {
            Name = name;
            DeviceID = deviceID;
            OldValue = oldValue;
        }

        public void SetValue(object value)
        {
            var log = new ConfigurationLog();
            log.DeviceID = DeviceID;
            log.CfgID = "CHANGE";
            log.AddData(Name, new object[] { OldValue, value });
            Manager.TPLogManager.Instance.WriteLog(log);
            OldValue = value;
        }
    }
}
