using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Device.RS232Device;

namespace FALibrary.Part.HeaterPart
{
    public class FAAutonicsTZHeater : FAPart
    {
        #region Field
        private double _temperature = 0;
        private double _targetTemperature = 0;
        private double _setTemperature = 0;
        private bool _communicationOn;
        #endregion

        #region Status
        [FAAttribute("Status")]
        public bool CommunicationOn
        {
            get { return _communicationOn; }
            set
            {
                if (value == _communicationOn) return;

                _communicationOn = value;
                NotifyPropertyChanged("CommunicationOn");
            }
        }
        #endregion

        #region Time
        [FAAttribute("Time")]
        public FALibrary.Utility.FATime TimeCommunicationReadTimeLimit { get; set; }
        #endregion

        #region Alarm
        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int AlarmCommunicationError { get; set; }
        #endregion

        public FADeviceAutonicsTZHeater Device
        {
            get;
            protected set;
        }

        #region Status
        [FAAttribute("Status")]
        public double Temperature
        {
            get { return _temperature; }
            set
            {
                if (_temperature == value) return;

                _temperature = value;
                NotifyPropertyChanged("Temperature");
            }
        }

        [FAAttribute("Status")]
        public double TargetTemperature
        {
            get { return _targetTemperature; }
            set
            {
                if (_targetTemperature == value) return;

                _targetTemperature = value;
                NotifyPropertyChanged("TargetTemperature");
            }
        }

        [FAAttribute("Status")]
        public double SetTemperature
        {
            get { return _setTemperature; }
            set
            {
                if (_setTemperature != value)
                {
                    _setTemperature = value;
                    NotifyPropertyChanged("SetTemperature");

                    if (SimulationMode)
                    {
                        TargetTemperature = value;
                        Temperature = value;
                    }
                }

                if (SimulationMode == false)
                    Device.WriteTemperature(value);
            }
        }
        #endregion

        public override void SetDevice(Device.FADevice aDevice)
        {
            if (aDevice is FADeviceAutonicsTZHeater)
                Device = aDevice as FADeviceAutonicsTZHeater;
            else
                throw new Exception("Device Type is not correct." + " Device Name : " + Name + " Device Type : " + aDevice.GetType().ToString());
        }

        public override void Validate()
        {
            if (SimulationMode == false)
            {
                if (DateTime.Now - Device.LastReadTime > TimeCommunicationReadTimeLimit.Time)
                    CommunicationOn = false;
                else
                    CommunicationOn = true;

                Device.ReadWrite();
                base.Validate();
                Temperature = Device.CurrentTemperature;
                TargetTemperature = Device.TargetTemperature;
            }
        }
    }
}
