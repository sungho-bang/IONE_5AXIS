using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FALibrary.Device.LanDevice
{
    public class ReceiveEventArgs : EventArgs
    {
        public IPEndPoint IPEndPoint { get; private set; }
        public byte[] Bytes { get; private set; }

        public ReceiveEventArgs(IPEndPoint ipEndPoint, byte[] bytes)
        {
            IPEndPoint = ipEndPoint;
            Bytes = bytes;
        }
    }

    public class FAUDPServerDevice : FADevice
    {
        private UdpClient Socket { get; set; }
        private Thread ClientThread { get; set; }

        public string IPAddress { get; set; }
        public int Port { get; set; }
        public bool Simulation { get; set; }

        public event EventHandler Disconnected = null;
        public event EventHandler<ReceiveEventArgs> OnReceiveData = delegate { };

        public override void Open()
        {
            if (Simulation) return;

            try
            {
                Socket = new UdpClient(Port);
            }
            catch (Exception e)
            {
                new Exception("Device " + Name + " can not open. exception =>" + e.ToString());
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

                ClientThread = new Thread(
                    new ThreadStart(
                        () =>
                            {
                                IPEndPoint remoteEP = new IPEndPoint(System.Net.IPAddress.Any, 0);

                                try
                                {
                                    while (true)
                                    {
                                        var bytes = Socket.Receive(ref remoteEP);
                                        if (OnReceiveData != null)
                                            OnReceiveData(this, new ReceiveEventArgs(remoteEP, bytes));
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
                            }));

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
        }

        public void SendData(string remoteIP, int remotePort, byte[] data)
        {
            if (Simulation) return;            

            IPEndPoint remoteEndPoint = null;
            
            try
            {
                remoteEndPoint = new IPEndPoint(System.Net.IPAddress.Parse(remoteIP), remotePort);
            }
            catch (Exception e)
            {
                Utility.Trace.WriteLine(this, "Device " + Name + " SendData Fail", e.ToString());
                return;
            }

            try
            {
                if (Socket != null)
                    Socket.Send(data, data.Length, remoteEndPoint);
                else
                    Open();
            }
            catch (ObjectDisposedException e)
            {
                Close();
                Open();
                Socket.Send(data, data.Length, remoteEndPoint);
                Utility.Trace.WriteLine(this, "Device", e.ToString());
            }
        }
    }
}
