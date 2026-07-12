using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary;
using FALibrary.Sequence;
using FALibrary.Utility;
using FAFramework.Utility;

namespace FAFramework.Module
{
    public class FADynamicPopingStacker : FAInlineModule
    {
        #region Sequences
        [FAAttribute("Sequences")]
        public FASequence Loop { get; set; }
        [FAAttribute("Sequences")]
        public FASequence PreLoading { get; set; }
        [FAAttribute("Sequences")]
        public FASequence Loading { get; set; }
        [FAAttribute("Sequences")]
        public FASequence Process { get; set; }
        [FAAttribute("Sequences")]
        public FASequence PrePop { get; set; }
        [FAAttribute("Sequences")]
        public FASequence Pop { get; set; }
        [FAAttribute("Sequences")]
        public FASequence ActionAfterPrePop { get; set; }
        #endregion

        #region Status
        private DateTime _loadingStartTime = DateTime.MinValue;
        [FAAttribute("Status")]
        public DateTime LoadingStartTime
        {
            get { return _loadingStartTime; }
            set
            {
                if (_loadingStartTime == value) return;
                _loadingStartTime = value;
                NotifyPropertyChanged("LoadingStartTime");
            }
        }

        private DateTime _lastDateTime = DateTime.MinValue;
        [FAAttribute("Status")]
        public DateTime LastDateTime
        {
            get { return _lastDateTime; }
            set
            {
                if (_lastDateTime == value) return;
                _lastDateTime = value;
                NotifyPropertyChanged("LastDateTime");
            }
        }

        private bool _popRequest = false;
        [FAAttribute("Status")]
        public bool PopRequest
        {
            get { return _popRequest; }
            set
            {
                if (_popRequest == value) return;
                _popRequest = value;
                NotifyPropertyChanged("PopRequest");
            }
        }

        private bool _popStandby;
        [FAAttribute("Status")]
        public bool PopStandby
        {
            get { return _popStandby; }
            set
            {
                if (_popStandby == value) return;
                _popStandby = value;
                NotifyPropertyChanged("PopStandby");
            }
        }

        private bool _popTerminated;
        [FAAttribute("Status")]
        public bool PopTerminated
        {
            get { return _popTerminated; }
            set
            {
                if (_popTerminated == value) return;
                _popTerminated = value;
                NotifyPropertyChanged("PopTerminated");

                if (value)
                    PopStandby = false;
            }
        }
        #endregion

        public override void InitializeSequence()
        {
            MakeLoop();
            MakePop();
        }

        public override void ClearProductInfo()
        {
            base.ClearProductInfo();

            LoadingStartTime = DateTime.MinValue;
            LastDateTime = DateTime.MinValue;
            PopStandby = false;
            PopTerminated = false;
            PopRequest = false;
        }

        public void RequestPoping()
        {
            PopRequest = true;
        }

        protected virtual bool IsEmptyStacker()
        {
            return true;
        }

        protected virtual void MakeLoop()
        {
            var seq = Loop;

            seq.AddStep("LoopHead").StepIndex = seq.AddItem((object obj) => TransferTerminatedFromFrontModule = false);
            seq.AddStep("PreLoading").StepIndex = seq.AddItem(PreLoading);
            seq.AddStep("ConfirmFrontModuleExist").StepIndex = seq.AddItem(ConfirmFrontModuleExist);
            seq.AddStep("ConfirmFrontModuleTransferReady").StepIndex = seq.AddItem(ConfirmFrontModuleTransferReady);
            seq.AddStep("Loading").StepIndex = seq.AddItem(Loading);
            seq.AddStep("ActionAfterLoading").StepIndex = seq.AddItem(ActionAfterLoading);
            seq.AddStep("Process").StepIndex = seq.AddItem(Process);
            seq.AddStep("ConfirmNextModuleExist").StepIndex = seq.AddItem(ConfirmNextModuleExist);
            seq.AddStep("ConfirmNextModuleTransferReadOff").StepIndex = seq.AddItem(ConfirmNextModuleTransferReadOff);
            seq.AddStep("ConfirmNextModuleTransferReadyOn").StepIndex = seq.AddItem(ConfirmNextModuleTransferReadyOn);
            seq.AddItem(ProductDataCopyToNextMachine);
            seq.AddStep("ConfirmNextModuleTransferTerminated").StepIndex = seq.AddItem(ConfirmNextModuleTransferTerminated);
            seq.AddStep("ActionOfTerminateOneCycle").StepIndex = seq.AddItem(ActionOfTerminateOneCycle);
            seq.AddItem("LoopHead");

            seq.AddStep("Pop").StepIndex = seq.AddItem(Pop);
            seq.AddItem("LoopHead");
        }

