using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FALibrary.Sequence
{
    public class FASequenceManager
    {
        private List<FASequence> _items = new List<FASequence>();
        private List<FASequenceManager> _sequenceManagerList = new List<FASequenceManager>();

        public FASequence this[int index]
        {
            get
            {
                if (_items.Count > index &&
                    index >= 0)
                    return _items[index];
                else
                    return null;
            }
        }

        public void Add(FASequence aSequence)
        {
            _items.Add(aSequence);
        }

        public void AddSubSequenceManager(FASequenceManager aSequenceManager)
        {
            _sequenceManagerList.Add(aSequenceManager);
        }

        public void Run()
        {
            foreach (FASequence item in _items)
            {
                item.Execute();
            }
        }

        public void Suspend()
        {
            foreach (FASequence item in _items)
            {
                var caller = item.Caller;
                item.PreSuspend();
                if (caller == null && item.Atomic == false && item.IsBackground == false)
                    item.Suspend();
            }
        }

        public void ForceSuspend()
        {
            foreach (FASequence item in _items)
            {
                item.ForceSuspend();
            }
        }

        public void Resume()
        {
            foreach (FASequence item in _items)
            {
                if (item.State == SequenceState.Suspended)
                    item.Resume();
            }
        }

        public bool IsSuspened()
        {
            foreach (FASequence item in _items)
            {
                if (item.IsBackground)
                    continue;

                if (item.State != SequenceState.Suspended &&
                    item.State != SequenceState.Available &&
                    item.State != SequenceState.Terminated &&
                    item.State != SequenceState.Aborted)
                    return false;
            }

            return true;
        }

        public void AllClearState()
        {
            foreach (FASequence item in _items)
            {
                item.ClearState();
                item.Locked = false;
            }
        }

        public void AllLock()
        {
            foreach (FASequence item in _items)
            {
                item.Locked = true;
            }
        }

        public void AllUnlock()
        {
            foreach (FASequence item in _items)
            {
                item.Locked = false;
            }
        }
    }
}
