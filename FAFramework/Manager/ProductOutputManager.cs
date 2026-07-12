using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;

namespace FAFramework.Manager
{
    public class ProductOutputManager : FAObject
    {
        private string _directoryName = "ProductOutput";
        public string DirectoryName
        {
            get { return _directoryName; }
            set { _directoryName = value; }
        }

        private static object threadRoot = new Object();
        private string _rootPath = string.Empty;
        public string ROOT_PATH
        {
            get
            {
                if (string.IsNullOrEmpty(_rootPath))
                {
                    var lotPath = Path.Combine(FAFramework.ConfigClasses.GlobalConst.ROOT_PATH, "Log");
                    if (EquipmentInstance != null)
                        _rootPath = Path.Combine(lotPath, EquipmentInstance.Name, DirectoryName);
                }

                return _rootPath;
            }
        }

        public Equipment.EquipmentBase EquipmentInstance { get; set; }

        public class LotProductOutputInfo : FAObject
        {
            public static readonly TimeSpan HOUR = new TimeSpan(1, 0, 0);
            public static readonly string FILE_NAME = "LotProductOutput_";

            private string _lotID;
            [FAAttribute("")]
            public string LotID
            {
                get { return _lotID; }
                set
                {
                    if (_lotID == value) return;
                    _lotID = value;
                    NotifyPropertyChanged("LotID");
                }
            }

            private string _partID;
            [FAAttribute("")]
            public string PartID
            {
                get { return _partID; }
                set
                {
                    if (_partID == value) return;
                    _partID = value;
                    NotifyPropertyChanged("PartID");
                }
            }

            private DateTime _trackInTime;
            [FAAttribute("")]
            public DateTime TrackInTime
            {
                get { return _trackInTime; }
                set
                {
                    if (_trackInTime == value) return;
                    _trackInTime = value;
                    NotifyPropertyChanged("TrackInTime");
                }
            }

            private DateTime _trackOutTime;
            [FAAttribute("")]
            public DateTime TrackOutTime
            {
                get { return _trackOutTime; }
                set
                {
                    if (_trackOutTime == value) return;
                    _trackOutTime = value;
                    NotifyPropertyChanged("TrackOutTime");
                }
            }

            private DateTime _lastUpdateTime;
            [FAAttribute("")]
            public DateTime LastUpdateTime
            {
                get { return _lastUpdateTime; }
                set
                {
                    if (_lastUpdateTime == value) return;
                    _lastUpdateTime = value;
                    NotifyPropertyChanged("LastUpdateTime");
                }
            }

            private bool _useSetUPHOnTrackOut;
            [FAAttribute("")]
            public bool UseSetUPHOnTrackOut
            {
                get { return _useSetUPHOnTrackOut; }
                set
                {
                    if (_useSetUPHOnTrackOut == value) return;
                    _useSetUPHOnTrackOut = value;
                    NotifyPropertyChanged("UseSetUPHOnTrackOut");
                }
            }

            private double _uph;
            [FAAttribute("")]
            public double UPH
            {
                get { return _uph; }
                set
                {
                    if (_uph == value) return;
                    _uph = value;
                    NotifyPropertyChanged("UPH");
                }
            }

            private int _outputCount;
            [FAAttribute("")]
            public int OutputCount
            {
                get { return _outputCount; }
                set
                {
                    if (_outputCount == value) return;
                    _outputCount = value;
                    NotifyPropertyChanged("OutputCount");
                }
            }

            private TimeSpan _tactTime;
            [FAAttribute("")]
            public TimeSpan TactTime
            {
                get { return _tactTime; }
                set
                {
                    if (_tactTime == value) return;
                    _tactTime = value;
                    NotifyPropertyChanged("TactTime");
                }
            }

            public void AddOutput(DateTime time, string lotID, string partID)
            {
                AddOutput(time, lotID, partID, 1);
            }

            public void AddOutput(DateTime time, string lotID, string partID, int count)
            {
                if (LotID != lotID)
                    TrackOutLot(time, lotID, partID);

                LotID = lotID;
                PartID = partID;
                OutputCount += count;
                LastUpdateTime = time;
                SetUPH();
                SetTactTime();
            }

            public void TrackInLot(DateTime time, string lotID, string partID)
            {
                LotID = lotID;
                PartID = partID;
                TrackInTime = time;
                LastUpdateTime = time;
            }

            public void TrackOutLot(DateTime time, string lotID, string partID)
            {
                TrackOutTime = time;
                LastUpdateTime = time;

                if (UseSetUPHOnTrackOut)
                {
                    SetUPH();
                    SetTactTime();
                }
            }

