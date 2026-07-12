using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace FALibrary.Device.Honeywell
{
    public class FAXenon1900Device : FAHoneywellScannerDevice
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

        private IPortInterface InputPort { get; set; }
        private IPortInterface OutputPort { get; set; }
        public string InputPortType { get; set; }
        public string OutputPortType { get; set; }
        public override event EventHandler<FAGenericEventArgs<string>> OnReadData;

        public bool IsConnected
        {
            get { return true; }
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
            else
                throw new Exception(Name + ", Wrong InputPortType :" + InputPortType);
            
            InputPort.LoadParameters(xml.Element("InputPort"));

            InputPort.OnReadStatus += this.ReadStatus;

            if (InputPortType == OutputPortType)
            {
                OutputPort = InputPort;
                return;
            }

            if (OutputPortType.Trim() == "Serial")
            {
                OutputPort = new SerialPortInterface();
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

        private void ReadStatus(object sender, FAGenericEventArgs<byte[]> e)
        {
            string str;
            try
            {
                str = Encoding.ASCII.GetString(e.Value);
            }
            catch
            {
                return;
            }

            if (OnReadData != null)
                OnReadData(sender, new FAGenericEventArgs<string>(str));
        }
    }
}
