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
    public abstract class FAUnstackModule : FAInlineModule
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
        #endregion

        public override void InitializeSequence()
        {
            MakeLoop();
        }

        public override void ClearProductInfo()
        {
            base.ClearProductInfo();

            LoadingStartTime = DateTime.MinValue;
            LastDateTime = DateTime.MinValue;
        }

        protected virtual bool IsTerminatedUnstack()
        {
            return false;
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
            seq.AddStep("ConfirmTerminatedUnstack").StepIndex = seq.AddItem(ConfirmTerminatedUnstack);
            seq.AddItem("LoopHead");
        }

        private void ConfirmFrontModuleExist(FASequence actor, TimeSpan time)
        {
            if (ExistFrontModule)
                actor.NextStep();
            else
                actor.NextStep("Loading");
        }

        private void ConfirmFrontModuleTransferReady(FASequence actor, TimeSpan time)
        {
            if (FrontModuleTransferReady)
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

        private void ConfirmTerminatedUnstack(FASequence actor, TimeSpan time)
        {
            if (IsTerminatedUnstack())
            {
                ProductInfo.Clear();
                actor.NextStep();
            }
            else
                actor.NextStep("Process");
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