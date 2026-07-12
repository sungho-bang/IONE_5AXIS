using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace FALibrary.Part.ScannerPart
{
    public class FACognexInsightControlPart : FAPart
    {
        static object _thisLock = new object();

        public FALibrary.Device.Cognex.FADeviceCognexInsightController Device { get; private set; }
        public event EventHandler<FAGenericEventArgs<string>> OnReceiveData = delegate { };
        public event EventHandler<FAGenericEventArgs<string>> OnSendData = delegate { };

        private string _targetCellValue = string.Empty;
        [FAAttribute("Parameters")]
        public string TargetCellValue
        {
            get { return _targetCellValue; }
            set
            {
                if (_targetCellValue == value) return;
                _targetCellValue = value;
                NotifyPropertyChanged("TargetCellValue");
            }
        }

        private string _targetJobName = string.Empty;
        [FAAttribute("Parameters")]
        public string TargetJobName
        {
            get { return _targetJobName; }
            set
            {
                if (_targetJobName == value) return;
                _targetJobName = value;
                NotifyPropertyChanged("TargetJobName");
            }
        }

        private string _jobName = string.Empty;
        /// <summary>
        /// 현재 선택된 Job.
        /// JobChange(string jobName, out bool result)를 호출시 성공하면
        /// 이 속성 값이 바뀐다.
        /// </summary>
        [FAAttribute("Status")]
        public string JobName
        {
            get { return _jobName; }
            set
            {
                if (_jobName == value) return;
                _jobName = value;
                NotifyPropertyChanged("JobName");
            }
        }

        private string _terminator = "\r";
        public string Terminator
        {
            get { return _terminator; }
            set
            {
                _terminator = value;
            }
        }

        private bool _commandSendResult;
        [FAAttribute("Status")]
        public bool CommandSendResult
        {
            get
            {
                lock (_thisLock)
                {
                    return _commandSendResult;
                }
            }
            set
            {
                if (_commandSendResult == value) return;
                _commandSendResult = value;
                NotifyPropertyChanged("CommandSendResult");
            }
        }

        private string _commandResultMessage;
        [FAAttribute("Status")]
        public string CommandResultMessage
        {
            get
            {
                lock (_thisLock)
                {
                    return _commandResultMessage;
                }
            }
            set
            {
                if (_commandResultMessage == value) return;
                _commandResultMessage = value;
                NotifyPropertyChanged("CommandResultMessage");
            }
        }

        public override void SetDevice(Device.FADevice aDevice)
        {
            if (aDevice is FALibrary.Device.Cognex.FADeviceCognexInsightController)
                Device = aDevice as FALibrary.Device.Cognex.FADeviceCognexInsightController;
            else
                throw new Exception("Device Type is not correct." + " Device Name : " + Name + " Device Type : " + aDevice.GetType().ToString());
            
            Device.ReceiveDataDelegate =
                delegate(string data)
                {
                    OnReceiveData(this, new FAGenericEventArgs<string>(data));
                    
                    var splitData = data.Split('\n');
                    string result = string.Empty;
                    if (splitData.Length > 0)
                        result = splitData[0].Trim();

                    lock (_thisLock)
                    {
                        if (splitData.Length > 1)
                            CommandResultMessage = splitData[1].Trim();

                        if (result == "1")
                            CommandSendResult = true;
                        else
                            CommandSendResult = false;
                    }
                };
        }

        [FAAttribute("Operation")]
        public void Online(object sender)
        {
            SendCommand("SO", "1"); // alphabet SO and number 1
        }

        [FAAttribute("Operation")]
        public void Offline(object sender)
        {
            SendCommand("SO", "0"); // alphabet SO and number 0
        }

        [FAAttribute("Operation")]
        public void GetOnlineStatus(object sender)
        {
            SendCommand("GO", "");
        }

        [FAAttribute("Operation")]
        public void GetActiveJob(object sender)
        {
            SendCommand("GF", "");
        }

        [FAAttribute("Operation")]
        public void SetString(object sender)
        {
            SendCommand("SS", TargetCellValue);
        }

        [FAAttribute("Operation")]
        public void JobChange(object sender)
        {
            SendCommand("LF", TargetJobName);
        }

        [FAAttribute("Operation")]
        public void TriggerOn(object sender)
        {
            SendCommand("SE", "8");
        }

        public void JobChange(string jobName, Action<bool, string> actionWhenSuccess, int jobChangeTimeout = 1000)
        {
            if (SimulationMode) return;
            int commInterval = 500;
            int retryCount = 0;
            int retryLimit = 3;
            System.Threading.Tasks.Task.Factory.StartNew(
                () =>
                {
                    while (true)
                    {
                        Offline(this);
                        Utility.FAUtility.Wait(commInterval);

                        TargetJobName = jobName;

                        JobChange(this);
                        Utility.FAUtility.Wait(commInterval);
                        GetActiveJob(this);
                        if (!Utility.FAUtility.Compare(
                            () => CommandResultMessage == TargetJobName, null, jobChangeTimeout))
                        {
                            if (retryCount++ < retryLimit)
                                continue;
                            else
                            {
                                if (actionWhenSuccess != null)
                                    actionWhenSuccess(false,
                                        "Can not change job target_job=" + TargetJobName +
                                        " changed_job=" + CommandResultMessage);
                                return;
                            }
                        }

                        Online(this);
                        if (Utility.FAUtility.Compare(
                            () => CommandSendResult, null, jobChangeTimeout))
                        {
                            if (actionWhenSuccess != null)
                                actionWhenSuccess(true, "Success");
                            return;
                        }
                        else
                        {
                            if (retryCount++ < retryLimit)
                                continue;
                            else
                            {
                                if (actionWhenSuccess != null)
                                    actionWhenSuccess(false,
                                        "Online change fail");
                                return;
                            }
                        }
                    }
                });
        }

        private void SendCommand(string command, string parameter)
        {
            if (!SimulationMode)
            {
                CommandSendResult = false;
                OnSendData(this, new FAGenericEventArgs<string>(command + parameter));
                Device.SendData(command + parameter);
            }
        }
    }
}
