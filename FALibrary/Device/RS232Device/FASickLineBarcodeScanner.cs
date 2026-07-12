using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace FALibrary.Device.RS232Device
{
    public class FASickLineBarcodeScanner : FASerialPortDevice
    {
        readonly protected byte STX = 2;
        readonly protected byte ETX = 3;

        public enum ScannerReadMode
        {
            OneTime, Allways
        }
        
        private bool _scanning;
        private bool _scanOK;
        private string _scanData;

        [FAAttribute("")]
        public ScannerReadMode ReadMode
        {
            get;
            protected set;
        }

        [FAAttribute("")]
        public bool Scanning
        {
            get { return _scanning; }
            protected set
            {
                if (_scanning == value) return;

                _scanning = value;
                NotifyPropertyChanged("Scanning");
            }
        }

        [FAAttribute("")]
        public bool ScanOK
        {
            get { return _scanOK; }
            protected set
            {
                if (_scanOK == value) return;

                _scanOK = value;
                NotifyPropertyChanged("ScanOK");
            }
        }

        [FAAttribute("")]
        public string ScanData
        {
            get { return _scanData; }
            protected set
            {
                if (_scanData == value) return;

                _scanData = value;
                NotifyPropertyChanged("ScanData");
            }
        }

        public FASickLineBarcodeScanner()
        {
            Port.DataReceived += OnDataReceived;
        }

        public void ReadOn()
        {
            if (ReadMode == ScannerReadMode.OneTime)
            {
                byte[] buffer = new byte[4];
                buffer[0] = STX;
                buffer[1] = Convert.ToByte('2');
                buffer[2] = Convert.ToByte('1');
                buffer[3] = ETX;

                Port.Write(buffer, 0, buffer.Length);
            }
                        
            ScanOK = false;
            ScanData = "";
            Scanning = true;            
        }

        public void ReadOff()
        {
            if (ReadMode == ScannerReadMode.Allways) return;

            byte[] buffer = new byte[4];
            buffer[0] = STX;
            buffer[1] = Convert.ToByte('2');
            buffer[2] = Convert.ToByte('2');
            buffer[3] = ETX;

            Port.Write(buffer, 0, buffer.Length);
        }

        protected virtual void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int len = Port.BytesToRead;
            byte[] buffer = new byte[len];
            Port.Read(buffer, 0, len);

            if (Scanning)
            {
                if (buffer[0] == STX && buffer[len - 1] == ETX)
                {
                    ScanData = Encoding.Default.GetString(buffer);
                    if (ScanData != "NOREAD")
                    {
                        ScanOK = true;
                        Scanning = false;
                    }                    
                }
            }
        }
    }
}
