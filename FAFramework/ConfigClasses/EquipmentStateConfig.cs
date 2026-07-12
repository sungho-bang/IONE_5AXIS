using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using FALibrary;

namespace FAFramework.ConfigClasses
{
    [Serializable]
    public class EquipmentStateConfig : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public EquipmentStateConfig()
        {
            SignalTower = new SignalTowerConfig();
            ButtonLamp = new ButtonLampConfig();
        }

        private SignalTowerConfig _signalTower;
        [FAAttribute("")]
        public SignalTowerConfig SignalTower
        {
            get { return _signalTower; }
            set
            {
                _signalTower = value;
                NotifyPropertyChanged("SignalTower");
            }
        }

        private ButtonLampConfig _buttonLamp;
        [FAAttribute("")]
        public ButtonLampConfig ButtonLamp
        {
            get { return _buttonLamp; }
            set
            {
                _buttonLamp = value;
                NotifyPropertyChanged("ButtonLamp");
            }
        }

        public void CopyTo(EquipmentStateConfig dest)
        {
            if (SignalTower != null && dest != null && dest.SignalTower != null)
            {
                SignalTower.CopyTo(dest.SignalTower);
            }

            if (ButtonLamp != null && dest != null && dest.ButtonLamp != null)
            {
                ButtonLamp.CopyTo(dest.ButtonLamp);
            }
        }
    }
}
