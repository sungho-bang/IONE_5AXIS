using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Diagnostics;

namespace FALibrary.Device.RS232Device
{
    public class FADeviceKeyenceContactSensor : FASerialPortDevice
    {
        private readonly Byte CR    = 0x0D; //0x0D(13);
        private readonly Byte LF    = 0x0A; //0x0A(10)
        private readonly Byte COMMA = 0x2C;

        class PortInfo
        {
            public string PortName { get; private set; }
            public SerialPort Port { get; private set; }
            private List<Action<byte[]>> _dataReceived = new List<Action<byte[]>>();
            private List<byte[]> _commands = new List<byte[]>();
            private Stopwatch _commandSendWatch = new Stopwatch();
            private Queue<byte[]> _commandQueue = new Queue<byte[]>();

            public PortInfo(string portName, SerialPort port)
            {
                PortName = portName;
                Port = port;
                port.DataReceived +=
                    (sender, e) =>
                    {
                        try
                        {
                            int len = port.BytesToRead;
                            byte[] buffer;
                            buffer = new byte[len];
                            port.Read(buffer, 0, len);
                            foreach (var method in _dataReceived)
                            {
                                if (method != null)
                                    method(buffer);
                            }
                        }
                        catch
                        {
                        }
                    };

                _commandSendWatch.Start();
            }

            public void AddDataReceivedMethod(Action<byte[]> method)
            {
                _dataReceived.Add(method);
            }

            public void AddCommand(byte[] command)
            {
                _commandQueue.Enqueue(command);
            }

