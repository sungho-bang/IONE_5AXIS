using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FALibrary.Part.MemoryBasePart
{
    public class FARealBitsNumber : FAMemoryBasePart
    {
        private double _inputValue;
        [FAAttribute("Status")]
        public double InputValue
        {
            get { return _inputValue; }
            set
            {
                if (_inputValue == value) return;
                _inputValue = value;
                NotifyPropertyChanged("InputValue");
            }
        }

        private double _outputValue;
        [FAAttribute("Status")]
        public double OutputValue
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

            InputValue = GetInputBits();
        }

        private double GetInputBits()
        {
            int len = InputIO.Count;
            if (len > sizeof(UInt64) * 8)
                len = sizeof(UInt64) * 8;

            UInt64 value = 0;

            for (int i = 0; i < len; i++)
            {
                UInt64 bit = 1;

                if (InputIO[i].Value == true)
                    value = value | (bit << i);
            }

            var bytes = BitConverter.GetBytes(value);
            return BitConverter.ToDouble(bytes, 0);
        }

        private void SetOutputBits(double number)
        {
            var longNumber = BitConverter.ToUInt64(BitConverter.GetBytes(number), 0);

            int len = OutputIO.Count;
            if (len > sizeof(ulong) * 8)
                len = sizeof(ulong) * 8;

            for (int i = 0; i < len; i++)
            {
                ulong compareBit = 1;
                compareBit = compareBit << i;

                if ((longNumber & compareBit) == 0)
                    OutputIO[i].Value = false;
                else
                    OutputIO[i].Value = true;
            }
        }
    }
}
