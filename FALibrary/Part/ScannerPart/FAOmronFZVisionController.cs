using System;
using FALibrary.Device.Omron;
using FALibrary.Sequence;
using FALibrary.Utility;
using System.Collections.Generic;

namespace FALibrary.Part.ScannerPart
{
    public class FAOmronFZVisionController : FAPart
    {
        public enum ECommandType
        {
            Measure, ChangeScene, ChangeSceneGroup
        }

        public class CommandInfo
        {
            public ECommandType CommandType { get; set; }
            public string Command { get; set; }
        }

        public FADeviceFH1050VisionController _device = null;        
        private string _receiveData = "";

        public FADeviceFH1050VisionController Device
        {
            get { return _device; }
            private set { _device = value; }
        }

        private List<string> _receiveBuffer = new List<string>();
        #region Status
        [FAAttribute("Status")]
        public string ReceiveData
        {
            get { return _receiveData; }
            set
            {
                if (_receiveData == value) return;

                _receiveData = value;
                NotifyPropertyChanged("ReceiveData");
            }
        }
        private CommandInfo _lastCommand = new CommandInfo();
        [FAAttribute("Status")]
        public CommandInfo LastCommand
        {
            get { return _lastCommand; }
            private set
            {
                _lastCommand = value;
            }
        }
        private bool _measureOk;
        [FAAttribute("Status")]
        public bool MeasureOk
        {
            get { return _measureOk; }
            set
            {
                if (_measureOk == value) return;

                _measureOk = value;
                NotifyPropertyChanged("MeasureOk");
            }
        }

        private bool _sceneChangeOk;
        [FAAttribute("Status")]
        public bool SceneChangeOk
        {
            get { return _sceneChangeOk; }
            set
            {
                if (_sceneChangeOk == value) return;

                _sceneChangeOk = value;
                NotifyPropertyChanged("SceneChangeOk");
            }
        }

        private bool _sceneGroupChangeOk;
        [FAAttribute("Status")]
        public bool SceneGroupChangeOk
        {
            get { return _sceneGroupChangeOk; }
            set
            {
                if (_sceneGroupChangeOk == value) return;

                _sceneGroupChangeOk = value;
                NotifyPropertyChanged("SceneGroupChangeOk");
            }
        }
        #endregion

        #region Parameters
        public int _changeSceneTarget;
        [FAAttribute("Parameters")]
        public int ChangeSceneTarget
        {
            get { return _changeSceneTarget; }
            set
            {
                if (_changeSceneTarget == value) return;

                _changeSceneTarget = value;
                NotifyPropertyChanged("ChangeSceneTarget");
            }
        }

        public int _changeSceneGroupTarget;
        [FAAttribute("Parameters")]
        public int ChangeSceneGroupTarget
        {
            get { return _changeSceneGroupTarget; }
            set
            {
                if (_changeSceneGroupTarget == value) return;

                _changeSceneGroupTarget = value;
                NotifyPropertyChanged("ChangeSceneGroupTarget");
            }
        }
        #endregion        

        #region Read Delegate
        protected virtual void OnReadMeasureResult(bool commResult, string data)
        {
            MeasureOk = true;
        }

        protected virtual void OnReadChangeSceneResult(bool commResult)
        {
            SceneChangeOk = commResult;
        }

        protected virtual void OnReadChangeSceneGroupResult(bool commResult)
        {
            SceneGroupChangeOk = commResult;
        }
        #endregion

        [FAAttribute("Operator")]
        public void Open(object sender)
        {
            Device.Open();
        }

        [FAAttribute("Operator")]
        public void Close(object sender)
        {
            Device.Close();
        }

        private void OnReadEventHandler(object sender, FAGenericEventArgs<string[]> e)
        {
            OnReadData(e.Value);
        }

        protected virtual void OnReadData(string[] data)
        {
            _receiveBuffer.AddRange(data);

            switch (LastCommand.CommandType)
            {
                case ECommandType.Measure:
                    if (_receiveBuffer.Count >= 2)
                    {
                        bool commOk = CommResultToBoolean(_receiveBuffer[0]);

                        OnReadMeasureResult(commOk, _receiveBuffer[1]);

                        _receiveBuffer.Clear();
                    }

                    break;

                case ECommandType.ChangeScene:
                    if (_receiveBuffer.Count >= 1)
                    {
                        bool commOk = CommResultToBoolean(_receiveBuffer[0]);

                        OnReadChangeSceneResult(commOk);

                        _receiveBuffer.Clear();
                    }

                    break;

                case ECommandType.ChangeSceneGroup:
                    if (_receiveBuffer.Count >= 1)
                    {
                        bool commOk = CommResultToBoolean(_receiveBuffer[0]);

                        OnReadChangeSceneGroupResult(commOk);

                        _receiveBuffer.Clear();
                    }

                    break;
            }
        }

        private bool CommResultToBoolean(string str)
        {
            bool commOk = false;
            if (str.Trim() == "OK")
                commOk = true;

            return commOk;
        }

        protected virtual void OnWriteData(CommandInfo commandType)
        {
            switch (commandType.CommandType)
            {
                case ECommandType.Measure:
                    MeasureOk = false;
                    break;

                case ECommandType.ChangeScene:
                    SceneChangeOk = false;
                    break;

                case ECommandType.ChangeSceneGroup:
                    SceneGroupChangeOk = false;
                    break;
            }
        }

        public override void Validate()
        {
            base.Validate();            
            ReceiveData = Device.ReceiveData;
        }
        
        public override void SetDevice(Device.FADevice aDevice)
        {
            if (aDevice is FADeviceFH1050VisionController)
            {
                Device = aDevice as FADeviceFH1050VisionController;                
                Device.OnRead += OnReadEventHandler;
            }
            else
                throw new Exception("Device Type is not correct." + " Device Name : " + Name + " Device Type : " + aDevice.GetType().ToString());
        }

        protected void WriteCommand(ECommandType commandType, string command)
        {
            LastCommand.CommandType = commandType;
            LastCommand.Command = command;
            var obj = new CommandInfo();
            obj.CommandType = commandType;
            obj.Command = command;
            OnWriteData(obj);
            Device.WriteCommand(command);
        }

        [FAAttribute("Operation")]
        public void ScanStart(object sender)
        {
            ScanStart();
        }

        public void ScanStart()
        {
            try
            {
                WriteCommand(ECommandType.Measure, "M");
            }
            catch
            {
            }
        }

        [FAAttribute("Operation")]
        public void ChangeScene(object sender)
        {
            ChangeScene(ChangeSceneTarget);
        }

        public void ChangeScene(int no)
        {
            try
            {
                if (no < 0) return;
                if (no > 99) return;
                WriteCommand(ECommandType.ChangeScene, "SCENE " + no.ToString());                
            }
            catch
            {
            }
        }

        [FAAttribute("Operation")]
        public void ChangeSceneGroup(object sender)
        {
            ChangeSceneGroup(ChangeSceneGroupTarget);
        }

        public void ChangeSceneGroup(int no)
        {
            try
            {
                if (no < 0) return;
                if (no > 99) return;
                WriteCommand(ECommandType.ChangeSceneGroup, "SCNGROUP " + no.ToString());                
            }
            catch
            {
            }
        }
    }
}
