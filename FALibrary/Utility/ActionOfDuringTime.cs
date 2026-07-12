using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Sequence;

namespace FALibrary.Utility
{
    public class ActionOfDuringTime
    {
        public FATime Time { get; set; }
        public FASequence Sequence { get; set; }

        protected Func<bool> MethodOfDuringTime { get; set; }
        protected Func<bool> MethodOfEndTime { get; set; }

        public void SetActionOfDuringTime(Action method)
        {
            MethodOfDuringTime =
                delegate()
                {
                    method();
                    return false;
                };
        }

        public void SetActionOfDuringTime(Action<object> method)
        {
            MethodOfDuringTime =
                delegate()
                {
                    method(this);
                    return false;
                };
        }

        public void SetActionOfDuringTime(Func<bool> method)
        {
            MethodOfDuringTime = method;
        }

        public void SetActionOfEndTime(Action method)
        {
            MethodOfEndTime =
                delegate()
                {
                    method();
                    return true;
                };
        }

        public void SetActionOfEndTime(Action<object> method)
        {
            MethodOfEndTime =
                delegate()
                {
                    method(this);
                    return true;
                };
        }

        public void SetActionOfEndTime(Func<bool> method)
        {
            MethodOfEndTime = method;
        }

        public void Initialize(bool isAtomic)
        {
            Sequence.Steps.Add("ConfirmTime", new StepInfo());
            Sequence.Steps.Add("Retry", new StepInfo());

            if (isAtomic)
                Sequence.AddAtomicItem(ConfirmTime);
            else
                Sequence.AddItem(ConfirmTime);

            Sequence.AddTerminate();

            Sequence.Steps["Retry"].StepIndex = Sequence.AddItem("ConfirmTime");
        }

        public void ConfirmTime(FASequence actor, TimeSpan time)
        {
            if (Time.Time < time)
            {
                if (MethodOfEndTime != null)
                {
                    if (MethodOfEndTime())
                        actor.NextStep();
                    else
                        actor.NextStep("Retry");
                }
                else
                    actor.NextStep();
            }
            else
            {
                if (MethodOfDuringTime != null)
                {
                    if (MethodOfDuringTime())
                        actor.NextStep();
                    else
                        actor.NextStep("Retry");
                }
            }
        }
    }
}
