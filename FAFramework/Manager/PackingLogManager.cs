using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace FAFramework.Manager
{
    public class PackingLogManager
    {
        public static readonly string LOG_PATH = "c:\\EQP_LOG\\";

        private static object threadRoot = new Object();

        private Queue<Action> _logQueue = new Queue<Action>();

        private Thread _thread;

        public bool Run { get; set; }

        public PackingLogManager()
        {
            Run = true;
            LogRetentionSetting.EnsureSettingFile();

            DateTime lastTime = DateTime.Now.AddMinutes(-1);

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

                            var now = DateTime.Now;

                            if (now.Day != lastTime.Day)
                            {
                                var retention = LogRetentionSetting.GetRetentionPeriod(LogRetentionSetting.KEY_PACKING_LOG);
                                DeleteAllFile(LOG_PATH,
                                    delegate (string filename)
                                    {
                                        return LogRetentionSetting.IsExpired(filename, now, retention);
                                    }, true);
                            }

                            lastTime = now;

                            Thread.Sleep(10);
                        }
                    });

            _thread.Start();
        }

        public DateTime WriteNotAppendLog(string logTypeName, DateTime date, params string[] log)
        {
            string path = Path.Combine(LOG_PATH, logTypeName);
            string pathAndFileName = Path.Combine(path,
                string.Format("{0}_{1}.log", logTypeName, date.ToString(@"yyyyMMdd")));

            lock (threadRoot)
            {
                _logQueue.Enqueue(
                    delegate ()
                    {
                        if (Directory.Exists(path) == false)
                            Directory.CreateDirectory(path);

                        StreamWriter sw = null;
                        try
                        {
                            using (sw = new StreamWriter(pathAndFileName, false))
                            {
                                foreach (var item in log)
                                    sw.WriteLine(item);
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

        public DateTime WriteLog(string logTypeName, Utility.PackingLog.LogBase log)
        {
            DateTime date = log.Date;
            string path = Path.Combine(LOG_PATH, logTypeName);
            string pathAndFileName = Path.Combine(path,
                string.Format("{0}_{1}.log", logTypeName, date.ToString(@"yyyyMMdd")));

            lock (threadRoot)
            {
                _logQueue.Enqueue(
                    delegate ()
                    {
                        if (Directory.Exists(path) == false)
                            Directory.CreateDirectory(path);

                        StreamWriter sw = null;
                        try
                        {
                            using (sw = new StreamWriter(pathAndFileName, true))
                                sw.WriteLine(log.ToString());
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

        public void WriteAlarmLog(Utility.PackingLog.LogBase log)
        {
            WriteLog("Error", log);
        }

        public void WriteEventLog(Utility.PackingLog.LogBase log)
        {
            WriteLog("Event", log);
        }

        public void WriteProductLog(Utility.PackingLog.LogBase log)
        {
            WriteLog("Product", log);
        }

        public void WriteMaterialLog(params string[] text)
        {
            WriteNotAppendLog("Mat", DateTime.Now, text);
        }

        private void DeleteAllFile(string directory, Func<string, bool> compare, bool includeSubDirectories)
        {
            if (!Directory.Exists(directory)) return;

            var files = Directory.GetFiles(directory);
            foreach (var item in files)
            {
                if (compare(item) == true)
                {
                    File.Delete(item);
                }
            }

            var dir = Directory.GetDirectories(directory);
            if (dir != null)
            {
                foreach (var item in dir)
                {
                    DeleteAllFile(item, compare, includeSubDirectories);
                }
            }
        }
    }
}
