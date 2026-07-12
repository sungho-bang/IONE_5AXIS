using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Device.RS232Device;
using FALibrary.Sequence;
using FALibrary.Utility;
using FALibrary.Alarm;

namespace FALibrary.Part.ContactSensorPart
{
    public class FAKeyenceContactSensor : FAPart
    {
        #region Status
        private bool _communicationOn;
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

        public FADeviceKeyenceContactSensor Device
        {
            get;
            protected set;
        }
        //191203
        #region Status

        private double _jigOffset = 0;
        [FAPropertyAttribute]
        [FAAttribute("Parameters")]
        public double JigOffset
        {
            get { return _jigOffset; }
            set
            {
                if (_jigOffset == value) return;

                _jigOffset = value;
                NotifyPropertyChanged("JigOffset");
                //------------------------------------------------
            }
        }

        private double _jigThickness = 5;
        [FAPropertyAttribute]
        [FAAttribute("Parameters")]
        public double JigThickness
        {
            get { return _jigThickness; }
            set
            {
                if (_jigThickness == value) return;

                _jigThickness = value;
                NotifyPropertyChanged("JigThickness");
                //------------------------------------------------
            }
        }

        private double _thickness1 = 0;
        [FAAttribute("Status")]
        public double Thickness1
        {
            get { return _thickness1; }
            set
            {
                if (_thickness1 == value) return;

                _thickness1 = value;
                NotifyPropertyChanged("Thickness1");
                //------------------------------------------------
            }
        }

        private double _thickness2 = 0;
        [FAAttribute("Status")]
        public double Thickness2
        {
            get { return _thickness2; }
            set
            {
                if (_thickness2 == value) return;

                _thickness2 = value;
                NotifyPropertyChanged("Thickness2");
                //------------------------------------------------
                
            }
        }

        private double _thickness3 = 0;
        [FAAttribute("Status")]
        public double Thickness3
        {
            get { return _thickness3; }
            set
            {
                if (_thickness3 == value) return;

                _thickness3 = value;
                NotifyPropertyChanged("Thickness3");
            }
        }
        #endregion

        public bool ReadThicknesse()
        {
            return Device.ReadThicknesse();
        }

        public bool Reset_Start(int nID)
        {
            return Device.Reset_Start(nID);
        }
        public bool Reset_End(int nID)
        {
            return Device.Reset_End(nID);
        }

        public override void SetDevice(Device.FADevice aDevice)
        {
            if (aDevice is FADeviceKeyenceContactSensor)
                Device = aDevice as FADeviceKeyenceContactSensor;
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

                Thickness1 = Device.CurrentThickness1;
                Thickness2 = Device.CurrentThickness2;
            }
        }
    }
}
