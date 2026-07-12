using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Xml.Linq;

namespace FALibrary.Device.Omron
{
    public class FADeviceFH1050VisionController : FADevice
    {
        private interface IPortInterface
        {
            event EventHandler<FAGenericEventArgs<byte[]>> OnReadStatus;
            void OpenPort();
            void ClosePort();
            void WriteData(byte[] buffer);
            void LoadParameters(XElement xml);
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

        private class UDPPortInterface : LanDevice.FAUDPDevice, IPortInterface
        {
            List<byte> _buffer = new List<byte>();
            public event EventHandler<FAGenericEventArgs<byte[]>> OnReadStatus;

            public string Delimiter { get; set; }

            public UDPPortInterface()
            {
                Delimiter = "\r";
            }

            public void OpenPort()
            {
                this.Open();
            }

            public void ClosePort()
            {
                this.Close();
            }

            public void WriteData(byte[] buffer)
            {
                this.Socket.Send(buffer, buffer.Length);
            }

            protected override void ReceiveData(byte[] data)
            {
                _buffer.AddRange(data);
                string receiveData = Encoding.ASCII.GetString(_buffer.ToArray());
                if (receiveData.Contains(Delimiter) == true)
                {
                    if (OnReadStatus != null)
                        OnReadStatus(this, new FAGenericEventArgs<byte[]>(_buffer.ToArray()));

                    _buffer.Clear();
                }
            }
        }

        private IPortInterface InputPort { get; set; }
        private IPortInterface OutputPort { get; set; }        
        public string InputPortType { get; set; }
        public string OutputPortType { get; set; }
        public char ResultSeperator { get; set; }
     
        public string ReceiveData { get; private set; }                

        public event EventHandler<FAGenericEventArgs<string>> OnWrite;
        public event EventHandler<FAGenericEventArgs<string[]>> OnRead;

        public FADeviceFH1050VisionController()
        {
            ReceiveData = "";
            ResultSeperator = '\r';
        }

        public override void LoadParameters(System.Xml.Linq.XElement xml)
        {
            base.LoadParameters(xml);

            if (InputPortType == null)
                throw new Exception("InputPortType is null");

            if (OutputPortType == null)
                throw new Exception("InputPortType is null");

            if (InputPortType.Trim() == "Lan")
                InputPort = new LanPortInterface();
            else if (InputPortType.Trim() == "UDP")
                InputPort = new UDPPortInterface();
            else
                throw new Exception(Name + ", Wrong InputPortType :" + InputPortType);
            
            InputPort.LoadParameters(xml.Element("InputPort"));
            InputPort.OnReadStatus += ReadStatus;

            if (InputPortType == OutputPortType)
            {
                OutputPort = InputPort;
                return;
            }

            if (OutputPortType.Trim() == "Lan")
                OutputPort = new LanPortInterface();
            else if (InputPortType.Trim() == "UDP")
                OutputPort = new UDPPortInterface();
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

        public void WriteCommand(string command)
        {
            string scanStartData = command + "\r";
            OutputPort.WriteData(Encoding.ASCII.GetBytes(scanStartData));
            if (OnWrite != null)
                OnWrite(this, new FAGenericEventArgs<string>(scanStartData));
        }

        private void ReadStatus(object sender, FAGenericEventArgs<byte[]> e)
        {
            string readData = Encoding.ASCII.GetString(e.Value);
            if (readData != null)
            {
                ReceiveData = readData;

                if (readData.Length > 0)
                {
                    string[] splitReadData = readData.Split(new char[] {ResultSeperator}, StringSplitOptions.RemoveEmptyEntries);
                    if (OnRead != null)
                        OnRead(this, new FAGenericEventArgs<string[]>(splitReadData));
                }
            }
        }
    }
}
