using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Diagnostics;
using FAFramework.Utility;
using FAFramework.Manager;
using System.Collections;

namespace FAFramework.LogSearcher
{
    public class AlarmLogSearcher : LogSearcherBase
    {
        public struct AlarmLogParameter
        {
            public DateTime BeginDate { get; set; }
            public DateTime EndDate { get; set; }
            public bool Error { get; set; }
            public bool Warning { get; set; }
            public bool Ranking { get; set; }
            public bool AutoRunning { get; set; }
            public bool Machine { get; set; }
            public bool Material { get; set; }
            public bool Human { get; set; }
            public bool Method { get; set; }
            public bool RankingAllDays { get; set; }
            public int[] filters { get; set; }
        }

        public class GeneralAlarmLog
        {
            public DateTime Date { get; set; }
            public int AlarmNo { get; set; }
            public string AlarmName { get; set; }
            public string Status { get; set; }
            public string Type { get; set; }

            public override string ToString()
            {
                return string.Format("{0}, {1}, {2}, {3}, {4}", Date,
                    AlarmNo,
                    AlarmName,
                    Status,
                    Type);
            }
        }

        public class RankingAlarmLog
        {
            public DateTime Date { get; set; }
            public int AlarmNo { get; set; }
            public string AlarmName { get; set; }
            public string Status { get; set; }
            public string Type { get; set; }
            public int Count { get; set; }

            public override string ToString()
            {
                return string.Format("{0}, {1}, {2}, {3}, {4}, {5}", Date,
                    Count,
                    AlarmNo,
                    AlarmName,
                    Status,
                    Type);
            }
        }

        public AlarmLogSearcher()
        {
            Search = new Utility.CommandHandler(SearchCommandHandler, true);
            SaveToCSV = new Utility.CommandHandler(SaveToCSVCommandHandler, true);
        }

        private void SearchCommandHandler(object param)
        {
            try
            {
                AlarmLogParameter alarmLogParam;
                if (ParsingAlarmSearchParameters(param, out alarmLogParam) == false) return;

                string path = Path.Combine(ConfigClasses.GlobalConst.ROOT_PATH, "Log", EquipmentInstance.Name, LogManager.ALARMLOG_PATH);

                System.Threading.Thread thread = new System.Threading.Thread(
                    delegate ()
                    {
                        Search.SetCanExecute(false);

                        if (alarmLogParam.Ranking)
                        {
                            if (alarmLogParam.RankingAllDays)
                                SearchRankingAlarmLogBySummaryAllDays(path, alarmLogParam);
                            else
                                SearchRankingAlarmLog(path, alarmLogParam);
                        }
                        else
                            SearchAlarmLog(path, alarmLogParam);

                        Search.SetCanExecute(true);
                    });

                thread.Start();
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

        private bool ParsingAlarmSearchParameters(object param, out AlarmLogParameter result)
        {
            result = new AlarmLogParameter();

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
                result.Error = (bool)parameters[2];
                result.Warning = (bool)parameters[3];
                result.Ranking = (bool)parameters[4];
                result.AutoRunning = (bool)parameters[5];
                result.Machine = (bool)parameters[6];
                result.Material = (bool)parameters[7];
                result.Human = (bool)parameters[8];
                result.Method = (bool)parameters[9];
                string filterString = (string)parameters[10];
                bool oneDay = (bool)parameters[11];
                result.RankingAllDays = (bool)parameters[12];

                if (oneDay)
                {
                    DateTime beginDate = result.BeginDate.AddDays(-1); ;
                    result.BeginDate = new DateTime(beginDate.Year, beginDate.Month, beginDate.Day,
                        22, 00, 00);
                    DateTime endDate = beginDate.AddDays(1);
                    result.EndDate = endDate;
                }

                if (string.IsNullOrEmpty(filterString) == false)
                {
                    List<int> filters = new List<int>();
                    foreach (var item in filterString.Split(new char[] { ',' }))
                    {
                        int alarmNo;
                        if (string.IsNullOrEmpty(item) == false &&
                            int.TryParse(item, out alarmNo))
                        {
                            filters.Add(alarmNo);
                        }
                    }

                    result.filters = filters.ToArray();
                }

                return true;
            }
            catch (Exception e)
            {
                Manager.LogManager.Instance.WriteSystemLog(e.ToString());
                return false;
            }
        }

        private void SearchAlarmLog(string path, AlarmLogParameter param)
        {
            var files = GetLogFiles(path, param.BeginDate, param.EndDate);

            var result = new List<object>();

            foreach (var file in files)
            {
                try
                {
                    if (File.Exists(file) == false) continue;

                    var xml = XElement.Load(file);
                    var fileSearchResult = from item in xml.Elements()
                                           where IsDate(item.Element("Date").Value)
                                           let date = DateTime.Parse(item.Element("Date").Value)
                                           where IsInDateTime(date, param.BeginDate, param.EndDate)
                                           let log = item.Element("AlarmLogInfo").ToObject<Manager.AlarmLogInfo>()
                                           where log.RankingData == true
                                           where log.AutoRunning == param.AutoRunning
                                           where ConstainInFilter(param.filters, log.Alarm.Alarm.AlarmNo)
                                           where IsMatchAlarmStatus(log.Alarm.Alarm.Status, param)
                                           where IsMatchAlarmType(log.Alarm.Alarm.Type, param)
                                           orderby date
                                           select new GeneralAlarmLog
                                           {
                                               Date = date,
                                               AlarmNo = log.Alarm.Alarm.AlarmNo,
                                               AlarmName = log.Alarm.Alarm.AlarmName,
                                               Status = GetStringErrorStatus(log.Alarm.Alarm.Status),
                                               Type = GetStringErrorType(log.Alarm.Alarm.Type)
                                           };

                    result.AddRange(fileSearchResult);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.ToString());
                    continue;
                }
            }

            Result =
                new
                {
                    AlarmCount = result.Count(),
                    LogList = result
                };
        }

