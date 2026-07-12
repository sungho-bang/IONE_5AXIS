using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO.Ports;

namespace FALibrary.Device.RS232Device
{    
    public class FASerialPortDevice : FADevice
    {
        public UInt16 Address { get; protected set; }

        protected SerialPort Port
        {
            get;
            set;
        }

        public FASerialPortDevice()
        {
            Port = new SerialPort();            
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
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
