using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using FALibrary.Device;
using FALibrary.Device.MemoryBaseDevice;
namespace FALibrary.Part.MemoryBasePart
{
    public class FAPartUInt16 : FAMemoryBaseNumericPart
    {
        private UInt16 _inputData;
        [FAAttribute("")]
        public UInt16 InputData
        {
            get { return _inputData; }
            set
            {
                if (_inputData == value) return;
                _inputData = value;
                NotifyPropertyChanged("InputData");             
            }
        }

        private UInt16 _outputData;
        [FAAttribute("")]
        public UInt16 OutputData
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

        public FAPartUInt16()
        {
            InputSize = sizeof(UInt16);
            OutputSize = sizeof(UInt16);
        }

        public override void Validate()
        {
            base.Validate();
            SetInputData();
        }

        private void SetInputData()
        {
            InputData = BitConverter.ToUInt16(InputBytes, 0);
        }

        private void WriteOutputData()
        {            
            WriteOutputData(BitConverter.GetBytes(OutputData));
        }
    }
}
