using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using FALibrary.Device;
using FALibrary.Device.MemoryBaseDevice;

namespace FALibrary.Part.MemoryBasePart
{
    public class FAPartSByte : FAMemoryBaseNumericPart
    {
        private SByte _inputData;
        [FAAttribute("")]
        public SByte InputData
        {
            get { return _inputData; }
            set
            {                
                if (_inputData == value) return;
                _inputData = value;
                NotifyPropertyChanged("InputData");
            }
        }

        private SByte _outputData;
        [FAAttribute("")]
        public SByte OutputData
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

        private Byte[] _outputBuffer = new Byte[1];

        public FAPartSByte()
        {
            InputSize = 1;
            OutputSize = 1;
        }

        public override void Validate()
        {
            base.Validate();
            SetInputData();
        }

        private void SetInputData()
        {
            InputData = (SByte)InputBytes[0];
        }

        private void WriteOutputData()
        {
            _outputBuffer[0] = (Byte)OutputData;
            WriteOutputData(_outputBuffer);
        }
    }
}
