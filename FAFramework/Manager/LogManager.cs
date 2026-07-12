using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Diagnostics;
using FAFramework.Utility;

namespace FAFramework.Manager
{
    public enum SendReceiveKind
    {
        Send, Receive
    }

    public class SystemLogInfo
    {
        public string Message { get; set; }

        public SystemLogInfo()
        {
        }

        public SystemLogInfo(string message)
        {
            Message = message;
        }

        public override string ToString()
        {
            return Message;
        }
    }

    public class TraceLogInfo
    {
        public string Message { get; set; }

        public TraceLogInfo()
        {
        }

        public TraceLogInfo(string message)
        {
            Message = message;
        }

        public override string ToString()
        {
            return Message;
        }
    }

    public class AlarmLogInfo
    {
        public FALibrary.Alarm.FAAlarmEventArgs Alarm { get; set; }
        public bool RankingData { get; set; }
        public bool AutoRunning { get; set; }
    }

    public class ECLogInfo
    {
        public SendReceiveKind SendReceive { get; set; }
        public string Message { get; set; }
    }

    public class SimaxLogInfo
    {
        public SendReceiveKind SendReceive { get; set; }
        public string Message { get; set; }
    }

    public class ProductLogInfo
    {
        public FAProductInfo Product { get; set; }
    }

    public class MachineStateLog
    {
        public enum EState
        {
            NONE, START, STOP, ALARM, RELEASE_ALARM, RUNDOWN
        }

        public DateTime Date { get; set; }
        public EState State { get; set; }
    }

    public class LogEventArgs : EventArgs
    {
        public Equipment.EquipmentBase Equipment { get; set; }
        public DateTime Date { get; set; }
        public object Log { get; set; }
    }

    public class LogManager
    {
        private readonly TimeSpan DEFAULT_AUTO_CLEAR_DATE = new TimeSpan(60, 0, 0, 0);

        public static readonly string LOG_ROOT_PATH = Path.Combine(FAFramework.ConfigClasses.GlobalConst.ROOT_PATH, "Log");
        public static readonly string SYSTEM_LOG_PATH = AppDomain.CurrentDomain.BaseDirectory + @"Log\SystemLog\";
        public static readonly string TRACELOG_PATH = @"TraceLog\";
        public static readonly string ALARMLOG_PATH = @"AlarmLog\";
        public static readonly string ECLOG_PATH = @"ECLog\";
        public static readonly string PRODUCT_PATH = @"ProductLog\";
        public static readonly string SIMAXLOG_PATH = @"SimaxLog\";
        public static readonly string EC_PARSING_LOG_PATH = @"ECLog\ParsingDataLog\";
        public static readonly string STATE_LOG_PATH = @"StateLog\";
        public static readonly string DEBUG_LOG_PATH = @"DebugLog\";
        public static readonly string MTBILOG_PATH = @"MTBI\";

        private static volatile LogManager _instance = null;
        private static object syncRoot = new Object();
        private static object threadRoot = new Object();

        private Queue<Action> _logQueue = new Queue<Action>();

        private Thread _thread;

        public event EventHandler<LogEventArgs> OnWriteSystemLog = null;
        public event EventHandler<LogEventArgs> OnWriteTraceLog = null;
        public event EventHandler<LogEventArgs> OnWriteAlarmLog = null;
        public event EventHandler<LogEventArgs> OnWriteECLog = null;
        public event EventHandler<LogEventArgs> OnWriteProductLog = null;
        public event EventHandler<LogEventArgs> OnWriteSimaxLog = null;
        public event EventHandler<LogEventArgs> OnWriteECParsingLog = null;
        public event EventHandler<LogEventArgs> OnWriteStateLog = null;

        public bool IsEnabledTraceLog { get; set; }

        public bool Run { get; set; }

        private LogManager()
        {
            Run = true;

            IsEnabledTraceLog = true;

            _thread = new Thread(
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

            _thread.Start();
        }

        public static LogManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                            _instance = new LogManager();
                    }
                }

