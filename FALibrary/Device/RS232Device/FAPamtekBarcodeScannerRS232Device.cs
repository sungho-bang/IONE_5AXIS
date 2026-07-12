using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Diagnostics;
using System.Xml.Linq;

namespace FALibrary.Device.RS232Device
{
    public class FAPamtekBarcodeScannerRS232Device : FASerialPortDevice
    {
        public class FAPamtekBarcodeScannerData : FAObject
        {
            public bool ReadyOk { get; set; }
            public bool FoundOk { get; set; }
            public string[] BarcodeData { get; set; }
        }

        private List<byte> _readData = new List<byte>();

        public int ScannerCount { get; set; }
        public List<FAPamtekBarcodeScannerData> ScanStatus { get; protected set; }

        public FAPamtekBarcodeScannerRS232Device()
        {
            ScanStatus = new List<FAPamtekBarcodeScannerData>();
            Port.DataReceived += OnDataReceived;
        }

        public override void LoadParameters(XElement xml)
        {
            base.LoadParameters(xml);
            for (int i = 0; i < ScannerCount; i++)
            {
                ScanStatus[i] = new FAPamtekBarcodeScannerData();
            }
        }

        public void Scan(int scannerNo)
        {
            ScanStatus[scannerNo].FoundOk = false;
            ScanStatus[scannerNo].BarcodeData = null;

            string strNo = scannerNo.ToString("$0d");

            byte[] bytes = Encoding.ASCII.GetBytes("START" + strNo);
            Port.Write(bytes, 0, bytes.Length);
        }

        public void ReadScannerStatus(int scannerNo)
        {
            ScanStatus[scannerNo].ReadyOk = false;

            string strNo = scannerNo.ToString("$0d");

            byte[] bytes = Encoding.ASCII.GetBytes("READY" + strNo);
            Port.Write(bytes, 0, bytes.Length);
        }

        protected virtual void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int len = Port.BytesToRead;
            byte[] buffer;
            int etxPos = 0;
            try
            {
                buffer = new byte[len];
                Port.Read(buffer, 0, len);
                _readData.AddRange(buffer);
                etxPos = _readData.IndexOf(13);

                if (etxPos < 0) return;

                for (int i = 0; i < ScannerCount; i++)
                {
                    ParsingData(i);
                }
            }
            catch
            {
            }
        }

        protected void ParsingData(int scannerNo)
        {
            try
            {
                _readData.RemoveAt(_readData.Count - 1);

                Encoding encoding = Encoding.ASCII;
                string stringData = encoding.GetString(_readData.ToArray<byte>());
                string strNo = scannerNo.ToString("$0d");

                if (stringData == "OKRDY" + strNo)
                    ScanStatus[scannerNo].ReadyOk = true;
                else if (stringData == "NORDY" + strNo)
                    ScanStatus[scannerNo].ReadyOk = false;
                else
                {
                    string command = stringData.Substring(0, 7);
                    if (command == "FOUND" + strNo)
                    {
                        string barcodeData = stringData.Substring(8);
                        ScanStatus[scannerNo].BarcodeData = barcodeData.Split(',');
                    }
                }
            }
            catch
            {
            }
        }
    }
}
