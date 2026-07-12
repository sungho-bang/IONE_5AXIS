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
using System.Text.RegularExpressions;

namespace FAFramework.LogSearcher
{
    public class ProductLogSearcher : LogSearcherBase
    {
        public struct ProductLogParameter
        {
            public DateTime BeginDate { get; set; }
            public DateTime EndDate { get; set; }
        }

        public class GeneralProduntLog
        {
            public DateTime Date { get; set; }
            public int OutCount { get; set; }
            public string Lotid { get; set; }

            public int TotalOutputCount { get; set; }
            public int DayOutputCount { get; set; }
            public int SwingOutputCount { get; set; }
            public int GYOutputCount { get; set; }

            public override string ToString()
            {
                return string.Format("{0}, {1}, {2},{3},{4},{5},{6}", Date,
                    OutCount,
                    Lotid, TotalOutputCount, DayOutputCount, SwingOutputCount, GYOutputCount);
            }
        }

        public class GeneralTotalLog
        {
            public DateTime Date { get; set; }
            public string Lotid { get; set; }

            public int TotalOutputCount { get; set; }

            public string Runtime { get; set; }

            public override string ToString()
            {
                return string.Format("{0}, {1}, {2},{3}", Date,
                    Lotid, TotalOutputCount, Runtime);
            }
        }

        public class GeneralTimeLog
        {          
            public TimeSpan Runtime { get; set;}
            public TimeSpan MTBI { get; set; }
            public int AlarmCount { get; set; }

            public override string ToString()
            {
                return string.Format("{0}, {1}, {2}",
                    Runtime, MTBI,AlarmCount);
            }
        }    

        public ProductLogSearcher()
        {
            Search = new Utility.CommandHandler(SearchCommandHandler, true);
            SaveToCSV = new Utility.CommandHandler(SaveToCSVCommandHandler, true);
        }

        private void SearchCommandHandler(object param)
        {
            try
            {
                ProductLogParameter productLogParam;
                if (ParsingProducyLogSearchParameters(param, out productLogParam) == false) return;

                string path = Path.Combine(ConfigClasses.GlobalConst.ROOT_PATH, "Log", EquipmentInstance.Name, LogManager.PRODUCT_PATH);
                string mtbipath = Path.Combine(ConfigClasses.GlobalConst.ROOT_PATH, "Log", EquipmentInstance.Name, LogManager.MTBILOG_PATH);                

                System.Threading.Thread thread = new System.Threading.Thread(
                    delegate()
                    {
                        Search.SetCanExecute(false);

                        SearchProductLog(path,mtbipath, productLogParam);
                        //SearchMTBILog(mtbipath,productLogParam);

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
                    //var data = item.ToString();
                    //var xml = XElement.Parse(data);
                    //var node = xml.Element("RunTime");

                    result.Add(item.ToString());
                }

                File.WriteAllLines(dlg.FileName, result);
            }
            catch (Exception e)
            {
                Manager.MessageWindowManager.Instance.Show(EquipmentInstance, "AlarmLogError", string.Format("Can not convert log to CSV", e.ToString()));
            }
        }

        private bool ParsingProducyLogSearchParameters(object param, out ProductLogParameter result)
        {
            result = new ProductLogParameter();

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
                bool oneDay = (bool)parameters[2];

                if (oneDay)
                {
                    DateTime beginDate = result.BeginDate;
                    result.BeginDate = new DateTime(beginDate.Year, beginDate.Month, beginDate.Day,
                        22, 00, 00);
                    DateTime endDate = beginDate.AddDays(1);                    
                    result.EndDate = endDate;
                }

                return true;
            }
            catch (Exception e)
            {
                Manager.LogManager.Instance.WriteSystemLog(e.ToString());
                return false;
            }
        }

