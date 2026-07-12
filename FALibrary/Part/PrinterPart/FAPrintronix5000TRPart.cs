using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Device.Zebra;
using FALibrary.Sequence;
using FALibrary.Utility;
using FALibrary.Alarm;

namespace FALibrary.Part.PrinterPart
{
    public class FAPrintronix5000TRPart : FAPart
    {
        private string _script;
        private bool _statusOnlineError = false;        
        private bool _statusPaperOutError = false;
        private bool _statusHeadOpenError = false;
        private bool _statusBufferOverflow = false;
        private bool _statusRibbonOut = false;
        
        private FADeviceZebraPrinter _device = null;

        public FADeviceZebraPrinter Device
        {
            get { return _device; }
            private set { _device = value; }
        }

        [FAAttribute("")]
        public string Script
        {
            get { return _script; }
            set
            {
                if (_script == value) return;

                _script = value;
                NotifyPropertyChanged("Script");
            }
        }

        #region Status
        [FAAttribute("Status")]
        public bool StatusOnlineError
        {
            get { return _statusOnlineError; }
            private set
            {
                if (_statusOnlineError == value) return;

                _statusOnlineError = value;
                NotifyPropertyChanged("StatusOnlineError");
            }
        }        
        [FAAttribute("Status")]
        public bool StatusPaperOutError
        {
            get { return _statusPaperOutError; }
            private set
            {
                if (_statusPaperOutError == value) return;

                _statusPaperOutError = value;
                NotifyPropertyChanged("StatusPaperOutError");
            }
        }
        [FAAttribute("Status")]
        public bool StatusHeadOpenError
        {
            get { return _statusHeadOpenError; }
            private set
            {
                if (_statusHeadOpenError == value) return;

                _statusHeadOpenError = value;
                NotifyPropertyChanged("StatusHeadOpenError");
            }
        }
        [FAAttribute("Status")]
        public bool StatusBufferOverflow
        {
            get { return _statusBufferOverflow; }
            private set
            {
                if (_statusBufferOverflow == value) return;

                _statusBufferOverflow = value;
                NotifyPropertyChanged("StatusBufferOverflow");
            }
        }
        [FAAttribute("Status")]
        public bool StatusRibbonOut
        {
            get { return _statusRibbonOut; }
            private set
            {
                if (_statusRibbonOut == value) return;

                _statusRibbonOut = value;
                NotifyPropertyChanged("StatusRibbonOut");
            }
        }
        #endregion

        #region Sequence
        [FAAttribute("Sequence")]
        public FASequence CheckStatus { get; set; }
        #endregion

        #region Alarm
        [FAAttribute("Alarm")]
        [FAPropertyAttribute]        
        public int AlarmLabelPrinterOnlineError { get; set; }
        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int AlarmLabelPrinterBufferOverflowError { get; set; }
        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int AlarmLabelPrinterHeadOpenError { get; set; }
        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int AlarmLabelPrinterPaperOutError { get; set; }
        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int AlarmLabelPrinterRibbonOutError { get; set; }
        #endregion

        #region Time
        [FAAttribute("Time")]
        public FATime TimeStatusCheckTimeout { get; set; }
        #endregion

        #region RetryInfo
        [FAAttribute("RetryInfo")]
        public FARetryInfo RetryInfoStatusCheck { get; set; }
        #endregion

        public FAPrintronix5000TRPart(FASequenceManager aSequenceManager)
        {
            CheckStatus = new FASequence(aSequenceManager);

            CheckStatus.Steps.Add("Start", new StepInfo());
            CheckStatus.OnStart += EventHandlerOnCheckStatus;

            CheckStatus.Steps["Start"].StepIndex = CheckStatus.AddItem(GetStatus);
            CheckStatus.AddItem(ConfirmStatus);
        }

        private void EventHandlerOnCheckStatus(object sender, EventArgs e)
        {
            RetryInfoStatusCheck.ClearCount();
        }

        public override void Validate()
        {
            base.Validate();
            if (Device != null)
            {
                StatusOnlineError = !Device.StatusOnline;
                StatusPaperOutError = Device.StatusPaperOutError;
                StatusHeadOpenError = Device.StatusHeadOpenError;
                StatusBufferOverflow = Device.StatusBufferOverflow;
                StatusRibbonOut = Device.StatusRibbonOut;
            }
        }

        public override void SetDevice(Device.FADevice aDevice)
        {
            if (aDevice is FADeviceZebraPrinter)
            {
                Device = aDevice as FADeviceZebraPrinter;
                Device.OnWrite += Write;
            }
            else
                throw new Exception("Device Type is not correct." + " Device Name : " + Name + " Device Type : " + aDevice.GetType().ToString());
        }

        [FAAttribute("Operation")]
        public void PrintScript(object sender)
        {
            PrintScript();
        }

        [FAAttribute("Operation")]
        public void GetStatus(object sender)
        {
            GetStatus();
        }

        public void PrintScript(object sender, string script)
        {
            try
            {
                Device.PrintScript(script);
            }
            catch
            {
            }
        }

        private void PrintScript()
        {
            try
            {
                Device.PrintScript(Script);
            }
            catch
            {
            }
        }

        private void GetStatus()
        {
            try
            {
                Device.SendStatusCheckCommand();
            }
            catch
            {
            }
        }

        private void Write(object sender, FAGenericEventArgs<string> e)
        {            
        }

        private void ConfirmStatus(FASequence actor, TimeSpan time)
        {
            if (StatusOnlineError == false &&
                StatusBufferOverflow == false &&
                StatusHeadOpenError == false &&
                StatusPaperOutError == false &&
                StatusRibbonOut == false)
            {
                actor.NextStep();
            }            
            else if (TimeStatusCheckTimeout.Time < time)
            {
                if (RetryInfoStatusCheck.IncreaseCount())
                {
                    actor.NextStep("Start");
                }
                else
                {
                    if (StatusOnlineError)
                    {
                        FAAlarmManager.Instance.RaiseAlarm(actor, AlarmLabelPrinterOnlineError);
                    }
                    else if (StatusBufferOverflow)
                    {
                        FAAlarmManager.Instance.RaiseAlarm(actor, AlarmLabelPrinterBufferOverflowError);
                    }
                    else if (StatusHeadOpenError)
                    {
                        FAAlarmManager.Instance.RaiseAlarm(actor, AlarmLabelPrinterHeadOpenError);
                    }
                    else if (StatusPaperOutError)
                    {
                        FAAlarmManager.Instance.RaiseAlarm(actor, AlarmLabelPrinterPaperOutError);
                    }
                    else if (StatusRibbonOut)
                    {
                        FAAlarmManager.Instance.RaiseAlarm(actor, AlarmLabelPrinterRibbonOutError);
                    }

                    actor.NextStep("Start");
                }
            }
        }
    }
}
