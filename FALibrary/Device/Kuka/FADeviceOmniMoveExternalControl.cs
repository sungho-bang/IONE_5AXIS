using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;
using System.Diagnostics;

namespace FALibrary.Device.Kuka
{
    public class FADeviceOmniMoveExternalControl : FADevice
    {
        enum ControlSignalNames
        {
            App_Start,
            App_Enable,
            Get_State
        }

        class ControlMessage
        {
            public long Timestamp { get; set; }
            public long SendedPacketCounter { get; set; }
            public ControlSignalNames ControlSignalName { get; set; }
            public bool ControlSignalValue { get; set; }

            public ControlMessage(ControlSignalNames controlSignalName, bool controlSignalValue)
            {
                ControlSignalName = controlSignalName;
                ControlSignalValue = controlSignalValue;
            }

            public string ToPacket()
            {
                return string.Join(";", new string[]
                {
                    Timestamp .ToString(),
                    SendedPacketCounter.ToString(),
                    ControlSignalName.ToString(),
                    ControlSignalValue.ToString().ToLower()
                });
            }

            public void SetTimestamp(DateTime dateTime)
            {
                Timestamp = (long)ConvertToUnixTimestamp(dateTime);
            }

            static DateTime ConvertFromUnixTimestamp(double timestamp)
            {
                DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                return origin.AddSeconds(timestamp);
            }

            static double ConvertToUnixTimestamp(DateTime date)
            {
                DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                TimeSpan diff = date - origin;
                return Math.Floor(diff.TotalMilliseconds);
            }
        }

        public class RobotStatus
        {
            public long Timestamp { get; set; }
            public long SendedPacketCounter { get; set; }
            public long ReceivedPacketCounter { get; set; }
            public long ErrorID { get; set; }
            public bool ActivedAutMode { get; set; }
            public bool IsAppReadyToStart { get; set; }
            public bool IsAppError { get; set; }
            public bool IsStatusError { get; set; }
            public string AppState { get; set; }
            public bool LastAppStartSignalStatus { get; set; }
            public bool LastAppEnableSignalStatus { get; set; }

            public static RobotStatus Parse(string str)
            {
                var arr = str.Split(';');

                RobotStatus newObj = new RobotStatus();
                newObj.Timestamp = long.Parse(arr[0]);
                newObj.SendedPacketCounter = long.Parse(arr[1]);
                newObj.ReceivedPacketCounter = long.Parse(arr[2]);
                newObj.ErrorID = long.Parse(arr[3]);
                newObj.ActivedAutMode = bool.Parse(arr[4]);
                newObj.IsAppReadyToStart = bool.Parse(arr[5]);
                newObj.IsAppError = bool.Parse(arr[6]);
                newObj.IsStatusError = bool.Parse(arr[7]);
                newObj.AppState = arr[8];
                newObj.LastAppStartSignalStatus = bool.Parse(arr[9]);
                newObj.LastAppEnableSignalStatus = bool.Parse(arr[10]);

                return newObj;
            }

            public RobotStatus Copy()
            {
                var obj = new RobotStatus
                {
                    Timestamp = this.Timestamp,
                    SendedPacketCounter = this.SendedPacketCounter,
                    ReceivedPacketCounter = this.ReceivedPacketCounter,
                    ErrorID = this.ErrorID,
                    ActivedAutMode = this.ActivedAutMode,
                    IsAppReadyToStart = this.IsAppReadyToStart,
                    IsAppError = this.IsAppError,
                    IsStatusError = this.IsStatusError,
                    AppState = this.AppState,
                    LastAppStartSignalStatus = this.LastAppStartSignalStatus,
                    LastAppEnableSignalStatus = this.LastAppEnableSignalStatus
                };

                return obj;
            }
        }

        object _thisLock = new object();
        object _robotStatusLock = new object();

        readonly int REMOTE_PORT = 30300;
        readonly int PORT = 30333;

