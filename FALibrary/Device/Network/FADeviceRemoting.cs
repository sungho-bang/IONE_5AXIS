using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FALibrary.Device.Network
{
    public abstract class FADeviceRemoting : FADevice
    {
        public abstract bool SetData(string data);
        public abstract bool GetData(out string data);
    }
}
