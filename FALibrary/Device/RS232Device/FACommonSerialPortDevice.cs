using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace FALibrary.Device.RS232Device
{
    public class FACommonSerialPortDevice : FASerialPortDevice
    {
        public event EventHandler<FAGenericEventArgs<byte[]>> DataReceived;

        public FACommonSerialPortDevice()
        {
            Port.DataReceived += DataReceivedEventHandler;
        }

        public override void Open()
        {
            Port.Open();
        }

        public override void Close()
        {
            Port.Close();
        }

        public void SendData(string text)
        {
            Port.Write(text);
        }

        public void SendData(byte[] buffer, int offset, int count)
        {
            Port.Write(buffer, offset, count);
        }

        public void SendData(char[] buffer, int offset, int count)
        {
            Port.Write(buffer, offset, count);
        }

        public void DiscardInBuffer()
        {
            Port.DiscardInBuffer();
        }

        private void DataReceivedEventHandler(object sender, SerialDataReceivedEventArgs e)
        {            
            int len = Port.BytesToRead;
            byte[] buffer;

            try
            {
                buffer = new byte[len];
                Port.Read(buffer, 0, len);
                DataReceived(this, new FAGenericEventArgs<byte[]>(buffer));
            }
            catch
            {
                Port.DiscardInBuffer();
            }
        }
    }
}