            public void Process()
            {
                try
                {
                    if (_commandSendWatch.ElapsedMilliseconds >= 200)
                    {
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
        }

        class SharedPorts
        {
            private Dictionary<string, PortInfo> _ports = new Dictionary<string, PortInfo>();

            public void AddPort(string portName, SerialPort port) 
            {
                if (this.Constains(portName))
                {
                    Utility.Trace.WriteLine(this, "Device", string.Format("Aready exist port[{0}]", portName));
                    return;
                }

                var portInfo = new PortInfo(portName, port);
                _ports.Add(portName, portInfo);
            }

            public void AddDataReceiveMethod(string portName, Action<byte[]> method)
            {
                if (_ports.ContainsKey(portName))
                {
                    _ports[portName].AddDataReceivedMethod(method);
                }
            }

            public void AddCommand(string portName, byte[] command)
            {
                if (_ports.ContainsKey(portName))
                {
                    _ports[portName].AddCommand(command);
                }
            }

            public void Process(string portName)
            {
                if (_ports.ContainsKey(portName))
                {
                    _ports[portName].Process();
                }
            }

            public bool Constains(string portName)
            {
                return _ports.ContainsKey(portName);
            }

            public SerialPort GetPort(string portName)
            {
                if (this.Constains(portName) == false) return null;
                return _ports[portName].Port;
            }
        }

        static SharedPorts _sharedPorts = new SharedPorts();

        private List<byte> _readData = new List<byte>();

        private Stopwatch _commandSendWatch = new Stopwatch();

        public double CurrentThickness1 { get; set; }
        public double CurrentThickness2 { get; set; }

        public DateTime LastReadTime { get; set; }
        public bool Slave { get; private set; }

        public FADeviceKeyenceContactSensor()
        {
            Address = 1;
            LastReadTime = DateTime.Now;
        }

        public override void LoadParameters(System.Xml.Linq.XElement xml)
        {
            base.LoadParameters(xml);

            if (Slave)
                Port = _sharedPorts.GetPort(Port.PortName);
            else
                _sharedPorts.AddPort(Port.PortName, Port);
        }

        public override void Open()
        {
            if (Slave == false)
                Port.Open();

            _sharedPorts.AddDataReceiveMethod(Port.PortName, OnDataReceived);

            _commandSendWatch.Start();

			System.Threading.Thread.Sleep(500);
            Reset_End(0);
            System.Threading.Thread.Sleep(500);
            Reset_End(1);
        }

        public override void Close()
        {
            if (Slave == false)
                Port.Close();
        }

        public override void ReadWrite()
        {
            try
            {
                if (!Slave)
                    _sharedPorts.Process(Port.PortName);

                if (_commandSendWatch.ElapsedMilliseconds >= 200)
                {
                    ReadThicknesse();

                    _commandSendWatch.Restart();
                }
            }
            catch
            {
            }
        }

        public bool ReadThicknesse()
        {
            bool bRet = WriteCommand_M0();
            return bRet;
        }
        public bool Reset_Start(int nID)
        {
            bool bRet = WriteCommand_SW(nID, 050, 1);
            //System.Threading.Thread.Sleep(100);
            return true;
        }
        public bool Reset_End(int nID)
        {
            bool bRet = WriteCommand_SW(nID, 050, 0);
            //System.Threading.Thread.Sleep(100);
            return true;
        }

        private bool WriteCommand_M0()
        {
            try
            {
                string szCmd = "M0";

                //COMMAND FORMAT ( "M0" + CRLF )
                byte[] command = { 0x00, 0x00, 0x0D, 0x0A };

                command[00] = System.Text.ASCIIEncoding.ASCII.GetBytes(szCmd)[0];
                command[01] = System.Text.ASCIIEncoding.ASCII.GetBytes(szCmd)[1];
                command[02] = CR;
                command[03] = LF;

                _sharedPorts.AddCommand(Port.PortName, command);
                //string szCommand = Encoding.Default.GetString(command); //Log출력 사용
                //------------------------------------------------------------------------------------------------------
                string szCommand = Encoding.Default.GetString(command);
                System.Diagnostics.Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + ", Keyence ContactSensor (len=" + command.Length.ToString("D2") +
                    ") Send=" + szCommand);
                //------------------------------------------------------------------------------------------------------
            }
            catch
            {
                return false;
            }

            return true;
        }

        private bool WriteCommand_SR(int nIDNo = 0, int nDataNo = 001)
        {
            // 000:Comparator value , 001:Calculation display value
            // 053:Reset request    , 054:Initial reset request
            try
            {
                string szCmd = "SR";
                string szIDNo = string.Format("{0:D2}", nIDNo);
                string szDataNo = string.Format("{0:D3}", nDataNo);

                //COMMAND FORMAT ( "SR" ,ID No.(2byte) , Data No.(3byte), CR, LF )
                byte[] command = new byte[11];
                command[00] = System.Text.ASCIIEncoding.ASCII.GetBytes(szCmd)[0];
                command[01] = System.Text.ASCIIEncoding.ASCII.GetBytes(szCmd)[1];
                command[02] = COMMA;
                command[03] = System.Text.ASCIIEncoding.ASCII.GetBytes(szIDNo)[0];
                command[04] = System.Text.ASCIIEncoding.ASCII.GetBytes(szIDNo)[1];
                command[05] = COMMA;
                command[06] = System.Text.ASCIIEncoding.ASCII.GetBytes(szDataNo)[0];
                command[07] = System.Text.ASCIIEncoding.ASCII.GetBytes(szDataNo)[1];
                command[08] = System.Text.ASCIIEncoding.ASCII.GetBytes(szDataNo)[2];
                command[09] = CR;
                command[10] = LF;

                _sharedPorts.AddCommand(Port.PortName, command);
                //string szCommand = Encoding.Default.GetString(command); //Log출력 사용
                //------------------------------------------------------------------------------------------------------
                string szCommand = Encoding.Default.GetString(command);
                System.Diagnostics.Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + ", Keyence ContactSensor (len=" + command.Length.ToString("D2") +
                    ") Send=" + szCommand);
                //------------------------------------------------------------------------------------------------------
            }
            catch
            {
                return false;
            }

            return true;
        }

