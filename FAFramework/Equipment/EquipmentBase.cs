using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection;
using System.ComponentModel;
using FALibrary.Sequence;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using FAFramework.Utility;

namespace FAFramework.Equipment
{
    public enum UserPermissionTypes
    {
        OPERATOR,
        MAINTENANCE,
        MASTER
    }

    public enum RunModeTypes
    {
        DRY_RUN,
        COLD_RUN,
        HOT_RUN
    }

    [Serializable]
    public class UserInfo : FALibrary.FAObject
    {
        private UserPermissionTypes _permission = UserPermissionTypes.OPERATOR;
        public UserPermissionTypes Permission
        {
            get { return _permission; }
            set
            {
                if (_permission == value) return;

                _permission = value;
                NotifyPropertyChanged("Permission");
            }
        }

        private string _name = "User";
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value) return;

                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                if (_password == value) return;

                _password = value;
                NotifyPropertyChanged("Password");
            }
        }

        public UserInfo()
        {
        }

        public UserInfo(UserPermissionTypes permission, string name, string password)
        {
            Permission = permission;
            Name = name;
            Password = password;
        }
    }

    public class EquipmentState : INotifyPropertyChanged
    {
        public delegate void CustomActionDelegate(EquipmentBase equipment, ref EquipmentState state);

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public EquipmentBase Equipment { get; private set; }

        private string _name;
        [FALibrary.FAAttribute("")]
        public string Name
        {
            get { return _name; }
            protected set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        public List<CustomActionDelegate> CustomActions { get; private set; }

        public EventHandler<FALibrary.FAGenericEventArgs<EquipmentState>> OnChangedState;
        public EventHandler OnTurnOffSound;
        public EventHandler OnClearAlarm;
        public Action<FALibrary.Alarm.FAAlarmEventArgs> ProcessAlarm;
        public EquipmentState(EquipmentBase equipment)
        {
            Name = this.GetType().Name;
            Equipment = equipment;
            CustomActions = new List<CustomActionDelegate>();

            OnChangedState +=
                delegate (object sender, FALibrary.FAGenericEventArgs<EquipmentState> e)
                {
                    string oldStatusName = "Null";
                    if (e.Value != null)
                        oldStatusName = e.Value.Name;
                    Manager.LogManager.Instance.WriteTraceLog(Equipment, string.Format("Equipment State Change. {0} -> {1}", oldStatusName, this.Name));
                };
        }

        public void DoCustomAction()
        {
            foreach (var method in CustomActions)
            {
                EquipmentState state = null;
                method(Equipment, ref state);
                if (state != null)
                {
                    Equipment.State = state;
                    return;
                }
            }
        }

        public void Execute()
        {
            DoCustomAction();
        }

        public virtual void RequestStart(EquipmentState oldStatus)
        {
        }

        public virtual void Start(EquipmentState oldStatus)
        {
        }

        public virtual void RequestStop(EquipmentState oldStatus)
        {
        }

        public virtual void Stop(EquipmentState oldStatus)
        {
        }

        public virtual void RequestInitialize(EquipmentState oldStatus)
        {
        }

        public virtual void Initialize(EquipmentState oldStatus)
        {
        }

        public virtual void SetRundown(EquipmentState oldStatus)
        {
        }

        public virtual void RequestEmergency(EquipmentState oldStatus)
        {
        }

        public virtual void SetEmergency(EquipmentState oldStatus)
        {
        }

        public virtual void ReleaseEmergency(EquipmentState oldStatus)
        {
        }

        public virtual void RaiseAlarm(EquipmentState oldStatus)
        {
        }

        public virtual void SetAlarm(EquipmentState oldStatus)
        {
        }

        public virtual void ClearAlarm(EquipmentState oldStatus)
        {
            if (OnClearAlarm != null)
                OnClearAlarm(this, EventArgs.Empty);
        }

        public virtual void TurnOffSound(EquipmentState oldStatus)
        {
            if (OnTurnOffSound != null)
                OnTurnOffSound(this, EventArgs.Empty);
        }

        public virtual void RaiseWarning(EquipmentState oldStatus)
        {
        }

        public virtual void SetWarning(EquipmentState oldStatus)
        {
        }

        public virtual void SetSuspend(EquipmentState oldStatus)
        {
        }

        public virtual void Suspend(EquipmentState oldStatus)
        {
        }

        public virtual void ReleaseSuspend(EquipmentState oldStatus)
        {
        }
    }

    public abstract class EquipmentBase : FALibrary.FAObject
    {
        #region Status
        private EquipmentState _oldState;
        [FALibrary.FAAttribute("Status")]
        public EquipmentState OldState
        {
            get { return _oldState; }
            set
            {
                if (_oldState == value) return;
                _oldState = value;
                NotifyPropertyChanged("OldState");
            }
        }

        private EquipmentState _state;
        [FALibrary.FAAttribute("Status")]
        public EquipmentState State
        {
            get { return _state; }
            set
            {
                if (_state == value) return;

                var oldState = _state;
                OldState = _state;

                _state = value;
                NotifyPropertyChanged("State");

                MainSequenceManager.AllClearState();

                if (_state.OnChangedState != null)
                    _state.OnChangedState(this, new FALibrary.FAGenericEventArgs<EquipmentState>(oldState));
            }
        }

        private UserInfo _currentUser = new UserInfo(UserPermissionTypes.OPERATOR, string.Empty, string.Empty);
        [FALibrary.FAAttribute("Status")]
        public UserInfo CurrentUser
        {
            get { return _currentUser; }
            set
            {
                if (_currentUser == value) return;

                _currentUser = value;
                NotifyPropertyChanged("CurrentUser");
                OnChangedUser(this, new FALibrary.FAGenericEventArgs<UserInfo>(value));
            }
        }

        private RunModeTypes _runMode = RunModeTypes.HOT_RUN;
        [FALibrary.FAAttribute("Status")]
        public RunModeTypes RunMode
        {
            get { return _runMode; }
            set
            {
                if (_runMode == value) return;

                _runMode = value;
                NotifyPropertyChanged("RunMode");
            }
        }

        private bool _testMode;
        [FALibrary.FAAttribute("Status")]
        public bool TestMode
        {
            get { return _testMode; }
            set
            {
                if (_testMode == value) return;

                _testMode = value;
                NotifyPropertyChanged("TestMode");
            }
        }

        private TimeSpan _fps;
        [FALibrary.FAAttribute("Status")]
        public TimeSpan FPS
        {
            get { return _fps; }
            set
            {
                if (_fps == value) return;
                _fps = value;
                NotifyPropertyChanged("FPS");
            }
        }

        private bool _isInitializedOk;
        [FALibrary.FAAttribute("Status")]
        public bool IsInitializedOk
        {
            get { return _isInitializedOk; }
            set
            {
                if (_isInitializedOk == value) return;
                _isInitializedOk = value;
                NotifyPropertyChanged("IsInitializedOk");
            }
        }

        private bool _maintenanceMode;
        [FALibrary.FAAttribute("Status")]
        public bool MaintenanceMode
        {
            get { return _maintenanceMode; }
            set
            {
                if (_maintenanceMode == value) return;
                _maintenanceMode = value;
                NotifyPropertyChanged("MaintenanceMode");
            }
        }
        #endregion

        #region States
        public EquipmentState StatePreRun { get; protected set; }
        public EquipmentState StateRun { get; protected set; }
        public EquipmentState StatePreStop { get; protected set; }
        public EquipmentState StateStop { get; protected set; }
        public EquipmentState StatePreInitialize { get; protected set; }
        public EquipmentState StateInitialize { get; protected set; }
        public EquipmentState StateRundown { get; protected set; }
        public EquipmentState StatePreEmergency { get; protected set; }
        public EquipmentState StateEmergency { get; protected set; }
        public EquipmentState StateEmergencyReset { get; protected set; }
        public EquipmentState StatePreAlarm { get; protected set; }
        public EquipmentState StateAlarm { get; protected set; }
        public EquipmentState StatePreWarning { get; protected set; }
        public EquipmentState StateWarning { get; protected set; }
        public EquipmentState StatePreSuspend { get; protected set; }
        public EquipmentState StateSuspend { get; protected set; }
        #endregion

        [Utility.NotAutoCreateModule]
        public Module.StateSignalModule StateSignalModuleReferance { get; protected set; }

        public string Name { get; set; }
        public FASequenceManager MainSequenceManager { get; private set; }
        public FASequenceManager SubSequenceManager { get; private set; }
        public FASequenceManager SystemBackgroundManager { get; private set; }

        public Manager.MTBISummaryManager _MTBIManager;
        [FALibrary.FAAttribute("Status")]
        public Manager.MTBISummaryManager MTBIManager
        {
            get { return _MTBIManager; }
            set
            {
                _MTBIManager = value;
            }
        }

        private Manager.ProductOutputManager _productOutput = new Manager.ProductOutputManager();
        [FALibrary.FAAttribute("Status")]
        public Manager.ProductOutputManager ProductOutput
        {
            get { return _productOutput; }
            set
            {
                _productOutput = value;
            }
        }

        private Manager.MaterialManagerBase _materialManagerInstance;
        [FALibrary.FAAttribute("Status")]
        public Manager.MaterialManagerBase MaterialManagerInstance
        {
            get { return _materialManagerInstance; }
            set
            {
                _materialManagerInstance = value;
            }
        }

        public Manager.PackingLogManager PackingLogManager { get; protected set; }

        public System.Windows.Window Window { get; set; }

        public Dictionary<string, SubUnitBase> SubUnitList { get; set; }
        public List<Module.FAModule> ModuleList { get; set; }

        private FAFramework.ConfigClasses.EquipmentConfig _config = new ConfigClasses.EquipmentConfig();
        [FALibrary.FAAttribute("")]
        public FAFramework.ConfigClasses.EquipmentConfig Config
        {
            get { return _config; }
            set
            {
                _config = value;
                NotifyPropertyChanged("Config");
            }
        }

        private string ConfigPath { get; set; }

        private Manager.AlarmRaisingStatusManager _alarmRaisingStatusManager;
        [FALibrary.FAAttribute("Status")]
        public Manager.AlarmRaisingStatusManager AlarmRaisingStatusManager
        {
            get { return _alarmRaisingStatusManager; }
            set
            {
                if (_alarmRaisingStatusManager == value) return;
                _alarmRaisingStatusManager = value;
                NotifyPropertyChanged("AlarmRaisingStatusManager");
            }
        }

        private Manager.LotManager _lotManager = new Manager.LotManager();

        public Manager.LotManager LotManager
        {
            get { return _lotManager; }
        }

        public event EventHandler<FALibrary.Alarm.FAAlarmEventArgs> OnRaiseAlarm = delegate { };
        public event EventHandler<FALibrary.FAGenericEventArgs<UserInfo>> OnChangedUser = delegate { };
        public event EventHandler OnEmergencyAlarmStop = delegate { };

        private DateTime _materialManagerLastUpdateTime = DateTime.Now;
        private TimeSpan _materialManagerUpdateInterval = new TimeSpan(0, 0, 10);

        public EquipmentBase()
        {
            Equipment.MainEquipment.Instance.OnChangedMinute +=
                delegate (object sender, FALibrary.FAGenericEventArgs<DateTime> e)
                {
                    ProductOutput.SaveDayOutput(e.Value);
                    ProductOutput.SaveUnitPerHour(e.Value);
                };

            Equipment.MainEquipment.Instance.OnChangedShift +=
                delegate (object sender, FALibrary.FAGenericEventArgs<Equipment.MainEquipment.ShiftType> e)
                {
                    if (e.Value == Equipment.MainEquipment.ShiftType.GY)
                        ProductOutput.AllClear();
                };

            FALibrary.Alarm.FAAlarmManager.Instance.OnRaiseAlarm +=
                delegate (object sender, FALibrary.Alarm.FAAlarmEventArgs e)
                {
                    bool isSequence = sender is FASequence;
                    bool isMatchEquipment = true;

                    if (isSequence)
                    {
                        isMatchEquipment = ConstainSubSequenceManager((sender as FASequence).SequenceManager);

                        if (isMatchEquipment ||
                            sender == MainSequenceManager)
                        {
                            if (OnRaiseAlarm != null)
                                OnRaiseAlarm(sender, e);
                        }
                    }
                    else if (OnRaiseAlarm != null)
                        OnRaiseAlarm(sender, e);
                };

            AlarmRaisingStatusManager = new Manager.AlarmRaisingStatusManager(this);
            MainSequenceManager = new FASequenceManager();
            SubSequenceManager = new FASequenceManager();
            SystemBackgroundManager = new FASequenceManager();
            SubUnitList = new Dictionary<string, SubUnitBase>();
            ModuleList = new List<Module.FAModule>();

            CreateStates();
            SetStatesCustomActions();
            SetStateChangedEventHandler();

            ProductOutput.OnTrackOutLot +=
                delegate (object sender, FALibrary.FAGenericEventArgs<Manager.ProductOutputManager.LotProductOutputInfo> e)
                {
                    if (PackingLogManager == null) return;

                    PackingLogManager.WriteProductLog(
                        new Utility.PackingLog.ProductLog
                        {
                            ProductID = e.Value.LotID,
                            CarryInTime = e.Value.TrackInTime,
                            CarryOutTime = e.Value.TrackOutTime,
                            CarryInQuantity = e.Value.OutputCount,
                            CarryOutQuantity = e.Value.OutputCount
                        });
                };
        }

        public virtual void DisposeEquipment()
        {
        }

        public virtual void InitializeEquipment()
        {
            MTBIManager = new Manager.MTBISummaryManager(this);
            ProductOutput.EquipmentInstance = this;
            ProductOutput.Load(DateTime.Now);
        }

        protected abstract void CreateStates();

        protected abstract void SetStatesCustomActions();

        protected abstract void SetStateChangedEventHandler();

        protected abstract void SetDefaultState();

        protected virtual void ActionAfterInitialize()
        {
        }

        #region State Method
        public void RequestStart()
        {
            if (Startable())
                State.RequestStart(State);
        }

        public void Start()
        {
            State.Start(State);
        }

        public void RequestStop()
        {
            State.RequestStop(State);
        }

        public void Stop()
        {
            State.Stop(State);
        }

        public void RequestInitialize()
        {
            if (Startable())
                State.RequestInitialize(State);
        }

        public void Initialize()
        {
            State.Initialize(State);
        }

        public void SetRundown()
        {
            State.SetRundown(State);
        }

        public void RequestEmergency()
        {
            State.RequestEmergency(State);
        }

        public void SetEmergency()
        {
            State.SetEmergency(State);
        }

        public void ReleaseEmergency()
        {
            State.ReleaseEmergency(State);
        }

        public void RaiseAlarm()
        {
            State.RaiseAlarm(State);
        }

        public void SetAlarm()
        {
            State.SetAlarm(State);
        }

        public void ClearAlarm()
        {
            State.ClearAlarm(State);
        }

        public virtual void TurnOffSound()
        {
            State.TurnOffSound(State);

            if (StateSignalModuleReferance != null)
            {
                try
                {
                    StateSignalModuleReferance.OffSound = true;
                }
                catch
                {
                }
            }
        }

        public void RaiseWarning()
        {
            State.RaiseWarning(State);
        }

        public void SetWarning()
        {
            State.SetWarning(State);
        }

        public void SetSuspend()
        {
            State.SetSuspend(State);
        }

        public void Suspend()
        {
            State.Suspend(State);
        }

        public void ReleaseSuspend()
        {
            State.ReleaseSuspend(State);
        }

        public void EmergencyAlarmStop(object sender, int alarmNo, string message)
        {
            OnEmergencyAlarmStop(sender, EventArgs.Empty);
            ForceSuspendSubSequence();
            FALibrary.Alarm.FAAlarmManager.Instance.RaiseAlarm(sender, alarmNo, message);
        }
        #endregion

        public void Initialize(string configPath)
        {
            ConfigPath = configPath;

            InitializeEquipment();
            LoadConfig(ConfigPath + "config.xml");
            CreateSubUnits();
            InitializeSubUnits();
            CreateModules();
            InitializeModules();
            SetMachineID();
            AssignModule();
            RegisterSequence();
            InitializeSequence();
            SetInterlockOfModules();
            SetInterlock();
            SetSimulationMode();
            SetDefaultState();
            SetRunMode();
            ActionAfterInitialize();
            WriteStartMessage();
        }

        public void LoadBackup(string path)
        {
            LoadConfig(Path.Combine(path, "config.xml"));
            LoadSubUnitsFromBackup(path);
            LoadModulesFromBackup(path);
        }

        object _thisLock = new object();

        public void Save()
        {
            if (MaterialManagerInstance != null)
                MaterialManagerInstance.Save();
            SaveSubUnits();
            SaveAllModules();

            SaveConfig(ConfigPath + "config.xml");
        }

        public void SaveModules(params Module.FAModule[] modules)
        {
            var xml = new XElement("Parameters");

            foreach (Module.FAModule module in ModuleList)
            {
                if (modules.Contains(module) == false) continue;

                if (xml.Element(module.Name) == null)
                {
                    xml.Add(new XElement(module.Name));
                }

                module.SaveParameters(xml.Element(module.Name));
            }

            var doc = new XDocument();
            doc.Add(xml);
            doc.Save(ConfigPath + "ModuleParameters.xml");
        }

        public void Execute()
        {
            if (MaterialManagerInstance != null)
            {
                if (DateTime.Now - _materialManagerLastUpdateTime > _materialManagerUpdateInterval)
                {
                    _materialManagerLastUpdateTime = DateTime.Now;
                    MaterialManagerInstance.Update();
                }
            }

            MTBIManager.SetState(State);

            ReadWriteParts();

            if (!MaintenanceMode)
            {
                AllwaysExecute();
                ProcInput();
                State.Execute();
                SystemBackgroundManager.Run();
                RunMainSequences();
                RunSubSequence();
            }
        }

        protected virtual void AllwaysExecute()
        {
        }

        protected virtual void ProcInput()
        {
        }

        private void ReadWriteParts()
        {
            foreach (var subUnit in SubUnitList)
            {
                subUnit.Value.ValidatePart();
            }
        }

        public void RunMainSequences()
        {
            MainSequenceManager.Run();
        }

        public virtual void RunSubSequence()
        {
            SubSequenceManager.Run();
        }

        public void SetMachineID()
        {
        }

        public virtual void SetInterlock()
        {
        }

        public virtual void AssignModule()
        {
        }

        public void RegisterSequence()
        {
            foreach (var module in ModuleList)
            {
                module.RegisterSequence();
            }
        }

        public void InitializeSequence()
        {
            foreach (var module in ModuleList)
            {
                module.InitializeSequence();
            }
        }

        public void SetSimulationMode()
        {
        }

        private void CreateSubUnits()
        {
            PropertyInfo[] propList = this.GetType().GetProperties();
            foreach (PropertyInfo info in propList)
            {
                if (info.PropertyType.IsSubclassOf(typeof(SubUnitBase)) == false) continue;

                var instance = (SubUnitBase)Activator.CreateInstance(info.PropertyType);

                info.SetValue(this, instance, null);

                var subUnit = instance as SubUnitBase;
                subUnit.Name = info.Name;

                SubUnitList.Add(info.Name, subUnit);
            }
        }

        private FASequenceManager GetSequenceManagerFromProperty(string propertyName)
        {
            var propertyInfo = this.GetType().GetProperty(propertyName);
            var attributes = Attribute.GetCustomAttributes(propertyInfo);

            foreach (var item in attributes)
            {
                if (item is Utility.SubSequenceManagerName)
                {
                    var attr = (Utility.SubSequenceManagerName)item;
                    if (attr != null)
                    {
                        var sequenceManager = this.GetType().GetProperty(attr.Name).GetValue(this, null);
                        if (sequenceManager != null && sequenceManager is FASequenceManager)
                            return sequenceManager as FASequenceManager;
                    }
                }
            }

            return SubSequenceManager;
        }

        private void InitializeSubUnits()
        {
            foreach (var item in SubUnitList)
            {
                var sequenceManager = GetSequenceManagerFromProperty(item.Value.Name);
                item.Value.Initialize(sequenceManager,
                    ConfigClasses.GlobalConst.ROOT_PATH + "config\\" + Name + "\\" + item.Key + "\\PartList.xml");
            }
        }

        private void LoadSubUnits()
        {
            foreach (var item in SubUnitList)
            {
                item.Value.Load();
            }
        }

        private void LoadSubUnitsFromBackup(string path)
        {
            foreach (var item in SubUnitList)
            {
                item.Value.LoadBackup(Path.Combine(path, item.Key, "PartList.xml"));
            }
        }

        private void SaveSubUnits()
        {
            foreach (var item in SubUnitList)
            {
                item.Value.Save();
            }
        }

        private void CreateModules()
        {
            PropertyInfo[] propList = this.GetType().GetProperties();
            foreach (PropertyInfo info in propList)
            {
                if (info.PropertyType.IsSubclassOf(typeof(Module.FAModule)) == false) continue;

                Module.FAModule module = (Module.FAModule)Activator.CreateInstance(info.PropertyType);
                if (Attribute.GetCustomAttribute(info, typeof(Utility.NotAutoCreateModule)) != null) continue;

                FASequenceManager sequenceManager = null;
                Attribute[] attr = Attribute.GetCustomAttributes(info);

                foreach (Attribute attrItem in attr)
                {
                    if (attrItem is FALibrary.FAAttribute)
                    {
                        FALibrary.FAAttribute faAttr = attrItem as FALibrary.FAAttribute;
                        if (faAttr is FALibrary.FAIncludeSequenceAttribute)
                        {
                            try
                            {
                                sequenceManager = (FASequenceManager)FALibrary.Utility.FAReflection.GetPropertyValue(this, (faAttr as FALibrary.FAIncludeSequenceAttribute).SequenceManagerName);
                            }
                            catch (Exception e)
                            {
                                throw new Exception(e.ToString());
                            }
                        }
                        else
                            throw new Exception(info.Name + " of " + module.FullName + " have not FAIncludeSequenceAttribute");
                    }
                }

                if (sequenceManager == null)
                    throw new Exception("Can't found SequenceManager of " + this.Name + "." + info.Name + ".");

                module.SequenceManager = sequenceManager;
                module.Name = info.Name;
                module.Equipment = this;
                info.SetValue(this, module, null);
                ModuleList.Add(module as Module.FAModule);
            }
        }

        private void InitializeModules()
        {
            string path = Path.Combine(ConfigPath, "ModuleParameters.xml");

            if (System.IO.File.Exists(path) == false) return;

            var xml = XElement.Load(path);

            foreach (Module.FAModule module in ModuleList)
            {
                if (xml.Element(module.Name) != null)
                {
                    module.LoadParameters(xml.Element(module.Name));
                    module.FullName = module.Name;
                }
            }
        }

        private void LoadModules()
        {
            string path = Path.Combine(ConfigPath, "ModuleParameters.xml");

            if (System.IO.File.Exists(path) == false) return;

            var xml = XElement.Load(path);

            foreach (Module.FAModule module in ModuleList)
            {
                if (xml.Element(module.Name) != null)
                {
                    module.LoadParameters(xml.Element(module.Name));
                }
            }
        }

        private void LoadModulesFromBackup(string configPath)
        {
            string path = Path.Combine(configPath, "ModuleParameters.xml");
            if (System.IO.File.Exists(path) == false) return;

            var xml = XElement.Load(path);

            foreach (Module.FAModule module in ModuleList)
            {
                if (xml.Element(module.Name) != null)
                {
                    module.LoadParameters(xml.Element(module.Name));
                }
            }
        }

        private void SaveAllModules()
        {
            var xml = new XElement("Parameters");

            foreach (Module.FAModule module in ModuleList)
            {
                if (xml.Element(module.Name) == null)
                {
                    xml.Add(new XElement(module.Name));
                }

                module.SaveParameters(xml.Element(module.Name));
            }

            var doc = new XDocument();
            doc.Add(xml);
            doc.Save(ConfigPath + "ModuleParameters.xml");
        }

        public void LoadConfig(string filename)
        {
            PreLoadConfig();

            if (File.Exists(filename) == false) return;

            try
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open))
                {
                    using (XmlReader sr = XmlReader.Create(fs))
                    {
                        XmlSerializer xs = FALibrary.Utility.FAUtility.GetXmlSerializer(Config.GetType());
                        Config = (ConfigClasses.EquipmentConfig)xs.Deserialize(sr);
                        AfterLoadConfig();
                    }
                }
            }
            catch (Exception e)
            {
                Manager.LogManager.Instance.WriteSystemLog(e.ToString());
            }
        }

        public void SaveConfig(string filename)
        {
            PreSaveConfig();

            XmlWriterSettings setting = new XmlWriterSettings();
            setting.Indent = true;
            setting.IndentChars = "  ";
            setting.NewLineOnAttributes = true;
            setting.OmitXmlDeclaration = true;

            try
            {
                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    using (XmlWriter xw = XmlWriter.Create(fs, setting))
                    {
                        XmlSerializer xs = FALibrary.Utility.FAUtility.GetXmlSerializer(Config.GetType());
                        xs.Serialize(xw, Config);
                    }
                }
            }
            catch
            {
            }
        }

        protected virtual void AfterLoadConfig()
        {
            if (Config != null)
            {
                if (Config.UserList == null)
                    Config.UserList = new FAFramework.Utility.ThreadSafeObservableCollection<UserInfo>();

                if (ExistUser("admin") == false)
                    Config.UserList.Add(new UserInfo(UserPermissionTypes.MASTER, "admin", string.Empty));
            }
        }

        private bool ExistUser(string userName)
        {
            if (Config == null) return false;
            if (Config.UserList == null) return false;

            foreach (var user in Config.UserList)
            {
                if (user.Name == userName) return true;
            }

            return false;
        }

        protected virtual void PreLoadConfig()
        {
        }

        protected virtual void PreSaveConfig()
        {
        }

        public virtual void TurnOnSound()
        {
        }

        public virtual void ResumeSubSequences()
        {
            SubSequenceManager.Resume();
        }

        public virtual void ForceSuspendSubSequence()
        {
            SubSequenceManager.ForceSuspend();
        }

        public virtual void SuspendSubSequences()
        {
            SubSequenceManager.Suspend();
        }

        public virtual bool IsSuspendedSubSuquences()
        {
            if (SubSequenceManager.IsSuspened())
                return true;
            else
                return false;
        }

        public virtual bool ConstainSubSequenceManager(FASequenceManager aSequenceManager)
        {
            return SubSequenceManager == aSequenceManager ||
                MainSequenceManager == aSequenceManager;
        }

        public virtual void ClearSubSequencesState()
        {
            SubSequenceManager.AllClearState();
        }

        public virtual void OpenDoor()
        {
        }

        public virtual void CloseDoor()
        {
        }

        public virtual bool AlarmClearable()
        {
            return true;
        }

        public virtual void WriteStartMessage()
        {
            Manager.LogManager.Instance.WriteTraceLog(this, "START PGM");
        }

        public virtual void WriteTerminateMessage()
        {
            Manager.LogManager.Instance.WriteTraceLog(this, "TERMINATE PGM");
        }

        protected virtual void SetRunMode()
        {
            var runmodeFilePath = System.IO.Path.Combine(ConfigClasses.GlobalConst.CONFIG_PATH, "runmode.txt");
            if (System.IO.File.Exists(runmodeFilePath))
            {
                var runmode = System.IO.File.ReadAllText(runmodeFilePath).Trim().ToUpper();

                if (runmode == "DRY_RUN")
                    RunMode = Equipment.RunModeTypes.DRY_RUN;
                else if (runmode == "COLD_RUN")
                    RunMode = Equipment.RunModeTypes.COLD_RUN;
                else if (runmode == "HOT_RUN")
                    RunMode = Equipment.RunModeTypes.HOT_RUN;
                else
                    RunMode = RunModeTypes.HOT_RUN;
            }
            else
                RunMode = RunModeTypes.HOT_RUN;
        }

        protected virtual bool Startable()
        {
            return true;
        }

        private void SetInterlockOfModules()
        {
            foreach (var moule in ModuleList)
            {
                moule.SetInterlock();
            }
        }
    }
}