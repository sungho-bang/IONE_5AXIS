using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Sequence;
using FALibrary.Utility;

namespace FALibrary.Part.ScannerPart
{
    public class FAKeyenceIV500CA : MemoryBasePart.FAMemoryBasePart
    {
        public readonly int RESULT_INPUT_INDEX = 0;
        public readonly int BUSY_INPUT_INDEX = 1;
        public readonly int ERROR_INPUT_INDEX = 2;
        public readonly int RESULT_FAIL_INDEX = 3;

        public readonly int TRIGGER_OUTPUT_INDEX = 0;
        public readonly int PROGRAM_OUTPUT_BIT_1_INDEX = 1;
        public readonly int PROGRAM_OUTPUT_BIT_2_INDEX = 2;
        public readonly int PROGRAM_OUTPUT_BIT_3_INDEX = 3;
        public readonly int PROGRAM_OUTPUT_BIT_4_INDEX = 4;
        public readonly int PROGRAM_OUTPUT_BIT_5_INDEX = 5;

        [FAAttribute("Action")]
        public FAPartAction Scan { get; private set; }

        #region Time
        [FAAttribute("Time")]
        public FATime TimeScanStartDelay { get; set; }
        [FAAttribute("Time")]
        public FATime TimeScanTimeout { get; set; }
        #endregion

        #region Status
        private bool _judgeOk;
        [FAAttribute("Status")]
        public bool JudgeOk
        {
            get { return _judgeOk; }
            set
            {
                if (_judgeOk == value) return;
                _judgeOk = value;
                NotifyPropertyChanged("JudgeOk");
            }
        }

        private bool _result;
        [FAAttribute("Status")]
        public bool Result
        {
            get { return _result; }
            set
            {
                if (InputIO.Count > RESULT_INPUT_INDEX)
                {
                    if (_result == value) return;
                    _result = value;
                    NotifyPropertyChanged("Result");
                }
            }
        }

        private bool _resultFail;
        [FAAttribute("Status")]
        public bool ResultFail
        {
            get { return _resultFail; }
            set
            {
                if (InputIO.Count > RESULT_FAIL_INDEX)
                {
                    if (_resultFail == value) return;
                    _resultFail = value;
                    NotifyPropertyChanged("ResultFail");
                }
            }
        }

        private bool _busy;
        [FAAttribute("Status")]
        public bool Busy
        {
            get { return _busy; }
            set
            {
                if (InputIO.Count > BUSY_INPUT_INDEX)
                {
                    if (_busy == value) return;
                    _busy = value;
                    NotifyPropertyChanged("Busy");
                }
            }
        }

        private bool _error;
        [FAAttribute("Status")]
        public bool Error
        {
            get { return _error; }
            set
            {
                if (InputIO.Count > ERROR_INPUT_INDEX)
                {
                    if (_error == value) return;
                    _error = value;
                    NotifyPropertyChanged("Error");
                }
            }
        }

        [FAAttribute("Status")]
        public bool ProgramOutputBit1
        {
            get
            {
                if (OutputIO.Count > PROGRAM_OUTPUT_BIT_1_INDEX)
                    return OutputIO[PROGRAM_OUTPUT_BIT_1_INDEX].CorrectionValue;
                else
                    return false;
            }

            set
            {
                if (OutputIO[PROGRAM_OUTPUT_BIT_1_INDEX].CorrectionValue == value) return;
                OutputIO[PROGRAM_OUTPUT_BIT_1_INDEX].CorrectionValue = value;
                NotifyPropertyChanged("ProgramOutputBit1");
            }
        }

        [FAAttribute("Status")]
        public bool ProgramOutputBit2
        {
            get
            {
                if (OutputIO.Count > PROGRAM_OUTPUT_BIT_2_INDEX)
                    return OutputIO[PROGRAM_OUTPUT_BIT_2_INDEX].CorrectionValue;
                else
                    return false;
            }

            set
            {
                if (OutputIO[PROGRAM_OUTPUT_BIT_2_INDEX].CorrectionValue == value) return;
                OutputIO[PROGRAM_OUTPUT_BIT_2_INDEX].CorrectionValue = value;
                NotifyPropertyChanged("ProgramOutputBit2");
            }
        }

        [FAAttribute("Status")]
        public bool ProgramOutputBit3
        {
            get
            {
                if (OutputIO.Count > PROGRAM_OUTPUT_BIT_3_INDEX)
                    return OutputIO[PROGRAM_OUTPUT_BIT_3_INDEX].CorrectionValue;
                else
                    return false;
            }

            set
            {
                if (OutputIO[PROGRAM_OUTPUT_BIT_3_INDEX].CorrectionValue == value) return;
                OutputIO[PROGRAM_OUTPUT_BIT_3_INDEX].CorrectionValue = value;
                NotifyPropertyChanged("ProgramOutputBit3");
            }
        }

