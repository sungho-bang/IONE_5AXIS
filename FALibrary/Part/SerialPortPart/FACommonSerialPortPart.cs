using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Device.RS232Device;

namespace FALibrary.Part.SerialPortPart
{
    public class FACommonSerialPortPart : FAPart
    {
        public FACommonSerialPortDevice Device { get; private set; }

        public void SendData(string text)
        {
            Device.SendData(text);
        }

        public void SendData(byte[] buffer, int offset, int count)
        {
            Device.SendData(buffer, offset, count);
        }

        public void SendData(char[] buffer, int offset, int count)
        {
            Device.SendData(buffer, offset, count);
        }

        public override void SetDevice(Device.FADevice aDevice)
        {
            if (aDevice is FACommonSerialPortDevice)
                Device = aDevice as FACommonSerialPortDevice;
            else
                throw new Exception("Device Type is not correct." + " Device Name : " + Name + " Device Type : " + aDevice.GetType().ToString());
        }
    }
}
