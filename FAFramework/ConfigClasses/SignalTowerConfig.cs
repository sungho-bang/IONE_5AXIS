using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using FALibrary;

namespace FAFramework.ConfigClasses
{
    [Serializable]
    public class SignalTowerConfig : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private int _greenLamp;
        private int _yellowLamp;
        private int _redLamp;
        private int _buzzer;

        [FAAttribute("")]
        public int GreenLamp
        {
            get { return _greenLamp; }
            set
            {
                if (_greenLamp == value) return;

                _greenLamp = value;
                NotifyPropertyChanged("GreenLamp");
            }
        }

        [FAAttribute("")]
        public int YellowLamp
        {
            get { return _yellowLamp; }
            set
            {
                if (_yellowLamp == value) return;

                _yellowLamp = value;
                NotifyPropertyChanged("YellowLamp");
            }
        }

        [FAAttribute("")]
        public int RedLamp
        {
            get { return _redLamp; }
            set
            {
                if (_redLamp == value) return;

                _redLamp = value;
                NotifyPropertyChanged("RedLamp");
            }
        }

        [FAAttribute("")]
        public int Buzzer
        {
            get { return _buzzer; }
            set
            {
                if (_buzzer == value) return;

                _buzzer = value;
                NotifyPropertyChanged("Buzzer");
            }
        }

        public void CopyTo(SignalTowerConfig dest)
        {
            if (dest == null) return;

            dest.GreenLamp = GreenLamp;
            dest.YellowLamp = YellowLamp;
            dest.RedLamp = RedLamp;
            dest.Buzzer = Buzzer;
        }
    }
}