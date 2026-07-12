using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FALibrary.Device.AbstractDevice
{
    public abstract class FADeviceIndicator : FADevice
    {
        public bool CommunicationError { get; set; }
        public abstract object[] GetValues();
        public abstract void WriteCommand(object command);
    }
}
