using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Device.LanDevice;

namespace FALibrary.Part.CommunicationPart
{
    public class FACommonAsyncClientPart : FAPart
    {
        int _port;
        private string _ipAddress;
        private string _sendString;
        private string _receivedString;

        [FAAttribute("")]
        [FAPropertyAttribute]
        public string IPAddress
        {
            get { return _ipAddress; }
            set
            {
                if (_ipAddress == value) return;

                _ipAddress = value;
                NotifyPropertyChanged("IPAddress");
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

        public FAAsyncSocketDevice Device
        {
            get;
            protected set;
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

        public override void SetDevice(Device.FADevice aDevice)
        {
            if (aDevice is FAAsyncSocketDevice)
            {
                Device = aDevice as FAAsyncSocketDevice;
                Device.OnRead += Read;
                Device.OnWrite += Write;
            }
            else
                throw new Exception("Device Type is not correct." + " Device Name : " + Name + " Device Type : " + aDevice.GetType().ToString());
        }

        public virtual void SendData(string data)
        {
            ReceivedString = "";
            _sendString = data;
            try
            {
                Device.Connect(IPAddress, Port);
            }
            catch
            {
            }
        }

        private void Write(object sender, EventArgs e)
        {
            if (_sendString != null)
            {
                Device.Write(Device.StreamEncoding.GetBytes(_sendString));
            }
            else
            {
                _sendString = "\r\n";
                Device.Write(Device.StreamEncoding.GetBytes(_sendString));
            }
        }

        private void Read(object sender, FAGenericEventArgs<byte[]> e)
        {
            ReceivedString = Device.StreamEncoding.GetString(e.Value);
            ReceiveData(ReceivedString);
        }

        public bool IsConnected()
        {
            return Device.IsConnected;
        }

        public virtual void ReceiveData(string data)
        {
        }
    }
}
