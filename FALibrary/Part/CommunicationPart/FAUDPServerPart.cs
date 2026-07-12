using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Device.LanDevice;
using System.Net;
using System.Net.Sockets;

namespace FALibrary.Part.CommunicationPart
{
    public class FAUDPServerPart : FAPart
    {
        public Action<IPEndPoint, byte[]> ReceivedData;

        public FAUDPServerDevice Device
        {
            get;
            private set;
        }

        public Encoding Encoding { get; set; }

        public override void SetDevice(Device.FADevice aDevice)
        {
            if (aDevice is FAUDPDevice)
            {
                Device = aDevice as FAUDPServerDevice;
                Device.OnReceiveData +=
                    (o, e) =>
                    {
                        if (ReceivedData != null)
                            ReceivedData(e.IPEndPoint, e.Bytes);
                    };
            }
            else
                throw new Exception("Device Type is not correct." + " Device Name : " + Name + " Device Type : " + aDevice.GetType().ToString());
        }

        public void SendData(string ip, int port, byte[] data)
        {
            Device.SendData(ip, port, data);
        }

        public void SendData(string ip, int port, string data)
        {
            Device.SendData(ip, port, Encoding.GetBytes(data));
        }
    }
}
