using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.ComponentModel;
using FAFramework.Utility;
using System.Windows;

namespace FAFramework.LogSearcher
{
    public class LogSearcherBase : DependencyObject, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public static readonly DependencyProperty EquipmentInstanceProperty =
            DependencyProperty.Register("EquipmentInstance", typeof(Equipment.EquipmentBase), typeof(LogSearcherBase));

        public Equipment.EquipmentBase EquipmentInstance
        {
            get { return (Equipment.EquipmentBase)GetValue(EquipmentInstanceProperty); }
            set
            {
                SetValue(EquipmentInstanceProperty, value);
            }
        }

        private object _result;
        public object Result
        {
            get { return _result; }
            set
            {
                _result = value;
                NotifyPropertyChanged("Result");
            }
        }

        private object _findResult;
        public object FindResult
        {
            get { return _findResult; }
            set
            {
                _findResult = value;
                NotifyPropertyChanged("FindResult");
            }
        }

        private CommandHandler _search;
        public CommandHandler Search
        {
            get { return _search; }
            set
            {
                _search = value;
                NotifyPropertyChanged("Search");
            }
        }

        private CommandHandler _saveToCSV;
        public CommandHandler SaveToCSV
        {
            get { return _saveToCSV; }
            set
            {
                _saveToCSV = value;
                NotifyPropertyChanged("SaveToCSV");
            }
        }

        private CommandHandler _find;
        public CommandHandler Find
        {
            get { return _find; }
            set
            {
                _find = value;
                NotifyPropertyChanged("Find");
            }
        }

        private CommandHandler _findNext;
        public CommandHandler FindNext
        {
            get { return _findNext; }
            set
            {
                _findNext = value;
                NotifyPropertyChanged("FindNext");
            }
        }

        private CommandHandler _findPrevious;
        public CommandHandler FindPrevious
        {
            get { return _findPrevious; }
            set
            {
                _findPrevious = value;
                NotifyPropertyChanged("FindPrevious");
            }
        }

        protected bool IsDate(string str)
        {
            DateTime date;
            if (DateTime.TryParse(str, out date) == true)
                return true;

            return false;
        }

        protected bool IsInDateTime(DateTime dateTime, DateTime startTime, DateTime endTime)
        {
            bool result = dateTime >= startTime &&
                        dateTime <= endTime;

            return result;
        }

        protected string[] GetLogFiles(string path, DateTime beginDate, DateTime endDate)
        {
            if (endDate.Hour == 00 &&
                endDate.Minute == 00 &&
                endDate.Second == 00)
            {
                endDate = endDate.AddMilliseconds(-1);
            }

            List<string> result = new List<string>();
            var beginDay = new DateTime(beginDate.Year, beginDate.Month, beginDate.Day);
            var endDay = new DateTime(endDate.Year, endDate.Month, endDate.Day);

            for (var date = beginDay; date <= endDay; date = date.AddDays(1))
            {
                string filename = System.IO.Path.Combine(path, date.ToString("yyyy"), date.ToString("MM"), date.ToString("yyyy-MM-dd") + ".log");
                result.Add(filename);
            }

            return result.ToArray();
        }

        protected string[] ProDuctGetLogFiles(string path, DateTime beginDate, DateTime endDate)
        {
            if (endDate.Hour == 00 &&
                endDate.Minute == 00 &&
                endDate.Second == 00)
            {
                endDate = endDate.AddDays(-1);
            }

            List<string> result = new List<string>();
            var beginDay = new DateTime(beginDate.Year, beginDate.Month, beginDate.Day);
            var endDay = new DateTime(endDate.Year, endDate.Month, endDate.Day);


            for (var date = beginDay; date <= endDay; date = date.AddDays(1.5))
            {
                string filename = System.IO.Path.Combine(path, date.ToString("yyyy"), date.ToString("MM"), date.ToString("dd"), date.ToString("LOTPRODUCTOUTPUT" + "_" + "yyyy-MM-dd") + ".log");
                result.Add(filename);
            }

            return result.ToArray();
        }

        protected string[] TotalCountGetLogFiles(string path, DateTime beginDate, DateTime endDate)
        {
            if (endDate.Hour == 00 &&
                endDate.Minute == 00 &&
                endDate.Second == 00)
            {
                endDate = endDate.AddDays(-1);
            }

            List<string> result = new List<string>();
            var beginDay = new DateTime(beginDate.Year, beginDate.Month, beginDate.Day);
            var endDay = new DateTime(endDate.Year, endDate.Month, endDate.Day);
            var f = "DAY_OUTPUT_";

            for (var date = beginDay; date <= endDay; date = date.AddDays(1.5))
            {
                string filename = System.IO.Path.Combine(path, date.ToString("yyyy"), date.ToString("MM"), date.ToString("dd"), date.ToString("DAY_OUTPUT" + "_" + "yyyy-MM-dd") + ".log");
                result.Add(filename);
            }

            return result.ToArray();
        }

        protected string[] RuntimeGetLogFiles(string path, DateTime beginDate, DateTime endDate)
        {
            if (endDate.Hour == 00 &&
                endDate.Minute == 00 &&
                endDate.Second == 00)
            {
                endDate = endDate.AddDays(-1);
            }

            List<string> result = new List<string>();
            var beginDay = new DateTime(beginDate.Year, beginDate.Month, beginDate.Day);
            var endDay = new DateTime(endDate.Year, endDate.Month, endDate.Day);

            for (var date = beginDay; date <= endDay; date = date.AddDays(1.5))
            {
                string filename = System.IO.Path.Combine(path, date.ToString("yyyy"), date.ToString("MM"), date.ToString("yyyy-MM-dd") + ".log");
                result.Add(filename);
            }

            return result.ToArray();
        }
    }
}