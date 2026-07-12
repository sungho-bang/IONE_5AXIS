using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Device.RFIDReader;

namespace FALibrary.Part.ScannerPart
{
    public class FARFIDReaderCRE1356 : FAPart
    {
        private byte _readChannel;
        private byte _readAddress;
        private byte _readDataLength;
        private string _channel1Tag;

        public FACRE1356Device Device { get; private set; }

        [FAAttribute("Parameters")]
        public byte ReadChannel
        {
            get { return _readChannel; }
            set
            {
                if (_readChannel == value) return;
                _readChannel = value;
                NotifyPropertyChanged("ReadChannel");
            }
        }
        [FAAttribute("Parameters")]
        public byte ReadAddress
        {
            get { return _readAddress; }
            set
            {
                if (_readAddress == value) return;
                _readAddress = value;
                NotifyPropertyChanged("ReadAddress");
            }
        }
        [FAAttribute("Parameters")]
        public byte ReadDataLength
        {
            get { return _readDataLength; }
            set
            {
                if (_readDataLength == value) return;
                _readDataLength = value;
                NotifyPropertyChanged("ReadDataLength");
            }
        }
        [FAAttribute("Status")]
        public string Channel1Tag
        {
            get
            {
                return _channel1Tag;
            }

            private set
            {
                if (_channel1Tag == value)
                    return;
                _channel1Tag = value;
                NotifyPropertyChanged("Channel1Tag");
            }
        }

        public override void SetDevice(Device.FADevice aDevice)
        {            
            if (aDevice == null)
                throw new Exception("Device instance is null." + " Device Name : " + Name + " Device Type : " + aDevice.GetType().ToString());
            else if (aDevice is FACRE1356Device)
                Device = aDevice as FACRE1356Device;            
            else
                throw new Exception("Device Type is not correct." + " Device Name : " + Name + " Device Type : " + aDevice.GetType().ToString());
        }

        public override void Validate()
        {
            Channel1Tag = Device.Channel1Tag;
        }

        [FAAttribute("Operation")]
        public void ReadTag()
        {
            Device.ReadTag(ReadChannel, ReadAddress, ReadDataLength);
        }
    }
}
