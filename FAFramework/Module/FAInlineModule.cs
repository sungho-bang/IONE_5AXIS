using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary;
using FALibrary.Sequence;
using FALibrary.Utility;
namespace FAFramework.Module
{
    public abstract class FAInlineModule : FAModule
    {
        #region Parameters
        private bool _useDefaultLog = true;
        [FAAttribute("Parameters")]
        public bool UseDefaultLog
        {
            get { return _useDefaultLog; }
            set
            {
                if (_useDefaultLog == value) return;
                _useDefaultLog = value;
                NotifyPropertyChanged("UseDefaultLog");
            }
        }

        private bool _existFrontModule;
        [FAAttribute("Parameters")]
        public bool ExistFrontModule
        {
            get { return _existFrontModule; }
            set
            {
                if (_existFrontModule == value) return;
                _existFrontModule = value;
                NotifyPropertyChanged("ExistFrontModule");
            }
        }

        private bool _existNextModule;
        [FAAttribute("Parameters")]
        public bool ExistNextModule
        {
            get { return _existNextModule; }
            set
            {
                if (_existNextModule == value) return;
                _existNextModule = value;
                NotifyPropertyChanged("ExistNextModule");
            }
        }
        #endregion

        #region Status
        private bool _transferReadyToNextModule;
        [FAAttribute("Status")]
        public bool TransferReadyToNextModule
        {
            get { return _transferReadyToNextModule; }
            protected set
            {
                if (_transferReadyToNextModule == value) return;
                _transferReadyToNextModule = value;
                NotifyPropertyChanged("TransferReadyToNextModule");
            }
        }

        private bool _transferReadyFromFrontModule;
        [FAAttribute("Status")]
        public bool TransferReadyFromFrontModule
        {
            get { return _transferReadyFromFrontModule; }
            protected set
            {
                if (_transferReadyFromFrontModule == value) return;
                _transferReadyFromFrontModule = value;
                NotifyPropertyChanged("TransferReadyFromFrontModule");
            }
        }

        private bool _transferTerminatedFromFrontModule;
        [FAAttribute("Status")]
        public bool TransferTerminatedFromFrontModule
        {
            get { return _transferTerminatedFromFrontModule; }
            protected set
            {
                if (_transferTerminatedFromFrontModule == value) return;
                _transferTerminatedFromFrontModule = value;
                NotifyPropertyChanged("TransferTerminatedFromFrontModule");
            }
        }

        [FAAttribute("Status")]
        public bool FrontModuleTransferReady
        {
            get
            {
                if (FrontModule == null) return false;

                return FrontModule.TransferReadyToNextModule;
            }
        }

        [FAAttribute("Status")]
        public bool NextModuleTransferReady
        {
            get
            {
                if (NextModule == null) return false;

                return NextModule.TransferReadyFromFrontModule;
            }
        }

        [FAAttribute("Status")]
        public bool NextModuleTransferTerminated
        {
            get
            {
                if (NextModule == null) return false;

                return NextModule.TransferTerminatedFromFrontModule;
            }
        }

        // 제품 한 개가 생산되는 시간.
        private TimeSpan _tactTime;
        [FAAttribute("Status")]
        public TimeSpan TactTime
        {
            get { return _tactTime; }
            set
            {
                if (_tactTime == value) return;
                _tactTime = value;
                NotifyPropertyChanged("TactTime");
            }
        }

        // Loading부터 Unloading이 끝나기까지 시간.
        private TimeSpan _totalProcessTime;
        [FAAttribute("Status")]
        public TimeSpan TotalProcessTime
        {
            get { return _totalProcessTime; }
            set
            {
                if (_totalProcessTime == value) return;
                _totalProcessTime = value;
                NotifyPropertyChanged("TotalProcessTime");
            }
        }
        #endregion

        private FAInlineModule _frontModule;
        public FAInlineModule FrontModule
        {
            get { return _frontModule; }
            set
            {
                _frontModule = value;
                if (_frontModule == null)
                    ExistFrontModule = false;
                else
                    ExistFrontModule = true;
            }
        }

        private FAInlineModule _nextModule;
        public FAInlineModule NextModule
        {
            get { return _nextModule; }
            set
            {
                _nextModule = value;
                if (_nextModule == null)
                    ExistNextModule = false;
                else
                    ExistNextModule = true;
            }
        }

        public override void ClearProductInfo()
        {
            base.ClearProductInfo();

            TransferReadyToNextModule = false;
            TransferReadyFromFrontModule = false;
            TransferTerminatedFromFrontModule = false;
        }

        protected void WriteDefaultTraceLog(string log)
        {
            if (UseDefaultLog)
                WriteTraceLog(log);
        }

        public virtual bool ExistProduct()
        {
            return false;
        }

        public virtual void UnloadingAction()
        {
        }

        public virtual void ActionWhenUnloadingSuspended()
        {
        }

        public virtual void ActionWhenUnloadingTerminated()
        {
        }

        protected bool ExistProductOnFrontModule()
        {
            if (ExistFrontModule == false)
                return true;
            else if (FrontModule == null)
                return false;
            else
                return FrontModule.ExistProduct();
        }

        protected void UnloadingActionOfFrontModule()
        {
            if (ExistFrontModule == false)
                return;
            else if (FrontModule == null)
                return;
            else
                FrontModule.UnloadingAction();
        }

        protected void ActionWhenUnloadingSuspendedOfFrontModule()
        {
            if (ExistFrontModule == false)
                return;
            else if (FrontModule == null)
                return;
            else
                FrontModule.ActionWhenUnloadingSuspended();
        }

        protected void ActionWhenUnloadingTerminatedOfFrontModule()
        {
            if (ExistFrontModule == false)
                return;
            else if (FrontModule == null)
                return;
            else
                FrontModule.ActionWhenUnloadingTerminated();
        }
    }
}