using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FALibrary;

namespace FAFramework.Utility.SamsungTPLog
{
    public static class LogUtility
    {
        public static void RegisterConfigChangeEventHandler(ObjectPropertyInfo info)
        {
            foreach (var item in info.Properties)
            {
                if (info.Value is FAObject &&
                    item.Value != null &&
                    item.Observable)
                {
                    var propertyName = item.PropertyName;
                    var owner = info.Value as FAObject;
                    var configInfo = new FAFramework.Utility.SamsungTPLog.ConfigChangeInfo(
                        item.PropertyName,
                        item.Description,
                        item.Value);

                    owner.PropertyChanged +=
                        (o, e) =>
                        {
                            if (propertyName != e.PropertyName) return;
                            var newValue = owner.GetType().GetProperty(e.PropertyName).GetValue(o, null);
                            configInfo.SetValue(newValue);
                        };
                }

                RegisterConfigChangeEventHandler(item);
            }
        }

        public static void AddFunctionLog(this FALibrary.Part.FAPartAction partAction,
            Module.FAModule module,
            SamsungTPLog.ParameterClass.FunctionLogParam param,
            params SamsungTPLog.ParameterClass.LogParamData[] datas)
        {
            SamsungTPLog.FunctionLog createFunctionLog(string status)
            {
                SamsungTPLog.FunctionLog log = new SamsungTPLog.FunctionLog();
                log.DeviceID = param.DeviceName;
                log.EventID = param.EventName;
                log.Status = status;
                log.MaterialID = param.GetMaterialName?.Invoke();
                log.MaterialType = param.MaterialType;
                foreach (var item in datas)
                    log.AddData(item.Name, item.GetData?.Invoke());

                return log;
            }

            partAction.Sequence.OnStart +=
                (obj, e) =>
                {
                    var seq = obj as FALibrary.Sequence.FASequence;
                    if (module.GetAllSequences().Contains(seq.Caller))
                    {
                        Manager.TPLogManager.Instance.WriteLog(
                            createFunctionLog("START"));
                    }
                };

            partAction.Sequence.OnTerminate +=
                (obj, e) =>
                {
                    var seq = obj as FALibrary.Sequence.FASequence;
                    if (module.GetAllSequences().Contains(seq.Caller))
                    {
                        Manager.TPLogManager.Instance.WriteLog(
                            createFunctionLog("END"));
                    }
                };
        }
    }
}
