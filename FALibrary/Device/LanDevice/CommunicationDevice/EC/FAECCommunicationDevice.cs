using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FALibrary.Device.LanDevice.CommunicationDevice.EC
{
    public class FAECCommunicationDevice : FAAsyncSocketDevice
    {              
        public FAECCommunicationDevice()
        {
        }

        public void SendData(string data)
        {
            if (Simulation) return;

            try
            {
                WriteString(data + "\n");
            }
            catch
            {
            }
        }
    }
}
