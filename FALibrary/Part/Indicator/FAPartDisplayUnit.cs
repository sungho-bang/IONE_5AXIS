using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FALibrary.Part.Indicator
{
    public class FAPartDisplayUnit : FAPart
    {
        private string _displayMessage;
        [FAAttribute("")]
        public string DisplayMessage
        {
            get { return _displayMessage; }
            set
            {
                if (_displayMessage == value) return;
                _displayMessage = value;
                NotifyPropertyChanged("DisplayMessage");
                SetString(value);
            }
        }

        private Device.AbstractDevice.FADeviceDisplayUnit _device;

        public override void SetDevice(Device.FADevice aDevice)
        {
            if (aDevice is Device.AbstractDevice.FADeviceDisplayUnit)
                _device = aDevice as Device.AbstractDevice.FADeviceDisplayUnit;
            else
                throw new Exception("Device Type is not correct." + " Device Name : " + Name + " Device Type : " + aDevice.GetType().ToString());
        }

        public void SetString(string msg)
        {
            _device.SetString(msg);
        }

        public void ClearString()
        {
            _device.Clear();
        }
    }
}