        private bool WriteCommand_SW(int nIDNo = 0, int nDataNo = 050, int nDataVal=1)
        {
            // 050:Reset request
            try
            {
                string szCmd = "SW";
                string szIDNo = string.Format("{0:D2}", nIDNo);
                string szDataNo = string.Format("{0:D3}", nDataNo);
                string szSetData = string.Format("{0:D1}", nDataVal);

                //COMMAND FORMAT ( "SW" ,ID No.(2byte) , Data No.(3byte), SetingData(1byte) , CR, LF )
                byte[] command = new byte[13];
                command[00] = System.Text.ASCIIEncoding.ASCII.GetBytes(szCmd)[0];
                command[01] = System.Text.ASCIIEncoding.ASCII.GetBytes(szCmd)[1];
                command[02] = COMMA;
                command[03] = System.Text.ASCIIEncoding.ASCII.GetBytes(szIDNo)[0];
                command[04] = System.Text.ASCIIEncoding.ASCII.GetBytes(szIDNo)[1];
                command[05] = COMMA;
                command[06] = System.Text.ASCIIEncoding.ASCII.GetBytes(szDataNo)[0];
                command[07] = System.Text.ASCIIEncoding.ASCII.GetBytes(szDataNo)[1];
                command[08] = System.Text.ASCIIEncoding.ASCII.GetBytes(szDataNo)[2];
                command[09] = COMMA;
                command[10] = System.Text.ASCIIEncoding.ASCII.GetBytes(szSetData)[0];
                command[11] = CR;
                command[12] = LF;

                _sharedPorts.AddCommand(Port.PortName, command);
                //string szCommand = Encoding.Default.GetString(command); //Log출력 사용
                //------------------------------------------------------------------------------------------------------
                string szCommand = Encoding.Default.GetString(command);
                System.Diagnostics.Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + ", Keyence ContactSensor (len=" + command.Length.ToString("D2") +
                    ") Send=" + szCommand);
                //------------------------------------------------------------------------------------------------------
            }
            catch
            {
                return false;
            }

            return true;
        }

        public string str2hex(string strData)
        {
            string resultHex = string.Empty;
            byte[] arr_byteStr = Encoding.Default.GetBytes(strData);

            foreach (byte byteStr in arr_byteStr)
                resultHex += string.Format("{0:x2} ", byteStr);

            return resultHex;
        }


        protected virtual void OnDataReceived(byte[] buffer)
        {
            LastReadTime = DateTime.Now;

            try
            {
                _readData.AddRange(buffer);

                bool bReadingOK = false;
                int nCRLFPos = 0;
                int nLastCommaPos = 0;
                if (_readData.Count >= 4)
                {
                    for (int i = 1; i < _readData.Count; i++)
                    {
                        if(_readData[i] == COMMA)
                            nLastCommaPos = i;

                        if (_readData[i-1] == CR && _readData[i] == LF)
                        {
                            nCRLFPos = i - 1;
                            bReadingOK = true;
                            break;
                        }
                    }
                }

                if (bReadingOK)
                {
                    //------------------------------------------------------------------------------------------------------
                    string szRead = Encoding.Default.GetString(_readData.ToArray()); //Log출력 사용
                    System.Diagnostics.Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + ", Keyence ContactSensor (len=" + _readData.Count.ToString("D2") +
                        ") Read=" + szRead);
                    System.Diagnostics.Trace.WriteLine(str2hex(szRead));
                    //------------------------------------------------------------------------------------------------------

                    int count = nCRLFPos;
                    byte[] dataBlock = new byte[count];
                    _readData.CopyTo(0, dataBlock, 0, count);

                    string str = Encoding.Default.GetString(dataBlock);
                    string[] splitData = str.Split(',');

                    _readData.Clear();
                    Port.DiscardInBuffer();

                    if (splitData.Length >= 3 && splitData[0] == "M0")
                    {
                        double dRet;
                        double.TryParse(splitData[1], out dRet);
                        CurrentThickness1 = dRet;

                        double.TryParse(splitData[2], out dRet);
                        CurrentThickness2 = dRet;

                        //int nThicknessCount = nCRLFPos - nLastCommaPos - 1;
                        //byte[] byteThickness = new byte[nThicknessCount];
                        //_readData.CopyTo(nLastCommaPos + 1, byteThickness, 0, nThicknessCount);
                        //CurrentThickness1 = BitConverter.ToDouble(byteThickness, 0);

                        //byte[] byteThickness = new byte[8];
                        //double.TryParse(Encoding.ASCII.GetString(byteThickness), out dRet);
                    }
                    else if (splitData.Length >= 1 && splitData[0] == "ER")
                    {
                        _readData.Clear();
                        Port.DiscardInBuffer();
                    }
                    else if (   splitData.Length >= 3 && splitData[0] == "SW" )
                    {
                        int iRead_ID;
                        int.TryParse(splitData[1], out iRead_ID);

                        int iRead_DataNo;
                        int.TryParse(splitData[2], out iRead_DataNo);

                        //WriteCommand_SW(0, 050, 0);
                        //WriteCommand_SW(1, 050, 0);
                    }
                }
                //else
                //{
                //    _readData.Clear();
                //    Port.DiscardInBuffer();
                //}
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
    }
}
