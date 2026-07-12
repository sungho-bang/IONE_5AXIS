using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Device.Honeywell;

namespace FALibrary.Part.ScannerPart
{
    public class FAHoneywellHandScannerPart : FAPart
    {
        private FAHoneywellScannerDevice Device { get; set; }

        private string _scanData;
        [FAAttribute("Status")]
        public string ScanData
        {
            get { return _scanData; }
            set
            {
                if (_scanData == value) return;
                _scanData = value;
                NotifyPropertyChanged("ScanData");
            }
        }
        private bool _scanAble;
        [FAAttribute("Parameter")]
        public bool ScanAble
        {
            get { return _scanAble; }
            set
            {
                if (_scanAble == value) return;
                _scanAble = value;
                NotifyPropertyChanged("ScanAble");
            }
        }

        public override void SetDevice(Device.FADevice aDevice)
        {
            if (aDevice is FAHoneywellScannerDevice)
            {
                Device = aDevice as FAHoneywellScannerDevice;
                Device.OnReadData += OnReadData;
            }
            else
                throw new Exception("Device Type is not correct." + " Device Name : " + Name + " Device Type : " + aDevice.GetType().ToString());
        }

        private void OnReadData(object sender, FAGenericEventArgs<string> e)
        {
            if (ScanAble)
            {
                ScanData = e.Value;
            }
        }
    }
}
