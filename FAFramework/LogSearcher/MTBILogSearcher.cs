using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Diagnostics;
using FAFramework.Utility;
using FAFramework.Manager;
using System.Threading.Tasks;
using System.Collections;

namespace FAFramework.LogSearcher
{
    public class MTBILogInfo
    {
        public DateTime Date { get; set; }
        public TimeSpan RunTime { get; set; }
        public TimeSpan RunDownTime { get; set; }
        public TimeSpan StopTime { get; set; }
        public TimeSpan AlarmTime { get; set; }
        public TimeSpan MTBI { get; set; }
        public int AlarmCount { get; set; }

        public override string ToString()
        {
            return string.Join("\t",
                new string[]
                {
                    Date.ToString("yyyy-MM-dd"),
                    RunTime.ToString(@"dd\.hh\:mm\:ss"),
                    RunDownTime.ToString(@"dd\.hh\:mm\:ss"),
                    StopTime.ToString(@"dd\.hh\:mm\:ss"),
                    AlarmTime.ToString(@"dd\.hh\:mm\:ss"),
                    MTBI.ToString(@"dd\.hh\:mm\:ss"),
                    AlarmCount.ToString()
                });
        }
    }

    public class MTBILogSearcher : LogSearcherBase
    {
        private struct MTBILogParameter
        {
            public DateTime BeginDate { get; set; }
            public DateTime EndDate { get; set; }
        }

        public MTBILogSearcher()
        {
            Search = new Utility.CommandHandler(SearchCommandHandler, true);
            SaveToCSV = new Utility.CommandHandler(SaveToCSVCommandHandler, true);
        }

        private void SearchCommandHandler(object param)
        {
            try
            {
                MTBILogParameter logParam;
                if (ParsingMTBISearchParameters(param, out logParam) == false) return;

                string path = Path.Combine(ConfigClasses.GlobalConst.ROOT_PATH, "Log", EquipmentInstance.Name, LogManager.MTBILOG_PATH);

                System.Threading.Tasks.Task.Factory.StartNew(
                    delegate ()
                    {
                        Search.SetCanExecute(false);

                        SearchTraceLog(path, logParam);

                        Search.SetCanExecute(true);
                    });
            }
            catch (Exception e)
            {
                Manager.MessageWindowManager.Instance.Show(EquipmentInstance, "AlarmLogError", string.Format("Alarm log search fail", e.ToString()));
            }
        }

        private void SaveToCSVCommandHandler(object param)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.Filter = "All CSV Files | *.csv";
                if ((bool)dlg.ShowDialog() == false) return;

                if (param == null) return;
                if ((param is IEnumerable) == false) return;

                List<string> result = new List<string>();

                foreach (dynamic item in param as IEnumerable)
                {
                    result.Add(item.ToString());
                }

                File.WriteAllLines(dlg.FileName, result, Encoding.UTF8);
            }
            catch (Exception e)
            {
                Manager.MessageWindowManager.Instance.Show(EquipmentInstance, "AlarmLogError", string.Format("Can not convert log to CSV", e.ToString()));
            }
        }

        private bool ParsingMTBISearchParameters(object param, out MTBILogParameter result)
        {
            result = new MTBILogParameter();

            if (param == null)
            {
                string msg = Utility.UtilityClass.GetStringResource(this, "ThereIsNoData", "There is no data.");
                Manager.MessageWindowManager.Instance.Show(EquipmentInstance, "UserSelectet", msg);
            }

            if ((param is object[]) == false)
            {
                Manager.LogManager.Instance.WriteSystemLog("SearchCommandHandler's param is not object[].");
                return false;
            }

            var parameters = param as object[];

            try
            {
                result.BeginDate = (parameters[0] as LogSearchingDateTime).ToDateTime();
                result.EndDate = (parameters[1] as LogSearchingDateTime).ToDateTime();

                return true;
            }
            catch (Exception e)
            {
                Manager.LogManager.Instance.WriteSystemLog(e.ToString());
                return false;
            }
        }

        private void SearchTraceLog(string path, MTBILogParameter param)
        {
            var files = GetLogFiles(path, param.BeginDate, param.EndDate);

            var result = new List<MTBILogInfo>();

            foreach (var file in files)
            {
                try
                {
                    if (File.Exists(file) == false) continue;

                    result.Add(GetMTBILog(file));
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.ToString());
                    continue;
                }
            }

            FindResult = null;

            Result =
                new
                {
                    LogList = result
                };
        }

        private MTBILogInfo GetMTBILog(string filename)
        {
            var newObj = new MTBILogInfo();
            newObj.Date = DateTime.Parse(System.IO.Path.GetFileNameWithoutExtension(filename));

            var xml = XElement.Load(filename);
            if (xml.Element("RunTime") != null)
                newObj.RunTime = TimeSpan.Parse(xml.Element("RunTime").Value);

            if (xml.Element("RunDownTime") != null)
                newObj.RunDownTime = TimeSpan.Parse(xml.Element("RunDownTime").Value);

            if (xml.Element("AlarmTime") != null)
                newObj.AlarmTime = TimeSpan.Parse(xml.Element("AlarmTime").Value);

            var stopTime = new TimeSpan(24, 0, 0) - newObj.RunTime - newObj.RunDownTime - newObj.AlarmTime;
            newObj.StopTime = stopTime;

            if (xml.Element("MTBI") != null)
                newObj.MTBI = TimeSpan.Parse(xml.Element("MTBI").Value);

            if (xml.Element("Alarm") != null)
            {
                var alarmXml = xml.Element("Alarm");
                if (alarmXml.Element("Count") != null)
                    newObj.AlarmCount = int.Parse(alarmXml.Element("Count").Value);
            }

            return newObj;
        }
    }
}
