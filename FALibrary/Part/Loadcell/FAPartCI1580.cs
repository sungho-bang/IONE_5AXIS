using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Device.CAS;
using FALibrary;

namespace FALibrary.Part.LoadCell
{
    public class FAPartCI1580 : FAPart
    {
        #region Field
        private string _status;
        private string _weightType;
        private double _weight;
        private double _lowerLimitPV;
        private double _upperLimitPV;
        private double _lowerLimitSV;
        private double _upperLimitSV;
        private bool _communicationOn;
        private double _scale = 0.01;
        private double _displayScale = 1;
        private DateTime _limitCheckPoleTime = new DateTime(0);
        private TimeSpan _limitCheckTime = new TimeSpan(0, 0, 0, 5);
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
        [FAAttribute("Status")]
        public string Status
        {
            get { return _status; }
            set
            {
                if (value == _status) return;

                _status = value;
                NotifyPropertyChanged("Status");
            }
        }
        [FAAttribute("Status")]
        public string WeightType
        {
            get { return _weightType; }
            set
            {
                if (value == _weightType) return;

                _weightType = value;
                NotifyPropertyChanged("WeightType");
            }
        }
        [FAAttribute("Status")]
        public double Weight
        {
            get { return _weight; }
            set
            {
                if (value == _weight) return;

                _weight = value;
                NotifyPropertyChanged("Weight");
            }
        }
        [FAAttribute("Status")]
        public double LowerLimitPV
        {
            get { return _lowerLimitPV; }
            set
            {
                if (value == _lowerLimitPV) return;

                _lowerLimitPV = value;
                NotifyPropertyChanged("LowerLimitPV");
            }
        }
        [FAAttribute("Status")]
        public double UpperLimitPV
        {
            get { return _upperLimitPV; }
            set
            {
                if (value == _upperLimitPV) return;

                _upperLimitPV = value;
                NotifyPropertyChanged("UpperLimitPV");
            }
        }
        [FAAttribute("Status")]
        [FAPropertyAttribute]
        public double LowerLimitSV
        {
            get { return _lowerLimitSV; }
            set
            {
                if (value == _lowerLimitSV) return;                

                _lowerLimitSV = value;
                NotifyPropertyChanged("LowerLimitSV");

                try
                {
                    Device.SetLowerLimitSV(value / Scale);
                }
                catch
                {
                }
            }
        }
        [FAAttribute("Status")]
        [FAPropertyAttribute]
        public double UpperLimitSV
        {
            get { return _upperLimitSV; }
            set
            {
                if (value == _upperLimitSV) return;                

                _upperLimitSV = value;
                NotifyPropertyChanged("UpperLimitSV");

                try
                {
                    Device.SetUppwerLimitSV(value / Scale);
                }
                catch
                {
                }
            }
        }
        #endregion

        #region Parameters
        [FAAttribute("Parameters")]
        [FAPropertyAttribute]
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
        [FAAttribute("Parameters")]
        [FAPropertyAttribute]
        public double DisplayScale
        {
            get { return _displayScale; }
            set
            {
                if (_displayScale == value) return;
                _displayScale = value;
                NotifyPropertyChanged("DisplayScale");
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
        
        public FACI1580Device Device
        {
            get;
            protected set;
        }

        public override void SetDevice(Device.FADevice aDevice)
        {
            if (aDevice is FACI1580Device)
                Device = aDevice as FACI1580Device;
            else
                throw new Exception("Device Type is not correct." + " Device Name : " + Name + " Device Type : " + aDevice.GetType().ToString());
        }

        public override void Validate()
        {
            base.Validate();

            if (Device != null)
            {
                if (DateTime.Now - Device.LastReadTime > TimeCommunicationReadTimeLimit.Time)
                    CommunicationOn = false;
                else
                    CommunicationOn = true;

                Device.ReadWrite();
                Status = Device.Status;
                WeightType = Device.WeightType;
                Weight = Device.Weight * DisplayScale;
                LowerLimitPV = Device.LowerLimitPV * Scale;
                UpperLimitPV = Device.UpperLimitPV * Scale;

                if (DateTime.Now - _limitCheckPoleTime > _limitCheckTime)
                {
                    if (LowerLimitPV != LowerLimitSV)
                    {
                        Device.SetLowerLimitSV(LowerLimitSV / Scale);
                    }

                    _limitCheckPoleTime = DateTime.Now;
                }
            }
        }

        [FAAttribute("Operation")]
        public void SetZero()
        {
            Device.SetZero();
        }
    }
}
