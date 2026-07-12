using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using FALibrary;

namespace FAFramework.ConfigClasses
{
    [Serializable]
    public class ButtonLampConfig : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private int _startLamp;
        private int _stopLamp;
        private int _jamClearLamp;
        private int _soundClearLamp;
        private int _initialLamp;

        [FAAttribute("")]
        public int StartLamp
        {
            get { return _startLamp; }
            set
            {
                if (_startLamp == value) return;

                _startLamp = value;
                NotifyPropertyChanged("StartLamp");
            }
        }

        [FAAttribute("")]
        public int StopLamp
        {
            get { return _stopLamp; }
            set
            {
                if (_stopLamp == value) return;

                _stopLamp = value;
                NotifyPropertyChanged("StopLamp");
            }
        }

        [FAAttribute("")]
        public int JamClearLamp
        {
            get { return _jamClearLamp; }
            set
            {
                if (_jamClearLamp == value) return;

                _jamClearLamp = value;
                NotifyPropertyChanged("JamClearLamp");
            }
        }

        [FAAttribute("")]
        public int SoundClearLamp
        {
            get { return _soundClearLamp; }
            set
            {
                if (_soundClearLamp == value) return;

                _soundClearLamp = value;
                NotifyPropertyChanged("SoundClearLamp");
            }
        }

        [FAAttribute("")]
        public int InitialLamp
        {
            get { return _initialLamp; }
            set
            {
                if (_initialLamp == value) return;

                _initialLamp = value;
                NotifyPropertyChanged("InitialLamp");
            }
        }

        public void CopyTo(ButtonLampConfig dest)
        {
            if (dest == null) return;

            dest.StartLamp = StartLamp;
            dest.StopLamp = StopLamp;
            dest.JamClearLamp = JamClearLamp;
            dest.SoundClearLamp = SoundClearLamp;
            dest.InitialLamp = InitialLamp;
        }
    }
}
