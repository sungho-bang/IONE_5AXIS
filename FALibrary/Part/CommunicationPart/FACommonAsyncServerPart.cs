using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Device.LanDevice;

namespace FALibrary.Part.CommunicationPart
{
    public class FACommonAsyncServerPart : FAPart
    {
        int _port;
        private string _clientIPAddress;        
        private string _receivedString;

        [FAAttribute("")]
        [FAPropertyAttribute]
        public string ClientIPAddress
        {
            get { return _clientIPAddress; }
            set
            {
                if (_clientIPAddress == value) return;

                _clientIPAddress = value;
                NotifyPropertyChanged("ClientIPAddress");
            }
        }

        [FAAttribute("")]
        [FAPropertyAttribute]
        public int Port
        {
            get { return _port; }
            set
            {
                if (_port == value) return;

                _port = value;
                NotifyPropertyChanged("Port");
            }
        }

        [FAAttribute("")]
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

        public FAAsyncServerSocketDevice Device
        {
            get;
            protected set;
        }

        public override void SetDevice(Device.FADevice aDevice)
        {
            if (aDevice is FAAsyncServerSocketDevice)
            {
                Device = aDevice as FAAsyncServerSocketDevice;
                Device.OnRead += Read;
            }
            else
                throw new Exception("Device Type is not correct." + " Device Name : " + Name + " Device Type : " + aDevice.GetType().ToString());
        }

        public void SendData(string[] data)
        {
            Device.SendData(data);
        }

        private void Read(object sender, FAGenericEventArgs<byte[]> e)
        {
            ReceivedString = Device.StreamEncoding.GetString(e.Value);
            ReceiveData(ReceivedString);
        }

        public virtual void ReceiveData(string data)
        {
        }
    }
}
