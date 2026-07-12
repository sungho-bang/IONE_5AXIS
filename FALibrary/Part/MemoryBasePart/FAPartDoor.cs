using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FALibrary.Part.MemoryBasePart
{
    public enum DoorLockStatus
    {
        Unknown,
        Lock,
        Unlock
    }

    public class FAPartDoor : FAMemoryBasePart
    {
        private bool _closed;
        [FAAttribute("Status")]
        public bool Closed
        {
            get { return _closed; }
            set
            {
                if (_closed == value) return;
                _closed = value;
                NotifyPropertyChanged("Closed");
            }
        }

        private DoorLockStatus _locked;
        [FAAttribute("Status")]
        public DoorLockStatus Locked
        {
            get { return _locked; }
            set
            {
                if (_locked == value) return;
                _locked = value;
                NotifyPropertyChanged("Locked");
            }
        }

        private bool _closeOnly;
        [FAAttribute("Status")]
        public bool CloseOnly
        {
            get { return _closeOnly; }
            private set
            {
                if (_closeOnly == value) return;
                _closeOnly = value;
                NotifyPropertyChanged("CloseOnly");
            }
        }

        private bool _closedStatusInverse;
        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool ClosedStatusInverse
        {
            get { return _closedStatusInverse; }
            set
            {
                if (_closedStatusInverse == value) return;
                _closedStatusInverse = value;
                NotifyPropertyChanged("ClosedStatusInverse");
            }
        }

        private bool _lockedStatusInverse;
        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool LockedStatusInverse
        {
            get { return _lockedStatusInverse; }
            set
            {
                if (_lockedStatusInverse == value) return;
                _lockedStatusInverse = value;
                NotifyPropertyChanged("LockedStatusInverse");
            }
        }

        private bool _lockedOutputInverse;
        [FAAttribute("Parameter")]
        [FAPropertyAttribute]
        public bool LockedOutputInverse
        {
            get { return _lockedOutputInverse; }
            set
            {
                if (_lockedOutputInverse == value) return;
                _lockedOutputInverse = value;
                NotifyPropertyChanged("LockedOutputInverse");
            }
        }

        [FAAttribute("Operation")]
        public void Lock()
        {
            if (LockedOutputInverse)
                Lock(false);
            else
                Lock(true);
        }

        [FAAttribute("Operation")]
        public void Unlock()
        {
            if (LockedOutputInverse)
                Lock(true);
            else
                Lock(false);
        }

        public override void Validate()
        {
            base.Validate();
            if (InputIO.Count <= 1)
                CloseOnly = true;

            SetClosed();
            SetLocked();
        }

        private void Lock(bool value)
        {
            if (OutputIO.Count > 0)
            {
                OutputIO[0].Value = value;
                if (OutputIO.Count > 1)
                    OutputIO[1].Value = !value;
            }
        }

        private void SetClosed()
        {
            if (ClosedStatusInverse)
                SetClosed(false);
            else
                SetClosed(true);
        }

        private void SetClosed(bool compareValue)
        {
            if (InputIO.Count > 0)
            {
                Closed = InputIO[0].CorrectionValue == compareValue;
            }
        }

        private void SetLocked()
        {
            if (CloseOnly)
                return;

            if (LockedStatusInverse)
                SetLocked(false);
            else
                SetLocked(true);
        }

        private void SetLocked(bool compareValue)
        {
            switch (InputIO.Count)
            {
                case 2:
                    if (InputIO[1].CorrectionValue == compareValue)
                        Locked = DoorLockStatus.Lock;
                    else
                        Locked = DoorLockStatus.Unlock;
                    return;

                case 3:
                    if (InputIO[1].CorrectionValue == compareValue &&
                        InputIO[2].CorrectionValue != compareValue)
                        Locked = DoorLockStatus.Lock;
                    else
                        Locked = DoorLockStatus.Unlock;
                    return;
            }

            Locked = DoorLockStatus.Unknown;
            return;
        }
    }
}
