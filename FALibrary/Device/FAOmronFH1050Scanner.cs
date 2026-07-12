using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Xml.Linq;

namespace FALibrary.Device.Omron
{
    class FAOmronFH1050Scanner : FADevice
    {
        private interface IPortInterface
        {
            event EventHandler<FAGenericEventArgs<byte[]>> OnReadStatus;
            void OpenPort();
            void ClosePort();
            void WriteData(byte[] buffer);
            void LoadParameters(XElement xml);
        }

        private class SerialPortInterface : RS232Device.FACommonSerialPortDevice, IPortInterface
        {
            public event EventHandler<FAGenericEventArgs<byte[]>> OnReadStatus;

            public void OpenPort()
            {
                Open();
            }

            public void ClosePort()
            {
                Close();
            }

            public void WriteData(byte[] buffer)
            {
                SendData(buffer, 0, buffer.Length);
            }

            public override void LoadParameters(XElement xml)
            {
                base.LoadParameters(xml);
                this.DataReceived += ReadStatus;
            }

            private void ReadStatus(object sender, FAGenericEventArgs<byte[]> e)
            {
                if (OnReadStatus != null)
                    OnReadStatus(sender, e);
            }
        }

        private class LanPortInterface : LanDevice.FAAsyncSocketDevice, IPortInterface
        {
            private byte[] _buffer;
            public event EventHandler<FAGenericEventArgs<byte[]>> OnReadStatus;

            public LanPortInterface()
            {
                base.OnWrite += WriteData;
            }

            public void OpenPort()
            {
                UseReceiveDataSize = true;
                ReceiveDataSize = 82;
                Open();
                Connect();
            }

            public void ClosePort()
            {
                Close();
            }

            public void WriteData(byte[] buffer)
            {
                try
                {
                    _buffer = new byte[buffer.Length];
                    buffer.CopyTo(_buffer, 0);

                    if (ClientSocket != null && IsConnected)
                    {
                        ClientSocket.Close();
                        System.Threading.Thread.Sleep(50);
                    }

                    Connect();
                }
                catch
                {
                }
            }

            public override void LoadParameters(XElement xml)
            {
                base.LoadParameters(xml);
                this.OnRead += ReadStatus;
            }

            private void ReadStatus(object sender, FAGenericEventArgs<byte[]> e)
            {
                if (OnReadStatus != null)
                    OnReadStatus(sender, e);
            }

            private void WriteData(object sender, EventArgs e)
            {
                try
                {
                    base.Write(_buffer);
                }
                catch
                {
                }
            }
        }

        private IPortInterface InputPort { get; set; }
        private IPortInterface OutputPort { get; set; }
        public string InputPortType { get; set; }
        public string OutputPortType { get; set; }
        
        public bool StatusOnline { get; private set; }
        public bool StatusPaperOutError { get; private set; }
        public bool IsConnected
        {
            get { return true; }
        }

        public event EventHandler<FAGenericEventArgs<string>> OnWrite;

        public FAOmronFH1050Scanner()
        {            
        }

        public override void LoadParameters(System.Xml.Linq.XElement xml)
        {
            base.LoadParameters(xml);

            if (InputPortType == null)
                throw new Exception("InputPortType is null");

            if (OutputPortType == null)
                throw new Exception("InputPortType is null");
            
            if (InputPortType.Trim() == "Serial")
            {
                InputPort = new SerialPortInterface();
            }
            else if (InputPortType.Trim() == "Lan")
            {
                InputPort = new LanPortInterface();
            }
            else
                throw new Exception(Name + ", Wrong InputPortType :" + InputPortType);
            
            InputPort.LoadParameters(xml.Element("InputPort"));

            if (InputPortType == OutputPortType)
            {
                OutputPort = InputPort;
                return;
            }

            if (OutputPortType.Trim() == "Serial")
            {
                OutputPort = new SerialPortInterface();
            }
            else if (OutputPortType.Trim() == "Lan")
            {
                OutputPort = new LanPortInterface();
            }
            else
                throw new Exception(Name + ", Wrong OutputPortType :" + OutputPortType);

            OutputPort.LoadParameters(xml.Element("OutputPort"));       
        }

        public override void Open()
        {
            try
            {                
                OutputPort.OpenPort();
                if (OutputPort != InputPort)
                    InputPort.OpenPort();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public override void Close()
        {
            try
            {
                InputPort.ClosePort();
                if (InputPort != OutputPort)
                    OutputPort.ClosePort();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void PrintScript(string script)
        {
            OutputPort.WriteData(Encoding.ASCII.GetBytes(script));
            if (OnWrite != null)
                OnWrite(this, new FAGenericEventArgs<string>(script));
        }


        private void InitialStatus()
        {
            StatusOnline = false;
            StatusPaperOutError = true;
        }
    }
}
