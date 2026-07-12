using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FALibrary.Device.LanDevice.CommunicationDevice.Simax
{
    public class FASimaxDevice : FAUDPDevice
    {
        public EventHandler<FAGenericEventArgs<string>> ReceivedData = null;

        public void SendData(string data)
        {
            try
            {
                byte[] bytes = Encoding.Default.GetBytes(data);
                Socket.Send(bytes, bytes.Length);
            }
            catch
            {
            }
        }

        protected override void ReceiveData(byte[] data)
        {
            try
            {
                Encoding encoding = Encoding.Default;
                string stringData = encoding.GetString(data);
                if (ReceivedData != null)
                    ReceivedData(this, new FAGenericEventArgs<string>(stringData));
            }
            catch
            {
            }
        }
    }
}
