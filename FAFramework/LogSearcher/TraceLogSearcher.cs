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
using System.ComponentModel;

namespace FAFramework.LogSearcher
{
    public class TraceLogSearcher : LogSearcherBase
    {
        public class FindResultInfo : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged = delegate { };

            private void NotifyPropertyChanged(string propertyName = "")
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            private object _selectedItem;
            public object SelectedItem
            {
                get { return _selectedItem; }
                set
                {
                    if (_selectedItem == value) return;
                    _selectedItem = value;
                    NotifyPropertyChanged("SelectedItem");
                }
            }

            private object _selectionStart;
            public object SelectionStart
            {
                get { return _selectionStart; }
                set
                {
                    if (_selectionStart == value) return;
                    _selectionStart = value;
                    NotifyPropertyChanged("SelectionStart");
                }
            }

            private object _selectionLength;
            public object SelectionLength
            {
                get { return _selectionLength; }
                set
                {
                    if (_selectionLength == value) return;
                    _selectionLength = value;
                    NotifyPropertyChanged("SelectionLength");
                }
            }
        }

        public struct TraceLogParameter
        {
            public DateTime BeginDate { get; set; }
            public DateTime EndDate { get; set; }
        }

        public TraceLogSearcher()
        {
            Search = new Utility.CommandHandler(SearchCommandHandler, true);
            SaveToCSV = new Utility.CommandHandler(SaveToCSVCommandHandler, true);
            Find = new Utility.CommandHandler(FindCommandHandler, true);
            FindPrevious = new Utility.CommandHandler(FindPreviousCommandHandler, true);
        }

        private void SearchCommandHandler(object param)
        {
            TraceLogParameter traceLogParam;
            if (ParsingAlarmSearchParameters(param, out traceLogParam) == false) return;

            string path = Path.Combine(ConfigClasses.GlobalConst.ROOT_PATH, "Log", EquipmentInstance.Name, LogManager.TRACELOG_PATH);

            System.Threading.Thread thread = new System.Threading.Thread(
                delegate ()
                {
                    Search.SetCanExecute(false);

                    SearchTraceLog(path, traceLogParam);

                    Search.SetCanExecute(true);
                });

            thread.Start();
        }

        private void SaveToCSVCommandHandler(object param)
        {
            if (param == null) return;
            if ((param is IEnumerable) == false) return;

            List<string> result = new List<string>();

            foreach (dynamic item in param as IEnumerable)
            {
                result.Add(String.Format("{0}, {1}", item.Date.ToString("yyyy-MM-dd HH:mm:ss.fff"), item.Message));
            }

            File.WriteAllLines("logs.csv", result);
        }

        private void FindCommandHandler(object param)
        {
            if (FindNextFromResult(param, false) == false)
                FindNextFromResult(param, true);
        }

        private void FindPreviousCommandHandler(object param)
        {
            if (FindPreviousFromResult(param, false) == false)
                FindPreviousFromResult(param, true);
        }

        private bool FindNextFromResult(object param, bool first)
        {
            try
            {
                var arr = param as object[];
                string findString = arr[0] as string;
                dynamic list = arr[1];
                bool matchCase = (bool)arr[2];
                object selectedItem = arr[3];

                int startIndex = 0;

                if (first)
                    startIndex = 0;
                else if (selectedItem != null)
                {
                    startIndex = list.IndexOf(selectedItem) + 1;
                    if (startIndex > list.Count + 1)
                        startIndex = 0;
                }

                for (int i = startIndex; i < list.Count; i++)
                {
                    dynamic item = list[i];
                    dynamic logItem = item;

                    string message = logItem.Message;

                    if (matchCase == false)
                    {
                        message = message.ToUpper();
                        findString = findString.ToUpper();
                    }

                    int findIndex = message.IndexOf(findString);
                    if (findIndex >= 0)
                    {
                        FindResultInfo findResultInfo = new FindResultInfo();
                        findResultInfo.SelectedItem = item;
                        findResultInfo.SelectionStart = findIndex;
                        findResultInfo.SelectionLength = findString.Length;
                        FindResult = findResultInfo;
                        return true;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private bool FindPreviousFromResult(object param, bool first)
        {
            try
            {
                var arr = param as object[];
                string findString = arr[0] as string;
                dynamic list = arr[1];
                bool matchCase = (bool)arr[2];
                object selectedItem = arr[3];

                int startIndex = 0;

                if (first)
                    startIndex = list.Count - 1;
                else if (selectedItem != null)
                {
                    startIndex = list.IndexOf(selectedItem) - 1;
                    if (startIndex < 0)
                        startIndex = list.Count - 1;
                }

                for (int i = startIndex; i >= 0; i--)
                {
                    dynamic item = list[i];
                    dynamic logItem = item;

                    string message = logItem.Message;

                    if (matchCase == false)
                    {
                        message = message.ToUpper();
                        findString = findString.ToUpper();
                    }

                    int findIndex = message.IndexOf(findString);
                    if (findIndex >= 0)
                    {
                        FindResultInfo findResultInfo = new FindResultInfo();
                        findResultInfo.SelectedItem = item;
                        findResultInfo.SelectionStart = findIndex;
                        findResultInfo.SelectionLength = findString.Length;
                        FindResult = findResultInfo;
                        return true;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private bool ParsingAlarmSearchParameters(object param, out TraceLogParameter result)
        {
            result = new TraceLogParameter();

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

        private void SearchTraceLog(string path, TraceLogParameter param)
        {
            var files = GetLogFiles(path, param.BeginDate, param.EndDate);

            var result = new List<object>();

            foreach (var file in files)
            {
                try
                {
                    if (File.Exists(file) == false) continue;

                    DateTime date = DateTime.MinValue;
                    string message = string.Empty;

                    var lines = File.ReadAllLines(file);
                    var fileSearchResult = from item in lines
                                           where ParseTraceLogInfo(item, out date, out message) == true
                                           where IsInDateTime(date, param.BeginDate, param.EndDate)
                                           select new
                                           {
                                               Date = date,
                                               Message = message
                                           };

                    result.AddRange(fileSearchResult);
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

        private bool ParseTraceLogInfo(string str, out DateTime date, out string message)
        {
            date = DateTime.MinValue;
            message = string.Empty;

            try
            {
                var arr = str.Split('\t');

                if (arr.Length < 2) return false;
                if (string.IsNullOrEmpty(arr[0]) == true) return false;

                if (DateTime.TryParse(arr[0], out date) == false) return false;

                message = arr[1];

                if (arr.Length > 2)
                {
                    var list = arr.ToList();

                    list.RemoveAt(0);

                    message = string.Join(" ", list);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
