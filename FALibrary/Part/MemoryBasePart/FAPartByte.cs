using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using FALibrary.Device;
using FALibrary.Device.MemoryBaseDevice;

namespace FALibrary.Part.MemoryBasePart
{
    public class FAPartByte : FAMemoryBaseNumericPart
    {
        private byte _inputData;
        [FAAttribute("")]
        public byte InputData
        {
            get { return _inputData; }
            set
            {
                if (_inputData == value) return;
                _inputData = value;
                NotifyPropertyChanged("InputData");
            }
        }

        private byte _outputData;
        [FAAttribute("")]
        public byte OutputData
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

        private byte[] _outputBuffer = new byte[1];

        public FAPartByte()
        {
            InputSize = 1;
            OutputSize = 1;
        }

        public override void Validate()
        {
            base.Validate();
            if (SimulationMode==false)
                SetInputData();
        }

        private void SetInputData()
        {
            if (InputStartIO != null)
                InputData = InputBytes[0];
        }

        private void WriteOutputData()
        {
            if (OutputStartIO != null)
            {
                _outputBuffer[0] = OutputData;
                WriteOutputData(_outputBuffer);
            }
        }
    }
}
