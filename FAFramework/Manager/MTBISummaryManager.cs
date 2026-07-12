using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary;
using FALibrary.Utility;
using FAFramework.Equipment;
using System.IO;
using System.Xml.Linq;
using System.Threading;

namespace FAFramework.Manager
{
    public class MTBISummaryManager : FAObject
    {
        private readonly TimeSpan TWO_HOUR = new TimeSpan(2, 0, 0);
        private readonly TimeSpan DAY_POLE = new TimeSpan(22, 0, 0);
        private readonly TimeSpan DAY = new TimeSpan(24, 0, 0);

        private Queue<Action> _logQueue = new Queue<Action>();
        private static object threadRoot = new Object();
        private string _rootPath = string.Empty;
        private string RootPath
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(_rootPath))
                    {
                        var rootPath = Path.Combine(FAFramework.ConfigClasses.GlobalConst.ROOT_PATH, "Log");
                        if (EquipmentInstance != null)
                            _rootPath = Path.Combine(rootPath, EquipmentInstance.Name, "MTBI");
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.Write(e.ToString());
                }

                return _rootPath;
            }
        }
        private DateTime _lastSaveTime = DateTime.Now;

        public bool Run { get; set; }

        public Equipment.EquipmentBase EquipmentInstance { get; set; }

        private EquipmentState _lastState;
        [FAAttribute("")]
        public EquipmentState LastState
        {
            get { return _lastState; }
            set
            {
                if (_lastState == value) return;
                _lastState = value;
                NotifyPropertyChanged("LastState");
            }
        }

        private FATime _autoSaveTime = new FATime(FATimeType.second, 5);
        [FAAttribute("")]
        public FATime AutoSaveTime
        {
            get { return _autoSaveTime; }
            set
            {
                _autoSaveTime = value;
            }
        }

        private DateTime _poleTime;
        [FAAttribute("")]
        public DateTime PoleTime
        {
            get { return _poleTime; }
            set
            {
                _poleTime = value;
                NotifyPropertyChanged("PoleTime");
            }
        }

        private DateTime _lastUpdateTime = DateTime.Now;
        [FAAttribute("")]
        public DateTime LastUpdateTime
        {
            get { return _lastUpdateTime; }
            set
            {
                _lastUpdateTime = value;
                NotifyPropertyChanged("LastUpdateTime");
            }
        }

        private TimeSpan _lastSummaryTime;
        [FAAttribute("")]
        public TimeSpan LastSummaryTime
        {
            get { return _lastSummaryTime; }
            set
            {
                _lastSummaryTime = value;
                NotifyPropertyChanged("LastSummaryTime");
            }
        }

        private TimeSpan _runTime;
        [FAAttribute("")]
        public TimeSpan RunTime
        {
            get { return _runTime; }
            set
            {
                _runTime = value;
                NotifyPropertyChanged("RunTime");
            }
        }

        private TimeSpan _runDownTime;
        [FAAttribute("")]
        public TimeSpan RunDownTime
        {
            get { return _runDownTime; }
            set
            {
                _runDownTime = value;
                NotifyPropertyChanged("RunDownTime");
            }
        }

        private TimeSpan _stopTime;
        [FAAttribute("")]
        public TimeSpan StopTime
        {
            get { return _stopTime; }
            set
            {
                _stopTime = value;
                NotifyPropertyChanged("StopTime");
            }
        }

        private TimeSpan _alarmTime;
        [FAAttribute("")]
        public TimeSpan AlarmTime
        {
            get { return _alarmTime; }
            set
            {
                _alarmTime = value;
                NotifyPropertyChanged("AlarmTime");
            }
        }

        private TimeSpan _mtbi;
        [FAAttribute("")]
        public TimeSpan MTBI
        {
            get { return _mtbi; }
            set
            {
                _mtbi = value;
                NotifyPropertyChanged("MTBI");
            }
        }

        private int _alarmCount;
        [FAAttribute("")]
        public int AlarmCount
        {
            get { return _alarmCount; }
            set
            {
                _alarmCount = value;
                NotifyPropertyChanged("AlarmCount");
            }
        }

        public List<string> AlarmList { get; set; }

        public MTBISummaryManager(Equipment.EquipmentBase equipment)
        {
            EquipmentInstance = equipment;
            AlarmList = new List<string>();
            Load();

            Run = true;

            Thread thread = new Thread(
                    delegate ()
                    {
                        while (Run)
                        {
                            Action log;

                            if (_logQueue.Count > 0)
                            {
                                lock (threadRoot)
                                {
                                    log = _logQueue.Dequeue();
                                }

                                log();
                            }

                            Thread.Sleep(10);
                        }
                    });

            thread.Start();
        }

        public void AddAlarm(FALibrary.Alarm.FAAlarm alarm)
        {
            AlarmList.Add(alarm.AlarmNo + ", " + alarm.AlarmName);
            AlarmCount++;
        }

        public void Clear()
        {
            RunTime = new TimeSpan(0, 0, 0);
            RunDownTime = new TimeSpan(0, 0, 0);
            StopTime = new TimeSpan(0, 0, 0);
            AlarmTime = new TimeSpan(0, 0, 0);
            MTBI = new TimeSpan(0, 0, 0);
            AlarmCount = 0;
            PoleTime = DateTime.Now;
            LastSummaryTime = new TimeSpan(0, 0, 0);
            AlarmList.Clear();
        }

        public void SetState(Equipment.EquipmentState status)
        {
            var now = DateTime.Now;

            var oldShift = Equipment.MainEquipment.GetShift(LastUpdateTime.TimeOfDay);
            var currentShift = Equipment.MainEquipment.GetShift(now.TimeOfDay);
            if (oldShift != currentShift ||
                (now - LastUpdateTime) > DAY)
            {
                if (currentShift == MainEquipment.ShiftType.GY)
                {
                    Clear();
                }
            }

            if (status != LastState)
            {
                bool stopState = status == EquipmentInstance.StatePreStop || status == EquipmentInstance.StateStop;
                bool lastStateIsAlarm = LastState == EquipmentInstance.StateAlarm;

                if ((stopState && lastStateIsAlarm) == false)
                    PoleTime = now;
            }

            if (status == EquipmentInstance.StateRun)
            {
                if (LastState == EquipmentInstance.StateRun)
                {
                    RunTime = LastSummaryTime + (now - PoleTime);
                }
                else
                {
                    LastSummaryTime = RunTime;
                    LastState = status;
                }
            }
            else if (status == EquipmentInstance.StateRundown)
            {
                if (LastState == EquipmentInstance.StateRundown)
                {
                    RunDownTime = LastSummaryTime + (now - PoleTime);
                    //StopTime = LastSummaryTime + (now - PoleTime);
                }
                else
                {
                    LastSummaryTime = RunDownTime;
                    LastState = status;
                }
            }
            else if (status == EquipmentInstance.StatePreAlarm ||
                status == EquipmentInstance.StatePreWarning)
            {
                if (LastState == EquipmentInstance.StateRun ||
                    LastState == EquipmentInstance.StateRundown)
                {
                    LastState = EquipmentInstance.StateRun;
                }
                else
                {
                    LastState = EquipmentInstance.StateStop;
                }
            }
            else if (status == EquipmentInstance.StateAlarm ||
                status == EquipmentInstance.StateWarning)
            {
                if (LastState == EquipmentInstance.StateRun ||
                    LastState == EquipmentInstance.StateRundown)
                {
                    LastSummaryTime = AlarmTime;
                    LastState = EquipmentInstance.StateAlarm;
                }

                if (LastState == EquipmentInstance.StateAlarm)
                {
                    AlarmTime = LastSummaryTime + (now - PoleTime);
                }
            }
            else if (status == EquipmentInstance.StatePreStop ||
                status == EquipmentInstance.StateStop)
            {
                if (LastState == EquipmentInstance.StateAlarm)
                {
                    AlarmTime = LastSummaryTime + (now - PoleTime);
                }
            }
            else
            {
                LastState = EquipmentInstance.StateStop;
            }

            if (now - _lastSaveTime > AutoSaveTime.Time)
            {
                _lastSaveTime = now;
                Save(now);
            }

            LastUpdateTime = now;

            TimeSpan totalTime = now.TimeOfDay;

            if (now.TimeOfDay < DAY_POLE)
                totalTime = now.AddHours(2).TimeOfDay;
            else
                totalTime = now.TimeOfDay - DAY_POLE;

            StopTime = totalTime - RunTime - AlarmTime - RunDownTime;

            if (StopTime < TimeSpan.Zero)
                StopTime = TimeSpan.Zero;

            if (AlarmCount <= 0)
            {
                MTBI = RunTime;
            }
            else
            {
                try
                {
                    MTBI = new TimeSpan(RunTime.Ticks / (AlarmCount + 1));
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.Write(e.ToString());
                }
            }
        }

        public void Load()
        {
            Clear();

            var date = DateTime.Now;
            LastUpdateTime = date;

            var correctionDate = date.AddHours(2);

            string path = Path.Combine(RootPath, correctionDate.ToString("yyyy"), correctionDate.ToString("MM"));
            string pathAndFileName = Path.Combine(path, correctionDate.ToString(@"yyyy-MM-dd") + ".log");

            if (Directory.Exists(path) == false) return;
            if (File.Exists(pathAndFileName) == false) return;

            try
            {
                XElement xml = XElement.Load(pathAndFileName);

                RunTime = LoadTime(xml, "RunTime");
                RunDownTime = LoadTime(xml, "RunDownTime");
                AlarmTime = LoadTime(xml, "AlarmTime");
                LoadAlarmList(xml);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.Write(e.ToString());
            }
        }

        private TimeSpan LoadTime(XElement xml, string elementName)
        {
            TimeSpan result = new TimeSpan(0, 0, 0);

            string str = xml.Element(elementName).Value;
            if (str == null) return result;

            if (TimeSpan.TryParse(str, out result))
            {
                return result;
            }
            else
                return new TimeSpan(0, 0, 0);
        }

        private void LoadAlarmList(XElement xml)
        {
            try
            {
                AlarmCount = 0;
                AlarmList.Clear();

                XElement alarm = xml.Element("Alarm");
                if (alarm == null) return;

                var alarmList = alarm.Element("AlarmList");
                if (alarmList == null) return;

                foreach (var item in alarmList.Elements())
                {
                    AlarmList.Add(item.Value);
                }

                AlarmCount = AlarmList.Count;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.Write(e.ToString());
            }
        }

        public void Save(DateTime date)
        {
            lock (threadRoot)
            {
                _logQueue.Enqueue(
                    delegate
                    {
                        try
                        {
                            var saveData = date;
                            var shift = Equipment.MainEquipment.GetShift(LastUpdateTime.TimeOfDay);
                            if (shift == Equipment.MainEquipment.ShiftType.GY &&
                                saveData.TimeOfDay > Equipment.MainEquipment.GY_SHIFT_START_TIME)
                            {
                                saveData = saveData.AddDays(1);
                            }

                            string path = Path.Combine(RootPath, saveData.ToString("yyyy"), saveData.ToString("MM"));
                            string pathAndFileName = Path.Combine(path, saveData.ToString(@"yyyy-MM-dd") + ".log");
                            if (Directory.Exists(path) == false)
                                Directory.CreateDirectory(path);

                            var doc = new XDocument();
                            doc.Add(GetSaveData());
                            doc.Save(pathAndFileName);
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Trace.Write(e.ToString());
                        }
                    });
            };
        }

        public XElement GetSaveData()
        {
            XElement xml = new XElement("Root");
            xml.Add(new XElement("RunTime", RunTime.ToString()));
            xml.Add(new XElement("RunDownTime", RunDownTime.ToString()));
            xml.Add(new XElement("StopTime", StopTime.ToString()));
            xml.Add(new XElement("AlarmTime", AlarmTime.ToString()));
            xml.Add(new XElement("MTBI", MTBI.ToString()));
            xml.Add(new XElement("Alarm",
                new XElement("Count", AlarmCount),
                SaveAlarmList()));
            xml.Add(new XElement("LastUpdateTime", LastUpdateTime.ToString()));
            return xml;
        }

        private XElement SaveAlarmList()
        {
            XElement result = new XElement("AlarmList");

            foreach (var item in AlarmList)
            {
                result.Add(new XElement("Item", item));
            }

            return result;
        }
    }
}
