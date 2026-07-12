using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace FALibrary.Device.Cognex
{
    public class FADeviceCognexInsightController : FADevice
    {
        private interface IPortInterface
        {
            LanDevice.FATelnetClientDevice.ConnectionState ConnectedState { get; }
            Action<string> ReceiveDataDelegate { get; set; }
            event EventHandler<EventArgs> OnConnected;
            event EventHandler<FAGenericEventArgs<string>> OnDiconnected;

            void OpenPort();
            void ClosePort();
            void WriteData(string data);
            void LoadParameters(XElement xml);
        }

        private class LanPortInterface : LanDevice.FATelnetClientDevice, IPortInterface
        {
            private LanDevice.FATelnetClientDevice.ConnectionState _connectedState;
            public LanDevice.FATelnetClientDevice.ConnectionState ConnectedState
            {
                get { return _connectedState; }
            }
            
            public string IPAddress { get; set; }
            public int Port { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }

            public LanPortInterface()
            {
                IPAddress = "127.0.0.1";
                Port = 23;
                UserName = "blank";
                Password = string.Empty;

                OnDiconnected +=
                    delegate
                    {
                        Task.Factory.StartNew(
                            delegate
                            {
                                string failMessage;
                                while (!ThreadStopRequest)
                                {
                                    var result = this.Open(IPAddress, Port, UserName, Password, out _connectedState, out failMessage);
                                    if (result)
                                    {
                                        break;
                                    }
                                }
                            });                        
                    };
            }

            public override void LoadParameters(XElement xml)
            {
                base.LoadParameters(xml);
            }

            public void OpenPort()
            {
                string failMessage;

                if (this.Open(IPAddress, Port, UserName, Password, out _connectedState, out failMessage) == false)
                    throw new Exception(failMessage);
            }

            public void ClosePort()
            {
                this.Close();
            }

            public void WriteData(string data)
            {
                this.SendData(data + "\r\n");
            }
        }

        private IPortInterface Port { get; set; }
        public string PortType { get; set; }

        public Action<string> ReceiveDataDelegate { get; set; }
        public event EventHandler<EventArgs> OnConnected = delegate { };
        public event EventHandler<FAGenericEventArgs<string>> OnDiconnected = delegate { };

        public override void LoadParameters(XElement xml)
        {
            base.LoadParameters(xml);

            if (string.IsNullOrEmpty(PortType))
                throw new Exception("InputPortType is null");

            if (PortType == "Lan")            
            {
                Port = new LanPortInterface();
            }

            Port.LoadParameters(xml.Element("Port"));
            Port.ReceiveDataDelegate =
                delegate(string data)
                {
                    if (ReceiveDataDelegate != null)
                        ReceiveDataDelegate(data);
                };

            Port.OnConnected +=
                delegate(object sender, EventArgs e)
                {
                    OnConnected(sender, e);
                };

            Port.OnDiconnected +=
                delegate(object sender, FAGenericEventArgs<string> e)
                {
                    OnDiconnected(sender, e);
                };
        }

        public override void Open()
        {
            base.Open();

            Port.OpenPort();
        }

        public override void Close()
        {
            base.Close();

            Port.ClosePort();
        }

        public void SendData(string data)
        {
            Port.WriteData(data);
        }
    }
}