        private void SearchMTBILog(string path, ProductLogParameter param)
        {
            var files = RuntimeGetLogFiles(path, param.BeginDate, param.EndDate);            

            var timeresult = new List<object>();

            DateTime date = DateTime.MinValue;
            //TimeSpan runtime = new TimeSpan(0, 0, 0);
            //TimeSpan mtbi = new TimeSpan(0, 0, 0);

            //TimeSpan rt = new TimeSpan(0, 0, 0);
            //TimeSpan mt = new TimeSpan(0, 0, 0);
            //int alarmcount;

            string strruntime = string.Empty;
            string ttime = string.Empty;

            foreach (var file in files)
            {
                try
                {
                    if (File.Exists(file) == false) continue;

                    var xml = XElement.Load(file);
                    var runtimeresult = TimeSpan.Parse(xml.Element("RunTime").Value);
                    var mtbiresult = TimeSpan.Parse(xml.Element("MTBI").Value);
                    var alarmcountresult = Int32.Parse(xml.Element("Count").Value);

                    //var SearchResult = from item in xml.Elements()
                    //                   where IsDate(item.Element("Date").Value)
                    //                   let runtime = TimeSpan.Parse(item.Element("RunTime").Value)
                    //                   let mtbi = TimeSpan.Parse(item.Element("MTBI").Value)
                    //                   //let alarmcount = int.Parse(xml.Element("Count").Value)
                    //                   where IsInDateTime(date, param.BeginDate, param.EndDate)
                    //                   select new GeneralTimeLog
                    //                       {
                    //                           Runtime = runtime,
                    //                           MTBI = mtbi
                    //                       };

                    timeresult.Add(new GeneralTimeLog 
                    { 
                        Runtime = runtimeresult,
                        MTBI = mtbiresult,
                        AlarmCount = alarmcountresult
                    });
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
                    TIMELIST = timeresult
                };
        }     

        private void SearchProductLog(string path, string mtbipath, ProductLogParameter param)
        {
            //param.BeginDate = param.BeginDate.Date.AddDays();
            //param.EndDate = param.EndDate.Date.AddDays(-1);
            var files = ProDuctGetLogFiles(path, param.BeginDate, param.EndDate);
            var countfiles = TotalCountGetLogFiles(path, param.BeginDate, param.EndDate);            
            var timefiles = RuntimeGetLogFiles(mtbipath, param.BeginDate, param.EndDate);

            var totalresult = new List<object>();
            var result = new List<object>();
            var countresult = new List<object>();
            var timeresult = new List<object>();
            string lotID = string.Empty;
            int outPutCount = 0;

            DateTime date = DateTime.MinValue;

            int totalcount = 0;
            int daycount = 0;
            int swingcount = 0;
            int gyCount = 0;
                        
            

            foreach (var file in countfiles)
            {                
                try
                {
                    if (File.Exists(file) == false) continue;

                    var lines = File.ReadAllLines(file);
                    var fileCountResult = from item in lines
                                           where ParseTotalCountLogInfo(item, out date,
                                           out totalcount, out daycount, out swingcount, out gyCount) == true
                                           //where IsInDateTime(date, param.BeginDate, param.EndDate)
                                           select new GeneralProduntLog
                                           {
                                               TotalOutputCount = totalcount,
                                               DayOutputCount = daycount,
                                               SwingOutputCount = swingcount,
                                               GYOutputCount = gyCount
                                           };

                    countresult.AddRange(fileCountResult);

                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.ToString());
                    continue;
                }
            }

            foreach (var file in timefiles)
            {
                try
                {
                    if (File.Exists(file) == false) continue;

                    var xml = XElement.Load(file);
                    var runtimeresult = TimeSpan.Parse(xml.Element("RunTime").Value);
                    var mtbiresult = TimeSpan.Parse(xml.Element("MTBI").Value);
                    var alarmcountresult = xml.Element("Alarm").Element("Count").Value;

                    //var SearchResult = from item in xml.Elements()
                    //                   where IsDate(item.Element("Date").Value)
                    //                   let runtime = TimeSpan.Parse(item.Element("RunTime").Value)
                    //                   let mtbi = TimeSpan.Parse(item.Element("MTBI").Value)
                    //                   //let alarmcount = int.Parse(xml.Element("Count").Value)
                    //                   where IsInDateTime(date, param.BeginDate, param.EndDate)
                    //                   select new GeneralTimeLog
                    //                       {
                    //                           Runtime = runtime,
                    //                           MTBI = mtbi
                    //                       };

                    timeresult.Add(new GeneralTimeLog
                    {
                        Runtime = runtimeresult,
                        MTBI = mtbiresult,
                        AlarmCount = Convert.ToInt16(alarmcountresult)
                    });

                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.ToString());
                    continue;
                }
            }