        protected virtual void MakePop()
        {
            var seq = Pop;

            seq.AddItem(
                delegate (object obj)
                {
                    PopStandby = false;
                    PopTerminated = false;
                    PopRequest = false;
                });
            seq.AddItem(PrePop);
            seq.AddItem((object obj) => PopStandby = true);
            seq.AddItem(
                delegate (FASequence actor, TimeSpan time)
                {
                    if (PopTerminated)
                    {
                        PopRequest = false;
                        actor.NextStep();
                    }
                });
            seq.AddItem(ActionAfterPrePop);
            seq.AddItem(
                delegate (object obj)
                {
                    PopTerminated = false;
                });
        }

        private void ConfirmFrontModuleExist(FASequence actor, TimeSpan time)
        {
            if (PopRequest && IsEmptyStacker() == false)
                actor.NextStep("Pop");
            else if (ExistFrontModule)
                actor.NextStep();
            else
                actor.NextStep("Loading");
        }

        private void ConfirmFrontModuleTransferReady(FASequence actor, TimeSpan time)
        {
            if (PopRequest && IsEmptyStacker() == false)
                actor.NextStep("Pop");
            else if (FrontModuleTransferReady)
            {
                WriteDefaultTraceLog(string.Format("Start Transfer. {0} -> {1}", FrontModule.Name, this.Name));
                TransferReadyFromFrontModule = true;
                actor.NextStep();
            }
        }

        private void ActionAfterLoading(object obj)
        {
            TransferTerminatedFromFrontModule = true;
            TransferReadyFromFrontModule = false;

            if (ExistFrontModule)
                WriteDefaultTraceLog(string.Format("Terminated Transfer. {0} -> {1}", FrontModule.Name, this.Name));
            else
                WriteDefaultTraceLog(string.Format("Terminated Transfer. {0}", this.Name));
        }

        private void ConfirmNextModuleExist(FASequence actor, TimeSpan time)
        {
            if (ExistNextModule)
                actor.NextStep();
            else
                actor.NextStep("ActionOfTerminateOneCycle");
        }

        private void ConfirmNextModuleTransferReadOff(FASequence actor, TimeSpan time)
        {
            if (NextModuleTransferReady == false)
            {
                TransferReadyToNextModule = true;
                actor.NextStep();
            }
        }

        private void ConfirmNextModuleTransferReadyOn(FASequence actor, TimeSpan time)
        {
            if (NextModuleTransferReady)
                actor.NextStep();
        }

        private void ConfirmNextModuleTransferTerminated(FASequence actor, TimeSpan time)
        {
            if (NextModuleTransferTerminated)
            {
                TransferReadyToNextModule = false;
                actor.NextStep();
            }
        }

        private void ActionOfTerminateOneCycle(object obj)
        {
            var now = DateTime.Now;

            TotalProcessTime = LoadingStartTime - now;

            if (LastDateTime == DateTime.MinValue)
                TactTime = TotalProcessTime;
            else
                TactTime = LastDateTime - now;

            LastDateTime = now;

            ProductInfo.Clear();
        }

        private void ProductDataCopyToNextMachine(object sender)
        {
            if (NextModule != null)
            {
                ProductInfo.CopyTo(NextModule.ProductInfo);
            }
        }
    }
}
