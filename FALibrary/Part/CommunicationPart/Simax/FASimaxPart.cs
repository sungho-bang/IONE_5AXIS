using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Device.LanDevice.CommunicationDevice.Simax;

namespace FALibrary.Part.CommunicationPart.Simax
{
    public class FASimaxPart : FAPart
    {
        private string _receivedString;

        public FASimaxDevice Device
        {
            get;
            protected set;
        }

        public string IPAddress
        {
            get
            {
                if (Device != null)
                    return Device.IPAddress;
                else
                    return "";
            }

            set
            {
                if (Device != null)
                {
                    if (Device.IPAddress == value) return;

                    Device.IPAddress = value;
                    NotifyPropertyChanged("IPAddress");
                }
            }
        }

        public int Port
        {
            get
            {
                if (Device != null)
                    return Device.Port;
                else
                    return 0;
            }

            set
            {
                if (Device != null)
                {
                    if (Device.Port == value) return;

                    Device.Port = value;
                    NotifyPropertyChanged("Port");
                }
            }
        }

        public string ReceivedString
        {
            get { return _receivedString; }
            protected set
            {
                if (_receivedString == value) return;

                _receivedString = value;
                NotifyPropertyChanged("ReceivedString");
            }
        }

        public override void SetDevice(Device.FADevice aDevice)
        {
            if (aDevice is FASimaxDevice)
            {
                Device = aDevice as FASimaxDevice;
                Device.ReceivedData += ReceivedData;
            }
            else
                throw new Exception("Device Type is not correct." + " Device Name : " + Name + " Device Type : " + aDevice.GetType().ToString());
        }

        public virtual void SendData(string data)
        {
            ReceivedString = "";
            Device.SendData(data);
        }

        public virtual void ReceivedData(object sender, FAGenericEventArgs<string> e)
        {
            ReceivedString = e.Value;
        }
    }
}
