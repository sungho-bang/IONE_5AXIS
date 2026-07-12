using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;
using FAFramework.Utility;

namespace FAFramework.Equipment
{
    public enum EDeviceOpenStatus
    {
        None, Success, Fail
    }

    public class MainEquipment : INotifyPropertyChanged
    {
        static object _saveLock = new object();

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public enum ShiftType
        {
            Day,
            Swing,
            GY
        }

        #region Field, Property, Event
        public static readonly TimeSpan DAY_SHIFT_START_TIME = new TimeSpan(6, 00, 00);
        public static readonly TimeSpan SWING_SHIFT_START_TIME = new TimeSpan(14, 00, 00);
        public static readonly TimeSpan GY_SHIFT_START_TIME = new TimeSpan(22, 00, 00);
        public static readonly TimeSpan MIDNIGHT_TIME = new TimeSpan(24, 00, 00);
        public static readonly TimeSpan OVERLOADED_FPS = new TimeSpan(0, 0, 0, 0, 100);

        private string _version;
        public string Version
        {
            get { return _version; }
            set
            {
                if (_version == value) return;
                _version = value;
                NotifyPropertyChanged("Version");
            }
        }

        public static bool SIMULATION_MODE { get; set; }

        private bool _useVirtualKeyboard;
        public bool UseVirtualKeyboard
        {
            get { return _useVirtualKeyboard; }
            set
            {
                if (_useVirtualKeyboard == value) return;
                _useVirtualKeyboard = value;
                NotifyPropertyChanged("UseVirtualKeyboard");
            }
        }

        private static volatile MainEquipment _instance = null;
        private static object syncRoot = new Object();

        public FAFramework.Manager.DeviceManager DeviceManager { get; private set; }
        public EquipmentManager EquipmentManagerInstance { get; private set; }
        public Manager.AlarmResourceManager AlarmResourceManager { get; private set; }

        private Thread _thread;
        private bool _threadStop = false;
        private DateTime _oldTime = DateTime.Now;

        public event EventHandler<FALibrary.FAGenericEventArgs<ShiftType>> OnChangedShift = delegate { };
        public event EventHandler<FALibrary.FAGenericEventArgs<DateTime>> OnChangedMinute = delegate { };

        private List<object> _debugList = new List<object>();
        public List<object> DebugList
        {
            get { return _debugList; }
        }

        private EDeviceOpenStatus _deviceOpenStatus = EDeviceOpenStatus.None;
        public EDeviceOpenStatus DeviceOpenStatus
        {
            get { return _deviceOpenStatus; }
            set
            {
                if (_deviceOpenStatus == value) return;
                _deviceOpenStatus = value;
                NotifyPropertyChanged("DeviceOpenStatus");
            }
        }

        private string _deviceOpenFailStatus;
        public string DeviceOpenFailStatus
        {
            get { return _deviceOpenFailStatus; }
            set
            {
                if (_deviceOpenFailStatus == value) return;
                _deviceOpenFailStatus = value;
                NotifyPropertyChanged("DeviceOpenFailStatus");
            }
        }

        private bool _deviceOpenRequest = true;
        public bool DeviceOpenRequest
        {
            get { return _deviceOpenRequest; }
            set
            {
                if (_deviceOpenRequest == value) return;
                _deviceOpenRequest = value;
                NotifyPropertyChanged("DeviceOpenRequest");
            }
        }
        #endregion

        private MainEquipment()
        {
            Manager.LogRetentionSetting.EnsureSettingFile();

            DeviceManager = new Manager.DeviceManager();
            EquipmentManagerInstance = new EquipmentManager();

            Version = GUI.WindowVersionInfo.Version;

            LoadKeyboardSetting();
        }

