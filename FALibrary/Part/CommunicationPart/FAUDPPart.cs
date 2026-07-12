using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Device.LanDevice;

namespace FALibrary.Part.CommunicationPart
{
    public class FAUDPPart : FAPart
    {
        private string _receivedString;

        public FAUDPDevice Device
        {
            get;
            protected set;
        }

        private int _port;
        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public int Port
        {
            get
            {
                return _port;
            }

            set
            {
                if (_port == value) return;
                _port = value;
                NotifyPropertyChanged("Port");
            }
        }

        private string _remoteIPAddress;
        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public string RemoteIPAddress
        {
            get
            {
                return _remoteIPAddress;
            }

            set
            {
                if (_remoteIPAddress == value) return;
                _remoteIPAddress = value;
                NotifyPropertyChanged("RemoteIPAddress");
            }
        }

        private int _remotePort;
        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public int RemotePort
        {
            get
            {
                return _remotePort;
            }

            set
            {
                if (_remotePort == value) return;
                _remotePort = value;
                NotifyPropertyChanged("RemotePort");
            }
        }

        [FAAttribute("Status")]
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
            if (aDevice is FAUDPDevice)
            {
                Device = aDevice as FAUDPDevice;
                Device.OnReceiveData += ReceivedData;
            }
            else
                throw new Exception("Device Type is not correct." + " Device Name : " + Name + " Device Type : " + aDevice.GetType().ToString());
        }

        public virtual void SendData(string data)
        {
            ReceivedString = "";
            Device.SendData(RemoteIPAddress, RemotePort, data);
        }

        protected virtual void ReceivedData(object sender, FAGenericEventArgs<byte[]> e)
        {
            ReceivedString = Device.StreamEncoding.GetString(e.Value, 0, e.Value.Length);
        }
    }
}
