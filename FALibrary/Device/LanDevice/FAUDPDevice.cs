using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace FALibrary.Device.LanDevice
{
    public class FAUDPDevice : FADevice
    {
        private IPEndPoint _remoteEP;

        private Thread ClientThread { get; set; }
        protected UdpClient Socket { get; set; }
        protected IPEndPoint RemoteEP
        {
            get { return _remoteEP; }
            set
            {
                _remoteEP = value;
            }
        }

        public string IPAddress { get; set; }
        public int Port { get; set; }
        public int RemotePort { get; set; }
        public int EncodingCodePage { get; set; }
        public Encoding StreamEncoding { get; protected set; }
        public bool Simulation { get; set; }
        public event EventHandler Disconnected = null;
        public event EventHandler<FAGenericEventArgs<byte[]>> OnReceiveData = delegate { };

        public FAUDPDevice()
        {
            EncodingCodePage = Encoding.Default.CodePage;
        }

        public override void LoadParameters(System.Xml.Linq.XElement xml)
        {
            base.LoadParameters(xml);

            StreamEncoding = Encoding.GetEncoding(EncodingCodePage);
            RemoteEP = CreateIPEndPoint(IPAddress, RemotePort);
        }

        public override void Open()
        {
            if (Simulation) return;

            StreamEncoding = Encoding.GetEncoding(EncodingCodePage);
            RemoteEP = CreateIPEndPoint(IPAddress, RemotePort);

            try
            {                
                Socket = new UdpClient();
            }
            catch
            {
                return;
            }

            try
            {
                if (ClientThread != null)
                {
                    try
                    {
                        ClientThread.Abort();
                    }
                    catch
                    {
                    }
                }

                ClientThread = new Thread(new ThreadStart(Receive));
                ClientThread.IsBackground = true;
                ClientThread.Start();
            }
            catch
            {
            }
        }

        public override void Close()
        {
            if (Simulation) return;

            if (ClientThread != null)
                ClientThread.Abort();

            if (Socket != null)
                Socket.Close();            
        }

        protected IPEndPoint CreateIPEndPoint(string ipAddress, int port)
        {            
            System.Net.IPAddress ip;
            if (System.Net.IPAddress.TryParse(ipAddress, out ip) == false)
                throw new Exception("Incorrect IPAddress. " + "Device Name : " + Name + ", IPAddress : " + IPAddress);

            IPEndPoint endPoint = new IPEndPoint(ip, port);

            return endPoint;
        }

        protected virtual void ReceiveData(byte[] data)
        {
            OnReceiveData(this, new FAGenericEventArgs<byte[]>(data));
        }

        private void Receive()
        {
            try
            {
                while (true)
                {
                    ReceiveData(Socket.Receive(ref _remoteEP));
                }
            }
            catch
            {
            }
            finally
            {
                Socket.Close();
                if (Disconnected != null)
                    Disconnected(this, EventArgs.Empty);
            }
        }

        public void SendData(string remoteIP, int remotePort, byte[] data)
        {
            if (Simulation) return;

            if (RemoteEP.Address.ToString() != remoteIP ||
                RemoteEP.Port != remotePort)
            {
                IPAddress = remoteIP;
                RemotePort = remotePort;
                RemoteEP = CreateIPEndPoint(IPAddress, RemotePort);
            }

            try
            {
                if (Socket != null)
                    Socket.Send(data, data.Length, RemoteEP);
                else
                    Open();
            }
            catch (ObjectDisposedException e)
            {
                Close();
                Open();
                Socket.Send(data, data.Length, RemoteEP);
                Utility.Trace.WriteLine(this, "Device", e.ToString());
            }
        }

        public void SendData(string remoteIP, int remotePort, string data)
        {
            if (Simulation) return;

            try
            {
                SendData(remoteIP, remotePort, StreamEncoding.GetBytes(data));
            }
            catch (ObjectDisposedException e)
            {
                Close();
                Open();
                Utility.Trace.WriteLine(this, "Device", e.ToString());
            }
            catch (Exception e)
            {
                Utility.Trace.WriteLine(this, "Device", e.ToString());
            }
        }
    }
}
