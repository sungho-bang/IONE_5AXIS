using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Device.RS232Device;
using System.IO.Ports;
using System.Diagnostics;

namespace FALibrary.Device.CAS
{
    public class FACI1580Device : FASerialPortDevice
    {
        private readonly Byte STX = 02;
        private readonly Byte ETX = 03;
        private readonly int BODY_POS = 7;
        private readonly string STR_STX = Encoding.ASCII.GetString(new byte[] { 02 });
        private readonly string STR_ETX = Encoding.ASCII.GetString(new byte[] { 03 });
                
        private Stopwatch _commandSendWatch = new Stopwatch();
        private Queue<byte[]> _commandQueue = new Queue<byte[]>();
        private List<byte> _readData = new List<byte>();
        private List<Action> _getStatusFunctionList = new List<Action>();
        private int _getStatusFunctionListIndex = 0;        

        public byte DeviceNo { get; set; }
        public DateTime LastReadTime { get; set; }
        
        public string Status { get; set; }
        public string WeightType { get; set; }
        public double Weight { get; set; }
        public double LowerLimitPV { get; set; }
        public double UpperLimitPV { get; set; }        

        public FACI1580Device()
        {
            Port.DataReceived += OnDataReceived;
            LastReadTime = DateTime.Now;

            _getStatusFunctionList.Add(GetWeight);
            _getStatusFunctionList.Add(GetLowerLimitSV);
            _getStatusFunctionList.Add(GetWeight);
            _getStatusFunctionList.Add(GetUpperLimitSV);
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
                if (_commandSendWatch.ElapsedMilliseconds > 50)
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

                if (_readData.IndexOf(STX) < 0 ||
                    _readData.IndexOf(ETX) < 0 ||
                    _readData.IndexOf(STX) == _readData.IndexOf(ETX) ||
                    _readData.IndexOf(STX) > _readData.IndexOf(ETX))
                {
                    return;
                }

                string data = Encoding.ASCII.GetString(_readData.ToArray());
                int stxPos = data.IndexOf(STR_STX);
                int etxPos = data.IndexOf(STR_ETX);
                data = data.Substring(stxPos, etxPos + 1);
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
            PushCommand("RCWT");
        }

        public void GetLowerLimitSV()
        {
            PushCommand("RSP1");
        }

        public void GetUpperLimitSV()
        {
            PushCommand("RSP2");
        }

        public void SetLowerLimitSV(double value)
        {
            int temp = (int)(value);
            PushCommand("WSP1", temp.ToString("D6"));
        }

        public void SetUppwerLimitSV(double value)
        {
            int temp = (int)(value);
            PushCommand("WSP2", temp.ToString("D6"));
        }

        public void SetZero()
        {
            PushCommand("WZER");
        }

        private void PushCommand(string command, string value)
        {
            List<byte> list = new List<byte>();
            string deviceNo = DeviceNo.ToString("D2");
            list.Add(STX);
            list.AddRange(Encoding.ASCII.GetBytes(deviceNo));
            list.AddRange(Encoding.ASCII.GetBytes(command));
            if (value != null && value != "")
                list.AddRange(Encoding.ASCII.GetBytes(value));
            list.Add(ETX);
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

        private string GetBody(string data)
        {
            if (data == null) return null;
            string result = null;

            try
            {
                int etxPos = data.IndexOf(STR_ETX);
                result = data.Substring(BODY_POS, etxPos - BODY_POS);
            }
            catch
            {
            }

            return result;
        }

        private void Parsing(string data)
        {
            string command = GetCommand(data);
            if (command == null) return;

            string body = GetBody(data);
            switch (command)
            {
                case "RCWT" :
                    ParsingWeight(body);
                    break;

                case "RSP1":
                    ParsingLowerLimitPV(body);
                    break;

                case "RSP2":
                    ParsingUpperLimitPV(body);
                    break;

                case "WSP1":
                    ParsingLowerLimitSV(body);
                    break;

                case "WSP2":
                    ParsingUpperLimitSV(body);
                    break;

                case "WZER":
                    ParsingZeroSet(body);
                    break;
            }
        }

        private void ParsingWeight(string data)
        {
            Status = data.Substring(0, 2);
            WeightType = data.Substring(2, 2);
            string strValue = data.Substring(4, 8).Trim();
            var arrOfSplitBySpace = strValue.Split(' ');
            bool negativeSign = false;
            if (arrOfSplitBySpace.Length == 2)
            {
                var sign = arrOfSplitBySpace[0];
                if (sign.Trim() == "-")
                    negativeSign = true;

                strValue = arrOfSplitBySpace[1];
            }

            double temp = 0;
            if (double.TryParse(strValue, out temp) == true)
                Weight = temp;

            if (negativeSign)
                Weight = -Weight;
        }

        private void ParsingLowerLimitPV(string data)
        {
            string strValue = data.Substring(0, 6);
            double temp = 0;
            if (double.TryParse(strValue, out temp) == true)
                LowerLimitPV = temp;
        }

        private void ParsingUpperLimitPV(string data)
        {
            string strValue = data.Substring(0, 6);
            double temp = 0;
            if (double.TryParse(strValue, out temp) == true)
                UpperLimitPV = temp;
        }

        private void ParsingLowerLimitSV(string data)
        {
            // PC에서 Indicator로 전송한 Data 그대로 돌아온다.
            // 따라서 Parsing 할 필요가 없다.
        }

        private void ParsingUpperLimitSV(string data)
        {
            // PC에서 Indicator로 전송한 Data 그대로 돌아온다.
            // 따라서 Parsing 할 필요가 없다.
        }

        private void ParsingZeroSet(string data)
        {
            // PC에서 Indicator로 전송한 Data 그대로 돌아온다.
            // 따라서 Parsing 할 필요가 없다.
        }
    }
}