        public static MainEquipment Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                            _instance = new MainEquipment();
                    }
                }

                return _instance;
            }
        }

        public void Initialize()
        {
            AlarmResourceManager = new Manager.AlarmResourceManager();

            Manager.StringResourceManager.Instance.OnChangedCulture +=
                delegate
                {
                    AlarmResourceManager.Load();
                };

            AlarmResourceManager.Load();

            string deviceListFilename = System.IO.Path.Combine(FAFramework.ConfigClasses.GlobalConst.CONFIG_PATH, "DeviceList.xml");
            DeviceManager.Load(deviceListFilename);
            EquipmentManagerInstance.Initialize();
            Manager.MachineManager.Instance.GiveIDToModule();
            Manager.MachineManager.Instance.GiveIDToPart();

            if (SIMULATION_MODE)
            {
                foreach (var eqp in EquipmentManagerInstance.EquipmentList.Values)
                {
                    foreach (var subUnit in eqp.SubUnitList.Values)
                    {
                        foreach (var partInfo in subUnit.PartList)
                        {
                            partInfo.Part.SimulationMode = true;
                        }
                    }
                }
            }

            CreateDebugList();
        }
        
        public void LoadBackup()
        {
            lock (_saveLock)
            {
                EquipmentManagerInstance.LoadBackup();
            }            
        }

        public void Save()
        {
            lock (_saveLock)
            {
                Manager.LogRetentionSetting.DeleteExpiredFiles(
                    FAFramework.ConfigClasses.GlobalConst.CONFIG_BACKUP_PATH,
                    Manager.LogRetentionSetting.KEY_CONFIG_BACKUP,
                    true);
                var backupPath = Path.Combine(FAFramework.ConfigClasses.GlobalConst.CONFIG_BACKUP_PATH, DateTime.Now.ToString("yyyy-MM-dd_HHmmss"));
                CopyAllData(backupPath);
                EquipmentManagerInstance.Save();
                AlarmResourceManager.Save();
            }
        }

        public void SaveModules(params Module.FAModule[] modules)
        {
            EquipmentManagerInstance.SaveModules(modules);
        }

        public void CopyAllData(string backupPath)
        {
            try
            {

                Utility.FileUtility.CopyFolder(FAFramework.ConfigClasses.GlobalConst.CONFIG_PATH, backupPath);
            }
            catch (Exception e)
            {
                Manager.LogManager.Instance.WriteSystemLog("CopyAllData Fail\n" + e.ToString());
            }
        }

        public void Start()
        {
            _thread = new Thread(
                delegate ()
                {

                    if (SIMULATION_MODE == true)
                    {
                        DeviceOpenStatus = EDeviceOpenStatus.Success;
                    }
                    else if (SIMULATION_MODE == false)
                    {
                        try
                        {
                            Manager.OpenFailDeviceInfo openFailDeviceInfo = null;

                            while (DeviceOpenStatus != EDeviceOpenStatus.Success)
                            {
                                if (DeviceOpenRequest == false)
                                {
                                    try
                                    {
                                        // Emergency Signal을 처리하기 위해 Execute 한다.
                                        Execute();
                                    }
                                    catch
                                    {

                                    }

                                    Thread.Sleep(10);
                                    continue;
                                }

                                DeviceOpenStatus = EDeviceOpenStatus.None;

                                List<string> openDeviceList = null;
                                if (openFailDeviceInfo != null)
                                {
                                    openDeviceList = new List<string>();
                                    openDeviceList.AddRange(openFailDeviceInfo.Items.Select(x => x.DeviceName));
                                }

                                if (DeviceManager.Open(openDeviceList, out openFailDeviceInfo) == false)
                                {
                                    DeviceOpenRequest = false;
                                    DeviceOpenStatus = EDeviceOpenStatus.Fail;
                                    DeviceOpenFailStatus = openFailDeviceInfo.ToString();
                                }
                                else
                                {
                                    DeviceOpenRequest = false;
                                    DeviceOpenStatus = EDeviceOpenStatus.Success;
                                    break;
                                }

                                Thread.Sleep(1);
                            }
                        }
                        catch (Exception e)
                        {
                            Stop();
                            Manager.LogManager.Instance.WriteSystemLog($"Equipment Start Fail. {e.ToString()}");
                            Manager.MessageWindowManager.Instance.Show("Device Loading Fail", e.ToString());

                            App.Current.Dispatcher.BeginInvoke(
                                new Action(
                                    delegate
                                    {
                                        System.Windows.Application.Current.Shutdown();
                                    }), null);
                        }
                    }

                    while (_threadStop == false)
                    {
                        Execute();
                    }

                    try
                    {
                        if (SIMULATION_MODE == false)
                            DeviceManager.Close();
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e.Message);
                        Trace.Flush();
                    }
                });

            _thread.SetApartmentState(ApartmentState.STA);
            _thread.Priority = ThreadPriority.Highest;
            _thread.Start();
        }

        public void Stop()
        {
            foreach (var eqp in EquipmentManagerInstance.EquipmentList.Values)
            {
                eqp.DisposeEquipment();
                eqp.WriteTerminateMessage();
            }

            Manager.LogManager.Instance.Run = false;
            Manager.TPLogManager.Instance.Run = false;
            foreach (var equip in Equipment.MainEquipment.Instance.EquipmentManagerInstance.EquipmentList)
            {
                equip.Value.MTBIManager.Run = false;
                equip.Value.ProductOutput.Run = false;

                if (equip.Value.PackingLogManager != null)
                    equip.Value.PackingLogManager.Run = false;
            }

            _threadStop = true;
        }

        public static ShiftType GetShift(TimeSpan time)
        {
            if (time >= DAY_SHIFT_START_TIME &&
                time < SWING_SHIFT_START_TIME)
                return ShiftType.Day;
            else if (time >= SWING_SHIFT_START_TIME &&
                time < GY_SHIFT_START_TIME)
                return ShiftType.Swing;
            else
                return ShiftType.GY;
        }

        private int _fpsCount = 0;
        private DateTime _fpsOldTime = DateTime.Now;

        private void Execute()
        {
            DateTime time = DateTime.Now;

            if (time.Minute != _oldTime.Minute)
                OnChangedMinute(this, new FALibrary.FAGenericEventArgs<DateTime>(time));

            var oldShiftType = GetShift(_oldTime.TimeOfDay);
            var currentShiftType = GetShift(time.TimeOfDay);
            if (oldShiftType != currentShiftType)
                OnChangedShift(this, new FALibrary.FAGenericEventArgs<ShiftType>(currentShiftType));

            if (SIMULATION_MODE == false)
                DeviceManager.ReadWrite();

            foreach (var item in EquipmentManagerInstance.EquipmentList)
            {
                item.Value.Execute();
            }

            _fpsCount++;

            if (_fpsCount >= 10)
            {
                foreach (var item in EquipmentManagerInstance.EquipmentList)
                {
                    item.Value.FPS = new TimeSpan((DateTime.Now - _fpsOldTime).Ticks / 10);
                    if (item.Value.FPS > OVERLOADED_FPS)
                        Manager.LogManager.Instance.WriteTraceLog(item.Value, string.Format("Overload FPS. {0}", item.Value.FPS.ToString()));
                }

                _fpsCount = 0;
                _fpsOldTime = DateTime.Now;
            }

            _oldTime = time;
        }

        private void LoadKeyboardSetting()
        {
            string filepath = Path.Combine(ConfigClasses.GlobalConst.CONFIG_PATH, "keyboard_setting.txt");

            try
            {
                string content = File.ReadAllText(filepath);
                if (content.Trim() == "UseVirtualKeyboard")
                    UseVirtualKeyboard = true;
                else
                    UseVirtualKeyboard = false;
            }
            catch
            {
            }
        }

        private void CreateDebugList()
        {
            foreach (var item in EquipmentManagerInstance.EquipmentList)
            {
                PropertyContainer pc = new PropertyContainer();
                pc.Value = ObjectElementExtractor.ExtractElement(item.Value, string.Empty, true, null, null, null);
                pc.Name = item.Key;
                DebugList.Add(pc);
            }
        }

        public void CreateDumpFile()
        {
            try
            {
                System.Xml.Linq.XDocument doc = new System.Xml.Linq.XDocument();
                System.Xml.Linq.XElement xel = new System.Xml.Linq.XElement("DumpList");

                foreach (var item in EquipmentManagerInstance.EquipmentList)
                {
                    var list = new List<object>();
                    var xml = Utility.UtilityClass.ObjectToXml(item.Value);
                    if (xml != null)
                    {
                        var equipXml = new System.Xml.Linq.XElement("Equipment");
                        var name = new System.Xml.Linq.XElement("Name");
                        name.Value = item.Key;

                        var value = new System.Xml.Linq.XElement("Value");
                        value.Add(xml);

                        equipXml.Add(name);
                        equipXml.Add(value);

                        xel.Add(equipXml);
                    }
                }

                doc.Add(xel);

                string filename = DateTime.Now.ToString("yyyy-MM-dd_HHmmss") + ".xml";
                string path = System.IO.Path.Combine(ConfigClasses.GlobalConst.ROOT_PATH, "DumpFile");
                if (System.IO.Directory.Exists(path) == false)
                    System.IO.Directory.CreateDirectory(path);

                string pathAndFilename = System.IO.Path.Combine(path, filename);

                doc.Save(pathAndFilename);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.ToString());
            }
        }

        public void CreateDumpFileInOtherThread()
        {
            try
            {
                Thread thread = new Thread(
                    delegate (object obj)
                    {
                        CreateDumpFile();
                    });
                thread.Start();
            }
            catch
            {
            }
        }
    }
}
