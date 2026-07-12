using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Device.LanDevice.ScannerDevice.Pamtek;
using FALibrary.Sequence;
using FALibrary.Utility;
using FALibrary.Alarm;

namespace FALibrary.Part.ScannerPart
{    
    public class FAPamtekBarcodeScannerPart : FAPart
    {
        private bool _readyOk = false;
        private bool _foundOk = false;
        private List<string> _barcodes = new List<string>();

        public FAPamtekBarcodeScannerUDPDevice Device { get; private set; }

        public event EventHandler OnScan;
        public event EventHandler OnContinueScan;
        public event EventHandler OnStop;
        public event EventHandler OnReadReadyStatus;

        [FAAttribute("")]
        [FAPropertyAttribute]
        public int ScannerNo { get; set; }
        [FAAttribute("")]
        public int BarcodeCount { get; protected set; }

        #region Status
        [FAAttribute("Status")]
        public bool ReadyOk
        {
            get { return _readyOk; }
            protected set
            {
                if (_readyOk == value) return;

                _readyOk = value;
                NotifyPropertyChanged("ReadyOk");
            }
        }

        [FAAttribute("Status")]
        public bool FoundOk
        {
            get { return _foundOk; }
            protected set
            {
                if (_foundOk == value) return;

                _foundOk = value;
                NotifyPropertyChanged("FoundOk");
            }
        }

        [FAAttribute("Status")]
        public List<string> Barcodes
        {
            get { return _barcodes; }
            protected set
            {                
                _barcodes = value;
                NotifyPropertyChanged("Barcodes");
            }
        }
        #endregion

        #region Sequence
        [FAAttribute("Sequence")]
        public FASequence CheckReadyStatus { get; set; }
        #endregion

        #region Alarm
        [FAAttribute("Alarm")]
        [FAPropertyAttribute]
        public int AlarmReadyStatusFail { get; set; }
        #endregion

        #region Time
        [FAAttribute("Time")]
        public FATime TimeReadyStatusCheckTimeout { get; set; }
        #endregion

        public FAPamtekBarcodeScannerPart(FASequenceManager aSequenceManager)
        {
            CheckReadyStatus = new FASequence(aSequenceManager);

            CheckReadyStatus.AddItem(delegate(object sender) { ReadReadyStatus(); });
            CheckReadyStatus.AddItem(ConfirmReadyStatus);
        }

        #region Operation
        [FAAttribute("Operation")]
        public void Connect()
        {
            Device.Open();
        }

        [FAAttribute("Operation")]
        public void Disconnect()
        {
            Device.Close();
        }

        [FAAttribute("Operation")]
        public void Scan()
        {         
            if (OnScan != null)
                OnScan(this, EventArgs.Empty);
            Device.Scan(ScannerNo);
        }

        [FAAttribute("Operation")]
        public void ContinueScan()
        {
            ClearBarcodes();
            if (OnContinueScan != null)
                OnContinueScan(this, EventArgs.Empty);
            Device.ContinueScan(ScannerNo);
        }

        [FAAttribute("Operation")]
        public void Stop()
        {
            ClearBarcodes();
            if (OnStop != null)
                OnStop(this, EventArgs.Empty);
            Device.StopScan(ScannerNo);
        }

        [FAAttribute("Operation")]
        public void ReadReadyStatus()
        {
            if (OnReadReadyStatus != null)
                OnReadReadyStatus(this, EventArgs.Empty);
            ReadyOk = false;
            Device.ReadScannerStatus(ScannerNo);
        }
        #endregion
        
        public override void Validate()
        {
            base.Validate();

            try
            {
                ReadyOk = Device.ScanStatus[ScannerNo - 1].ReadyOk;
                FoundOk = Device.ScanStatus[ScannerNo - 1].FoundOk;

                if (Device.ScanStatus[ScannerNo - 1].BarcodeData != null &&
                    Barcodes != null)
                {
                    for (int i = 0; i < BarcodeCount; i++)
                    {
                        if (Device.ScanStatus[ScannerNo - 1].BarcodeData.Length > i)
                        {
                            Barcodes[i] = Device.ScanStatus[ScannerNo - 1].BarcodeData[i];
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public override void SetDevice(Device.FADevice aDevice)
        {
            if (aDevice is FAPamtekBarcodeScannerUDPDevice)
                Device = aDevice as FAPamtekBarcodeScannerUDPDevice;
            else
                throw new Exception("Device Type is not correct." + " Device Name : " + Name + " Device Type : " + aDevice.GetType().ToString());
        }

        private bool _loadedParameters = false;

        public override void LoadParameters(System.Xml.Linq.XElement xml)
        {
            base.LoadParameters(xml);

            if (_loadedParameters == false)
            {
                _loadedParameters = true;

                for (int i = 0; i < BarcodeCount; i++)
                {
                    Barcodes.Add("");
                }
            }
        }

        protected void ClearBarcodes()
        {
            for (int i = 0; i < Barcodes.Count; i++)
                Barcodes[i] = "";
        }

        protected void ConfirmReadyStatus(FASequence actor, TimeSpan time)
        {
            if (ReadyOk)
            {
                actor.NextStep();
            }
            else if (TimeReadyStatusCheckTimeout.Time < time)
            {
                FAAlarmManager.Instance.RaiseAlarm(actor, AlarmReadyStatusFail);
                actor.NextStep("Start");
            }
        }
    }
}