            foreach (var file in files)
            {
                try
                {
                    if (File.Exists(file) == false) continue;

                    var lines = File.ReadAllLines(file);
                    var fileSearchResult = from item in lines
                                           where ParseProductLogInfo(item, out date, out lotID, out outPutCount) == true
                                           //where IsInDateTime(date, param.BeginDate, param.EndDate)
                                           select new GeneralProduntLog
                                           {
                                               Date = date,
                                               Lotid = lotID,
                                               OutCount = outPutCount
                                           };

                    result.AddRange(fileSearchResult);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.ToString());
                    continue;
                }
            }

            foreach (var file in files)
            {
                foreach (var timefile in timefiles)
                {
                    try
                    {
                        if (File.Exists(file) == false) continue;
                        if (File.Exists(timefile) == false) continue;

                        var xml = XElement.Load(timefile);
                        var runtimeresult = TimeSpan.Parse(xml.Element("RunTime").Value);
                        var node = xml.Element("RunTime");
                        var lines = File.ReadAllLines(file);
                        var fileSearchResult = from item in lines
                                               where ParseProductLogInfo(item, out date, out lotID, out outPutCount) == true
                                               //where IsInDateTime(date, param.BeginDate, param.EndDate)
                                               select new GeneralTotalLog
                                               {
                                                   Date = date,
                                                   Lotid = lotID,
                                                   TotalOutputCount = totalcount,
                                                   Runtime = node.ToString()
                                               };
                        totalresult.AddRange(fileSearchResult);


                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e.ToString());
                        continue;
                    }
                }
            }

            try
            {
                Result =
                    new
                    {
                        TimeList = timeresult.Last(),
                        CountList = countresult.Last(),
                        LogList = result,
                        TotalList = totalresult
                    };
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
            }

        }       

        private bool ParseTotalCountLogInfo(string str, out DateTime date, out int totalcount, out int daycount, out int swingcount, out int gyCount)
        {
            date = DateTime.MinValue;

            totalcount = 0;
            daycount = 0;
            gyCount = 0;
            swingcount = 0;

            try
            {
                var arr = str.Split(',');

                if (arr.Length < 2) return false;
                if (string.IsNullOrEmpty(arr[0]) == true) return false;

                if (DateTime.TryParse(arr[0], out date) == false) return false;

                string TotalCountarr = arr[1];
                string DayCountarr = arr[2];
                string SwingCountarr = arr[3];
                string GYCountarr = arr[4];

                Regex reg = new Regex("=");
                string[] TotalCountResult = reg.Split(TotalCountarr, 2);
                string[] DayCountResult = reg.Split(DayCountarr, 2);
                string[] SwingCountResult = reg.Split(SwingCountarr, 2);
                string[] GYCountResult = reg.Split(GYCountarr, 2);


                totalcount = Convert.ToInt32(TotalCountResult[1]);
                daycount = Convert.ToInt32(DayCountResult[1]);
                swingcount = Convert.ToInt32(SwingCountResult[1]);
                gyCount = Convert.ToInt32(GYCountResult[1]);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool ParseProductLogInfo(string str, out DateTime date, out string lotid, out int OutPutCount)
        {
            date = DateTime.MinValue;
            lotid = string.Empty;
            OutPutCount = 0;

            try
            {
                var arr = str.Split(',');

                if (arr.Length < 2) return false;
                if (string.IsNullOrEmpty(arr[0]) == true) return false;

                if (DateTime.TryParse(arr[0], out date) == false) return false;

                string lotidarr = arr[1];
                string outputcountarr = arr[3];

                Regex reg = new Regex("=");
                string[] LoIDsplitResult = reg.Split(lotidarr, 2);
                string[] OutPutsplitResult = reg.Split(outputcountarr, 2);


                 lotid = LoIDsplitResult[1];
                 OutPutCount = Convert.ToInt32(OutPutsplitResult[1]);

                return true;
            }
            catch
            {
                return false;
            }
        }        

        private bool ConstainInFilter(int[] filter, int alarmNo)
        {
            if (filter == null) return true;
            if (filter.Length == 0) return true;

            return filter.Contains(alarmNo);
        }
    }
}
