using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Device.Network;

namespace FALibrary.Part.CommunicationPart
{
    public class FAPartRemoting : FAPart
    {
        public FADeviceRemoting Device
        {
            get;
            protected set;
        }

        public override void SetDevice(Device.FADevice aDevice)
        {
            if (aDevice is FADeviceRemoting)
            {
                Device = aDevice as FADeviceRemoting;
            }
            else
                throw new Exception("Device Type is not correct." + " Device Name : " + Name + " Device Type : " + aDevice.GetType().ToString());
        }

        public bool SetData(string data)
        {
            return Device.SetData(data);
        }

        public bool GetData(out string data)
        {
            return Device.GetData(out data);
        }
    }
}
