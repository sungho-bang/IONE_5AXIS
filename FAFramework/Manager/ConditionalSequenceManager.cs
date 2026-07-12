using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Sequence;
using FAFramework.Utility;

namespace FAFramework.Manager
{
    public class ConditionalSequence
    {
        public Func<bool> StartCondition { get; set; }
        public Action TerminateResult { get; set; }
        public List<FALibrary.Sequence.FASequence> Sequences { get; set; }

        public ConditionalSequence()
        {
            Sequences = new List<FASequence>();
        }

        public ConditionalSequence(Func<bool> startCondition, Action terminateResult, params FASequence[] sequences)
        {
            StartCondition = startCondition;
            TerminateResult = terminateResult;
            Sequences = new List<FASequence>();
            Sequences.AddRange(sequences);
        }

        public void Start(FASequence actor, TimeSpan time)
        {
            if (StartCondition == null)
            {
                Start();
                actor.NextStep();
            }
            else if (StartCondition() == true)
            {
                Start();
                actor.NextStep();
            }
        }

        public void ConfirmTerminated(FASequence actor, TimeSpan time)
        {
            if (Sequences == null)
            {
                TerminateResult();
                actor.NextStep();
            }
            else
            {
                foreach (var seq in Sequences)
                {
                    if (seq.IsStartable() == false)
                        return;
                }

                if (TerminateResult != null)
                    TerminateResult();

                actor.NextStep();
            }
        }

        private void Start()
        {
            foreach (var seq in Sequences)
            {
                if (seq.IsStartable() == false)
                    return;
            }

            foreach (var seq in Sequences)
            {
                seq.Start();
            }
        }
    }

    public class ConditionalSequenceManager
    {
        private Queue<Queue<ConditionalSequence>> Items { get; set; }
        private Queue<ConditionalSequence> CurrentSequenceQueue { get; set; }
        private ConditionalSequence CurrentSequence { get; set; }

        public FASequence Sequence { get; set; }

        public void InitializeSequence(FASequence sequence)
        {
            Sequence = sequence;

            var seq = Sequence;

            seq.AddStep("ConfirmQueue").StepIndex = Sequence.AddItem(ConfirmQueue);
            seq.AddItem(StartSequenceList);
            seq.AddItem(StartCurrentSequence);
            seq.AddItem(ConfirmTerminatedCurrentSequence);
            seq.AddItem("ConfirmQueue");
        }

        public void AddItem(params ConditionalSequence[] arr)
        {
            Queue<ConditionalSequence> queue = new Queue<ConditionalSequence>();
            foreach (var item in arr)
                queue.Enqueue(item);

            Items.Enqueue(queue);
        }

        public void AddItem(List<ConditionalSequence> list)
        {
            Queue<ConditionalSequence> queue = new Queue<ConditionalSequence>();
            foreach (var item in list)
                queue.Enqueue(item);

            Items.Enqueue(queue);
        }

        public void Clear()
        {
            Items.Clear();
        }

        private void ConfirmQueue(FASequence actor, TimeSpan time)
        {
            if (Items.Count > 0)
            {
                CurrentSequenceQueue = Items.Dequeue();
                actor.NextStep();
            }
        }

        private void StartSequenceList(FASequence actor, TimeSpan time)
        {
            if (CurrentSequenceQueue.Count <= 0)
                actor.NextTerminate();
            else
            {
                CurrentSequence = CurrentSequenceQueue.Dequeue();
                actor.NextStep();
            }
        }

        private void StartCurrentSequence(FASequence actor, TimeSpan time)
        {
            CurrentSequence.Start(actor, time);
        }

        private void ConfirmTerminatedCurrentSequence(FASequence actor, TimeSpan time)
        {
            CurrentSequence.ConfirmTerminated(actor, time);
        }
    }
}