using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Device.LanDevice.CommunicationDevice.EC;

namespace FALibrary.Part.CommunicationPart.EC
{
    public class FAECServerPart : FAPart
    {
        int _port;
        private string _IPAddress;        
        private string _ecString;

        [FAAttribute("")]
        [FAPropertyAttribute]
        public string IPAddress
        {
            get { return _IPAddress; }
            set
            {
                if (_IPAddress == value) return;

                _IPAddress = value;
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

        public FAECServerDevice Device
        {
            get;
            protected set;
        }

        public override void SetDevice(Device.FADevice aDevice)
        {
            if (aDevice is FAECServerDevice)
            {
                Device = aDevice as FAECServerDevice;
                Device.OnRead += Read;                
            }
            else
                throw new Exception("Device Type is not correct." + " Device Name : " + Name + " Device Type : " + aDevice.GetType().ToString());
        }

        public void SendData(params string[] data)
        {
            try
            {
                string[] buffer = new string[data.Length];
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = data[i];
                }

                Device.SendData(buffer);
            }
            catch (System.Exception e)
            {
                throw e;
            }
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

        public virtual void ReceivedData(string data)
        {
        }

        public virtual void ReceivedAllData(string data)
        {
        }

        [FAAttribute("Operation")]
        public void Open()
        {
            try
            {
                Device.Open();
            }
            catch (Exception e)
            {
                Utility.Trace.WriteLine(this, "Part", e.ToString());
            }
        }

        [FAAttribute("Operation")]
        public void Close()
        {
            try
            {
                Device.Close();
            }
            catch (Exception e)
            {
                Utility.Trace.WriteLine(this, "Part", e.ToString());
            }
        }
    }
}