        private void SearchRankingAlarmLog(string path, AlarmLogParameter param)
        {
            var files = GetLogFiles(path, param.BeginDate, param.EndDate);

            var result = new List<object>();
            int sumCount = 0;

            foreach (var file in files)
            {
                try
                {
                    if (File.Exists(file) == false) continue;

                    var xml = XElement.Load(file);
                    var fileSearchResult = from item in xml.Elements()
                                           where IsDate(item.Element("Date").Value)
                                           let date = DateTime.Parse(item.Element("Date").Value)
                                           where IsInDateTime(date, param.BeginDate, param.EndDate)
                                           let log = item.Element("AlarmLogInfo").ToObject<Manager.AlarmLogInfo>()
                                           where log.RankingData == true
                                           where log.AutoRunning == param.AutoRunning
                                           where ConstainInFilter(param.filters, log.Alarm.Alarm.AlarmNo)
                                           where IsMatchAlarmStatus(log.Alarm.Alarm.Status, param)
                                           where IsMatchAlarmType(log.Alarm.Alarm.Type, param)
                                           select new
                                           {
                                               Date = date,
                                               AlarmNo = log.Alarm.Alarm.AlarmNo,
                                               AlarmName = log.Alarm.Alarm.AlarmName,
                                               Status = log.Alarm.Alarm.Status,
                                               Type = log.Alarm.Alarm.Type
                                           } into alarm
                                           group alarm by alarm.AlarmNo into alarmGroup
                                           let firstAlarm = alarmGroup.ElementAt(0)
                                           let alarmCount = alarmGroup.Count()
                                           select new RankingAlarmLog
                                           {
                                               Date = firstAlarm.Date,
                                               AlarmNo = firstAlarm.AlarmNo,
                                               AlarmName = firstAlarm.AlarmName,
                                               Status = GetStringErrorStatus(firstAlarm.Status),
                                               Type = GetStringErrorType(firstAlarm.Type),
                                               Count = alarmCount
                                           };

                    result.AddRange(fileSearchResult.OrderByDescending(x => x.Count));

                    foreach (var item in fileSearchResult)
                    {
                        sumCount += item.Count;
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.ToString());
                    continue;
                }
            }

            Result =
                new
                {
                    AlarmCount = sumCount,
                    LogList = result
                };
        }

        private void SearchRankingAlarmLogBySummaryAllDays(string path, AlarmLogParameter param)
        {
            var files = GetLogFiles(path, param.BeginDate, param.EndDate);

            XElement rootXML = new XElement("Root");

            foreach (var file in files)
            {
                try
                {
                    if (File.Exists(file) == false) continue;

                    var xml = XElement.Load(file);
                    rootXML.Add(xml.Elements());
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.ToString());
                    continue;
                }
            }

