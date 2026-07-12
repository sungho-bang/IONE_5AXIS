using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Device.RS232Device;

namespace FALibrary.Part.HeaterPart
{
    public class FAM9Heater : FAPart
    {
        private bool _communicationOn;

        private double _ch1PV = 0;
        private double _ch2PV = 0;
        private double _ch3PV = 0;
        private double _ch4PV = 0;

        private double _ch1SVRead = 0;
        private double _ch2SVRead = 0;
        private double _ch3SVRead = 0;
        private double _ch4SVRead = 0;

        private double _ch1SV = 0;
        private double _ch2SV = 0;
        private double _ch3SV = 0;
        private double _ch4SV = 0;

        private double _ch1TargetSV = 0;
        private double _ch2TargetSV = 0;
        private double _ch3TargetSV = 0;
        private double _ch4TargetSV = 0;

        public FADeviceM9Heater Device
        {
            get;
            protected set;
        }

        #region Time
        [FAAttribute("Time")]
        public FALibrary.Utility.FATime TimeCommunicationReadTimeLimit { get; set; }
        #endregion

        #region Alarm
        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int AlarmCommunicationError { get; set; }
        #endregion

        #region PV READ SV WRITE
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

        [FAAttribute("Status")]
        public double Ch1PV
        {
            get { return _ch1PV; }
            set
            {
                if (_ch1PV == value) return;

                _ch1PV = value;
                NotifyPropertyChanged("Ch1PV");
            }
        }

        [FAAttribute("Status")]
        public double Ch2PV
        {
            get { return _ch2PV; }
            set
            {
                if (_ch2PV == value) return;

                _ch2PV = value;
                NotifyPropertyChanged("Ch2PV");
            }
        }

        [FAAttribute("Status")]
        public double Ch3PV
        {
            get { return _ch3PV; }
            set
            {
                if (_ch3PV == value) return;

                _ch3PV = value;
                NotifyPropertyChanged("Ch3PV");
            }
        }

        [FAAttribute("Status")]
        public double Ch4PV
        {
            get { return _ch4PV; }
            set
            {
                if (_ch4PV == value) return;

                _ch4PV = value;
                NotifyPropertyChanged("Ch4PV");
            }
        }

        [FAAttribute("Status")]
        public double Ch1SV
        {
            get { return _ch1SV; }
            set
            {
                if (_ch1SV == value) return;

                _ch1SV = value;
                NotifyPropertyChanged("Ch1SV");
                Device.WriteTemperature(1, value);
            }
        }

        [FAAttribute("Status")]
        public double Ch2SV
        {
            get { return _ch2SV; }
            set
            {
                if (_ch2SV == value) return;

                _ch2SV = value;
                NotifyPropertyChanged("Ch2SV");
                Device.WriteTemperature(2, value);
            }
        }

        [FAAttribute("Status")]
        public double Ch3SV
        {
            get { return _ch3SV; }
            set
            {
                if (_ch3SV == value) return;

                _ch3SV = value;
                NotifyPropertyChanged("Ch3SV");
                Device.WriteTemperature(3, value);
            }
        }

        [FAAttribute("Status")]
        public double Ch4SV
        {
            get { return _ch4SV; }
            set
            {
                if (_ch4SV == value) return;

                _ch4SV = value;
                NotifyPropertyChanged("Ch4SV");
                Device.WriteTemperature(4, value);
            }
        }
        #endregion

        #region Parameter
        [FAAttribute("Status")]
        public double Ch1TargetSV
        {
            get { return _ch1TargetSV; }
            set
            {
                if (_ch1TargetSV == value) return;

                _ch1TargetSV = value;
                NotifyPropertyChanged("Ch1TargetSV");
            }
        }

        [FAAttribute("Status")]
        public double Ch2TargetSV
        {
            get { return _ch2TargetSV; }
            set
            {
                if (_ch2TargetSV == value) return;

                _ch2TargetSV = value;
                NotifyPropertyChanged("Ch2TargetSV");
            }
        }

        [FAAttribute("Status")]
        public double Ch3TargetSV
        {
            get { return _ch3TargetSV; }
            set
            {
                if (_ch3TargetSV == value) return;

                _ch3TargetSV = value;
                NotifyPropertyChanged("Ch3TargetSV");
            }
        }

        [FAAttribute("Status")]
        public double Ch4TargetSV
        {
            get { return _ch4TargetSV; }
            set
            {
                if (_ch4TargetSV == value) return;

                _ch4TargetSV = value;
                NotifyPropertyChanged("Ch4TargetSV");
            }
        }
        #endregion

        [FAAttribute("Status")]
        public double Ch1SVRead
        {
            get { return _ch1SVRead; }
            set
            {
                if (_ch1SVRead == value) return;

                _ch1SVRead = value;
                NotifyPropertyChanged("Ch1SVRead");
            }
        }

        [FAAttribute("Status")]
        public double Ch2SVRead
        {
            get { return _ch2SVRead; }
            set
            {
                if (_ch2SVRead == value) return;

                _ch2SVRead = value;
                NotifyPropertyChanged("Ch2SVRead");
            }
        }

        [FAAttribute("Status")]
        public double Ch3SVRead
        {
            get { return _ch3SVRead; }
            set
            {
                if (_ch3SVRead == value) return;

                _ch3SVRead = value;
                NotifyPropertyChanged("Ch3SVRead");
            }
        }

        [FAAttribute("Status")]
        public double Ch4SVRead
        {
            get { return _ch4SVRead; }
            set
            {
                if (_ch4SVRead == value) return;

                _ch4SVRead = value;
                NotifyPropertyChanged("Ch4SVRead");
            }
        }

        public override void SetDevice(Device.FADevice aDevice)
        {
            if (aDevice is FADeviceM9Heater)
                Device = aDevice as FADeviceM9Heater;
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

                //Device.ReadWrite();
                base.Validate();
                Ch1PV = Device.Ch1Temperature;
                Ch2PV = Device.Ch2Temperature;
                Ch3PV = Device.Ch3Temperature;
                Ch4PV = Device.Ch4Temperature;

                Ch1SVRead = Device.SVCH1;
                Ch2SVRead = Device.SVCH2;
                Ch3SVRead = Device.SVCH3;
                Ch4SVRead = Device.SVCH4;
            }
        }

        public void SetAllTargetSV(double temperature)
        {
            Device.WriteTemperatureToMultiChannel(new FADeviceM9Heater.ChannelSetTemperatureInfo { ChannelNo = 1, Temperature = temperature },
                new FADeviceM9Heater.ChannelSetTemperatureInfo { ChannelNo = 2, Temperature = temperature },
                new FADeviceM9Heater.ChannelSetTemperatureInfo { ChannelNo = 3, Temperature = temperature },
                new FADeviceM9Heater.ChannelSetTemperatureInfo { ChannelNo = 4, Temperature = temperature });
        }
    }
}
