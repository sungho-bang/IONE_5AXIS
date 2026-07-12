using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace FALibrary
{
    [Serializable]
    public class FARange : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private double _min;
        private double _max;

        [FAAttribute("")]
        public double Min
        {
            get { return _min; }
            set
            {
                if (_min == value) return;

                _min = value;
                NotifyPropertyChanged("Min");
            }
        }

        [FAAttribute("")]
        public double Max
        {
            get { return _max; }
            set
            {
                if (_max == value) return;

                _max = value;
                NotifyPropertyChanged("Max");
            }
        }

        public FARange()
        {
        }

        public FARange(double min, double max)
        {
            Min = min;
            Max = max;
        }
    }
}