        public string HostIPAddress { get; set; }
        public string LocalIPAddress { get; set; }
        IPEndPoint _ipEndPoint;
        IPEndPoint _remoteIPEndPoint;
        UdpClient _udp;
        Queue<ControlMessage> _commandQueue = new Queue<ControlMessage>();
        static long _packetCounter;
        bool _threadStop = false;
        private RobotStatus _robotStatus;
        private bool _disconnectRequest = false;
        private bool _connected = false;
        private int _errorCount;
        public bool Error { get; private set; }        
        ControlMessage _appEnableMessage = new ControlMessage(ControlSignalNames.App_Enable, true);

        public override void Open()
        {
            base.Open();

            _ipEndPoint = new System.Net.IPEndPoint(IPAddress.Parse(HostIPAddress), REMOTE_PORT);
            _remoteIPEndPoint = new IPEndPoint(IPAddress.Parse(LocalIPAddress), PORT);            

            System.Threading.Thread thread = new System.Threading.Thread(
                delegate(object obj)
                {                    
                    while (_threadStop == false)
                    {
                        _connected = false;
                        if (_disconnectRequest)
                            continue;

                        CommunicationWithHost();
                    }
                });

            thread.Start();
        }

        public override void Close()
        {
            _threadStop = true;
            base.Close();
        }

        public void StartRobotApp()
        {
            lock (_thisLock)
            {
                AddCommand(new ControlMessage(ControlSignalNames.App_Start, false));
                AddCommand(new ControlMessage(ControlSignalNames.App_Start, true));
            }
        }

        public void GetState()
        {
            lock (_thisLock)
            {
                AddCommand(new ControlMessage(ControlSignalNames.Get_State, true));
                AddCommand(new ControlMessage(ControlSignalNames.Get_State, false));
            }
        }

        public RobotStatus GetRobotStatus()
        {
            RobotStatus robotStatus = null;
            lock(_robotStatusLock)
            {
                if (_robotStatus != null)
                    robotStatus = _robotStatus.Copy();
            }

            return robotStatus;
        }
        
        public bool GetConnectedStatus()
        {
            return _connected;
        }

        public void RequestConnect()
        {
            _disconnectRequest = false;
            Error = false;
        }

        public void RequestDisconnect()
        {
            _disconnectRequest = true;
        }

        private void AddCommand(ControlMessage command)
        {
            _commandQueue.Enqueue(command);
        }

        private void SendCommand(ControlMessage command)
        {
            command.SendedPacketCounter = ++_packetCounter;
            command.SetTimestamp(DateTime.Now);
            var msg = Encoding.UTF8.GetBytes(command.ToPacket());
            _udp.Send(msg, msg.Length, _ipEndPoint);
        }

        private ControlMessage GetControlMessage()
        {
            var result = _appEnableMessage;

            lock (_thisLock)
            {
                if (_commandQueue.Count > 0)
                    result = _commandQueue.Dequeue();
            }

            return result;
        }

        private void ReceiveData()
        {
            var data = Encoding.UTF8.GetString(_udp.Receive(ref _remoteIPEndPoint));
            lock (_robotStatusLock)
            {
                _robotStatus = RobotStatus.Parse(data);
            }

            _packetCounter = _robotStatus.ReceivedPacketCounter;

            if (_robotStatus.ErrorID < 0)
            {
                _errorCount++;
                if (_errorCount > 3)
                    Error = true;
            }
        }

        private void CommunicationWithHost()
        {
            try
            {
                _udp = new UdpClient(PORT);
            }
            catch (SocketException e)
            {
                Error = true;
                Utility.Trace.WriteLine(this, "Device", e.ToString());
                
                if (_udp != null)
                    _udp.Close();
                return;
            }

            try
            {
                _connected = true;
                _packetCounter = 0;
                _udp.Client.ReceiveTimeout = 5000;
                Error = false;
                _errorCount = 0;

                while (_threadStop == false && _disconnectRequest == false && Error == false)
                {
                    SendCommand(GetControlMessage());
                    ReceiveData();
                }

                _udp.Close();
                System.Threading.Thread.Sleep(1000);
            }
            catch (Exception e)
            {
                Error = true;
                var msg = string.Format("Exception int FAOmniMoveExternalControl. Name is {0}",
                    Name,
                    e.ToString());
                Utility.Trace.WriteLine(this, "Device", msg);

                if (_udp != null)
                    _udp.Close();
            }
        }
    }
}
