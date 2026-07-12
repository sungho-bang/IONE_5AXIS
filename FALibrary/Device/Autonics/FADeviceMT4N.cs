using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Device.AbstractDevice;
using System.Threading;
using System.IO.Ports;
using System.Xml.Linq;

namespace FALibrary.Device.Autonics
{
    public class FADeviceMT4N : FADeviceIndicator
    {
        private Thread _communicationThread;

        public UInt16 Address { get; set; }
        private List<byte> _readData = new List<byte>();
        private double _readValue;

        protected SerialPort Port
        {
            get;
            set;
        }

        protected bool ReadOK { get; set; }        

        public FADeviceMT4N()
        {
            Port = new SerialPort();
            Port.DataReceived += OnDataReceived;
        }
        
        public override void Open()
        {
            Port.Open();
            IsOpened = true;
            _communicationThread = new Thread(CommunicationThreadProc);
            _communicationThread.Start();
        }

        public override void Close()
        {
            IsOpened = false;
            try
            {
                Port.Close();
            }
            catch
            {
            }
        }

        public override void WriteCommand(object command)
        {            
        }

        public override object[] GetValues()
        {
            object[] obj = new object[1];
            obj[0] = _readValue;
            return obj;
        }

        private void ReadValue()
        {
            ReadOK = false;
            _readData.Clear();
            byte[] command = { 1, 4, 0, 0, 0, 4, 0xF1, 0xC9 };

            try
            {
                Port.Write(command, 0, command.Length);
            }
            catch
            {
            }
        }

        protected virtual void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                byte[] buffer = new byte[Port.BytesToRead];
                Port.Read(buffer, 0, buffer.Length);
                _readData.AddRange(buffer);
                if (_readData.Count >= 13)
                {
                    _readValue = _readData[3] * 256 + _readData[4];
                    _readData.Clear();
                    ReadOK = true;
                    CommunicationError = false;
                }
            }
            catch
            {
            }
        }

        public override void LoadParameters(XElement xml)
        {
            try
            {
                base.LoadParameters(xml);

                if (xml.Element("PortName") != null)
                    Port.PortName = xml.Element("PortName").Value.ToString().Trim();
                else
                    throw new Exception("PortName is not exist. " + "DeviceName : " + Name);

                if (xml.Element("BaudRate") != null)
                {
                    int temp;
                    if (int.TryParse(xml.Element("BaudRate").Value.ToString(), out temp))
                        Port.BaudRate = temp;
                    else
                        throw new Exception("BaudRate is not digit");
                }
                else
                    throw new Exception("BaudRate is not exit. " + "DeviceName : " + Name);

                if (xml.Element("Parity") != null)
                    Port.Parity = (Parity)Enum.Parse(Parity.Even.GetType(),
                        xml.Element("Parity").Value.ToString().Trim());

                if (xml.Element("StopBits") != null)
                    Port.StopBits = (StopBits)Enum.Parse(StopBits.None.GetType(),
                        xml.Element("StopBits").Value.Trim());

                if (xml.Element("Address") != null)
                {
                    UInt16 temp;
                    if (UInt16.TryParse(xml.Element("Address").Value.ToString().Trim(), out temp))
                        Address = temp;
                    else
                        throw new Exception("Address is not digit");
                }                
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private void CommunicationThreadProc()
        {
            Thread.Sleep(100);

            TimeSpan communicationErrorTimeout = new TimeSpan(0, 0, 10);

            while (IsOpened)
            {
                try
                {
                    ReadValue();
                    DateTime readStartTime = DateTime.Now;
                    while (true)
                    {
                        if (IsOpened == false) return;
                        if (ReadOK && CommunicationError == false) break;
                        if (DateTime.Now - readStartTime > communicationErrorTimeout)
                        {
                            ReadOK = false;
                            CommunicationError = true;
                            break;
                        }

                        Thread.Sleep(1);
                    }

                    Thread.Sleep(1);
                }
                catch
                {
                }
            }
        }
    }
}
