using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace FAFramework.Utility
{
    public class LogSearchingDateTime
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private DateTime _date;
        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                NotifyPropertyChanged("Date");
            }
        }

        private ushort _hour;
        public ushort Hour
        {
            get { return _hour; }
            set
            {
                _hour = value;
                NotifyPropertyChanged("Hour");
            }
        }

        private ushort _minute;
        public ushort Minute
        {
            get { return _minute; }
            set
            {
                _minute = value;
                NotifyPropertyChanged("Minute");
            }
        }

        public DateTime ToDateTime()
        {
            DateTime result = new DateTime(Date.Date.Ticks);
            result = result.AddHours(Hour);
            result = result.AddMinutes(Minute);

            return result;
        }
    }
}
