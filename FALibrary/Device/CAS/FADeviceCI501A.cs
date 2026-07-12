using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Device.RS232Device;
using System.IO.Ports;
using System.Diagnostics;

namespace FALibrary.Device.CAS
{
    public class FADeviceCI501A : FASerialPortDevice
    {
        private Stopwatch _commandSendWatch = new Stopwatch();
        private Queue<byte[]> _commandQueue = new Queue<byte[]>();
        private List<byte> _readData = new List<byte>();
        private List<Action> _getStatusFunctionList = new List<Action>();
        private int _getStatusFunctionListIndex = 0;        

        public byte DeviceNo { get; set; }
        public DateTime LastReadTime { get; set; }

        public EStableStatus Status { get; set; }
        public string WeightType { get; set; }
        public double Weight { get; set; }
        public double LowerLimitPV { get; set; }
        public double UpperLimitPV { get; set; }

        public FADeviceCI501A()
        {
            Port.DataReceived += OnDataReceived;
            LastReadTime = DateTime.Now;

            _getStatusFunctionList.Add(GetWeight);
        }

        public override void Open()
        {
            Port.Open();
            _commandSendWatch.Start();
        }

        public override void Close()
        {
            Port.Close();
        }

        public override void ReadWrite()
        {
            try
            {
                if (_commandSendWatch.ElapsedMilliseconds > 100)
                {
                    if (_commandQueue.Count <= 0)
                        ReadStatus();
                    if (_commandQueue.Count > 0)
                    {
                        byte[] bytes = _commandQueue.Dequeue();
                        Port.Write(bytes, 0, bytes.Length);
                    }                    

                    _commandSendWatch.Restart();
                }
            }
            catch
            {
            }
        }

        public void ReadStatus()
        {
            if (_commandQueue.Count > 0) return;

            _getStatusFunctionListIndex++;
            if (_getStatusFunctionListIndex >= _getStatusFunctionList.Count)
            {
                _getStatusFunctionListIndex = 0;
            }

            Action function = _getStatusFunctionList[_getStatusFunctionListIndex];
            function();            
        }

        protected virtual void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            LastReadTime = DateTime.Now;

            int len = Port.BytesToRead;
            byte[] buffer;

            try
            {
                buffer = new byte[len];
                Port.Read(buffer, 0, len);

                _readData.AddRange(buffer);

                string data = Encoding.ASCII.GetString(_readData.ToArray());
                int index = data.IndexOf("\r\n");

                if (index < 0)
                {
                    return;
                }
                
                int startPoint = index - 20; // cas format is 22 bytes.
                if (startPoint < 0)
                {
                    if (_readData.Count > 22)
                    {
                        _readData.Clear();
                    }

                    return;
                }

                if (index - startPoint != 20)
                {
                    if (_readData.Count > 22)
                    {
                        _readData.Clear();
                    }
                }

                data = data.Substring(startPoint, index);
                Parsing(data);
                _readData.Clear();
            }
            catch
            {
                Port.DiscardInBuffer();
                return;
            }
            finally
            {
                if (_readData.Count > 200)
                    _readData.Clear();
                Port.DiscardInBuffer();
            }
        }

        public void GetWeight()
        {
            PushCommand("RW\r\n");
        }

        public void SetZero()
        {
            PushCommand("MZ\r\n");
        }

        private void PushCommand(string command, string value)
        {
            List<byte> list = new List<byte>();
            string deviceNo = DeviceNo.ToString("D2");            
            list.AddRange(Encoding.ASCII.GetBytes(deviceNo));
            list.AddRange(Encoding.ASCII.GetBytes(command));
            if (value != null && value != "")
                list.AddRange(Encoding.ASCII.GetBytes(value));
            _commandQueue.Enqueue(list.ToArray());
        }

        private void PushCommand(string command)
        {
            PushCommand(command, null);
        }

        private string GetCommand(string data)
        {
            string command = null;
            try
            {
                command = data.Substring(3, 4);
            }
            catch
            {
            }

            return command;
        }

        private void Parsing(string data)
        {
            ParsingStatus(data);
            ParsingWeight(data);
        }

        private void ParsingStatus(string data)
        {
            string status = data.Substring(0, 2);

            if (status == "US")
                Status = EStableStatus.Unstable;
            else if (status == "ST")
                Status = EStableStatus.Stable;
            else if (status == "OL")
                Status = EStableStatus.Overload;
            else
                Status = EStableStatus.None;
        }

        private void ParsingWeight(string data)
        {
            var weightData = data.Substring(9, 11);            
            WeightType = weightData.Substring(9, 2).Trim();
            string strValue = weightData.Substring(0, 8);
            double temp = 0;
            if (double.TryParse(strValue, out temp) == true)
            {
                Weight = temp;
            }
        }
    }
}
