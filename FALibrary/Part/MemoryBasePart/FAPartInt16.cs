using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using FALibrary.Device;
using FALibrary.Device.MemoryBaseDevice;

namespace FALibrary.Part.MemoryBasePart
{
    public class FAPartInt16 : FAMemoryBaseNumericPart
    {
        private Int16 _inputData;
        [FAAttribute("")]
        public Int16 InputData
        {
            get { return _inputData; }
            set
            {
                if (_inputData == value) return;
                _inputData = value;
                NotifyPropertyChanged("InputData");             
            }
        }

        private Int16 _outputData;
        [FAAttribute("")]
        public Int16 OutputData
        {
            get { return _outputData; }
            set
            {
                if (_outputData == value) return;
                _outputData = value;
                NotifyPropertyChanged("OutputData");
                WriteOutputData();
            }
        }

        public FAPartInt16()
        {
            InputSize = sizeof(Int16);
            OutputSize = sizeof(Int16);
        }

        public override void Validate()
        {
            base.Validate();
            SetInputData();
        }

        private void SetInputData()
        {
            InputData = BitConverter.ToInt16(InputBytes, 0);
        }

        private void WriteOutputData()
        {            
            WriteOutputData(BitConverter.GetBytes(OutputData));
        }
    }
}
