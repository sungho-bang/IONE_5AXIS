using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Sequence;
using FALibrary.Utility;

namespace FALibrary.Part.StrappingMachine
{
    public class FAMoscaStrappingMachine : MemoryBasePart.FAMemoryBasePart
    {
        public FASequence TieStraps { get; set; }

        private bool _terminateStrapWhenAlarmOccurred;
        /// <summary>
        /// TieStraps 수행 중 알람이 발생한 경우 TieStraps.Result = False로 하고 Strap 시퀀스를 종료한다.
        /// </summary>
        [FAAttribute("Status")]
        [FAProperty]
        public bool TerminateStrapWhenAlarmOccurred
        {
            get
            {
                return _terminateStrapWhenAlarmOccurred;
            }

            private set
            {
                if (_terminateStrapWhenAlarmOccurred == value) return;
                _terminateStrapWhenAlarmOccurred = value;
                NotifyPropertyChanged("TerminateStrapWhenAlarmOccurred");
            }
        }

        private bool _readyForStrapping;
        [FAAttribute("Status")]
        public bool ReadyForStrapping
        {
            get
            {
                return _readyForStrapping;
            }

            private set
            {
                if (_readyForStrapping == value) return;
                _readyForStrapping = value;
                NotifyPropertyChanged("ReadyForStrapping");
            }
        }

        private bool _strapEmpty;
        [FAAttribute("Status")]
        public bool StrapEmpty
        {
            get
            {
                return _strapEmpty;
            }

            private set
            {
                if (_strapEmpty == value) return;
                _strapEmpty = value;
                NotifyPropertyChanged("StrapEmpty");
            }
        }

        private bool _error;
        [FAAttribute("Status")]
        public bool Error
        {
            get
            {
                return _error;
            }

            private set
            {
                if (_error == value) return;
                _error = value;
                NotifyPropertyChanged("Error");
            }
        }

        private bool _upperSlideOpened;
        [FAAttribute("Status")]
        public bool UpperSlideOpened
        {
            get
            {
                return _upperSlideOpened;
            }

            private set
            {
                if (_upperSlideOpened == value) return;
                _upperSlideOpened = value;
                NotifyPropertyChanged("UpperSlideOpened");
            }
        }

        [FAAttribute("Alarm")]
        [FAProperty]
        public int AlarmMachineError { get; set; }

        [FAAttribute("Alarm")]
        [FAProperty]
        public int AlarmStrapEmpty { get; set; }

        [FAAttribute("Alarm")]
        [FAProperty]
        public int AlarmUpperSlideOpened { get; set; }

        [FAAttribute("Alarm")]
        [FAProperty]
        public int AlarmNotReady { get; set; }

        [FAAttribute("Time")]
        public FATime TimeStrapOnOffInverval { get; set; }

        public FAMoscaStrappingMachine(FASequenceManager aSequenceManager)
        {
            TieStraps = new FASequence(aSequenceManager);
            var seq = TieStraps;

            seq.AddItem((o) => StrapOff());
            seq.AddItem(
                (actor, time) =>
                {
                    var alarm = -1;
                    if (Error)
                        alarm = AlarmMachineError;
                    else if (UpperSlideOpened)
                        alarm = AlarmUpperSlideOpened;
                    else if (StrapEmpty)
                        alarm = AlarmStrapEmpty;
                    else if (!ReadyForStrapping)
                        alarm = AlarmNotReady;

                    if (alarm == -1)
                        actor.NextStep();
                    else
                    {
                        Alarm.FAAlarmManager.Instance.RaiseAlarm(actor, alarm);
                        if (TerminateStrapWhenAlarmOccurred)
                        {
                            seq.Result = false;
                            actor.NextTerminate();
                        }
                    }
                });

            seq.AddItem((o) => StrapOn());
            seq.AddItem(TimeStrapOnOffInverval);
            seq.AddItem((o) => StrapOff());
        }

        public override void Validate()
        {
            base.Validate();

            if (InputIO.Count > 0)
            {
                ReadyForStrapping = InputIO[0].Value;
                StrapEmpty = InputIO[1].Value;
                Error = InputIO[2].Value;
                UpperSlideOpened = InputIO[3].Value;
            }
        }

        [FAAttribute("Operation")]
        public void StrapOn()
        {
            if (OutputIO.Count >= 1)
            {
                OutputIO[0].Value = true;
            }
        }

        [FAAttribute("Operation")]
        public void StrapOff()
        {
            if (OutputIO.Count >= 1)
            {
                OutputIO[0].Value = false;
            }
        }
    }
}
