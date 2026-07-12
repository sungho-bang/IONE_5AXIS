using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Net;
using System.Net.Sockets;

namespace FALibrary.Device.RFIDReader
{
    public class FACRE1356Device : RS232Device.FACommonSerialPortDevice
    {
        private interface IPortInterface
        {
            event EventHandler<FAGenericEventArgs<byte[]>> OnRead;
            void OpenPort();
            void ClosePort();
            void WriteData(byte[] buffer);
            void LoadParameters(XElement xml);
        }

        private class LanPortInterface : FADevice, IPortInterface
        {
            private bool _running;

            public event EventHandler<FAGenericEventArgs<byte[]>> OnRead;
            public string IPAddress { get; set; }
            public int Port { get; set; }
            /// <summary>
            /// millisecond
            /// </summary>
            public int ConnectTimeout { get; set; }

            /// <summary>
            /// millisecond
            /// </summary>
            public int SendTimeout { get; set; }

            /// <summary>
            /// millisecond
            /// </summary>
            public int ReceiveTimeout { get; set; }

            public LanPortInterface()
            {
            }

            public void OpenPort()
            {
                Open();
            }

            public void ClosePort()
            {
                Close();
            }

            public void WriteData(byte[] buffer)
            {
                try
                {
                    if (_running)
                        return;

                    var thread = new System.Threading.Thread(
                        new System.Threading.ParameterizedThreadStart(Process));

                    _running = true;
                    thread.Start(buffer);
                }
                catch
                {
                }
            }

            private void Process(object obj)
            {
                TcpClient client = null;
                try
                {
                    using (client = new TcpClient())
                    {
                        client.SendTimeout = SendTimeout;
                        client.ReceiveTimeout = ReceiveTimeout;
                        if (!Connect(client))
                            throw new TimeoutException();

                        var data = obj as byte[];
                        client.GetStream().Write(data, 0, data.Length);

                        var buffer = new byte[255];
                        int numberOfRead = client.GetStream().Read(buffer, 0, buffer.Length);
                        var readedData = new byte[numberOfRead];
                        Array.Copy(buffer, readedData, numberOfRead);
                        OnRead(this, new FAGenericEventArgs<byte[]>(readedData));
                    }
                }
                catch
                {
                    if (client != null && client.Connected)
                        client.Close();
                }
                finally
                {
                    _running = false;
                }
            }

            private bool Connect(TcpClient client)
            {
                bool result = false;
                IAsyncResult ar = client.BeginConnect(System.Net.IPAddress.Parse(IPAddress), Port, null, null);
                System.Threading.WaitHandle wh = ar.AsyncWaitHandle;

                try
                {
                    if (!ar.AsyncWaitHandle.WaitOne(ConnectTimeout, false))
                    {
                        throw new TimeoutException();
                    }

                    client.EndConnect(ar);
                    result = true;
                }
                catch
                {
                    result = false;
                }
                finally
                {
                    wh.Close();
                }

                return result;
            }
        }

        private readonly byte STX = 0x02;
        private readonly byte ETX = 0x03;
        private readonly byte ENQ = 0x05;
        private readonly byte TAG_READ_COMMAND = 0x08;
        private readonly byte TAG_WRITE_COMMAND = 0x09;

        private List<byte> _readData = new List<byte>();

        private byte TagReadCommand
        {
            get
            {
                return NumericToByte(TAG_READ_COMMAND);
            }
        }

        private byte TagWriteCommand
        {
            get
            {
                return NumericToByte(TAG_WRITE_COMMAND);
            }
        }

        public enum Protocol
        {
            p13, p13s
        }

        public byte DeviceID { get; set; }
        public Protocol ProtocolVersion { get; set; }
        public string Channel1Tag
        {
            get;
            private set;
        }
        public string PortType { get; set; }

        private IPortInterface CommunicationPort { get; set; }

        public FACRE1356Device()
        {
            DataReceived += EventHandlerDataReceived;
        }

        public override void Open()
        {
            if (!string.IsNullOrEmpty(PortType) && PortType.ToUpper() == "LAN")
            {
                CommunicationPort.OpenPort();
                CommunicationPort.OnRead += EventHandlerDataReceived;
            }
            else
                base.Open();
        }

        public override void Close()
        {
            if (!string.IsNullOrEmpty(PortType) && PortType.ToUpper() == "LAN")
                CommunicationPort.ClosePort();
            else
                base.Close();
        }

        public override void LoadParameters(XElement xml)
        {
            base.LoadParameters(xml);
            if (!string.IsNullOrEmpty(PortType) && PortType.ToUpper() == "LAN")
            {
                CommunicationPort = new LanPortInterface();
                CommunicationPort.LoadParameters(xml.Element("CommunicationPort"));
            }
        }

        public void ReadTag(byte channel, byte address, byte length)
        {
            Channel1Tag = "";
            _readData.Clear();
            List<byte> buffer = GetCommandHeader();
            buffer.AddRange(GetCommand(TagReadCommand, channel));
            buffer.AddRange(DataToBytes(address));
            buffer.AddRange(DataToBytes(length));
            byte[] command = buffer.ToArray();

            AddCRC(ref command);

            Send(command);
        }

