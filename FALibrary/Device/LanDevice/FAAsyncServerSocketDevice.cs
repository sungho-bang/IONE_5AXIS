using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text.RegularExpressions;

namespace FALibrary.Device.LanDevice
{
    public class FAAsyncServerSocketDevice : FADevice
    {
        public class StateObject
        {
            // Client  socket.
            public Socket workSocket = null;
            // Size of receive buffer.
            public const int BufferSize = 1024;
            // Receive buffer.
            public byte[] buffer = new byte[BufferSize];
            // Received data string.
            public List<byte> receivedData = new List<byte>();
        }

        private Socket _listener;
        private List<byte[]> _sendData = new List<byte[]>();
        private Thread _thread;
        private bool _threadAbortFlag = false;

        private ManualResetEvent _readDone = new ManualResetEvent(false);
        private ManualResetEvent _allDone = new ManualResetEvent(false);
        protected TcpListener ClientSocket { get; set; }
        public Encoding StreamEncoding { get; protected set; }
        public string IPAddress { get; set; }
        public int Port { get; set; }
        public int EncodingCodePage { get; set; }
        public bool UseEOFString { get; set; }
        public string EOFString { get; set; }
        public bool UseRegex { get; set; }
        public string RegexPattern { get; set; }

        public event EventHandler<FAGenericEventArgs<byte[]>> OnWrite;
        public event EventHandler<FAGenericEventArgs<byte[]>> OnRead;
        public event EventHandler<FAGenericEventArgs<string>> OnException;

        public Func<byte[], bool> CustomRead { get; set; }

        public int SendTimeout { get; set; }
        public int ReceiveTimeout { get; set; }
        public bool UseReceiveDataSize { get; set; }
        public int ReceiveDataSize { get; set; }        

        public FAAsyncServerSocketDevice()
        {
            EncodingCodePage = Encoding.ASCII.CodePage;
            StreamEncoding = Encoding.ASCII;
        }      

        public override void Open()
        {
            StreamEncoding = Encoding.GetEncoding(EncodingCodePage);

            try
            {
                IPAddress ip;

                ip = System.Net.IPAddress.Parse(IPAddress);

                IPEndPoint localEndPoint = new IPEndPoint(ip, Port);

                _listener = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                _listener.Bind(localEndPoint);
                _listener.Listen(100);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }

            _thread = new Thread(
                delegate()
                {
                    while (true)
                    {
                        if (_threadAbortFlag == true) break;
                        _allDone.Reset();
                        StartListening();
                        _allDone.WaitOne();
                        Thread.Sleep(1);
                    }
                });

            _thread.Start();
        }

        public override void Close()
        {
            try
            {
                _threadAbortFlag = true;
                _allDone.Set();
                _readDone.Set();
                Thread.Sleep(1000);
                _thread.Abort();

                _listener.Close();
                _listener = null;                
            }
            catch
            {
            }
        }

        public void ForceClose()
        {
            try
            {
                _threadAbortFlag = true;
                _allDone.Set();
                _readDone.Set();
                //Thread.Sleep(100);

                _listener.Close();
                _listener = null;
                //_thread.Abort();

                //if (_listener != null)
                //    _listener.Close();
            }
            catch
            {
            }
        }

