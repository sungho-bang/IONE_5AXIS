using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace FALibrary.Device.LanDevice
{
    public class FASocketDevice : FADevice
    {
        private Thread ClientThread { get; set; }
        protected TcpClient ClientSocket { get; set; }
        protected Encoding StreamEncoding { get; set; }

        public string IPAddress { get; set; }
        public int Port { get; set; }
        public int BufferSize { get; set; }

        public event EventHandler OnConnect = null;
        public event EventHandler OnConnected = null;
        public event EventHandler OnDisconnect = null;
        public event EventHandler OnDisconnected = null;
        //public event EventHandler<EventArgs> OnReceiveData = null;

        public FASocketDevice()
        {
            BufferSize = 8192;
            IsOpened = false;
            StreamEncoding = Encoding.ASCII;            
        }

        public override void Close()
        {
            if (ClientSocket != null)
            {
                ClientSocket.Close();
                Disconnect(this, EventArgs.Empty);
            }
        }

        protected void StartReceiveProcedure()
        {
            ClientThread = new Thread(new ThreadStart(ClientReceive));
            ClientThread.IsBackground = true;
            ClientThread.Start();
        }

        protected virtual void Connect(object sender, EventArgs e)
        {
            if (OnConnect != null) OnConnect(sender, e);
        }

        protected virtual void Connected(object sender, EventArgs e)
        {
            if (OnConnected != null) OnConnected(sender, e);
        }

        protected virtual void Disconnect(object sender, EventArgs e)
        {
            if (OnDisconnect != null) OnDisconnect(sender, e);
        }

        protected virtual void Disconnected(object sender, EventArgs e)
        {
            if (OnDisconnected != null) OnDisconnected(sender, e);
        }

        protected virtual void ReceiveData(object sender, EventArgs e)
        {
        }

        protected virtual void ClientReceive()
        {
        }

        protected IPEndPoint CreateSocket()
        {               
            ClientSocket = new TcpClient();
            ClientSocket.ReceiveBufferSize = BufferSize;

            System.Net.IPAddress ip;
            if (System.Net.IPAddress.TryParse(IPAddress, out ip) == false)
                throw new Exception("Incorrect IPAddress. " + "Device Name : " + Name + ", IPAddress : " + IPAddress);

            IPEndPoint endPoint = new IPEndPoint(ip, Port);

            return endPoint;
        }
    }
}
