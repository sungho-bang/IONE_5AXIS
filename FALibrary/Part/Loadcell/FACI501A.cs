using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Device.CAS;
using FALibrary;

namespace FALibrary.Part.LoadCell
{
    public class FACI501A : FAPart
    {
        #region Field        
        private string _weightType;
        private double _weight;
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

        private FALibrary.Device.CAS.EStableStatus _status;
        [FAAttribute("Status")]
        public FALibrary.Device.CAS.EStableStatus Status
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

        public FADeviceCI501A Device
        {
            get;
            protected set;
        }

        public override void SetDevice(Device.FADevice aDevice)
        {
            if (aDevice is FADeviceCI501A)
                Device = aDevice as FADeviceCI501A;
            else
                throw new Exception("Device Type is not correct." + " Device Name : " + Name + " Device Type : " + aDevice.GetType().ToString());
        }

        public override void Validate()
        {
            if (DateTime.Now - Device.LastReadTime > TimeCommunicationReadTimeLimit.Time)
                CommunicationOn = false;
            else
                CommunicationOn = true;

            Device.ReadWrite();
            base.Validate();
            Status = Device.Status;
            WeightType = Device.WeightType;
            Weight = Device.Weight;
        }

        [FAAttribute("Operation")]
        public void SetZero()
        {
            Device.SetZero();
        }
    }
}