        public void SendData(byte[][] data)
        {
            try
            {
                if (data != null)
                {
                    _sendData.Clear();
                    int i = 0;
                    foreach (var item in data)
                    {
                        _sendData.Add(new byte[item.Length]);
                        Array.Copy(item, _sendData[i], item.Length);
                        i++;
                    }
                }

                _readDone.Set();
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        public void SendData(string[] data)
        {
            try
            {
                if (data == null)
                {
                    byte[][] bytes = null;
                    SendData(bytes);
                }
                else
                {
                    byte[][] bytes = new byte[data.Length][];

                    int i = 0;
                    foreach (var item in data)
                    {
                        bytes[i] = new byte[item.Length];
                        bytes[i] = StreamEncoding.GetBytes(item);
                        i++;
                    }

                    SendData(bytes);
                }
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        public void StartListening()
        {
            byte[] bytes = new Byte[1024];
            
            try
            {
                BeginAccept();
            }
            catch (Exception e)
            {
                if (OnException != null)
                    OnException(this, new FAGenericEventArgs<string>("Exception SendCallBack()\n" + e.ToString()));
            }
        }

        private void BeginAccept()
        {                       
            _readDone.Reset();

            Thread.Sleep(10);            

            _listener.BeginAccept(
                new AsyncCallback(AcceptCallback),
                _listener);
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            if (_listener == null) return;

            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);
            
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            int bytesRead = 0;
            bool clientClosed = false;

            try
            {
                bytesRead = handler.EndReceive(ar);
            }
            catch
            {
                clientClosed = true;
            }

            if (clientClosed)
            {
                if (OnRead != null)
                    OnRead(this, new FAGenericEventArgs<byte[]>(state.receivedData.ToArray()));

                _readDone.WaitOne();

                try
                {
                    Send(handler, _sendData);
                }
                catch (System.Exception e)
                {
                    if (OnException != null)
                        OnException(this, new FAGenericEventArgs<string>(e.ToString()));
                }
            }
            else if (bytesRead > 0)
            {
                byte[] buffer = new byte[bytesRead];
                Array.Copy(state.buffer, 0, buffer, 0, bytesRead);
                state.receivedData.AddRange(buffer);

                bool isEOF = false;

                if (CustomRead != null)
                {
                    isEOF = CustomRead(state.receivedData.ToArray());
                }

                if (UseEOFString && isEOF == false)
                {
                    string temp = StreamEncoding.GetString(state.receivedData.ToArray());
                    int eofIndex = temp.IndexOf(EOFString);
                    if (eofIndex >= 0)
                    {
                        state.receivedData.Clear();
                        state.receivedData.AddRange(StreamEncoding.GetBytes(temp.Substring(0, eofIndex)));
                        isEOF = true;
                    }                    
                }
                
                if (UseRegex && isEOF == false)
                {
                    string temp = StreamEncoding.GetString(state.receivedData.ToArray());
                    var matchResult = Regex.Match(temp, RegexPattern);
                    if (matchResult.Success == true)
                    {
                        isEOF = true;                        
                        state.receivedData.Clear();
                        state.receivedData.AddRange(StreamEncoding.GetBytes(matchResult.Value));
                    }
                }

                if (isEOF)
                {
                    if (OnRead != null)
                        OnRead(this, new FAGenericEventArgs<byte[]>(state.receivedData.ToArray()));

                    _readDone.WaitOne();

                    try
                    {
                        Send(handler, _sendData);
                    }
                    catch (System.Exception e)
                    {
                        if (OnException != null)
                            OnException(this, new FAGenericEventArgs<string>(e.ToString()));
                    }
                }
                else
                {
                    var res = handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReadCallback), state);
                    
                    if (res.AsyncWaitHandle.WaitOne(5000) == false)
                    {
                        string msg = string.Format("Data Receive Timeout. Received data = {0}", StreamEncoding.GetString(state.receivedData.ToArray()));
                        Utility.Trace.WriteLine(this, "Device", msg);
                        if (OnRead != null)
                            OnRead(this, new FAGenericEventArgs<byte[]>(state.receivedData.ToArray()));

                        _readDone.WaitOne();

                        try
                        {
                            Send(handler, _sendData);
                        }
                        catch (System.Exception e)
                        {
                            if (OnException != null)
                                OnException(this, new FAGenericEventArgs<string>(e.ToString()));
                        }
                    }
                }
            }
            else
            {
                if (OnRead != null)
                    OnRead(this, new FAGenericEventArgs<byte[]>(state.receivedData.ToArray()));

                _readDone.WaitOne();

                try
                {
                    Send(handler, _sendData);
                }
                catch (System.Exception e)
                {
                    if (OnException != null)
                        OnException(this, new FAGenericEventArgs<string>(e.ToString()));
                }
            }
        }

        private void Send(Socket handler, List<byte[]> data)
        {
            try
            {
                if (data.Count <= 0)
                {
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    _allDone.Set();
                    return;
                }

                byte[] sendData = data.First();
                data.RemoveAt(0);

                if (OnWrite != null)
                {
                    OnWrite(this, new FAGenericEventArgs<byte[]>(sendData));
                }

                handler.BeginSend(sendData, 0, sendData.Length, 0,
                    new AsyncCallback(SendCallback), handler);
            }
            catch (System.Exception e)
            {
                if (OnException != null)
                    OnException(this, new FAGenericEventArgs<string>(e.ToString()));
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;

                int bytesSent = handler.EndSend(ar);

                Send(handler, _sendData);
            }
            catch (Exception e)
            {
                if (OnException != null)
                    OnException(this, new FAGenericEventArgs<string>("Exception SendCallBack()\n" + e.ToString()));
            }
        }
    }
}