            public void SetUPH()
            {
                var time = LastUpdateTime - TrackInTime;
                double rate = HOUR.TotalSeconds / time.TotalSeconds;
                UPH = OutputCount * rate;
            }

            public void SetTactTime()
            {
                try
                {
                    var time = LastUpdateTime - TrackInTime;
                    var tactSecond = time.Ticks / OutputCount;
                    TactTime = new TimeSpan(tactSecond);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.Write(e.ToString());
                }
            }

            public void Load(StreamReader sr)
            {
                try
                {
                    string line = sr.ReadLine();
                    LotProductOutputInfo result;
                    if (TryParse(line, out result) == true)
                        result.CopyTo(this);

                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.Write(e.ToString());
                }
            }

            public void Save(StreamWriter sw)
            {
                try
                {
                    sw.WriteLine(LastUpdateTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + ", " + ToString());
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.Write(e.ToString());
                }
            }

            public bool TryParse(string str, out LotProductOutputInfo result)
            {
                result = null;
                if (string.IsNullOrEmpty(str)) return false;

                result = new LotProductOutputInfo();
                result.UseSetUPHOnTrackOut = UseSetUPHOnTrackOut;

                try
                {
                    string[] splitData = str.Split(',');
                    foreach (var item in splitData)
                    {
                        string[] itemSplit = item.Split('=');
                        if (itemSplit.Length < 2) continue;

                        string value = itemSplit[1].Trim();

                        switch (itemSplit[0].Trim())
                        {
                            case "LotID":
                                result.LotID = value;
                                break;

                            case "PartID":
                                result.PartID = value;
                                break;

                            case "OutputCount":
                                {
                                    int temp;
                                    if (int.TryParse(value, out temp))
                                        result.OutputCount = temp;
                                }

                                break;

                            case "UPH":
                                {
                                    double temp;
                                    if (double.TryParse(value, out temp))
                                        result.UPH = temp;
                                }

                                break;

                            case "TrackInTime":
                                {
                                    DateTime temp;
                                    if (DateTime.TryParse(value, out temp))
                                        result.TrackInTime = temp;
                                }

                                break;

                            case "TrackOutTime":
                                {
                                    DateTime temp;
                                    if (DateTime.TryParse(value, out temp))
                                        result.TrackOutTime = temp;
                                }

                                break;

                            case "LastUpdateTime":
                                {
                                    DateTime temp;
                                    if (DateTime.TryParse(value, out temp))
                                        result.LastUpdateTime = temp;
                                }

                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.Write(e.ToString());
                    result = null;
                    return false;
                }

                return true;
            }

            public void CopyTo(LotProductOutputInfo dest)
            {
                dest.LastUpdateTime = this.LastUpdateTime;
                dest.LotID = this.LotID;
                dest.PartID = this.PartID;
                dest.OutputCount = this.OutputCount;
                dest.TrackInTime = this.TrackInTime;
                dest.TrackOutTime = this.TrackOutTime;
                dest.UPH = this.UPH;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                AddString(sb, "LotID", LotID.ToString());
                AddString(sb, "PartID", PartID.ToString());
                AddString(sb, "OutputCount", OutputCount.ToString());
                AddString(sb, "UPH", UPH.ToString());
                AddString(sb, "TrackInTime", TrackInTime.ToString());
                AddString(sb, "TrackOutTime", TrackOutTime.ToString());
                AddString(sb, "LastUpdateTime", LastUpdateTime.ToString());

                return sb.ToString();
            }

            private void AddString(StringBuilder sb, string name, string value)
            {
                if (sb.Length != 0)
                    sb.Append(", ");
                sb.Append(name);
                sb.Append(" = ");
                sb.Append(value);
            }
        }

        public class UnitPerHourInfo : FAObject
        {
            public static readonly string FILE_NAME = "UPEH_";

            private DateTime _startTime;
            [FAAttribute("")]
            public DateTime StartTime
            {
                get { return _startTime; }
                set
                {
                    if (_startTime == value) return;
                    _startTime = value;
                    NotifyPropertyChanged("StartTime");
                }
            }

            private DateTime _lastUpdateTime;
            [FAAttribute("")]
            public DateTime LastUpdateTime
            {
                get { return _lastUpdateTime; }
                set
                {
                    if (_lastUpdateTime == value) return;
                    _lastUpdateTime = value;
                    NotifyPropertyChanged("LastUpdateTime");
                }
            }

            private int _outputCount;
            [FAAttribute("")]
            public int OutputCount
            {
                get { return _outputCount; }
                set
                {
                    if (_outputCount == value) return;
                    _outputCount = value;
                    NotifyPropertyChanged("OutputCount");
                }
            }

            public UnitPerHourInfo()
            {
                OutputCount = 0;
                LastUpdateTime = DateTime.MinValue;
                StartTime = DateTime.MinValue;
            }

            public void AddOutput(DateTime time, int count)
            {
                OutputCount += count;
                LastUpdateTime = time;
                StartTime = time;
            }

            public void Load(StreamReader sr)
            {
                try
                {
                    string line = sr.ReadLine();
                    UnitPerHourInfo result;
                    if (TryParse(line, out result) == true)
                        result.CopyTo(this);

                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.Write(e.ToString());
                }
            }

            public void Save(StreamWriter sw)
            {
                try
                {
                    sw.WriteLine(LastUpdateTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + ", " + ToString());
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.Write(e.ToString());
                }
            }

            public bool TryParse(string str, out UnitPerHourInfo result)
            {
                result = null;
                if (string.IsNullOrEmpty(str)) return false;

                result = new UnitPerHourInfo();

                try
                {
                    string[] splitData = str.Split(',');
                    foreach (var item in splitData)
                    {
                        string[] itemSplit = item.Split('=');
                        if (itemSplit.Length < 2) continue;

                        string value = itemSplit[1].Trim();

                        switch (itemSplit[0].Trim())
                        {
                            case "OutputCount":
                                {
                                    int temp;
                                    if (int.TryParse(value, out temp))
                                        result.OutputCount = temp;
                                }

                                break;

                            case "LastUpdateTime":
                                {
                                    DateTime temp;
                                    if (DateTime.TryParse(value, out temp))
                                        result.LastUpdateTime = temp;
                                }

                                break;

                            case "StartTime":
                                {
                                    DateTime temp;
                                    if (DateTime.TryParse(value, out temp))
                                        result.StartTime = temp;
                                }

                                break;
                        }
                    }
                }
                catch
                {
                    result = null;
                    return false;
                }

                return true;
            }

            public void CopyTo(UnitPerHourInfo dest)
            {
                dest.LastUpdateTime = this.LastUpdateTime;
                dest.OutputCount = this.OutputCount;
                dest.StartTime = this.StartTime;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                AddString(sb, "StartTime", StartTime.ToString());
                AddString(sb, "OutputCount", OutputCount.ToString());
                AddString(sb, "LastUpdateTime", LastUpdateTime.ToString());

                return sb.ToString();
            }

            private void AddString(StringBuilder sb, string name, string value)
            {
                if (sb.Length != 0)
                    sb.Append(", ");
                sb.Append(name);
                sb.Append(" = ");
                sb.Append(value);
            }
        }

        public class DayProductOutputInfo : FAObject
        {
            public static readonly string FILE_NAME = "DAY_OUTPUT_";

            private int _totalOutputCount;
            [FAAttribute("")]
            public int TotalOutputCount
            {
                get { return _totalOutputCount; }
                set
                {
                    if (_totalOutputCount == value) return;
                    _totalOutputCount = value;
                    NotifyPropertyChanged("TotalOutputCount");
                }
            }

            private int _dayOutputCount;
            [FAAttribute("")]
            public int DayOutputCount
            {
                get { return _dayOutputCount; }
                set
                {
                    if (_dayOutputCount == value) return;
                    _dayOutputCount = value;
                    NotifyPropertyChanged("DayOutputCount");
                }
            }

            private int _swingOutputCount;
            [FAAttribute("")]
            public int SwingOutputCount
            {
                get { return _swingOutputCount; }
                set
                {
                    if (_swingOutputCount == value) return;
                    _swingOutputCount = value;
                    NotifyPropertyChanged("SwingOutputCount");
                }
            }

            private int _gyOutputCount;
            [FAAttribute("")]
            public int GYOutputCount
            {
                get { return _gyOutputCount; }
                set
                {
                    if (_gyOutputCount == value) return;
                    _gyOutputCount = value;
                    NotifyPropertyChanged("GYOutputCount");
                }
            }

            private int _totalOutputCountOfBundleUnit;
            [FAAttribute("")]
            public int TotalOutputCountOfBundleUnit
            {
                get { return _totalOutputCountOfBundleUnit; }
                set
                {
                    if (_totalOutputCountOfBundleUnit == value) return;
                    _totalOutputCountOfBundleUnit = value;
                    NotifyPropertyChanged("TotalOutputCountOfBundleUnit");
                }
            }

            private int _dayOutputCountOfBundleUnit;
            [FAAttribute("")]
            public int DayOutputCountOfBundleUnit
            {
                get { return _dayOutputCountOfBundleUnit; }
                set
                {
                    if (_dayOutputCountOfBundleUnit == value) return;
                    _dayOutputCountOfBundleUnit = value;
                    NotifyPropertyChanged("DayOutputCountOfBundleUnit");
                }
            }

            private int _swingOutputCountOfBundleUnit;
            [FAAttribute("")]
            public int SwingOutputCountOfBundleUnit
            {
                get { return _swingOutputCountOfBundleUnit; }
                set
                {
                    if (_swingOutputCountOfBundleUnit == value) return;
                    _swingOutputCountOfBundleUnit = value;
                    NotifyPropertyChanged("SwingOutputCountOfBundleUnit");
                }
            }

            private int _gyOutputCountOfBundleUnit;
            [FAAttribute("")]
            public int GYOutputCountOfBundleUnit
            {
                get { return _gyOutputCountOfBundleUnit; }
                set
                {
                    if (_gyOutputCountOfBundleUnit == value) return;
                    _gyOutputCountOfBundleUnit = value;
                    NotifyPropertyChanged("GYOutputCountOfBundleUnit");
                }
            }

            private DateTime _lastUpdateTime = DateTime.Now;
            [FAAttribute("")]
            public DateTime LastUpdateTime
            {
                get { return _lastUpdateTime; }
                set
                {
                    if (_lastUpdateTime == value) return;
                    _lastUpdateTime = value;
                    NotifyPropertyChanged("LastUpdateTime");
                }
            }

            public void AddOutput(DateTime time)
            {
                AddOutput(time, 1);
            }

            public void AddOutput(DateTime time, int count)
            {
                TotalOutputCount += count;
                var shift = Equipment.MainEquipment.GetShift(time.TimeOfDay);
                if (shift == Equipment.MainEquipment.ShiftType.Day)
                    DayOutputCount += count;
                else if (shift == Equipment.MainEquipment.ShiftType.Swing)
                    SwingOutputCount += count;
                else if (shift == Equipment.MainEquipment.ShiftType.GY)
                    GYOutputCount += count;

                LastUpdateTime = time;
            }

            public void AddOutputOfBundleUnit(DateTime time)
            {
                TotalOutputCountOfBundleUnit++;
                var shift = Equipment.MainEquipment.GetShift(time.TimeOfDay);
                if (shift == Equipment.MainEquipment.ShiftType.Day)
                    DayOutputCountOfBundleUnit++;
                else if (shift == Equipment.MainEquipment.ShiftType.Swing)
                    SwingOutputCountOfBundleUnit++;
                else if (shift == Equipment.MainEquipment.ShiftType.GY)
                    GYOutputCountOfBundleUnit++;

                LastUpdateTime = time;
            }

            public void Load(StreamReader sr)
            {
                try
                {
                    string line = sr.ReadLine();
                    DayProductOutputInfo result;
                    if (TryParse(line, out result) == true)
                        result.CopyTo(this);

                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.Write(e.ToString());
                }
            }

            public void Save(StreamWriter sw)
            {
                try
                {
                    sw.WriteLine(LastUpdateTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + ", " + ToString());
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.Write(e.ToString());
                }
            }

            public bool TryParse(string str, out DayProductOutputInfo result)
            {
                result = null;
                if (string.IsNullOrEmpty(str)) return false;

                result = new DayProductOutputInfo();

                try
                {
                    string[] splitData = str.Split(',');
                    foreach (var item in splitData)
                    {
                        string[] itemSplit = item.Split('=');
                        if (itemSplit.Length < 2) continue;

                        string value = itemSplit[1].Trim();

                        switch (itemSplit[0].Trim())
                        {
                            case "TotalOutputCount":
                                {
                                    int temp;
                                    if (int.TryParse(value, out temp))
                                        result.TotalOutputCount = temp;
                                }

                                break;

                            case "DayOutputCount":
                                {
                                    int temp;
                                    if (int.TryParse(value, out temp))
                                        result.DayOutputCount = temp;
                                }

                                break;

                            case "SwingOutputCount":
                                {
                                    int temp;
                                    if (int.TryParse(value, out temp))
                                        result.SwingOutputCount = temp;
                                }

                                break;

                            case "GYOutputCount":
                                {
                                    int temp;
                                    if (int.TryParse(value, out temp))
                                        result.GYOutputCount = temp;
                                }

                                break;

                            case "TotalOutputCountOfBundleUnit":
                                {
                                    int temp;
                                    if (int.TryParse(value, out temp))
                                        result.TotalOutputCountOfBundleUnit = temp;
                                }

                                break;

                            case "DayOutputCountOfBundleUnit":
                                {
                                    int temp;
                                    if (int.TryParse(value, out temp))
                                        result.DayOutputCountOfBundleUnit = temp;
                                }

                                break;

                            case "SwingOutputCountOfBundleUnit":
                                {
                                    int temp;
                                    if (int.TryParse(value, out temp))
                                        result.SwingOutputCountOfBundleUnit = temp;
                                }

                                break;

                            case "GYOutputCountOfBundleUnit":
                                {
                                    int temp;
                                    if (int.TryParse(value, out temp))
                                        result.GYOutputCountOfBundleUnit = temp;
                                }

                                break;

                            case "LastUpdateTime":
                                {
                                    DateTime temp;
                                    if (DateTime.TryParse(value, out temp))
                                        result.LastUpdateTime = temp;
                                }

                                break;
                        }
                    }
                }
                catch
                {
                    result = null;
                    return false;
                }

                return true;
            }

            public void CopyTo(DayProductOutputInfo dest)
            {
                dest.TotalOutputCount = this.TotalOutputCount;
                dest.DayOutputCount = this.DayOutputCount;
                dest.SwingOutputCount = this.SwingOutputCount;
                dest.GYOutputCount = this.GYOutputCount;
                dest.TotalOutputCountOfBundleUnit = this.TotalOutputCountOfBundleUnit;
                dest.DayOutputCountOfBundleUnit = this.DayOutputCountOfBundleUnit;
                dest.SwingOutputCountOfBundleUnit = this.SwingOutputCountOfBundleUnit;
                dest.GYOutputCountOfBundleUnit = this.GYOutputCountOfBundleUnit;
                dest.LastUpdateTime = this.LastUpdateTime;
            }

            public void Clear()
            {
                TotalOutputCount = 0;
                DayOutputCount = 0;
                SwingOutputCount = 0;
                GYOutputCount = 0;
                TotalOutputCountOfBundleUnit = 0;
                DayOutputCountOfBundleUnit = 0;
                SwingOutputCountOfBundleUnit = 0;
                GYOutputCountOfBundleUnit = 0;
                LastUpdateTime = DateTime.MinValue;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                AddString(sb, "TotalOutputCount", TotalOutputCount.ToString());
                AddString(sb, "DayOutputCount", DayOutputCount.ToString());
                AddString(sb, "SwingOutputCount", SwingOutputCount.ToString());
                AddString(sb, "GYOutputCount", GYOutputCount.ToString());
                AddString(sb, "TotalOutputCountOfBundleUnit", TotalOutputCountOfBundleUnit.ToString());
                AddString(sb, "DayOutputCountOfBundleUnit", DayOutputCountOfBundleUnit.ToString());
                AddString(sb, "SwingOutputCountOfBundleUnit", SwingOutputCountOfBundleUnit.ToString());
                AddString(sb, "GYOutputCountOfBundleUnit", GYOutputCountOfBundleUnit.ToString());
                AddString(sb, "LastUpdateTime", LastUpdateTime.ToString());

                return sb.ToString();
            }

            private void AddString(StringBuilder sb, string name, string value)
            {
                if (sb.Length != 0)
                    sb.Append(", ");
                sb.Append(name);
                sb.Append(" = ");
                sb.Append(value);
            }
        }

        private bool _useSetUPHOnTrackOut;
        [FAAttribute("")]
        public bool UseSetUPHOnTrackOut
        {
            get { return _useSetUPHOnTrackOut; }
            set
            {
                if (_useSetUPHOnTrackOut == value) return;
                _useSetUPHOnTrackOut = value;
                NotifyPropertyChanged("UseSetUPHOnTrackOut");
            }
        }

        private DateTime _lastUpdateTime;
        [FAAttribute("")]
        public DateTime LastUpdateTime
        {
            get { return _lastUpdateTime; }
            set
            {
                if (_lastUpdateTime == value) return;
                _lastUpdateTime = value;
                NotifyPropertyChanged("LastUpdateTime");
            }
        }

        public LotProductOutputInfo _currentLotProductOutput;
        [FAAttribute("")]
        public LotProductOutputInfo CurrentLotProductOutput
        {
            get { return _currentLotProductOutput; }
            set
            {
                if (_currentLotProductOutput == value) return;
                _currentLotProductOutput = value;
                NotifyPropertyChanged("CurrentLotProductOutput");
            }
        }

        public UnitPerHourInfo _currentUnitPerHour;
        [FAAttribute("")]
        public UnitPerHourInfo CurrentUnitPerHour
        {
            get { return _currentUnitPerHour; }
            set
            {
                if (_currentUnitPerHour == value) return;
                _currentUnitPerHour = value;
                NotifyPropertyChanged("CurrentUnitPerHour");
            }
        }

        public DayProductOutputInfo _dayProductOutput;
        [FAAttribute("")]
        public DayProductOutputInfo DayProductOutput
        {
            get { return _dayProductOutput; }
            set
            {
                if (_dayProductOutput == value) return;
                _dayProductOutput = value;
                NotifyPropertyChanged("DayProductOutput");
            }
        }

        [FAAttribute("")]
        public FAFramework.Utility.ThreadSafeObservableCollection<LotProductOutputInfo> LotProductOutputList { get; set; }
        [FAAttribute("")]
        public FAFramework.Utility.ThreadSafeObservableCollection<UnitPerHourInfo> UnitPerHourList { get; set; }

        private string _lotID;
        [FAAttribute("")]
        public string LotID
        {
            get { return _lotID; }
            set
            {
                if (_lotID == value) return;
                _lotID = value;
                NotifyPropertyChanged("LotID");
            }
        }

        private string _upehString;
        [FAAttribute("")]
        public string UPEHString
        {
            get { return _upehString; }
            set
            {
                if (_upehString == value) return;
                _upehString = value;
                NotifyPropertyChanged("UPEHString");
            }
        }

        public bool Run { get; set; }

        private Queue<Action> _logQueue = new Queue<Action>();

        public event EventHandler<FAGenericEventArgs<LotProductOutputInfo>> OnAddedLotProduct = delegate { };
        public event EventHandler<FAGenericEventArgs<LotProductOutputInfo>> OnTrackInLot = delegate { };
        public event EventHandler<FAGenericEventArgs<LotProductOutputInfo>> OnTrackOutLot = delegate { };

        public ProductOutputManager()
        {
            LotProductOutputList = new FAFramework.Utility.ThreadSafeObservableCollection<LotProductOutputInfo>();
            UnitPerHourList = new FAFramework.Utility.ThreadSafeObservableCollection<UnitPerHourInfo>();
            DayProductOutput = new DayProductOutputInfo();

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

        [FAAttribute("Operation")]
        public void AddProduct(object sender)
        {
            AddProduct(DateTime.Now, LotID, string.Empty);

            UPEHString = GetUPEHString();
        }

        public void AddProduct(DateTime time, string lotID, string partID)
        {
            AddProduct(time, lotID, partID, 1);
        }

        public void AddProduct(DateTime time, string lotID, string partID, int count)
        {
            var oldShiftType = Equipment.MainEquipment.GetShift(LastUpdateTime.TimeOfDay);
            var currentShiftType = Equipment.MainEquipment.GetShift(time.TimeOfDay);
            if (oldShiftType != currentShiftType)
            {
                if (currentShiftType == Equipment.MainEquipment.ShiftType.GY)
                {
                    AllClear();
                }
            }

            DayProductOutput.AddOutput(time, count);
            DayProductOutput.AddOutputOfBundleUnit(time);

            if (CurrentLotProductOutput == null)
            {
                CurrentLotProductOutput = new LotProductOutputInfo();
                CurrentLotProductOutput.UseSetUPHOnTrackOut = UseSetUPHOnTrackOut;
                CurrentLotProductOutput.LotID = lotID;
                LotProductOutputList.Add(CurrentLotProductOutput);
            }

            if (CurrentLotProductOutput.LotID != lotID)
            {
                TrackOutLot(time, CurrentLotProductOutput.LotID, CurrentLotProductOutput.PartID);
                TrackInLot(time, lotID, partID);
                CurrentLotProductOutput.LotID = lotID;
                CurrentLotProductOutput.LastUpdateTime = time;
            }

            if (UnitPerHourList != null && UnitPerHourList.Count > 0)
            {
                var last = UnitPerHourList.Last();
                if (last.StartTime.Date == time.Date)
                {
                    CurrentUnitPerHour = last;
                }
            }

            if (CurrentUnitPerHour == null)
            {
                CurrentUnitPerHour = new UnitPerHourInfo();
                CurrentUnitPerHour.LastUpdateTime = time;
                UnitPerHourList.Add(CurrentUnitPerHour);
            }

            if (CurrentUnitPerHour.LastUpdateTime.Hour != time.Hour)
            {
                UnitPerHourList.Add(new UnitPerHourInfo());
                CurrentUnitPerHour = UnitPerHourList.Last();
            }

            CurrentLotProductOutput.AddOutput(time, lotID, partID, count);
            OnAddedLotProduct(this, new FAGenericEventArgs<LotProductOutputInfo>(CurrentLotProductOutput));

            CurrentUnitPerHour.AddOutput(time, count);

            Save(time);

            LastUpdateTime = time;

            UPEHString = GetUPEHString();
        }

        public void AddProductOfBundleUnit(DateTime time)
        {
            DayProductOutput.AddOutputOfBundleUnit(time);
        }

        public void TrackInLot(DateTime time, string lotID, string partID)
        {
            var newObj = new LotProductOutputInfo();
            newObj.UseSetUPHOnTrackOut = UseSetUPHOnTrackOut;
            LotProductOutputList.Add(newObj);
            CurrentLotProductOutput = LotProductOutputList.Last();

            if (CurrentLotProductOutput != null)
                CurrentLotProductOutput.TrackInLot(time, lotID, partID);

            OnTrackInLot(this, new FAGenericEventArgs<LotProductOutputInfo>(CurrentLotProductOutput));
            Save(time);
        }

        public void TrackOutLot(DateTime time, string lotID, string partID)
        {
            if (LotProductOutputList != null && LotProductOutputList.Count > 0)
            {
                var selectedItems = LotProductOutputList.Where(x => x.LotID == lotID);
                if (selectedItems != null && selectedItems.Count() > 0)
                {
                    try
                    {
                        var trackOutedLot = selectedItems.First();
                        trackOutedLot.TrackOutLot(time, lotID, partID);
                        OnTrackOutLot(this, new FAGenericEventArgs<LotProductOutputInfo>(trackOutedLot));
                    }
                    catch (Exception e)
                    {
                        Manager.LogManager.Instance.WriteDebugLog(EquipmentInstance, e.Message);
                    }
                }
            }

            Save(time);
        }

        public void AllClear()
        {
            UnitPerHourList.Clear();
            DayProductOutput.Clear();
            LotProductOutputList.Clear();

            if (CurrentLotProductOutput != null)
                LotProductOutputList.Add(CurrentLotProductOutput);
        }

        public void Load(DateTime date)
        {
            try
            {
                LoadLotProductOutput(date);
                LoadUnitPerHour(date);
                LoadDayOutput(date);

                if (LotProductOutputList.Count > 0)
                    CurrentLotProductOutput = LotProductOutputList.First();

                if (UnitPerHourList.Count > 0)
                {
                    CurrentUnitPerHour = UnitPerHourList.First();
                    LastUpdateTime = CurrentUnitPerHour.LastUpdateTime;
                }
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
                        SaveLotProductOutput(date);
                        SaveUnitPerHour(date);
                        SaveDayOutput(date);
                    });
            };
        }

