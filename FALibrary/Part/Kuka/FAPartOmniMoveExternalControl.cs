using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using FALibrary.Device.Kuka;

namespace FALibrary.Part.Kuka
{
    public class RobotStatus : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private long _timestamp;
        [FAAttribute("")]
        public long Timestamp
        {
            get { return _timestamp; }
            set
            {
                if (_timestamp == value) return;
                _timestamp = value;
                NotifyPropertyChanged("Timestamp");
            }
        }

        private long _sendedPacketCounter;
        [FAAttribute("")]
        public long SendedPacketCounter
        {
            get { return _sendedPacketCounter; }
            set
            {
                if (_sendedPacketCounter == value) return;
                _sendedPacketCounter = value;
                NotifyPropertyChanged("SendedPacketCounter");
            }
        }

        private long _receivedPacketCounter;
        [FAAttribute("")]
        public long ReceivedPacketCounter
        {
            get { return _receivedPacketCounter; }
            set
            {
                if (_receivedPacketCounter == value) return;
                _receivedPacketCounter = value;
                NotifyPropertyChanged("ReceivedPacketCounter");
            }
        }

        private long _errorID;
        [FAAttribute("")]
        public long ErrorID
        {
            get { return _errorID; }
            set
            {
                if (_errorID == value) return;
                _errorID = value;
                NotifyPropertyChanged("ErrorID");
            }
        }

        private bool _activedAutMode;
        [FAAttribute("")]
        public bool ActivedAutMode
        {
            get { return _activedAutMode; }
            set
            {
                if (_activedAutMode == value) return;
                _activedAutMode = value;
                NotifyPropertyChanged("ActivedAutMode");
            }
        }

        private bool _isAppReadyToStart;
        [FAAttribute("")]
        public bool IsAppReadyToStart
        {
            get { return _isAppReadyToStart; }
            set
            {
                if (_isAppReadyToStart == value) return;
                _isAppReadyToStart = value;
                NotifyPropertyChanged("IsAppReadyToStart");
            }
        }

        private bool _isAppError;
        [FAAttribute("")]
        public bool IsAppError
        {
            get { return _isAppError; }
            set
            {
                if (_isAppError == value) return;
                _isAppError = value;
                NotifyPropertyChanged("IsAppError");
            }
        }

        private bool _isStatusError;
        [FAAttribute("")]
        public bool IsStatusError
        {
            get { return _isStatusError; }
            set
            {
                if (_isStatusError == value) return;
                _isStatusError = value;
                NotifyPropertyChanged("IsStatusError");
            }
        }

        private string _appState;
        [FAAttribute("")]
        public string AppState
        {
            get { return _appState; }
            set
            {
                if (_appState == value) return;
                _appState = value;
                NotifyPropertyChanged("AppState");
            }
        }

        private bool _lastAppStartSignalStatus;
        [FAAttribute("")]
        public bool LastAppStartSignalStatus
        {
            get { return _lastAppStartSignalStatus; }
            set
            {
                if (_lastAppStartSignalStatus == value) return;
                _lastAppStartSignalStatus = value;
                NotifyPropertyChanged("LastAppStartSignalStatus");
            }
        }

        private bool _lastAppEnableSignalStatus;
        [FAAttribute("")]
        public bool LastAppEnableSignalStatus
        {
            get { return _lastAppEnableSignalStatus; }
            set
            {
                if (_lastAppEnableSignalStatus == value) return;
                _lastAppEnableSignalStatus = value;
                NotifyPropertyChanged("LastAppEnableSignalStatus");
            }
        }

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
    }

    public class FAPartOmniMoveExternalControl : FAPart
    {        
        public FADeviceOmniMoveExternalControl Device { get; private set; }

        public override void SetDevice(Device.FADevice aDevice)
        {
            if (aDevice is FADeviceOmniMoveExternalControl)
            {
                Device = aDevice as FADeviceOmniMoveExternalControl;
            }
            else
                throw new Exception("Device Type is not correct." + " Device Name : " + Name + " Device Type : " + aDevice.GetType().ToString());
            
        }

        #region Status
        private RobotStatus _robotStatus;
        [FAAttribute("Status")]
        public RobotStatus RobotStatus
        {
            get { return _robotStatus; }
            set
            {
                if (_robotStatus == value) return;
                _robotStatus = value;
                NotifyPropertyChanged("RobotStatus");
            }
        }

        private bool _connected;
        [FAAttribute("Status")]
        public bool Connected
        {
            get { return _connected; }
            set
            {
                if (_connected == value) return;
                _connected = value;
                NotifyPropertyChanged("Connected");
            }
        }

        private bool _communicationError;
        [FAAttribute("Status")]
        public bool CommunicationError
        {
            get { return _communicationError; }
            set
            {
                if (_communicationError == value) return;
                _communicationError = value;
                NotifyPropertyChanged("CommunicationError");
            }
        }
        #endregion

        public FAPartOmniMoveExternalControl()
        {
            this.RobotStatus = new RobotStatus();
        }

        [FAAttribute("Operation")]
        public void Connect()
        {
            Device.RequestConnect();
        }

        [FAAttribute("Operation")]
        public void Disconnect()
        {
            Device.RequestDisconnect();
        }

        [FAAttribute("Operation")]
        public void StartApp()
        {
            Device.StartRobotApp();
        }

        public override void Validate()
        {
            base.Validate();
            var robotStatus = Device.GetRobotStatus();
            if (robotStatus != null)
            {
                this.RobotStatus.ActivedAutMode = robotStatus.ActivedAutMode;
                this.RobotStatus.AppState = robotStatus.AppState;
                this.RobotStatus.ErrorID = robotStatus.ErrorID;
                this.RobotStatus.IsAppError = robotStatus.IsAppError;
                this.RobotStatus.IsAppReadyToStart = robotStatus.IsAppReadyToStart;
                this.RobotStatus.IsStatusError = robotStatus.IsStatusError;
                this.RobotStatus.LastAppEnableSignalStatus = robotStatus.LastAppEnableSignalStatus;
                this.RobotStatus.LastAppStartSignalStatus = robotStatus.LastAppStartSignalStatus;
                this.RobotStatus.ReceivedPacketCounter = robotStatus.ReceivedPacketCounter;
                this.RobotStatus.SendedPacketCounter = robotStatus.SendedPacketCounter;
                this.RobotStatus.Timestamp = robotStatus.Timestamp;
            }

            Connected = Device.GetConnectedStatus();
            CommunicationError = Device.Error;
        }
    }
}
