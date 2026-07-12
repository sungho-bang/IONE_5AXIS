using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using FALibrary.Device;
using FALibrary.Device.MemoryBaseDevice;

namespace FALibrary.Part.MemoryBasePart
{
    public class FAMemoryBaseNumericPart : FAPart
    {
        public FAPartInputIOInfo InputStartIO { get; set; }
        public FAPartOutputIOInfo OutputStartIO { get; set; }
        protected int InputSize { get; set; }
        protected int OutputSize { get; set; }
        protected byte[] InputBytes { get; set; }        

        private FAMemoryBaseDevice _device = null;

        protected FAMemoryBaseDevice Device
        {
            get { return _device; }
            private set { _device = value; }
        }

        public FAMemoryBaseNumericPart()
        {
            SimulationModeChanged += SimulationModeChangedEventHandler;
        }

        private void SimulationModeChangedEventHandler(object sender, FAGenericEventArgs<bool> e)
        {
        }

        protected void CreateInputOutputBytes(int inputSize, int outputSize)
        {
            if (inputSize > 0)
                InputBytes = new byte[inputSize];
        }

        public override void SetDevice(FADevice aDevice)
        {
            if (aDevice is FAMemoryBaseDevice)
                Device = aDevice as FAMemoryBaseDevice;
            else
                throw new Exception("Device Type is not correct.");
        }

        private bool _loadedParameters = false;

        public override void LoadParameters(XElement xml)
        {
            base.LoadParameters(xml);

            if (_loadedParameters == false)
            {
                CreateInputOutputBytes(InputSize, OutputSize);

                if (xml.Element("InputStartIO") != null)
                {
                    var ioName = xml.Element("InputStartIO").Value;
                    FAIOInfo ioInfo = Device.GetInputIOInfo(ioName);
                    if (ioInfo == null)
                        throw new Exception("Not Found IO(" + ioName + ") In IOList. Part Name is " + Name);

                    InputStartIO = new FAPartInputIOInfo(Device, ioName, ioInfo.Index, ioInfo.Description);
                }

                if (xml.Element("OutputStartIO") != null)
                {
                    var ioName = xml.Element("OutputStartIO").Value;
                    FAIOInfo ioInfo = Device.GetOutputIOInfo(ioName);
                    if (ioInfo == null)
                        throw new Exception("Not Found IO(" + ioName + ") In IOList. Part Name is " + Name);

                    OutputStartIO = new FAPartInputIOInfo(Device, ioName, ioInfo.Index, ioInfo.Description);
                }

                _loadedParameters = true;
            }            
        }

        public override void Validate()
        {
            base.Validate();
            if (SimulationMode == false)
                ReadInputData();
        }

        private void ReadInputData()
        {
            if (SimulationMode == false)
            {
                if (InputStartIO != null)
                    Device.GetInputIOBytes(InputStartIO.Index, InputBytes);
            }
        }

        protected void WriteOutputData(byte[] outputBytes)
        {
            if (SimulationMode == false)
            {
                if (OutputStartIO != null)
                    Device.SetOutputIOBytes(OutputStartIO.Index, outputBytes);
            }
        }
    }
}
