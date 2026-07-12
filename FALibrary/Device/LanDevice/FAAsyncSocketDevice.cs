using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace FALibrary.Device.LanDevice
{
    public class FAAsyncSocketDevice : FADevice
    {
        private List<byte> _readData = new List<byte>();
        protected TcpClient ClientSocket { get; set; }
        public Encoding StreamEncoding { get; protected set; }
        public string IPAddress { get; set; }
        public int Port { get; set; }
        public int EncodingCodePage { get; set; }        
        public bool UseEOFString { get; set; }
        public string EOFString { get; set; }

        public event EventHandler OnWrite;
        public event EventHandler<FAGenericEventArgs<byte[]>> OnRead;

        public int SendTimeout { get; set; }
        public int ReceiveTimeout { get; set; }
        public bool UseReceiveDataSize { get; set; }
        public int ReceiveDataSize { get; set; }
        public bool Simulation { get; set; }

        public FAAsyncSocketDevice()
        {
            UseReceiveDataSize = false;
            SendTimeout = 5000;
            ReceiveTimeout = 5000;
            EncodingCodePage = Encoding.ASCII.CodePage;
            StreamEncoding = Encoding.ASCII;
        }

        public override void Open()
        {
            if (Simulation) return;

            try
            {
                ClientSocket = new TcpClient();
                StreamEncoding = Encoding.GetEncoding(EncodingCodePage);
            }
            catch
            {
            }
        }

        public override void Close()
        {
            if (Simulation) return;

            if (ClientSocket != null)
            {
                try
                {
                    ClientSocket.Close();
                }
                catch
                {
                }
            }
        }

        public bool IsConnected
        {
            get
            {
                if (ClientSocket == null)
                    return false;

                try
                {
                    bool result = ClientSocket.Connected;
                    return result;
                }
                catch
                {
                    return false;
                }                
            }
        }

        public void Connect()
        {
            if (Simulation) return;

            Connect(IPAddress, Port);
        }

        public void Connect(string ipAddress, int port)
        {
            if (Simulation) return;

            try
            {
                if (ClientSocket != null &&
                    ClientSocket.Connected) return;
                
                ClientSocket = null;
                ClientSocket = new TcpClient();
                ClientSocket.ReceiveTimeout = SendTimeout;
                ClientSocket.SendTimeout = ReceiveTimeout;

                _readData.Clear();
                System.Net.IPAddress ip;
                if (System.Net.IPAddress.TryParse(ipAddress, out ip) == false)
                    throw new Exception("Incorrect IPAddress. " + "Device Name : " + Name + ", IPAddress : " + ipAddress);
                ClientSocket.BeginConnect(ip, port, ConnectCallback, null);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void WriteString(string data)
        {
            if (Simulation) return;

            try
            {
                Write(StreamEncoding.GetBytes(data));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Write(byte[] bytes)
        {
            if (Simulation) return;

            try
            {
                NetworkStream ns = ClientSocket.GetStream();
                ns.BeginWrite(bytes, 0, bytes.Length, WriteCallback, null);
            }
            catch
            {
            }
        }

        private void ConnectCallback(IAsyncResult result)
        {
            if (Simulation) return;

            try
            {
                ClientSocket.EndConnect(result);
            }
            catch
            {
                return;
            }

            try
            {
                OnWrite(this, EventArgs.Empty);
                NetworkStream ns = ClientSocket.GetStream();
                byte[] buffer = new byte[ClientSocket.ReceiveBufferSize];
                ns.BeginRead(buffer, 0, buffer.Length, ReadCallback, buffer);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private void ReadCallback(IAsyncResult result)
        {
            if (Simulation) return;

            int read;
            NetworkStream ns;
            try
            {
                ns = ClientSocket.GetStream();
                read = ns.EndRead(result);
            }
            catch
            {
                if (ClientSocket != null)
                {
                    ClientSocket.Close();
                }

                ClientSocket = null;
                return;
            }

            if (read <= 0)
            {
                OnRead(this, new FAGenericEventArgs<byte[]>(_readData.ToArray<byte>()));

                if (ClientSocket != null)
                {
                    ClientSocket.Close();
                }

                ClientSocket = null;
                return;
            }

            try
            {
                byte[] buffer = result.AsyncState as byte[];
                byte[] data = new byte[read];
                Array.Copy(buffer, data, read);
                _readData.AddRange(data);

                bool isEOF = false;

                if (UseReceiveDataSize && _readData.Count >= ReceiveDataSize)
                    isEOF = true;
                else if (UseEOFString)
                {
                    string temp = StreamEncoding.GetString(_readData.ToArray());
                    int eofIndex = temp.IndexOf(EOFString);
                    if (eofIndex >= 0)
                    {
                        _readData.Clear();
                        _readData.AddRange(StreamEncoding.GetBytes(temp.Substring(0, eofIndex)));
                        isEOF = true;
                    }
                }

                if (isEOF)
                {
                    OnRead(this, new FAGenericEventArgs<byte[]>(_readData.ToArray<byte>()));

                    if (ClientSocket != null)
                    {
                        ClientSocket.Close();
                    }

                    ClientSocket = null;
                    return;
                }
                else if (UseReceiveDataSize == false && ClientSocket.Connected == false)
                {
                    OnRead(this, new FAGenericEventArgs<byte[]>(_readData.ToArray<byte>()));

                    if (ClientSocket != null)
                    {
                        ClientSocket.Close();
                    }

                    ClientSocket = null;
                    return;
                }
                else
                {
                    buffer = new byte[ClientSocket.ReceiveBufferSize];
                    ns.BeginRead(buffer, 0, buffer.Length, ReadCallback, buffer);
                }
            }
            catch
            {
                if (ClientSocket != null)
                {
                    ClientSocket.Close();
                }

                ClientSocket = null;
            }
        }

        private void WriteCallback(IAsyncResult result)
        {
            if (Simulation) return;

            try
            {
                NetworkStream ns = ClientSocket.GetStream();
                ns.EndWrite(result);
            }
            catch
            {
            }
        }
    }
}
