using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FALibrary.Part.MemoryBasePart
{
    public class FAUnsignedBitsNumber : FAMemoryBasePart
    {
        private ulong _inputValue;
        [FAAttribute("Status")]
        public ulong InputValue
        {
            get { return _inputValue; }
            set
            {
                if (_inputValue == value) return;
                _inputValue = value;
                NotifyPropertyChanged("InputValue");
                ConvertedInputValue.Value = value;
            }
        }

        private ulong _outputValue;
        [FAAttribute("Status")]
        public ulong OutputValue
        {
            get { return _outputValue; }
            set
            {
                if (_outputValue == value) return;
                _outputValue = value;
                SetOutputBits(value);
                NotifyPropertyChanged("OutputValue");
                ConvertedOutputValue.Value = value;
            }
        }

        private Indicator.IndicatorValue _convertedInputValue = new Indicator.IndicatorValue();
        [FAAttribute("Status")]
        [FASerializable]
        public Indicator.IndicatorValue ConvertedInputValue
        {
            get { return _convertedInputValue; }
            set
            {
                if (_convertedInputValue == value) return;
                _convertedInputValue = value;
                NotifyPropertyChanged("ConvertedInputValue");
            }
        }

        private Indicator.IndicatorValue _convertedOutputValue = new Indicator.IndicatorValue();
        [FAAttribute("Status")]
        [FASerializable]
        public Indicator.IndicatorValue ConvertedOutputValue
        {
            get { return _convertedOutputValue; }
            set
            {
                if (_convertedOutputValue == value) return;
                _convertedOutputValue = value;
                NotifyPropertyChanged("_convertedOutputValue");
            }
        }

        public override void Validate()
        {
            base.Validate();

            InputValue = GetInputBits();
        }

        private ulong GetInputBits()
        {
            int len = InputIO.Count;
            if (len > sizeof(ulong) * 8)
                len = sizeof(ulong) * 8;

            UInt64 value = 0;

            for (int i = 0; i < len; i++)
            {
                UInt64 bit = 1;

                if (InputIO[i].Value == true)
                    value = value | (bit << i);
            }

            return value;
        }

        private void SetOutputBits(ulong number)
        {            
            int len = OutputIO.Count;
            if (len > sizeof(ulong) * 8)
                len = sizeof(ulong) * 8;

            for (int i = 0; i < len; i++)
            {
                ulong compareBit = 1;
                compareBit = compareBit << i;

                if ((number & compareBit) == 0)
                    OutputIO[i].Value = false;
                else
                    OutputIO[i].Value = true;                
            }
        }
    }
}
