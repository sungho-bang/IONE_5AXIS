using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using FALibrary.Device;
using FALibrary.Device.MemoryBaseDevice;

namespace FALibrary.Part.MemoryBasePart
{
    public class FAPartDouble : FAMemoryBaseNumericPart
    {
        private double _inputData;
        [FAAttribute("")]
        public double InputData
        {
            get { return _inputData; }
            set
            {
                if (_inputData == value) return;
                _inputData = value;
                NotifyPropertyChanged("InputData");
            }
        }

        private double _outputData;
        [FAAttribute("")]
        public double OutputData
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

        private double _inputOffset = 1;
        [FAAttribute("")]
        [FAPropertyAttribute]
        public double InputOffset
        {
            get { return _inputOffset; }
            set
            {
                if (_inputOffset == value) return;
                _inputOffset = value;
                NotifyPropertyChanged("InputOffset");
            }
        }

        private double _inputScale = 1;
        [FAAttribute("")]
        [FAPropertyAttribute]
        public double InputScale
        {
            get { return _inputScale; }
            set
            {
                if (_inputScale == value) return;
                _inputScale = value;
                NotifyPropertyChanged("InputScale");
            }
        }

        private double _outputScale = 1;
        [FAAttribute("")]
        [FAPropertyAttribute]
        public double OutputScale
        {
            get { return _outputScale; }
            set
            {
                if (_outputScale == value) return;
                _outputScale = value;
                NotifyPropertyChanged("OutputScale");
            }
        }

        public FAPartDouble()
        {
            InputSize = sizeof(double);
            OutputSize = sizeof(double);
        }

        public override void Validate()
        {
            base.Validate();
            SetInputData();
        }

        private void SetInputData()
        {
            InputData = InputOffset + BitConverter.ToDouble(InputBytes, 0) * InputScale;
        }

        private void WriteOutputData()
        {            
            WriteOutputData(BitConverter.GetBytes(OutputData * OutputScale));
        }
    }
}
