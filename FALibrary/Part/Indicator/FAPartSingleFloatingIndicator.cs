using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Device.AbstractDevice;

namespace FALibrary.Part.Indicator
{
    public class FAPartSingleFloatingIndicator : FAPart
    {
        private FADeviceIndicator _device;

        private IndicatorValue _value;
        [FAAttribute("Status")]
        [FASerializable]
        public IndicatorValue Value
        {
            get { return _value; }
            set
            {
                if (_value == value) return;

                _value = value;
                NotifyPropertyChanged("Value");
            }
        }

        private bool _communicationError;
        [FAAttribute("Status")]
        public bool CommunicationError
        {
            get { return _communicationError; }
            set
            {
                if (_communicationError == value) return;

                _communicationError = value;
                NotifyPropertyChanged("CommunicationError");
            }
        }        

        public FAPartSingleFloatingIndicator()
        {
            Value = new IndicatorValue();
        }

        public override void SetDevice(Device.FADevice aDevice)
        {
            if (aDevice is FADeviceIndicator)
                _device = aDevice as FADeviceIndicator;
            else
                throw new Exception("Device Type is not correct." + " Device Name : " + Name + " Device Type : " + aDevice.GetType().ToString());
        }

        public override void Validate()
        {
            base.Validate();

            if (_device != null)
            {
                object[] deviceValue = _device.GetValues();
                if (deviceValue == null) return;
                if (deviceValue.Length <= 0) return;
                if (deviceValue[0] is double)
                {
                    Value.Value = (double)deviceValue[0];
                }

                CommunicationError = _device.CommunicationError;
            }
        }
    }
}
