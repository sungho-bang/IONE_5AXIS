using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary;
using FALibrary.Part;
using FALibrary.Alarm;
using FAFramework.Utility;

namespace FAFramework.Manager
{
    public class MachineManager
    {
        private static readonly int MACHINE_TYPE_MODULE = 2;
        private static readonly int MACHINE_TYPE_PART = 3;

        private static volatile MachineManager _instance = null;
        private static object syncRoot = new Object();

        private List<FAMachine> _partList = new List<FAMachine>();
        private List<FAMachine> _moduleList = new List<FAMachine>();
        private SortedList<int, int> _partIDList = new SortedList<int, int>();
        private SortedList<int, int> _moduleIDList = new SortedList<int, int>();

        private MachineManager()
        {
        }

        public static MachineManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                            _instance = new MachineManager();
                    }
                }

                return _instance;
            }
        }

        public void GiveIDToPart()
        {
            foreach (var item in _partList)
            {
                if (_partIDList.ContainsKey(item.MachineID) == false)
                {
                    _partIDList.Add(item.MachineID, item.MachineID);
                }
            }

            foreach (var item in _partList)
            {
                if (item.MachineID == 0)
                {
                    item.MachineID = CreatePartID();
                }
            }
        }

        public void GiveIDToModule()
        {
            foreach (var item in _moduleList)
            {
                if (_moduleIDList.ContainsKey(item.MachineID) == false)
                {
                    _moduleIDList.Add(item.MachineID, item.MachineID);
                }
            }

            foreach (var item in _moduleList)
            {
                if (item.MachineID == 0)
                {
                    item.MachineID = CreateModuleID();
                }
            }
        }

        public void AddPart(FAMachine obj)
        {
            _partList.Add(obj);
        }

        public void AddModule(FAMachine obj)
        {
            _moduleList.Add(obj);
        }

        public int CreatePartID()
        {
            int id = 0;

            if (_partIDList.Count > 0)
                id = _partIDList.Last().Value + 1;
            else
                id = 1;

            _partIDList.Add(id, id);

            return id;
        }

        public int CreateModuleID()
        {
            int id = 0;

            if (_moduleIDList.Count > 0)
                id = _moduleIDList.Last().Value + 1;
            else
                id = 1;

            _moduleIDList.Add(id, id);

            return id;
        }

        public void SetAlarmIDToParts(bool allReset)
        {
            foreach (var item in _partList)
            {
                SetAlarmIDToMachine(MACHINE_TYPE_PART, item, allReset);
            }
        }

        public void SetAlarmIDToModules(bool allReset)
        {
            foreach (var item in _moduleList)
            {
                SetAlarmIDToMachine(MACHINE_TYPE_MODULE, item, allReset);
            }
        }

        public void AddAlarmOfParts()
        {
            foreach (var item in _partList)
            {
                AddAlarmOfMachine(item, item.FullName + " ");
            }
        }

        public void AddAlarmOfModules()
        {
            foreach (var item in _moduleList)
            {
                AddAlarmOfMachine(item, item.FullName + " ");
            }
        }

        private void SetAlarmIDToMachine(int machineTypeID, FAMachine machine, bool allReset)
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();

            foreach (var propInfo in machine.GetType().GetProperties())
            {
                foreach (var attr in Attribute.GetCustomAttributes(propInfo, typeof(FAAttribute), false))
                {
                    FAAttribute faAttr = (FAAttribute)attr;
                    if (faAttr.GroupName == "Alarm")
                    {
                        int alarmNo = 0;

                        if (allReset == false)
                        {
                            alarmNo = (int)propInfo.GetValue(machine, null);
                        }

                        dic.Add(propInfo.Name, alarmNo);

                        break;
                    }
                }
            }

            var machineType = machine.GetType();

            var keys = dic.Keys.ToList();
            for (int i = 0; i < keys.Count; i++)
            {
                var property = machineType.GetProperty(keys[i]);
                int propValue = dic[keys[i]];

                if (propValue > 0)
                    continue;

                for (int alarm = 1; alarm < 999; alarm++)
                {
                    int alarmID = machineTypeID * 1000000 + machine.MachineID * 1000 + alarm;

                    if (dic.Values.Contains(alarmID) == false)
                    {
                        property.SetValue(machine, alarmID, null);
                        dic[keys[i]] = alarmID;
                        break;
                    }
                }
            }
        }

        private void AddAlarmOfMachine(FAMachine machine, string preName)
        {
            foreach (var propInfo in machine.GetType().GetProperties())
            {
                foreach (var attr in Attribute.GetCustomAttributes(propInfo, typeof(FAAttribute), false))
                {
                    FAAttribute faAttr = (FAAttribute)attr;
                    if (faAttr.GroupName == "Alarm")
                    {
                        int alarmNo = (int)propInfo.GetValue(machine, null);

                        if (alarmNo == 0) continue;

                        if (FAAlarmManager.Instance.Items.ContainsKey(alarmNo) == false)
                        {
                            try
                            {
                                DefaultAlarmInfo alarmInfo = (DefaultAlarmInfo)Attribute.GetCustomAttribute(propInfo, typeof(DefaultAlarmInfo), false);
                                var culture = StringResourceManager.Instance.CurrentCultureInstance;
                                var cultureName = culture == null ? string.Empty : culture.Name;
                                AlarmDescription alarmDescription = null;
                                foreach (var item in Attribute.GetCustomAttributes(propInfo, typeof(AlarmDescription), false))
                                {
                                    var tempAlarmDescription = item as AlarmDescription;
                                    if (string.Equals(tempAlarmDescription.Culture, cultureName, StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        alarmDescription = tempAlarmDescription;
                                        break;
                                    }
                                }

                                if (alarmDescription == null)
                                {
                                    var temp = Attribute.GetCustomAttribute(propInfo, typeof(AlarmDescription), false);
                                    if (temp != null)
                                    {
                                        alarmDescription = (AlarmDescription)temp;
                                    }
                                }

                                FAAlarm alarm = new FAAlarm();
                                alarm.AlarmNo = alarmNo;
                                alarm.AlarmName = "[" + preName.Trim() + "] " + propInfo.Name;
                                alarm.Description = "[" + preName.Trim() + "] " + propInfo.Name;
                                if (alarmInfo != null)
                                {
                                    alarm.Type = alarmInfo.AlarmType;
                                    alarm.Status = alarmInfo.AlarmStatus;
                                    alarm.Solution = "[" + preName.Trim() + "] ";
                                    if (alarmDescription != null)
                                    {
                                        alarm.Solution += alarmDescription.Solution;
                                        alarm.AlarmName = "[" + preName.Trim() + "] " + alarmDescription.Name;
                                    }
                                }

                                FAAlarmManager.Instance.Items.Add(alarmNo, alarm);
                            }
                            catch
                            {
                            }
                        }

                        break;
                    }
                }
            }
        }
    }
}
