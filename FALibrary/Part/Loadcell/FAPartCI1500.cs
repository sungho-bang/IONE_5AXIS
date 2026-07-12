using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Device.CAS;

namespace FALibrary.Part.LoadCell
{
    public class FAPartCI1500 : FAPart
    {
        #region Field
        private string _status;
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

        public FACI1500Device Device
        {
            get;
            protected set;
        }

        public override void SetDevice(Device.FADevice aDevice)
        {
            if (aDevice is FACI1500Device)
                Device = aDevice as FACI1500Device;
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
    }
}
