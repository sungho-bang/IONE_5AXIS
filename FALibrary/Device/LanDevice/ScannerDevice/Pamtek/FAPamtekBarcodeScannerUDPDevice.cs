using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FALibrary.Device.LanDevice.ScannerDevice.Pamtek
{
    public class FAPamtekBarcodeScannerUDPDevice : FAUDPDevice
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

        public FAPamtekBarcodeScannerUDPDevice()
        {
            ScanStatus = new List<FAPamtekBarcodeScannerData>();
        }

        public void Scan(int scannerNo)
        {
            try
            {
                _readData.Clear();
                ScanStatus[scannerNo - 1].FoundOk = false;
                ScanStatus[scannerNo - 1].BarcodeData = null;

                string strNo = scannerNo.ToString("D2");
                SendData("START" + strNo);
            }
            catch
            {
            }
        }

        public void ContinueScan(int scannerNo)
        {
            try
            {
                _readData.Clear();
                ScanStatus[scannerNo - 1].FoundOk = false;
                ScanStatus[scannerNo - 1].BarcodeData = null;

                string strNo = scannerNo.ToString("D2");
                SendData("CONTI" + strNo);
            }
            catch
            {
            }
        }

        public void StopScan(int scannerNo)
        {
            try
            {
                string strNo = scannerNo.ToString("D2");
                SendData("STOP0" + strNo);
            }
            catch
            {
            }
        }

        public void ReadScannerStatus(int scannerNo)
        {
            try
            {
                _readData.Clear();
                ScanStatus[scannerNo - 1].ReadyOk = false;

                string strNo = scannerNo.ToString("D2");

                SendData("READY" + strNo);
            }
            catch
            {
            }
        }

        public override void LoadParameters(System.Xml.Linq.XElement xml)
        {
            base.LoadParameters(xml);
            for (int i = 0; i < ScannerCount; i++)
            {
                ScanStatus.Add(new FAPamtekBarcodeScannerData());
            }
        }

        protected override void ReceiveData(byte[] data)
        {
            int len = data.Length;
            int etxPos = 0;
            try
            {
                _readData.AddRange(data);
                etxPos = _readData.IndexOf(13);

                if (etxPos < 0) return;

                for (int i = 0; i < ScannerCount; i++)
                {
                    ParsingData(i + 1);
                }

                _readData.Clear();
            }
            catch
            {
                _readData.Clear();
            }
        }

        protected void ParsingData(int scannerNo)
        {
            try
            {
                List<byte> buffer = new List<byte>(_readData);
                buffer.RemoveAt(buffer.Count - 1);

                Encoding encoding = Encoding.ASCII;
                string stringData = encoding.GetString(buffer.ToArray<byte>());
                string strNo = scannerNo.ToString("D2");

                if (stringData == "OKRDY" + strNo)
                    ScanStatus[scannerNo - 1].ReadyOk = true;
                else if (stringData == "NORDY" + strNo)
                    ScanStatus[scannerNo - 1].ReadyOk = false;
                else
                {
                    string command = stringData.Substring(0, 7);
                    if (command == "FOUND" + strNo)
                    {
                        ScanStatus[scannerNo - 1].FoundOk = true;
                        string barcodeData = stringData.Substring(8);
                        ScanStatus[scannerNo - 1].BarcodeData = barcodeData.Split(',');
                    }
                }
            }
            catch
            {
            }
        }

        protected void SendData(string data)
        {
            try
            {
                List<byte> sendData = new List<byte>();
                sendData.AddRange(Encoding.ASCII.GetBytes(data));
                sendData.Add(13);
                SendData(IPAddress, RemotePort, sendData.ToArray());
            }
            catch
            {
            }
        }
    }
}
