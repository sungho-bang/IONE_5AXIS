using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Device.RS232Device;
using FALibrary.Part.SerialPortPart;

namespace FALibrary.Part.ScannerPart
{
    public class FACognexInsight5000Part : FACommonSerialPortPart
    {
        private string _setStringParameter = string.Empty;
        [FAAttribute("Parameters")]
        public string SetStringParameter
        {
            get { return _setStringParameter; }
            set
            {
                if (_setStringParameter == value) return;
                _setStringParameter = value;
                NotifyPropertyChanged("SetStringParameter");
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
            get { return _commandSendResult; }
            set
            {
                if (_commandSendResult == value) return;
                _commandSendResult = value;
                NotifyPropertyChanged("CommandSendResult");
            }
        }

        [FAAttribute("Operation")]
        public void Online(object sender)
        {
            SendCommand("SO", "1"); // alphabet SO and number 0
        }

        [FAAttribute("Operation")]
        public void Offline(object sender)
        {
            SendCommand("SO", "0");
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
            SendCommand("SS", SetStringParameter);
        }

        [FAAttribute("Operation")]
        public void JobChange(object sender)
        {
            SendCommand("LF", TargetJobName);
        }

        private void SendCommand(string command, string parameter)
        {
            CommandSendResult = false;
            SendData(command + parameter + "\r");
        }

        public override void SetDevice(Device.FADevice aDevice)
        {
            base.SetDevice(aDevice);

            Device.DataReceived +=
                delegate(object sender, FAGenericEventArgs<byte[]> e)
                {
                    var result = Encoding.ASCII.GetString(e.Value).Trim();
                    if (result == "1")
                        CommandSendResult = true;
                    else
                        CommandSendResult = false;
                };
        }
    }
}