            var fileSearchResult = from item in rootXML.Elements()
                                   where IsDate(item.Element("Date").Value)
                                   let date = DateTime.Parse(item.Element("Date").Value)
                                   where IsInDateTime(date, param.BeginDate, param.EndDate)
                                   let log = item.Element("AlarmLogInfo").ToObject<Manager.AlarmLogInfo>()
                                   where log.RankingData == true
                                   where log.AutoRunning == param.AutoRunning
                                   where ConstainInFilter(param.filters, log.Alarm.Alarm.AlarmNo)
                                   where IsMatchAlarmStatus(log.Alarm.Alarm.Status, param)
                                   where IsMatchAlarmType(log.Alarm.Alarm.Type, param)
                                   select new
                                   {
                                       Date = date,
                                       AlarmNo = log.Alarm.Alarm.AlarmNo,
                                       AlarmName = log.Alarm.Alarm.AlarmName,
                                       Status = log.Alarm.Alarm.Status,
                                       Type = log.Alarm.Alarm.Type
                                   } into alarm
                                   group alarm by alarm.AlarmNo into alarmGroup
                                   let firstAlarm = alarmGroup.ElementAt(0)
                                   let alarmCount = alarmGroup.Count()
                                   select new RankingAlarmLog
                                   {
                                       Date = firstAlarm.Date,
                                       AlarmNo = firstAlarm.AlarmNo,
                                       AlarmName = firstAlarm.AlarmName,
                                       Status = GetStringErrorStatus(firstAlarm.Status),
                                       Type = GetStringErrorType(firstAlarm.Type),
                                       Count = alarmCount
                                   };

            var result = new List<object>();
            result.AddRange(fileSearchResult.OrderByDescending(x => x.Count));

            int sumCount = 0;
            foreach (var item in fileSearchResult)
            {
                sumCount += item.Count;
            }

            Result =
                new
                {
                    AlarmCount = sumCount,
                    LogList = result
                };
        }

        private bool IsMatchAlarmStatus(int alarmStatus, AlarmLogParameter param)
        {
            if (alarmStatus == ConfigClasses.GlobalConst.ALARM)
            {
                if (param.Error == true)
                    return true;
                else
                    return false;
            }
            else if (alarmStatus == ConfigClasses.GlobalConst.WARNING)
            {
                if (param.Warning == true)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        private bool IsMatchAlarmType(int alarmType, AlarmLogParameter param)
        {
            if (alarmType == ConfigClasses.GlobalConst.ALARM_TYPE_MACHINE && param.Machine == true)
                return true;
            else if (alarmType == ConfigClasses.GlobalConst.ALARM_TYPE_MATERIAL && param.Material == true)
                return true;
            else if (alarmType == ConfigClasses.GlobalConst.ALARM_TYPE_HUMAN && param.Human == true)
                return true;
            else if (alarmType == ConfigClasses.GlobalConst.ALARM_TYPE_METHOD && param.Method == true)
                return true;
            else
                return false;
        }

        private string GetStringMachine()
        {
            try
            {
                return Utility.UtilityClass.GetStringResource(this, "Machine", "MACHINE");
            }
            catch
            {
                return "MACHINE";
            }
        }

        private string GetStringMaterial()
        {
            try
            {
                return Utility.UtilityClass.GetStringResource(this, "Material", "MATERIAL");
            }
            catch
            {
                return "MATERIAL";
            }
        }

        private string GetStringHuman()
        {
            try
            {
                return Utility.UtilityClass.GetStringResource(this, "Human", "HUMAN");
            }
            catch
            {
                return "HUMAN";
            }
        }

        private string GetStringMethod()
        {
            try
            {
                return Utility.UtilityClass.GetStringResource(this, "Method", "METHOD");
            }
            catch
            {
                return "METHOD";
            }
        }

        private string GetStringErrorType(int type)
        {
            if (type == ConfigClasses.GlobalConst.ALARM_TYPE_MACHINE)
                return GetStringMachine();
            else if (type == ConfigClasses.GlobalConst.ALARM_TYPE_MATERIAL)
                return GetStringMaterial();
            else if (type == ConfigClasses.GlobalConst.ALARM_TYPE_HUMAN)
                return GetStringHuman();
            else if (type == ConfigClasses.GlobalConst.ALARM_TYPE_METHOD)
                return GetStringMethod();
            return "UNKNOWN";
        }

        private string GetStringErrorStatus(int status)
        {
            if (status == ConfigClasses.GlobalConst.ALARM)
                return "ALARM";
            else if (status == ConfigClasses.GlobalConst.WARNING)
                return "WARNING";
            else
                return "UNKNOWN";
        }

        private bool ConstainInFilter(int[] filter, int alarmNo)
        {
            if (filter == null) return true;
            if (filter.Length == 0) return true;

            return filter.Contains(alarmNo);
        }
    }
}
