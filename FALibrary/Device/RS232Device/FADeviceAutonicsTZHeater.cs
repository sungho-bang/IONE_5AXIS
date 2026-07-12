using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Diagnostics;

namespace FALibrary.Device.RS232Device
{
    public class FADeviceAutonicsTZHeater : FASerialPortDevice
    {
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
                    if (_commandSendWatch.ElapsedMilliseconds > 250)
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

        protected readonly byte STX = 2;
        protected readonly byte ETX = 3;
        protected readonly UInt16 RX = 0x5258;
        protected readonly UInt16 RD = 0x5244;
        protected readonly UInt16 WX = 0x5758;
        protected readonly UInt16 WD = 0x5744;
        protected readonly UInt16 PO = 0x5030;
        protected readonly UInt16 SO = 0x5330;

        private List<byte> _readData = new List<byte>();
        private bool _readType = false;
        private Stopwatch _commandSendWatch = new Stopwatch();

        public double CurrentTemperature { get; set; }
        public double TargetTemperature { get; set; }
        public DateTime LastReadTime { get; set; }
        public bool Slave { get; private set; }

        public FADeviceAutonicsTZHeater()
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

                if (_commandSendWatch.ElapsedMilliseconds > 1000)
                {
                    if (_readType == true)
                    {
                        ReadTemperature();
                        _readType = false;
                    }
                    else
                    {
                        ReadTargetTemperature();
                        _readType = true;
                    }

                    _commandSendWatch.Restart();
                }
            }
            catch
            {
            }
        }

        public bool ReadTemperature()
        {
            try
            {
                byte[] command = { STX, 0, 0, 0x52, 0x58, 0x50, 0x30, ETX, 0 };

                command[1] = System.Text.ASCIIEncoding.ASCII.GetBytes((Address >> 8).ToString())[0];
                command[2] = System.Text.ASCIIEncoding.ASCII.GetBytes((Address & 0xFF).ToString())[0];

                command[3] = (byte)(RX >> 8);
                command[4] = (byte)(RX & 0xFF);
                command[5] = 0x50;
                command[6] = 0x30;
                command[7] = ETX;
                command[8] = GetBCC(command, 7);

                _sharedPorts.AddCommand(Port.PortName, command);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool ReadTargetTemperature()
        {
            try
            {
                byte[] command = { STX, 0, 0, 0x52, 0x58, 0x53, 0x30, ETX, 0 };

                command[1] = System.Text.ASCIIEncoding.ASCII.GetBytes((Address >> 8).ToString())[0];
                command[2] = System.Text.ASCIIEncoding.ASCII.GetBytes((Address & 0xFF).ToString())[0];

                command[3] = (byte)(RX >> 8);
                command[4] = (byte)(RX & 0xFF);
                command[5] = 0x53;
                command[6] = 0x30;
                command[7] = ETX;
                command[8] = GetBCC(command, 7);

                _sharedPorts.AddCommand(Port.PortName, command);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool WriteTemperature(double temp)
        {
            try
            {
                byte[] command = { STX, 0, 0, 0x57, 0x58, 0x53, 0x30, 0x20, 0, 0, 0, 0, ETX, 0 };

                command[1] = System.Text.ASCIIEncoding.ASCII.GetBytes((Address >> 8).ToString())[0];
                command[2] = System.Text.ASCIIEncoding.ASCII.GetBytes((Address & 0xFF).ToString())[0];

                command[3] = (byte)(WX >> 8);
                command[4] = (byte)(WX & 0xFF);

                if (temp < 0)
                    command[7] = 0x2D;

                double absTemp = Math.Abs(temp);

                command[8] = (byte)(absTemp / 1000);
                command[9] = (byte)((absTemp % 1000) / 100);
                command[10] = (byte)((absTemp % 100) / 10);
                command[11] = (byte)(absTemp % 10);
                command[12] = ETX;
                command[13] = GetBCC(command, 12);

                _sharedPorts.AddCommand(Port.PortName, command);
            }
            catch
            {
                return false;
            }

            return true;
        }

        protected bool GetAddress(byte[] arr, out ushort address)
        {
            address = 0;
            if (ushort.TryParse(Encoding.ASCII.GetString(arr, 2, 2), out address))
            {
                return true;
            }

            return false;
        }

        protected ushort GetCommand(byte[] arr)
        {
            ushort commandHead = (ushort)(arr[4]);
            return (ushort)((ushort)(commandHead << 8) | (ushort)(arr[5]));
        }

        protected ushort GetCommandType(byte[] arr)
        {
            ushort commandHead = (ushort)(arr[6]);
            return (ushort)((ushort)(commandHead << 8) | (ushort)(arr[7]));
        }

        protected double GetTemperature(byte[] dataBlock)
        {
            double temperature = 0;

            try
            {
                byte byte1 = dataBlock[9];
                byte byte2 = dataBlock[10];
                byte byte3 = dataBlock[11];
                byte byte4 = dataBlock[12];
                byte dot = dataBlock[13];

                string str = (byte1 - 48).ToString() + (byte2 - 48).ToString() +
                    (byte3 - 48).ToString() + (byte4 - 48).ToString();

                temperature = double.Parse(str);

                if (dot == 0x20)
                    temperature = temperature / 10;

                if (dataBlock[8] == 0x2D)
                    temperature = -temperature;
            }
            catch
            {
            }

            return temperature;
        }

        protected virtual void OnDataReceived(byte[] buffer)
        {
            LastReadTime = DateTime.Now;

            try
            {
                _readData.AddRange(buffer);
                int ackPos = _readData.IndexOf(0x06);
                if (ackPos < 0) return;

                int stxPos = _readData.IndexOf(STX, ackPos);
                if (stxPos < 0) return;

                int etxPos = _readData.IndexOf(ETX, stxPos);
                if (etxPos < 0) return;

                for (int i = _readData.Count - 1; i == 0; i--)
                {
                    if (_readData[i] == 0)
                        _readData.RemoveAt(i);
                }

                int count = etxPos - ackPos + 1;

                if (count != 15)
                {
                    Port.DiscardInBuffer();
                    return;
                }

                byte[] dataBlock = new byte[count];
                _readData.CopyTo(ackPos, dataBlock, 0, count);

                if (dataBlock.Length < 3)
                    return;

                byte bcc = GetBCC(dataBlock, dataBlock.Length - 1);
                ushort address;
                var addressResult = GetAddress(dataBlock, out address);
                if (addressResult && address == Address)
                {
                    var command = GetCommand(dataBlock);
                    var commandType = GetCommandType(dataBlock);

                    if (command == RD)
                    {
                        if (commandType == PO)
                            CurrentTemperature = GetTemperature(dataBlock);
                        else if (commandType == SO)
                            TargetTemperature = GetTemperature(dataBlock);
                    }
                    else if (command == WD)
                    {
                        if (commandType == SO)
                            TargetTemperature = GetTemperature(dataBlock);
                    }
                }

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

        protected byte GetBCC(byte[] arr, int lastIndex)
        {
            try
            {
                if (arr == null) return 0;
                byte result = arr[0];
                for (int i = 1; i <= lastIndex; i++)
                {
                    result = (byte)(result ^ arr[i]);
                }

                return result;
            }
            catch
            {
                return 0;
            }
        }
    }
}
