using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FAFramework.Manager
{
    public class LogRetentionSettingItem : INotifyPropertyChanged
    {
        private int _days;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public string Key { get; set; }
        public string DisplayName { get; set; }
        public string TargetPath { get; set; }

        public int Days
        {
            get { return _days; }
            set
            {
                if (value < 1) value = 1;
                if (_days == value) return;

                _days = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Days"));
            }
        }

        public LogRetentionSettingItem Clone(int days)
        {
            return new LogRetentionSettingItem
            {
                Key = Key,
                DisplayName = DisplayName,
                TargetPath = TargetPath,
                Days = days
            };
        }
    }

    public static class LogRetentionSetting
    {
        public const int DEFAULT_RETENTION_DAYS = 7;
        public const string SETTING_FILE_NAME = "log_setting.cfg";

        public const string KEY_IMARK_LOG = "IMarkLog";
        public const string KEY_TRACE_LOG = "TraceLog";
        public const string KEY_ALARM_LOG = "AlarmLog";
        public const string KEY_SYSTEM_LOG = "SystemLog";
        public const string KEY_STATE_LOG = "StateLog";
        public const string KEY_DEBUG_LOG = "DebugLog";
        public const string KEY_EC_LOG = "ECLog";
        public const string KEY_PRODUCT_LOG = "ProductLog";
        public const string KEY_MTBI = "MTBI";
        public const string KEY_TP_LOG = "TPLog";
        public const string KEY_TP_LOG_FTP = "TPLogFTP";
        public const string KEY_PACKING_LOG = "PackingLog";
        public const string KEY_CONFIG_BACKUP = "ConfigBackup";

        private static readonly object _syncRoot = new Object();

        private static readonly LogRetentionSettingItem[] DEFAULT_ITEMS =
        {
            new LogRetentionSettingItem { Key = KEY_IMARK_LOG, DisplayName = "IMarkLog", TargetPath = @"Log\장비명\IMarkLog" },
            new LogRetentionSettingItem { Key = KEY_TRACE_LOG, DisplayName = "TraceLog", TargetPath = @"Log\장비명\TraceLog" },
            new LogRetentionSettingItem { Key = KEY_ALARM_LOG, DisplayName = "AlarmLog", TargetPath = @"Log\장비명\AlarmLog" },
            new LogRetentionSettingItem { Key = KEY_SYSTEM_LOG, DisplayName = "SystemLog", TargetPath = @"Log\SystemLog" },
            new LogRetentionSettingItem { Key = KEY_STATE_LOG, DisplayName = "StateLog", TargetPath = @"Log\장비명\StateLog" },
            new LogRetentionSettingItem { Key = KEY_DEBUG_LOG, DisplayName = "DebugLog", TargetPath = @"Log\장비명\DebugLog" },
            new LogRetentionSettingItem { Key = KEY_EC_LOG, DisplayName = "ECLog", TargetPath = @"Log\장비명\ECLog" },
            new LogRetentionSettingItem { Key = KEY_PRODUCT_LOG, DisplayName = "ProductLog", TargetPath = @"Log\장비명\ProductLog / ProductOutput" },
            new LogRetentionSettingItem { Key = KEY_MTBI, DisplayName = "MTBI", TargetPath = @"Log\장비명\MTBI" },
            new LogRetentionSettingItem { Key = KEY_TP_LOG, DisplayName = "TPLog", TargetPath = @"Log\TPLog" },
            new LogRetentionSettingItem { Key = KEY_TP_LOG_FTP, DisplayName = @"TPLog\FTP", TargetPath = @"Log\TPLog\FTP" },
            new LogRetentionSettingItem { Key = KEY_PACKING_LOG, DisplayName = "PackingLog", TargetPath = @"c:\EQP_LOG" },
            new LogRetentionSettingItem { Key = KEY_CONFIG_BACKUP, DisplayName = "config backup 설정 백업", TargetPath = @"c:\backup\config" }
        };

        public static string SettingFilePath
        {
            get { return Path.Combine(ConfigClasses.GlobalConst.CONFIG_PATH, SETTING_FILE_NAME); }
        }

        public static List<LogRetentionSettingItem> LoadItems()
        {
            Dictionary<string, int> settings = LoadSettings();

            return DEFAULT_ITEMS
                .Select(item => item.Clone(GetDays(settings, item.Key)))
                .ToList();
        }

        public static void SaveItems(IEnumerable<LogRetentionSettingItem> items)
        {
            if (items == null) return;

            Dictionary<string, int> settings = LoadSettings();

            foreach (var item in items)
            {
                if (item == null) continue;
                if (string.IsNullOrWhiteSpace(item.Key)) continue;
                if (IsKnownKey(item.Key) == false) continue;

                settings[item.Key] = item.Days < 1 ? DEFAULT_RETENTION_DAYS : item.Days;
            }

            SaveSettings(settings);
        }

        public static int GetRetentionDays()
        {
            return GetRetentionDays(KEY_TRACE_LOG);
        }

        public static int GetRetentionDays(string key)
        {
            try
            {
                Dictionary<string, int> settings = LoadSettings();
                return GetDays(settings, key);
            }
            catch (Exception e)
            {
                Trace.WriteLine(DateTime.Now + "," + e.ToString());
            }

            return DEFAULT_RETENTION_DAYS;
        }

        public static TimeSpan GetRetentionPeriod()
        {
            return TimeSpan.FromDays(GetRetentionDays());
        }

        public static TimeSpan GetRetentionPeriod(string key)
        {
            return TimeSpan.FromDays(GetRetentionDays(key));
        }

        public static void EnsureSettingFile()
        {
            try
            {
                lock (_syncRoot)
                {
                    EnsureSettingDirectory();

                    if (File.Exists(SettingFilePath) == false)
                    {
                        SaveSettings(CreateDefaultSettings());
                        return;
                    }

                    Dictionary<string, int> settings = ReadSettingsFromFile();
                    if (NeedsRewriteSettingFile() || HasAllKeys(settings) == false)
                        SaveSettings(settings);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(DateTime.Now + "," + e.ToString());
            }
        }

        public static bool IsExpired(string path, DateTime now, TimeSpan retention)
        {
            try
            {
                if (File.Exists(path) == false) return false;

                return now - File.GetLastWriteTime(path) > retention;
            }
            catch (Exception e)
            {
                Trace.WriteLine(DateTime.Now + "," + e.ToString());
                return false;
            }
        }

        public static void DeleteExpiredFiles(string directory, bool includeSubDirectories)
        {
            DeleteExpiredFiles(directory, KEY_TRACE_LOG, includeSubDirectories);
        }

        public static void DeleteExpiredFiles(string directory, string key, bool includeSubDirectories)
        {
            DeleteExpiredFiles(directory, GetRetentionPeriod(key), includeSubDirectories);
        }

        public static void DeleteExpiredFiles(string directory, TimeSpan retention, bool includeSubDirectories)
        {
            if (string.IsNullOrWhiteSpace(directory)) return;
            if (Directory.Exists(directory) == false) return;

            DateTime now = DateTime.Now;

            foreach (var file in Directory.GetFiles(directory))
            {
                try
                {
                    if (IsExpired(file, now, retention))
                        File.Delete(file);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(DateTime.Now + "," + e.ToString());
                }
            }

            if (includeSubDirectories == false) return;

            foreach (var subDirectory in Directory.GetDirectories(directory))
            {
                DeleteExpiredFiles(subDirectory, retention, true);
                DeleteEmptyDirectory(subDirectory, now, retention);
            }
        }

        private static Dictionary<string, int> LoadSettings()
        {
            lock (_syncRoot)
            {
                EnsureSettingDirectory();

                if (File.Exists(SettingFilePath) == false)
                    SaveSettings(CreateDefaultSettings());

                Dictionary<string, int> settings = ReadSettingsFromFile();
                if (NeedsRewriteSettingFile() || HasAllKeys(settings) == false)
                    SaveSettings(settings);

                return settings;
            }
        }

        private static Dictionary<string, int> ReadSettingsFromFile()
        {
            Dictionary<string, int> settings = CreateDefaultSettings();

            try
            {
                string[] lines = File.ReadAllLines(SettingFilePath);
                if (lines.Length == 1)
                {
                    int legacyDays;
                    if (int.TryParse(lines[0].Trim(), out legacyDays) && legacyDays > 0)
                    {
                        foreach (var key in GetKnownKeys())
                            settings[key] = legacyDays;

                        return settings;
                    }
                }

                foreach (var line in lines)
                {
                    string text = line.Trim();
                    if (string.IsNullOrWhiteSpace(text)) continue;
                    if (text.StartsWith("#")) continue;

                    string[] values = text.Split(new[] { '=' }, 2);
                    if (values.Length != 2) continue;

                    string key = values[0].Trim();
                    string value = values[1].Trim();
                    int days;

                    if (IsKnownKey(key) && int.TryParse(value, out days) && days > 0)
                        settings[key] = days;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(DateTime.Now + "," + e.ToString());
            }

            return settings;
        }

        private static void SaveSettings(Dictionary<string, int> settings)
        {
            try
            {
                EnsureSettingDirectory();

                List<string> lines = new List<string>();
                lines.Add("# Log retention days");
                lines.Add("# The program uses 7 days when a value is missing or invalid.");

                foreach (var item in DEFAULT_ITEMS)
                {
                    int days = GetDays(settings, item.Key);
                    lines.Add(item.Key + "=" + days);
                }

                File.WriteAllLines(SettingFilePath, lines.ToArray());
            }
            catch (Exception e)
            {
                Trace.WriteLine(DateTime.Now + "," + e.ToString());
            }
        }

        private static Dictionary<string, int> CreateDefaultSettings()
        {
            Dictionary<string, int> settings = new Dictionary<string, int>();

            foreach (var key in GetKnownKeys())
                settings[key] = DEFAULT_RETENTION_DAYS;

            return settings;
        }

        private static IEnumerable<string> GetKnownKeys()
        {
            return DEFAULT_ITEMS.Select(item => item.Key);
        }

        private static bool IsKnownKey(string key)
        {
            return DEFAULT_ITEMS.Any(item => item.Key == key);
        }

        private static bool HasAllKeys(Dictionary<string, int> settings)
        {
            return GetKnownKeys().All(key => settings.ContainsKey(key));
        }

        private static bool NeedsRewriteSettingFile()
        {
            try
            {
                string[] lines = File.ReadAllLines(SettingFilePath);
                if (lines.Length == 1)
                {
                    int days;
                    if (int.TryParse(lines[0].Trim(), out days) && days > 0)
                        return true;
                }

                return GetKnownKeys().Any(key =>
                    lines.Any(line => line.Trim().StartsWith(key + "=", StringComparison.OrdinalIgnoreCase)) == false);
            }
            catch (Exception e)
            {
                Trace.WriteLine(DateTime.Now + "," + e.ToString());
                return false;
            }
        }

        private static int GetDays(Dictionary<string, int> settings, string key)
        {
            int days;
            if (settings != null && settings.TryGetValue(key, out days) && days > 0)
                return days;

            return DEFAULT_RETENTION_DAYS;
        }

        private static void EnsureSettingDirectory()
        {
            string directory = Path.GetDirectoryName(SettingFilePath);
            if (Directory.Exists(directory) == false)
                Directory.CreateDirectory(directory);
        }

        private static void DeleteEmptyDirectory(string directory, DateTime now, TimeSpan retention)
        {
            try
            {
                if (Directory.Exists(directory) == false) return;
                if (Directory.GetFiles(directory).Length > 0) return;
                if (Directory.GetDirectories(directory).Length > 0) return;
                if (now - Directory.GetLastWriteTime(directory) <= retention) return;

                Directory.Delete(directory);
            }
            catch (Exception e)
            {
                Trace.WriteLine(DateTime.Now + "," + e.ToString());
            }
        }
    }
}
