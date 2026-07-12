using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace FALibrary.Device.LanDevice
{
    public class FATelnetClientDevice : FADevice
    {
        public class ConnectionState
        {
            public enum EState
            {
                NONE,
                CONNECTED,
                DISCONNECTED
            }

            public EState State { get; set; }
            public string Message { get; set; }
        }

        private class ThreadProcessParameter
        {
            public string UserName { get; set; }
            public string Password { get; set; }
            public ConnectionState ConnectionState { get; set; }
        }

        private static object _syncRoot = new Object();
        Socket _tcpClient;
        bool _threadStopRequest = false;

        private Encoding _streamEncoding = Encoding.ASCII;
        private Queue<string> _sendDataQueue = null;

        protected bool ThreadStopRequest
        {
            get
            {
                return _threadStopRequest;
            }
        }

        public bool AutoReconnection { get; set; }
        public Action<string> ReceiveDataDelegate { get; set; }

        public event EventHandler<EventArgs> OnConnected = delegate { };
        public event EventHandler<FAGenericEventArgs<string>> OnDiconnected = delegate { };

        public FATelnetClientDevice()
        {
            AutoReconnection = true;
        }

        public bool Open(string ip, int port, string userName, string password, out ConnectionState connectionState, out string failMsg)
        {
            var newConnectionState = new ConnectionState();
            connectionState = newConnectionState;
            
            failMsg = string.Empty;
            _tcpClient = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);
            _tcpClient.ReceiveTimeout = 1;
            IPAddress ipAddress;
            if (IPAddress.TryParse(ip, out ipAddress))
            {
                try
                {
                    _tcpClient.Connect(ipAddress, port);
                }
                catch (Exception e)
                {
                    failMsg = e.Message;
                    return false;
                }

                System.Threading.Tasks.Task.Factory.StartNew(
                    delegate
                    {
                        ThreadProcess(new ThreadProcessParameter
                        {
                            UserName = userName,
                            Password = password,
                            ConnectionState = newConnectionState
                        });
                    });                

                return true;
            }
            else
            {
                failMsg = string.Format("Invalid IP Address {0}", ip);
                return false;
            }
        }

        public override void Close()
        {
            _threadStopRequest = true;
            _tcpClient.Close();
            OnDiconnected(this, new FAGenericEventArgs<string>("Call Close Method"));
        }

        public void SendData(string data)
        {
            PushSendData(data);
        }

        private void ThreadProcess(ThreadProcessParameter param)
        {
            lock (_syncRoot)
            {
                _sendDataQueue = new Queue<string>();
            }
                         
            string failMsg = string.Empty;

            if (EnterUserName(param.UserName, out failMsg) == false)
            {
                _tcpClient.Close();
                param.ConnectionState.State = ConnectionState.EState.NONE;
                param.ConnectionState.Message = failMsg;
                param.ConnectionState.State = ConnectionState.EState.DISCONNECTED;
                OnDiconnected(this, new FAGenericEventArgs<string>(failMsg));
                return;
            }

            if (EnterPassowrd(param.Password, out failMsg) == false)
            {
                _tcpClient.Close();
                param.ConnectionState.State = ConnectionState.EState.NONE;
                param.ConnectionState.Message = failMsg;
                param.ConnectionState.State = ConnectionState.EState.DISCONNECTED;
                OnDiconnected(this, new FAGenericEventArgs<string>(failMsg));
                return;
            }

            OnConnected(this, EventArgs.Empty);

            try
            {
                while (_threadStopRequest == false)
                {
                    System.Threading.Thread.Sleep(1);

                    var readData = ReadStream(1);

                    if (readData.Length > 0)
                    {
                        if (ReceiveDataDelegate != null)
                            ReceiveDataDelegate(readData);
                    }

                    var sendData = GetSendData();
                    if (string.IsNullOrEmpty(sendData) == false)
                        WriteStream(_streamEncoding.GetBytes(sendData));
                }
            }
            catch (Exception e)
            {
                _tcpClient.Close();
                param.ConnectionState.State = ConnectionState.EState.DISCONNECTED;
                param.ConnectionState.Message = e.Message;
                OnDiconnected(this, new FAGenericEventArgs<string>(e.Message));
            }
        }

        private void WriteStream(byte[] data)
        {
            _tcpClient.Send(data);
        }

        private string ReadStream(int timeout)
        {
            _tcpClient.ReceiveTimeout = timeout;

            StringBuilder sb = new StringBuilder();
            byte[] buffer = null;

            if (!_tcpClient.Connected)
            {
                throw new Exception("Disconnected Socket");
            }

            while (true)
            {
                var len = _tcpClient.Available;
                
                buffer = new byte[len];
                int read = 0;
                try
                {
                    read = _tcpClient.Receive(buffer);
                }
                catch
                {
                    break;
                }

                sb.Append(_streamEncoding.GetString(buffer));

                if (_tcpClient.Available <= 0)
                    break;
            }


            return sb.ToString();
        }

        private bool EnterUserName(string userName, out string msg)
        {
            msg = string.Empty;
            var readData = ReadStream(1000).ToUpper();
            if (readData.Contains("USER"))
            {
                var b = _streamEncoding.GetBytes(userName + "\r\n");
                WriteStream(b);
                return true;
            }
            else
                return false;
        }

        private bool EnterPassowrd(string password, out string msg)
        {
            msg = string.Empty;
            var readData = ReadStream(1000).ToUpper();
            if (readData.Contains("PASSWORD"))
            {
                var b = _streamEncoding.GetBytes(password + "\r\n");
                WriteStream(b);
                return true;
            }
            else
                return false;
        }

        private void PushSendData(string data)
        {
            lock (_syncRoot)
            {
                _sendDataQueue.Enqueue(data);
            }
        }

        private string GetSendData()
        {
            lock (_syncRoot)
            {
                if (_sendDataQueue.Count > 0)
                    return _sendDataQueue.Dequeue();
                else
                    return string.Empty;
            }
        }
    }
}
