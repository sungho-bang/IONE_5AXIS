using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FALibrary.Part.MemoryBasePart
{
    // most significant bit is sign bit
    public class FAPartMSBSignInteger : FAMemoryBasePart
    {
        private long _inputValue;
        [FAAttribute("Status")]
        public long InputValue
        {
            get { return _inputValue; }
            set
            {
                if (_inputValue == value) return;
                _inputValue = value;
                NotifyPropertyChanged("InputValue");
            }
        }

        private long _outputValue;
        [FAAttribute("Status")]
        public long OutputValue
        {
            get { return _outputValue; }
            set
            {
                if (_outputValue == value) return;
                _outputValue = value;
                SetOutputBits(value);
                NotifyPropertyChanged("OutputValue");
            }
        }

        public override void Validate()
        {
            base.Validate();

            if(SimulationMode==false)
                InputValue = GetInputBits();
        }

        private long GetInputBits()
        {
            int len = InputIO.Count;
            if (len <= 0) return 0;
            if (len > sizeof(ulong) * 8)
                len = sizeof(ulong) * 8;

            int msbIndex = InputIO.Count - 1;
            ulong value = 0;

            for (int i = 0; i < msbIndex; i++)
            {
                ulong bit = 1;

                if (InputIO[i].Value == true)
                    value = value | (bit << i);
            }

            if (InputIO[msbIndex].Value == true)
                return -(long)value;
            else
                return (long)value;
        }

        private void SetOutputBits(long number)
        {
            int len = OutputIO.Count;
            if (len > sizeof(ulong) * 8)
                len = sizeof(ulong) * 8;

            int msbIndex = InputIO.Count - 1;
            for (int i = 1; i < msbIndex; i++)
            {
                long compareBit = 1;
                compareBit = compareBit << i;

                if ((number & compareBit) == 0)
                    OutputIO[i].Value = false;
                else
                    OutputIO[i].Value = true;
            }

            if (number >= 0)
                OutputIO[msbIndex].Value = false;
            else
                OutputIO[msbIndex].Value = true;
        }
    }
}