                return _instance;
            }
        }

        public void WriteSystemLog(string log)
        {
            WriteSystemLog(new SystemLogInfo(log));
        }

        public void WriteSystemLog(object log)
        {
            try
            {

                DateTime date = WriteLog(SYSTEM_LOG_PATH, string.Empty, log);

                if (OnWriteSystemLog != null)
                {
                    LogEventArgs e = new LogEventArgs();
                    e.Date = date;
                    e.Log = log;
                    OnWriteSystemLog(this, e);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(DateTime.Now + "," + e.ToString());
            }
        }

        public void WriteTraceLog(Equipment.EquipmentBase equipment, string log)
        {
            WriteTraceLog(equipment, new TraceLogInfo(log));
        }

        public void WriteTraceLog(Equipment.EquipmentBase equipment, object log)
        {
            try
            {
                if (IsEnabledTraceLog == false) return;

                string path = Path.Combine(LOG_ROOT_PATH, equipment.Name, TRACELOG_PATH);
                DateTime date = WriteCSVLog(path, string.Empty, log, "\t");

                if (OnWriteTraceLog != null)
                {
                    LogEventArgs e = new LogEventArgs();
                    e.Equipment = equipment;
                    e.Date = date;
                    e.Log = log;
                    OnWriteTraceLog(this, e);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(DateTime.Now + "," + e.ToString());
            }
        }

        public void WriteAlarmLog(Equipment.EquipmentBase equipment, object log)
        {
            try
            {
                string path = Path.Combine(LOG_ROOT_PATH, equipment.Name, ALARMLOG_PATH);
                DateTime date = WriteLog(path, string.Empty, log);

                if (OnWriteAlarmLog != null)
                {
                    LogEventArgs e = new LogEventArgs();
                    e.Equipment = equipment;
                    e.Date = date;
                    e.Log = log;
                    OnWriteAlarmLog(this, e);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(DateTime.Now + "," + e.ToString());
            }
        }

        public void WriteECCommLog(Equipment.EquipmentBase equipment, object log)
        {
            try
            {
                string path = Path.Combine(LOG_ROOT_PATH, equipment.Name, ECLOG_PATH);
                DateTime date = WriteCSVLog(path, string.Empty, log, "\t");

                if (OnWriteECLog != null)
                {
                    LogEventArgs e = new LogEventArgs();
                    e.Equipment = equipment;
                    e.Date = date;
                    e.Log = log;
                    OnWriteECLog(this, e);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(DateTime.Now + "," + e.ToString());
            }
        }

        public void WriteECParsingLog(Equipment.EquipmentBase equipment, object log)
        {
            try
            {
                string path = Path.Combine(LOG_ROOT_PATH, equipment.Name, EC_PARSING_LOG_PATH);
                DateTime date = WriteCSVLog(path, string.Empty, log, "\t");

                if (OnWriteECParsingLog != null)
                {
                    LogEventArgs e = new LogEventArgs();
                    e.Equipment = equipment;
                    e.Date = date;
                    e.Log = log;
                    OnWriteECParsingLog(this, e);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(DateTime.Now + "," + e.ToString());
            }
        }

        public void WriteProductLog(Equipment.EquipmentBase equipment, object log)
        {
            try
            {
                string operatorID = "";
                try
                {
                    //if (Equipment.EquipmentManager.Instance.MainEquip.OperatorID != null)
                    //    operatorID = Equipment.EquipmentManager.Instance.MainEquip.OperatorID;
                }
                catch
                {
                }

                string line = operatorID + ", " + log;
                string path = Path.Combine(LOG_ROOT_PATH, equipment.Name, PRODUCT_PATH);
                DateTime date = WriteCSVLog(path, string.Empty, log, "\t");

                if (OnWriteProductLog != null)
                {
                    LogEventArgs e = new LogEventArgs();
                    e.Equipment = equipment;
                    e.Date = date;
                    e.Log = log;
                    OnWriteProductLog(this, e);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(DateTime.Now + "," + e.ToString());
            }
        }

        public void WriteSimaxLog(Equipment.EquipmentBase equipment, object log)
        {
            try
            {
                string path = Path.Combine(LOG_ROOT_PATH, equipment.Name, SIMAXLOG_PATH);
                DateTime date = WriteLog(path, string.Empty, log);

                if (OnWriteSimaxLog != null)
                {
                    LogEventArgs e = new LogEventArgs();
                    e.Equipment = equipment;
                    e.Date = date;
                    e.Log = log;
                    OnWriteSimaxLog(this, e);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(DateTime.Now + "," + e.ToString());
            }
        }

        public void WriteStateLog(Equipment.EquipmentBase equipment, object log)
        {
            try
            {
                string path = Path.Combine(LOG_ROOT_PATH, equipment.Name, STATE_LOG_PATH);
                DateTime date = WriteLog(path, string.Empty, log);

                if (OnWriteStateLog != null)
                {
                    LogEventArgs e = new LogEventArgs();
                    e.Equipment = equipment;
                    e.Date = date;
                    e.Log = log;
                    OnWriteStateLog(this, e);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(DateTime.Now + "," + e.ToString());
            }
        }

        public void WriteDebugLog(Equipment.EquipmentBase equipment, object log)
        {
            try
            {
                string path = Path.Combine(LOG_ROOT_PATH, equipment.Name, DEBUG_LOG_PATH);
                DateTime date = WriteLog(path, string.Empty, log);

                if (OnWriteStateLog != null)
                {
                    LogEventArgs e = new LogEventArgs();
                    e.Equipment = equipment;
                    e.Date = date;
                    e.Log = log;
                    OnWriteStateLog(this, e);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(DateTime.Now + "," + e.ToString());
            }
        }

        public DateTime WriteLog(string logRootPath, string filename_prefix, object log, bool useShiftTime = false)
        {
            DateTime date = DateTime.Now;
            var saveDate = date;
            if (useShiftTime)
            {
                var shift = Equipment.MainEquipment.GetShift(saveDate.TimeOfDay);
                if (shift == Equipment.MainEquipment.ShiftType.GY &&
                    saveDate.TimeOfDay > Equipment.MainEquipment.GY_SHIFT_START_TIME)
                {
                    saveDate = saveDate.AddDays(1);
                }
            }

            string path = Path.Combine(logRootPath, saveDate.ToString("yyyy"), saveDate.ToString("MM"));
            string pathAndFileName = Path.Combine(path, filename_prefix + saveDate.ToString(@"yyyy-MM-dd") + ".log");

            lock (threadRoot)
            {
                _logQueue.Enqueue(
                    delegate ()
                    {
                        if (Directory.Exists(path) == false)
                            Directory.CreateDirectory(path);

                        XElement root = new XElement("Logs");

                        try
                        {
                            if (File.Exists(pathAndFileName) == true)
                                root = XElement.Load(pathAndFileName);

                            var sub = new XElement("Log");

                            sub.Add(new XElement("Date", date));
                            sub.Add(log.ToXElement());
                            root.AddFirst(sub);
                            root.Save(pathAndFileName);

                            if (log is Utility.SamsungTPLog.LogBase)
                            {

                            }
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine(DateTime.Now + "," + e.ToString());
                        }
                    });
            }

            return date;
        }

        public DateTime WriteNotAppedLog(string logRootPath, string filename_prefix, object log, string seperator, bool useShiftTime = false)
        {
            DateTime date = DateTime.Now;
            var saveDate = date;
            if (useShiftTime)
            {
                var shift = Equipment.MainEquipment.GetShift(saveDate.TimeOfDay);
                if (shift == Equipment.MainEquipment.ShiftType.GY &&
                    saveDate.TimeOfDay > Equipment.MainEquipment.GY_SHIFT_START_TIME)
                {
                    saveDate = saveDate.AddDays(1);
                }
            }

            string path = Path.Combine(logRootPath, saveDate.ToString("yyyy"), saveDate.ToString("MM"));
            string pathAndFileName = Path.Combine(path, filename_prefix + saveDate.ToString(@"yyyy-MM-dd") + ".log");

            lock (threadRoot)
            {
                _logQueue.Enqueue(
                    delegate ()
                    {
                        if (Directory.Exists(path) == false)
                            Directory.CreateDirectory(path);

                        XElement root = new XElement("Logs");

                        try
                        {
                            var sub = new XElement("Log");

                            sub.Add(new XElement("Date", date));
                            sub.Add(log.ToXElement());
                            root.AddFirst(sub);
                            root.Save(pathAndFileName);
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine(DateTime.Now + "," + e.ToString());
                        }
                    });
            }

            return date;
        }

        public DateTime WriteCSVLog(string logRootPath, string filename, object log, string seperator, bool useShiftTime = false)
        {
            DateTime date = DateTime.Now;
            var saveDate = date;
            if (useShiftTime)
            {
                var shift = Equipment.MainEquipment.GetShift(saveDate.TimeOfDay);
                if (shift == Equipment.MainEquipment.ShiftType.GY &&
                    saveDate.TimeOfDay > Equipment.MainEquipment.GY_SHIFT_START_TIME)
                {
                    saveDate = saveDate.AddDays(1);
                }
            }

            string path = Path.Combine(logRootPath, saveDate.ToString("yyyy"), saveDate.ToString("MM"));
            string pathAndFileName = Path.Combine(path, filename + saveDate.ToString(@"yyyy-MM-dd") + ".log");

            lock (threadRoot)
            {
                _logQueue.Enqueue(
                    delegate ()
                    {
                        var now = DateTime.Now;
                        Utility.FileUtility.DeleteAllFile(FAFramework.ConfigClasses.GlobalConst.CONFIG_BACKUP_PATH,
                                    delegate (string deleteFileName)
                                    {
                                        var creationTime = File.GetCreationTime(deleteFileName);
                                        if (now - creationTime > DEFAULT_AUTO_CLEAR_DATE)
                                            return true;
                                        else
                                            return false;
                                    }, true);

                        if (Directory.Exists(path) == false)
                            Directory.CreateDirectory(path);

                        StreamWriter sw = null;
                        try
                        {
                            if (!File.Exists(pathAndFileName))
                            {
                                var method = log.GetType().GetMethod("GetHeaders");
                                if (method != null)
                                {
                                    var headers = new List<string>(method.Invoke(log, null) as string[]);
                                    headers.Insert(0, "DateTime");
                                    using (var s = new StreamWriter(pathAndFileName, true))
                                        s.WriteLine(string.Join(seperator, headers));
                                }
                            }

                            using (sw = new StreamWriter(pathAndFileName, true))
                            {
                                sw.WriteLine(date.ToString("yyyy-MM-dd HH:mm:ss.fff") + seperator + log);
                            }
                        }
                        catch
                        {
                        }
                        finally
                        {
                            if (sw != null)
                            {
                                sw.Close();
                                sw.Dispose();
                            }
                        }
                    });
            }

            return date;
        }
    }
}
