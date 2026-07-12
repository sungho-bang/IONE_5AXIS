using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Device.LanDevice.CommunicationDevice.EC;

namespace FALibrary.Part.CommunicationPart.EC
{
    public class FAECPart : FAPart
    {
        int _port;
        private string _ipAddress;
        private List<string> _sendString = new List<string>();
        private string _ecString;

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

        public FAECCommunicationDevice Device
        {
            get;
            protected set;
        }

        [FAAttribute("")]
        public string ECString
        {
            get { return _ecString; }
            protected set
            {
                if (_ecString == value) return;

                _ecString = value;
                NotifyPropertyChanged("ECString");
            }
        }

        public override void SetDevice(Device.FADevice aDevice)
        {
            if (aDevice is FAECCommunicationDevice)
            {
                Device = aDevice as FAECCommunicationDevice;
                Device.OnRead += Read;
                Device.OnWrite += Write;
            }
            else
                throw new Exception("Device Type is not correct." + " Device Name : " + Name + " Device Type : " + aDevice.GetType().ToString());
        }

        public virtual void SendData(params string[] data)
        {
            ECString = "";
            _sendString.Clear();
            _sendString.AddRange(data);
            Device.Connect(IPAddress, Port);
        }

        private void Write(object sender, EventArgs e)
        {
            foreach(var item in _sendString)
                Device.SendData(item);

            _sendString.Clear();
        }

        private void Read(object sender, FAGenericEventArgs<byte[]> e)
        {
            string temp = Device.StreamEncoding.GetString(e.Value);

            ReceivedAllData(temp);

            string[] splitData = temp.Split('\n');
            if (splitData.Length > 0)
            {
                ECString = splitData[0];
                ReceivedData(ECString);
            }
        }

        public bool IsConnected()
        {
            return Device.IsConnected;
        }

        public virtual void ReceivedData(string data)
        {
        }

        public virtual void ReceivedAllData(string data)
        {
        }
    }
}