        public void WriteTag(byte channel, byte address, byte[] data)
        {
            Channel1Tag = "";
            _readData.Clear();
            List<byte> buffer = GetCommandHeader();
            buffer.AddRange(GetCommand(TagWriteCommand, channel));
            buffer.AddRange(DataToBytes(address));
            buffer.AddRange(DataToBytes((byte)data.Length));
            buffer.AddRange(data);
            byte[] command = buffer.ToArray();

            AddCRC(ref command);

            Send(command);
        }

        private void Send(byte[] data)
        {
            if (string.IsNullOrEmpty(PortType))
            {
                SendData(data, 0, data.Length);
            }
            else
            {
                if (PortType.ToUpper() == "LAN")
                {
                    CommunicationPort.WriteData(data);
                }
            }
        }

        private void EventHandlerDataReceived(object sender, FAGenericEventArgs<byte[]> e)
        {
            try
            {
                _readData.AddRange(e.Value);

                int stxPos = _readData.IndexOf(STX);
                if (stxPos < 0) return;

                int etxPos = _readData.IndexOf(ETX, stxPos);
                if (stxPos < 0) return;

                int dataSize = etxPos - stxPos + 1;
                byte[] stxToEtxData = new byte[dataSize];
                _readData.CopyTo(stxPos, stxToEtxData, 0, dataSize);
                byte[] dataBlock = GetDataBlock(stxToEtxData);
                if (ProtocolVersion == Protocol.p13)
                    Channel1Tag = BytesToAsciiString(dataBlock);
                else
                    Channel1Tag = Encoding.ASCII.GetString(dataBlock);
            }
            catch
            {
            }
        }

        private byte[] GetDataBlock(byte[] data)
        {
            try
            {
                if (ProtocolVersion == Protocol.p13)
                {
                    int length = data.Length - 6;
                    byte[] result = new byte[length];
                    Array.Copy(data, 5, result, 0, length);
                    return result;
                }
                else
                {
                    int length = data.Length - 4;
                    byte[] result = new byte[length];
                    Array.Copy(data, 3, result, 0, length);
                    return result;
                }
            }
            catch
            {
                return null;
            }
        }

        private List<byte> GetCommandHeader()
        {
            List<byte> buffer = new List<byte>();
            buffer.Add(ENQ);
            buffer.AddRange(DataToBytes(DeviceID));

            return buffer;
        }

        private byte[] DataToBytes(byte data)
        {
            byte[] result;

            switch (ProtocolVersion)
            {
                case Protocol.p13:
                    result = new byte[2];
                    result[0] = GetHighHex(data);
                    result[1] = GetLowHex(data);
                    break;

                default:
                    result = new byte[1];
                    result[0] = data;
                    break;
            }

            return result;
        }

        private byte NumericToByte(byte value)
        {
            switch (ProtocolVersion)
            {
                case Protocol.p13:
                    return NumericToAscii(value);

                case Protocol.p13s:
                    return value;

                default:
                    return NumericToAscii(value);
            }
        }

        private void AddCRC(ref byte[] data)
        {
            byte crc = ComputeAdditionChecksum(data);

            if (ProtocolVersion == Protocol.p13)
            {
                Array.Resize<byte>(ref data, data.Length + 2);
                data[data.Length - 2] = GetHighHex(crc);
                data[data.Length - 1] = GetLowHex(crc);
            }
            else
            {
                Array.Resize<byte>(ref data, data.Length + 1);
                data[data.Length - 1] = crc;
            }
        }

        private byte ComputeAdditionChecksum(byte[] data)
        {
            byte sum = 0;
            unchecked // Let overflow occur without exceptions
            {
                foreach (byte b in data)
                {
                    sum += b;
                }
            }
            return sum;
        }

        private byte GetHighHex(byte value)
        {
            string hex = ByteToHexString(value);

            byte[] bytes = Encoding.ASCII.GetBytes(hex);
            return bytes[0];
        }

        public byte GetLowHex(byte value)
        {
            string hex = ByteToHexString(value);

            byte[] bytes = Encoding.ASCII.GetBytes(hex);
            return bytes[1];
        }

        private string ByteToHexString(byte value)
        {
            string hexOutput = String.Format("{0:X}", value);
            if (hexOutput.Length == 1)
                hexOutput = "0" + hexOutput;

            return hexOutput;
        }

        private byte NumericToAscii(byte value)
        {
            byte asciiZeroIndex = 0x30;
            return (byte)(value + asciiZeroIndex);
        }

        private string BytesToAsciiString(byte[] data)
        {
            try
            {
                StringBuilder hexArray = new StringBuilder();
                int length = data.Length;
                if (length % 2 == 1)
                    length = length - 1;

                for (int i = 0; i < length; i += 2)
                {
                    byte highByte = Convert.ToByte(data[i]);
                    byte lowByte = Convert.ToByte(data[i + 1]);
                    string hex = "";
                    hex += Convert.ToChar(highByte);
                    hex += Convert.ToChar(lowByte);
                    hexArray.Append(hex);
                }

                return System.Text.ASCIIEncoding.ASCII.GetString(StringToByteArray(hexArray.ToString()));
            }
            catch
            {
                return "";
            }
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        private byte[] GetCommand(byte commandCode, byte channel)
        {
            byte[] buffer = null;

            if (ProtocolVersion == Protocol.p13)
            {
                buffer = new byte[2];
                buffer[0] = commandCode;
                buffer[1] = NumericToByte(channel);
            }
            else
            {
                buffer = new byte[1];
                buffer[0] = (byte)((commandCode * (byte)16) + channel);
            }

            return buffer;
        }        
    }
}
