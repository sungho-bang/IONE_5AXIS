using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace FALibrary.Device.Autonics
{
    public sealed class FADeviceDA16Segment : RS232Device.FASerialPortDevice, AbstractDevice.FADeviceDisplayUnit
    {
        class SharedPorts
        {
            private Dictionary<string, SerialPort> _ports = new Dictionary<string,SerialPort>();

            public void AddPort(string portName, SerialPort port)
            {
                if (this.Constains(portName))
                {
                    Utility.Trace.WriteLine(this, "Device", string.Format("Aready exist port[{0}]", portName));
                    return;
                }

                _ports.Add(portName, port);
            }

            public bool Constains(string portName)
            {
                return _ports.ContainsKey(portName);
            }

            public SerialPort GetPort(string portName)
            {
                if (this.Constains(portName) == false) return null;
                return _ports[portName];
            }
        }

        static SharedPorts _sharedPorts = new SharedPorts();

        private Dictionary<string, byte> _characterSetDic = new Dictionary<string, byte>();

        public int DisplayCount { get; private set; }
        public bool Slave { get; private set; }
        public string DisplayString { get; private set; }
        
        public FADeviceDA16Segment()
        {
            InitializeCharacterSet();
        }

        public override void Open()
        {
            if (Slave == false)
                Port.Open();
            else
                Port.DataReceived += DataReceivedEventHandler;
        }

        public override void Close()
        {
            if (Slave == false)
                Port.Close();
        }

        public void SetString(string msg)
        {
            List<byte> query = new List<byte>();

            var data = GetData(msg.ToUpper());
            if (data == null) return;

            var dataLength = (data.Length / 2) + (data.Length % 2) + 1; // +1 is Zero Blanking ON
            query.Add((byte)Address); // Slave Address
            query.Add(0x10); // Function
            query.Add(0x00); // Starting Address Hi
            query.Add(0x00); // Starting Address Lo
            query.Add(0x00); // No. of Register Hi
            query.Add((byte)dataLength); // No. of Register Lo
            query.Add((byte)(dataLength * 2)); // Byte Counter. Register is 2Byte
            query.Add(0x00);
            query.Add(0x01);
            query.AddRange(data);

            var crc = Utility.Crc16.ComputeChecksumBytes(query.ToArray());
            query.AddRange(crc);

            try
            {
                Port.Write(query.ToArray(), 0, query.Count);
            }
            catch (Exception e)
            {
                Utility.Trace.WriteLine(this, "Device", e.ToString());
            }
        }

        public void Clear()
        {
            SetString(new string(' ', DisplayCount));
        }

        private byte[] GetData(string str)
        {
            if (string.IsNullOrEmpty(str)) return null;

            List<byte> data = new List<byte>();

            foreach (var item in str)
            {
                if (_characterSetDic.ContainsKey(item.ToString()))
                    data.Add(_characterSetDic[item.ToString()]);
                else
                    data.Add(_characterSetDic[" "]);
            }

            for (int i = 0; i < DisplayCount - str.Length; i++)
            {
                data.Add(_characterSetDic[" "]);
            }

            return data.ToArray();
        }

        private void DiscardInBuffer()
        {
            Port.DiscardInBuffer();
        }

        private void DataReceivedEventHandler(object sender, SerialDataReceivedEventArgs e)
        {
            int len = Port.BytesToRead;
            byte[] buffer;

            try
            {
                buffer = new byte[len];
                Port.Read(buffer, 0, len);
            }
            catch
            {
                Port.DiscardInBuffer();
            }
        }

        private void InitializeCharacterSet()
        {
            AddCharacter("0");
            AddCharacter("1");
            AddCharacter("2");
            AddCharacter("3");
            AddCharacter("4");
            AddCharacter("5");
            AddCharacter("6");
            AddCharacter("7");
            AddCharacter("8");
            AddCharacter("9");
            AddCharacter("A");
            AddCharacter("B");
            AddCharacter("C");
            AddCharacter("D");
            AddCharacter("E");
            AddCharacter("F");

            AddCharacter("G");
            AddCharacter("H");
            AddCharacter("I");
            AddCharacter("J");
            AddCharacter("K");
            AddCharacter("L");
            AddCharacter("M");
            AddCharacter("N");
            AddCharacter("O");
            AddCharacter("P");
            AddCharacter("Q");
            AddCharacter("R");
            AddCharacter("S");
            AddCharacter("T");
            AddCharacter("U");
            AddCharacter("V");

            AddCharacter("W");
            AddCharacter("X");
            AddCharacter("Y");
            AddCharacter("Z");
            AddCharacter("-1");
            AddCharacter("(");
            AddCharacter(")");
            AddCharacter("'");
            AddCharacter("\"");
            AddCharacter("^");
            AddCharacter(".");
            AddCharacter("/");
            AddCharacter("?");
            AddCharacter("-");
            AddCharacter("_");
            AddCharacter("=");

            AddCharacter("]");
            AddCharacter("[");
            AddCharacter("+");
            AddCharacter(":");
            AddCharacter(";");
            AddCharacter("<");
            AddCharacter(">");
            AddCharacter("|");
            AddCharacter("!");
            AddCharacter("@");
            AddCharacter("#");
            AddCharacter("$");
            AddCharacter("%");
            AddCharacter("&");
            AddCharacter("*");
            AddCharacter(" ");

            for (byte i = 0; i < _characterSetDic.Count; i++)
            {
                var key = _characterSetDic.ElementAt(i).Key;
                _characterSetDic[key] = i;
            }
        }

        private void AddCharacter(string chr, byte value = 0)
        {
            _characterSetDic.Add(chr, value);
        }

        public override void LoadParameters(System.Xml.Linq.XElement xml)
        {
            base.LoadParameters(xml);

            if (Slave)
                Port = _sharedPorts.GetPort(Port.PortName);
            else
                _sharedPorts.AddPort(Port.PortName, Port);
        }
    }
}
