using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FALibrary.Device.Honeywell
{
    public abstract class FAHoneywellScannerDevice : FADevice
    {
        public abstract event EventHandler<FAGenericEventArgs<string>> OnReadData;
    }
}