        [FAAttribute("Status")]
        public bool ProgramOutputBit4
        {
            get
            {
                if (OutputIO.Count > PROGRAM_OUTPUT_BIT_4_INDEX)
                    return OutputIO[PROGRAM_OUTPUT_BIT_4_INDEX].CorrectionValue;
                else
                    return false;
            }

            set
            {
                if (OutputIO[PROGRAM_OUTPUT_BIT_4_INDEX].CorrectionValue == value) return;
                OutputIO[PROGRAM_OUTPUT_BIT_4_INDEX].CorrectionValue = value;
                NotifyPropertyChanged("ProgramOutputBit4");
            }
        }

        [FAAttribute("Status")]
        public bool ProgramOutputBit5
        {
            get
            {
                if (OutputIO.Count > PROGRAM_OUTPUT_BIT_5_INDEX)
                    return OutputIO[PROGRAM_OUTPUT_BIT_5_INDEX].CorrectionValue;
                else
                    return false;
            }

            set
            {
                if (OutputIO[PROGRAM_OUTPUT_BIT_5_INDEX].CorrectionValue == value) return;
                OutputIO[PROGRAM_OUTPUT_BIT_5_INDEX].CorrectionValue = value;
                NotifyPropertyChanged("ProgramOutputBit5");
            }
        }

        private byte _programNo;
        [FAAttribute("Status")]
        public byte ProgramNo
        {
            get { return _programNo; }
            set
            {
                if (GetProgramBits() == value) return;
                SetProgramBits(value);
                _programNo = value;
                NotifyPropertyChanged("ProgramNo");
            }
        }
        #endregion

        
        public FAKeyenceIV500CA(FASequenceManager aSequenceManager)
        {
            MakeScan(aSequenceManager);
        }

        public override void Validate()
        {
            base.Validate();

            if (InputIO.Count > RESULT_INPUT_INDEX)
                Result = InputIO[RESULT_INPUT_INDEX].CorrectionValue;

            if (InputIO.Count > RESULT_FAIL_INDEX)
                ResultFail = InputIO[RESULT_FAIL_INDEX].CorrectionValue;

            if (InputIO.Count > BUSY_INPUT_INDEX)
                Busy = InputIO[BUSY_INPUT_INDEX].CorrectionValue;

            if (InputIO.Count > ERROR_INPUT_INDEX)
                Error = InputIO[ERROR_INPUT_INDEX].CorrectionValue;
        }

        [FAAttribute("Operation")]
        public void TriggerOn(object sender)
        {
            if (OutputIO.Count > TRIGGER_OUTPUT_INDEX)
            {
                OutputIO[TRIGGER_OUTPUT_INDEX].CorrectionValue = true;
            }
        }

        [FAAttribute("Operation")]
        public void TriggerOff(object sender)
        {
            if (OutputIO.Count > TRIGGER_OUTPUT_INDEX)
            {
                OutputIO[TRIGGER_OUTPUT_INDEX].CorrectionValue = false;
            }
        }        

        private void MakeScan(FASequenceManager aSequenceManager)
        {
            Scan = new FAPartAction();
            Scan.CreateSequence(aSequenceManager);
            Scan.SetActionMethod(TriggerOn);

            var seq = Scan.Sequence;

            seq.OnStart += delegate { JudgeOk = false; TriggerOff(this); };
            seq.OnStop += delegate { TriggerOff(this); };
            seq.OnTerminate += delegate { TriggerOff(this); };

            seq.AddItem((object obj) => JudgeOk = false);
            seq.AddItem(TimeScanStartDelay);
            seq.AddItem(TriggerOff);
            seq.AddItem(TriggerOn);
            seq.AddItem(
                delegate(FASequence actor, TimeSpan time)
                {
                    if (ResultFail)
                    {
                        JudgeOk = false;
                        actor.NextStep();
                    }
                    else if (Result)
                    {
                        JudgeOk = true;                        
                        actor.NextStep();
                    }
                    else if (TimeScanTimeout.Time < time)
                    {
                        JudgeOk = false;                        
                        actor.NextStep();
                    }
                });
        }

        private byte GetProgramBits()
        {
            byte result = 0;
            if (ProgramOutputBit1)
                result += 1;

            if (ProgramOutputBit2)
                result += 2;

            if (ProgramOutputBit3)
                result += 4;

            if (ProgramOutputBit4)
                result += 8;

            if (ProgramOutputBit5)
                result += 16;

            return result;
        }

        private void SetProgramBits(byte n)
        {
            if ((n & 1) != 0) ProgramOutputBit1 = true;
            else ProgramOutputBit1 = false;

            if ((n & 2) != 0) ProgramOutputBit2 = true;
            else ProgramOutputBit2 = false;

            if ((n & 4) != 0) ProgramOutputBit3 = true;
            else ProgramOutputBit3 = false;

            if ((n & 8) != 0) ProgramOutputBit4 = true;
            else ProgramOutputBit4 = false;

            if ((n & 16) != 0) ProgramOutputBit5 = true;
            else ProgramOutputBit5 = false;
        }
    }
}
