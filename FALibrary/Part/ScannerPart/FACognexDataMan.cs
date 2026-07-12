using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Device.Cognex;
using System.Text.RegularExpressions;
using FALibrary.Sequence;
using FALibrary.Utility;

namespace FALibrary.Part.ScannerPart
{
    public class FACognexDataMan : FAPart
    {
        public enum DelimiterType
        {
            none, space, comma, tab, label, xml
        }

        #region "Action"
        [FAAttribute("Action")]
        public FAPartAction ConnectAction
        {
            get;
            set;
        }
        #endregion

        private bool _scanAble;
        [FAAttribute("Status")]
        public bool ScanAble
        {
            get { return _scanAble; }
            set
            {
                if (_scanAble == value) return;
                _scanAble = value;
                NotifyPropertyChanged("ScanAble");
            }
        }
        private string _scanData;
        [FAAttribute("Status")]
        public string ScanData
        {
            get { return _scanData; }
            set
            {
                if (_scanData == value) return;
                _scanData = value;
                NotifyPropertyChanged("ScanData");
            }
        }        

        private string _terminator = "\r\n";
        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public string Terminator
        {
            get { return _terminator; }
            set
            {
                if (_terminator == value) return;

                _terminator = value;
                NotifyPropertyChanged("Terminator");
            }
        }

        private DelimiterType _delimiter = DelimiterType.comma;
        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public DelimiterType Delimiter
        {
            get { return _delimiter; }
            set
            {
                if (_delimiter == value) return;

                _delimiter = value;
                NotifyPropertyChanged("Delimiter");
            }
        }

        private string _targetConfigFileName;
        [FAAttribute("Parameter")]
        public string TargetConfigFileName
        {
            get { return _targetConfigFileName; }
            set
            {
                if (_targetConfigFileName == value) return;

                _targetConfigFileName = value;
                NotifyPropertyChanged("TargetConfigFileName");
            }
        }

        private string _currentConfigFileName;
        [FAAttribute("Parameter")]
        public string CurrentConfigFileName
        {
            get { return _currentConfigFileName; }
            set
            {
                if (_currentConfigFileName == value) return;

                _currentConfigFileName = value;
                NotifyPropertyChanged("CurrentConfigFileName");
            }
        }

        public FADataManDevice Device { get; private set; }

        [FAAttribute("Parameter")]
        public string CameraName { get; set; }

        [FAAttribute("Time")]
        public FATime TimeConnectTimeout { get; set; }

        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int AlarmConnectFail { get; set; }

        public event EventHandler OnTriggerOn;

        public FACognexDataMan(FASequenceManager aSequenceManager)
        {
            ConnectAction = new FAPartAction();

            ConnectAction.SetActionMethod(Connect);

            ConnectAction.CreateSequence(aSequenceManager);

            var seq = ConnectAction.Sequence;

            seq.AddItem(Disconnect);
            seq.AddItem(ConfirmConnect);
        }

        private void ConfirmConnect(FASequence actor, TimeSpan time)
        {
            bool result;
            Connect(out result);
            System.Threading.Thread.Sleep(50);

            if (TimeConnectTimeout.Time < time)
            {
                Alarm.FAAlarmManager.Instance.RaiseAlarm(actor, AlarmConnectFail, this.FullName);
            }
            else if (result == true)
                actor.NextStep();
        }

        private bool _loadedParameters = false;

        public override void LoadParameters(System.Xml.Linq.XElement xml)
        {
            base.LoadParameters(xml);

            if (_loadedParameters == false)
            {
                if (Device != null)
                {
                    Device.AddReadMethod(CameraName, OnReadData);
                    Device.AddArrivedImageMethod(CameraName, OnArrivedImage);
                }
                _loadedParameters = true;
            }
        }

        public override void SetDevice(Device.FADevice aDevice)
        {
            if (aDevice is FADataManDevice)
            {
                Device = aDevice as FADataManDevice;
                Device.OnLoadConfig +=
                    delegate(object sender, FADataManDevice.LoadConfigEventArgs e)
                    {
                        if (CameraName == e.CameraName)
                            CurrentConfigFileName = e.FileName;
                    };
            }
            else
                throw new Exception("Device Type is not correct." + " Device Name : " + Name + " Device Type : " + aDevice.GetType().ToString());
        }        

        protected void SendData(string data)
        {
            if (Device != null)
                Device.WriteCommand(CameraName, data + Terminator);
        }

        protected virtual void OnReadData(string data)
        {
            if (ScanAble)
            {
                ScanData = data;
                ScanAble = false;
                ParsingBarcode(data);
            }
        }

        protected virtual void OnArrivedImage(System.Drawing.Image image)
        {
        }

        protected virtual void ParsingBarcode(string data)
        {
        }
        
        [FAAttribute("Operation")]
        public void TriggerOn(object sender)
        {
            if (Device != null)
            {
                SendData("TRIGGER OFF");
                ScanAble = true;
                ScanData = "";
                if (OnTriggerOn != null)
                    OnTriggerOn(sender, EventArgs.Empty);
                SendData("TRIGGER ON");
            }
        }

        [FAAttribute("Operation")]
        public void TriggerOff(object sender)
        {
            if (Device != null)
            {
                ScanAble = false;
                ScanData = "";
                SendData("TRIGGER OFF");
            }
        }

        [FAAttribute("Operation")]
        public void Connect(object sender)
        {
            if (Device != null)
                Device.Connect(CameraName);
        }

        [FAAttribute("Operation")]
        public void Disconnect(object sender)
        {
            if (Device != null)
                Device.Disconnect(CameraName);
        }

        public void Connect(out bool result)
        {
            if (Device == null)
                result = false;
            else
                result = Device.Connect(CameraName);
        }
        
        public void Disconnect(out bool result)
        {
            if (Device == null)
                result = false;
            else
                result = Device.Disconnect(CameraName);
        }

        [FAAttribute("Operation")]
        public void LoadConfig()
        {
            Device.LoadConfig(CameraName, TargetConfigFileName);
        }

        public void LoadConfig(string filename)
        {
            TargetConfigFileName = filename;
            Device.LoadConfig(CameraName, filename);
        }

        protected System.Windows.Media.Imaging.BitmapImage ImageToImageSource(System.Drawing.Image bmp)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;
            var bi = new System.Windows.Media.Imaging.BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();

            return bi;
        }
    }
}
