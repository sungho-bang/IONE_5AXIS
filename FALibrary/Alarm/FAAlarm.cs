using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FALibrary.Alarm
{
    [Serializable]
    public class FAAlarm : FAObject
    {
        private int _alarmNo;
        private string _alarmCodeName;
        private string _alarmName;
        private string _description;
        private string _solution;
        private string _imagePath;
        private int _status;
        private int _type;

        public int AlarmNo
        {
            get { return _alarmNo; }
            set
            {
                if (_alarmNo == value) return;

                _alarmNo = value;
                NotifyPropertyChanged("AlarmNo");
            }
        }

        public string AlarmCodeName
        {
            get { return _alarmCodeName; }
            set
            {
                if (_alarmCodeName == value) return;

                _alarmCodeName = value;
                NotifyPropertyChanged("AlarmCodeName");
            }
        }

        public string AlarmName
        {
            get { return _alarmName; }
            set
            {
                if (_alarmName == value) return;

                _alarmName = value;
                NotifyPropertyChanged("AlarmName");
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (_description == value) return;

                _description = value;
                NotifyPropertyChanged("Description");
            }
        }

        public string Solution
        {
            get { return _solution; }
            set
            {
                if (_solution == value) return;

                _solution = value;
                NotifyPropertyChanged("Solution");
            }
        }

        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                if (_imagePath == value) return;

                _imagePath = value;
                NotifyPropertyChanged("ImagePath");
            }
        }

        public int Status
        {
            get { return _status; }
            set
            {
                if (_status == value) return;

                _status = value;
                NotifyPropertyChanged("Status");
            }
        }

        public int Type
        {
            get { return _type; }
            set
            {
                if (_type == value) return;

                _type = value;
                NotifyPropertyChanged("Type");
            }
        }

        public void CopyTo(FAAlarm alarm)
        {
            alarm.AlarmNo = this.AlarmNo;
            alarm.AlarmCodeName = this.AlarmCodeName;
            alarm.AlarmName = this.AlarmName;
            alarm.Description = this.Description;
            alarm.Solution = this.Solution;
            alarm.ImagePath = this.ImagePath;
            alarm.Status = this.Status;
            alarm.Type = this.Type;
        }
    }
}
