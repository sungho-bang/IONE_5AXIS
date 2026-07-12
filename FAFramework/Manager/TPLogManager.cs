using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.IO.Compression;

namespace FAFramework.Manager
{
    public class TPLogManager
    {
        public static readonly string LOG_PATH = Path.Combine(FAFramework.ConfigClasses.GlobalConst.ROOT_PATH, @"Log\TPLog\");
        public static readonly string FTP_LOG_PATH = Path.Combine(FAFramework.ConfigClasses.GlobalConst.ROOT_PATH, @"Log\TPLog\FTP\");

        private static volatile TPLogManager _instance = null;
        private static object syncRoot = new Object();
        private static object threadRoot = new Object();

        private Queue<Action> _logQueue = new Queue<Action>();

        private Thread _thread;

        public bool Run { get; set; }

        private readonly int DEFAULT_AUTO_CLEAR_DATE = 30;
        private int _autoClearDate;

        private TPLogManager()
        {
            Run = true;

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

                            DateTime now = DateTime.Now.AddMinutes(-1);
                            if (now.Hour != lastTime.Hour)
                            {
                                string filename = now.ToString(@"yyyyMMddHH") + ".log";
                                MoveFileToFTPFolder(LOG_PATH, filename);
                            }

                            AutoClearLogFiles(LOG_PATH);
                            AutoClearLogFiles(FTP_LOG_PATH);
                            lastTime = DateTime.Now.AddMinutes(-1);
                            Thread.Sleep(10);
                        }
                    });

            _thread.Start();
        }

        public static TPLogManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                            _instance = new TPLogManager();
                    }
                }

                return _instance;
            }
        }

        public DateTime WriteLog(Utility.SamsungTPLog.LogBase log)
        {
            DateTime date = DateTime.Now;

            string pathAndFileName = Path.Combine(LOG_PATH, date.ToString(@"yyyyMMddHH") + ".log");

            lock (threadRoot)
            {
                _logQueue.Enqueue(
                    delegate ()
                    {
                        if (Directory.Exists(LOG_PATH) == false)
                            Directory.CreateDirectory(LOG_PATH);

                        try
                        {
                            File.AppendAllLines(pathAndFileName, new string[] { log.ToString() });
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine(DateTime.Now + "," + e.ToString());
                        }
                    });
            }

            return date;
        }

        private void MoveFileToFTPFolder(string path, string filename)
        {
            try
            {
                string sourceFile = Path.Combine(path, filename);
                if (Directory.Exists(path) == false)
                    Directory.CreateDirectory(path);

                if (File.Exists(sourceFile) == false)
                    File.WriteAllText(sourceFile, "");

                if (Directory.Exists(FTP_LOG_PATH) == false)
                    Directory.CreateDirectory(FTP_LOG_PATH);

                string destFilename = Path.Combine(FTP_LOG_PATH, filename);

                Compress(new FileInfo(sourceFile), new FileInfo(destFilename));
            }
            catch (Exception e)
            {
                Trace.WriteLine(DateTime.Now + "," + e.ToString());
            }
        }

        public static void Compress(FileInfo sourceFile, FileInfo fileToCompress)
        {
            using (FileStream originalFileStream = sourceFile.OpenRead())
            {
                if ((File.GetAttributes(sourceFile.FullName) & FileAttributes.Hidden) != FileAttributes.Hidden & sourceFile.Extension != ".gz")
                {
                    using (FileStream compressedFileStream = File.Create(fileToCompress.FullName + ".gz"))
                    {
                        using (GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
                        {
                            originalFileStream.CopyTo(compressionStream);
                        }
                    }
                }
            }
        }

        private void AutoClearLogFiles(string path)
        {
            if (Directory.Exists(path) == false) return;

            TimeSpan autoClearDate = new TimeSpan((int)GetAutoClearDate(), 0, 0, 0);
            DateTime now = DateTime.Now;

            foreach (var file in Directory.GetFiles(path))
            {
                if (File.Exists(file) == false) continue;

                var fileInfo = new FileInfo(file);
                if (now - fileInfo.CreationTime > autoClearDate)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch
                    {
                    }
                }
            }
        }

        private int GetAutoClearDate()
        {
            string path = Path.Combine(FAFramework.ConfigClasses.GlobalConst.CONFIG_PATH, "log_setting.cfg");
            if (File.Exists(path))
            {
                try
                {
                    string text = File.ReadAllText(path);

                    if (int.TryParse(text, out _autoClearDate) == false)
                        _autoClearDate = DEFAULT_AUTO_CLEAR_DATE;

                }
                catch
                {
                    _autoClearDate = DEFAULT_AUTO_CLEAR_DATE;
                }
            }
            else
            {
                _autoClearDate = DEFAULT_AUTO_CLEAR_DATE;
            }

            return _autoClearDate;
        }
    }
}
