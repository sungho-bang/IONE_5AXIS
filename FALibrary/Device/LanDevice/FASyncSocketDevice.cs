using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Net;

namespace FALibrary.Device.LanDevice
{
    public class FASyncSocketDevice : FASocketDevice
    {
        private NetworkStream _netStream = null;
        protected StreamReader StmReader { get; private set; }
        protected StreamWriter StmWriter { get; private set; }

        public override void Open()
        {
            try
            {
                IPEndPoint endPoint = CreateSocket();
                ClientSocket.Connect(endPoint);
                Connect(this, EventArgs.Empty);
                StartReceiveProcedure();
            }
            catch
            {
            }
        }

        protected override void ClientReceive()
        {
            try
            {                
                if (ClientSocket.Connected)
                {                    
                    _netStream = ClientSocket.GetStream();
                    StmReader = new StreamReader(_netStream, StreamEncoding);
                    StmWriter = new StreamWriter(_netStream, StreamEncoding);
                    Connected(this, EventArgs.Empty);

                    while (true)
                    {
                        ReceiveData(this, EventArgs.Empty);
                    }
                }
                else return;
            }
            catch
            {                
            }
            finally
            {
                StmReader.Close();
                StmWriter.Close();
                _netStream.Close();
                ClientSocket.Close();
                Disconnected(this, EventArgs.Empty);
            }
        }
    }
}