        public void LoadLotProductOutput(DateTime date)
        {
            var loadTime = GetTimeForSamsungShift(date);

            StreamReader sr = null;

            try
            {
                string path = Path.Combine(ROOT_PATH, loadTime.ToString("yyyy"), loadTime.ToString("MM"), loadTime.ToString("dd"));
                string filename = Path.Combine(path, LotProductOutputInfo.FILE_NAME + loadTime.ToString(@"yyyy-MM-dd") + ".log");
                if (File.Exists(filename) == false) return;

                using (sr = new StreamReader(filename))
                {
                    while (sr.EndOfStream == false)
                    {
                        LotProductOutputInfo obj = new LotProductOutputInfo();
                        obj.Load(sr);
                        obj.UseSetUPHOnTrackOut = UseSetUPHOnTrackOut;
                        LotProductOutputList.Add(obj);
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.Write(e.ToString());
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }
        }

        public void LoadUnitPerHour(DateTime date)
        {
            var loadTime = GetTimeForSamsungShift(date);

            StreamReader sr = null;

            try
            {
                string path = Path.Combine(ROOT_PATH, loadTime.ToString("yyyy"), loadTime.ToString("MM"), loadTime.ToString("dd"));
                string filename = Path.Combine(path, UnitPerHourInfo.FILE_NAME + loadTime.ToString(@"yyyy-MM-dd") + ".log");
                if (File.Exists(filename) == false) return;

                using (sr = new StreamReader(filename))
                {
                    while (sr.EndOfStream == false)
                    {
                        UnitPerHourInfo obj = new UnitPerHourInfo();
                        obj.Load(sr);
                        UnitPerHourList.Add(obj);
                    }
                }

                UPEHString = GetUPEHString();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.Write(e.ToString());
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }
        }

        public void LoadDayOutput(DateTime date)
        {
            var loadTime = GetTimeForSamsungShift(date);

            StreamReader sr = null;

            try
            {
                string path = Path.Combine(ROOT_PATH, loadTime.ToString("yyyy"), loadTime.ToString("MM"), loadTime.ToString("dd"));
                string filename = Path.Combine(path, DayProductOutputInfo.FILE_NAME + loadTime.ToString(@"yyyy-MM-dd") + ".log");
                if (File.Exists(filename) == false) return;

                using (sr = new StreamReader(filename))
                {
                    while (sr.EndOfStream == false)
                    {
                        DayProductOutput.Load(sr);
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.Write(e.ToString());
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }
        }

        public void SaveLotProductOutput(DateTime date)
        {
            var saveDate = GetTimeForSamsungShift(date);

            StreamWriter sw = null;

            try
            {
                string path = Path.Combine(ROOT_PATH, saveDate.ToString("yyyy"), saveDate.ToString("MM"), saveDate.ToString("dd"));
                string filename = Path.Combine(path, LotProductOutputInfo.FILE_NAME + saveDate.ToString(@"yyyy-MM-dd") + ".log");
                if (Directory.Exists(path) == false)
                    Directory.CreateDirectory(path);

                using (sw = new StreamWriter(filename, false))
                {
                    foreach (var item in LotProductOutputList)
                    {
                        item.Save(sw);
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.Write(e.ToString());
            }
            finally
            {
                if (sw != null)
                {                    
                    sw.Close();
                    sw.Dispose();
                }
            }
        }

        public void SaveUnitPerHour(DateTime date)
        {
            var saveDate = GetTimeForSamsungShift(date);

            StreamWriter sw = null;

            try
            {
                string path = Path.Combine(ROOT_PATH, saveDate.ToString("yyyy"), saveDate.ToString("MM"), saveDate.ToString("dd"));
                string filename = Path.Combine(path, UnitPerHourInfo.FILE_NAME + saveDate.ToString(@"yyyy-MM-dd") + ".log");
                if (Directory.Exists(path) == false)
                    Directory.CreateDirectory(path);

                using (sw = new StreamWriter(filename, false))
                {
                    foreach (var item in UnitPerHourList)
                    {
                        item.Save(sw);
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.Write(e.ToString());
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                    sw.Dispose();
                }
            }
        }

        public void SaveDayOutput(DateTime date)
        {
            var saveDate = GetTimeForSamsungShift(date);

            StreamWriter sw = null;

            try
            {
                string path = Path.Combine(ROOT_PATH, saveDate.ToString("yyyy"), saveDate.ToString("MM"), saveDate.ToString("dd") + @"\");
                string filename = Path.Combine(path, DayProductOutputInfo.FILE_NAME + saveDate.ToString(@"yyyy-MM-dd") + ".log");
                if (Directory.Exists(path) == false)
                    Directory.CreateDirectory(path);

                using (sw = new StreamWriter(filename, false))
                    DayProductOutput.Save(sw);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.Write(e.ToString());
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                    sw.Dispose();
                }
            }
        }

        private DateTime GetTimeForSamsungShift(DateTime date)
        {
            var result = date;
            var shift = Equipment.MainEquipment.GetShift(result.TimeOfDay);
            if (shift == Equipment.MainEquipment.ShiftType.GY &&
                result.TimeOfDay > Equipment.MainEquipment.GY_SHIFT_START_TIME)
            {
                result = result.AddDays(1);
            }

            return result;
        }

        private string GetUPEHString()
        {
            try
            {
                if (UnitPerHourList == null) return string.Empty;
                if (UnitPerHourList.Count <= 0) return string.Empty;

                return string.Join("\n", UnitPerHourList.Select(
                    x => string.Format("{0}H, {1}",
                        x.StartTime.ToString("yyyy-MM-dd HH"),
                        x.OutputCount)));
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.Write(e.ToString());
                return string.Empty;
            }
        }
    }
}
