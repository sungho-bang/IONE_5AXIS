using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary;
using System.ComponentModel;

namespace FALibrary.Part.Indicator
{
    public class IndicatorValue : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private double _scale;
        [FAAttribute("")]
        public double Scale
        {
            get { return _scale; }
            set
            {
                if (_scale == value) return;

                _scale = value;
                NotifyPropertyChanged("Scale");
            }
        }

        private double _gradient;
        [FAAttribute("")]
        public double Gradient
        {
            get { return _gradient; }
            set
            {
                if (_gradient == value) return;

                _gradient = value;
                NotifyPropertyChanged("Gradient");
            }
        }

        private double _interceptY;
        [FAAttribute("")]
        public double InterceptY
        {
            get { return _interceptY; }
            set
            {
                if (_interceptY == value) return;

                _interceptY = value;
                NotifyPropertyChanged("InterceptY");
            }
        }

        private double _offset;
        [FAAttribute("")]
        public double Offset
        {
            get { return _offset; }
            set
            {
                if (_offset == value) return;

                _offset = value;
                NotifyPropertyChanged("Offset");
            }
        }

        private double _value;
        [FAAttribute("")]
        public double Value
        {
            get { return _value; }
            set
            {
                if (_value == value) return;

                _value = value * Scale * Gradient + InterceptY + Offset;
                NotifyPropertyChanged("Value");
            }
        }
    }
}
