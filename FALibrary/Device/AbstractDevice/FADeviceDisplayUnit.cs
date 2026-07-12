using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FALibrary.Device.AbstractDevice
{
    public interface FADeviceDisplayUnit
    {
        void SetString(string msg);
        void Clear();
    }
}
