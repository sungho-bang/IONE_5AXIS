using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using FALibrary.Device;
using FALibrary.Device.MemoryBaseDevice;

namespace FALibrary.Part.MemoryBasePart
{
    public class FAPartOutputIOInfo : FAObject
    {
        private string _name;
        private string _description;
        private bool _simulationMode;        

        protected FAMemoryBaseDevice Device { get; private set; }

        [FAAttribute("")]
        public virtual string Name
        {
            get
            {
                return _name;
            }

            set
            {
                if (_name == value) return;

                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        [FAAttribute("")]
        public virtual string Description
        {
            get
            {
                return _description;
            }

            set
            {
                if (_description == value) return;

                _description = value;
                NotifyPropertyChanged("Description");
            }
        }
        
        public int Index { get; private set; }

        private bool _oldValue;
        private bool _oldCorrectionValue;
        private bool _isInverse;

        [FAAttribute("")]
        [FAPropertyAttribute]
        public bool IsInverse
        {
            get { return _isInverse; }
            set
            {
                if (_isInverse == value) return;

                _isInverse = value;
                NotifyPropertyChanged("IsInverse");
            }
        }

        [FAAttribute("")]
        public virtual bool Value
        {
            get
            {                
                //if (SimulationMode)
                //    return SimulationValue;
                //else
                {
                    if (Device != null)
                        return Device.GetOutputIOValue(Index);
                    else
                        return false;
                }
            }

            set
            {
                if (IgnoreInterlock == false && InterlockManager.Instance.IsSystemInterlock() == true)
                {
                    return;
                }

                //if (SimulationMode)
                //{
                //    SimulationValue = value;
                //    NotifyPropertyChanged("Value");
                //    NotifyPropertyChanged("CorrectionValue");
                //}
                //else
                {
                    if (Device != null)
                    {
                        try
                        {
                            Device.SetOutputIOValue(Index, value);
                        }
                        catch (Exception e)
                        {
                            throw new Exception(string.Format("Raise exception when Set SimulationMode. Name={0}, Index={1}\n{2}", Name, Index, e.ToString()));
                        }

                        NotifyPropertyChanged("Value");
                        NotifyPropertyChanged("CorrectionValue");
                    }
                }
            }
        }

        [FAAttribute("")]        
        public bool CorrectionValue
        {
            get
            {
                if (IsInverse)
                    return !Value;
                else
                    return Value;
            }

            set
            {
                if (IsInverse)
                    Value = !value;
                else
                    Value = value;
            }
        }

        [FAAttribute("")]
        public virtual bool SimulationMode
        {
            get { return _simulationMode; }
            set
            {
                if (_simulationMode == value) return;

                _simulationMode = value;
                NotifyPropertyChanged("SimulationMode");
                Value = Device.GetOutputIOValue(Index);
            }
        }

        //protected bool SimulationValue { get; set; }

        public bool IgnoreInterlock { get; set; }

        public void Validate()
        {
            if (_oldValue != Value)
            {
                _oldValue = Value;
                NotifyPropertyChanged("Value");
            }

            if (_oldCorrectionValue != CorrectionValue)
            {
                _oldCorrectionValue = CorrectionValue;
                NotifyPropertyChanged("CorrectionValue");
            }
        }

        public FAPartOutputIOInfo(FAMemoryBaseDevice aDevice, string aName, int aIndex, string aDescription)
        {
            Device = aDevice;
            Name = aName;
            Index = aIndex;
            Description = aDescription;
        }

        private string BoolToOnOff(bool value)
        {
            if (value == true)
                return "On";
            else
                return "Off";
        }

        public override string ToString()
        {
            return string.Format("Name={0}. Description={1}. Value={2}", Name, Description, Value);
        }
    }

    public class FAPartInputIOInfo : FAPartOutputIOInfo
    {
        private bool _simulationMode;

        public FAPartInputIOInfo(FAMemoryBaseDevice aDevice, string aName, int aIndex, string aDescription)
            : base(aDevice, aName, aIndex, aDescription)
        {            
        }

        public override string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                base.Name = value;
            }
        }

        public override string Description
        {
            get
            {
                return base.Description;
            }
            set
            {
                base.Description = value;
            }
        }

        public override bool Value
        {
            get
            {
                //if (SimulationMode)
                //    return SimulationValue;
                //else
                {
                    if (Device != null)
                    {
                        bool result = false;
                        try
                        {
                            result = Device.GetInputIOValue(Index);
                        }
                        catch (Exception e)
                        {
                            throw new Exception(string.Format("Raise exception when Set SimulationMode. Name={0}, Index={1}\n{2}", Name, Index, e.ToString()));
                        }

                        return result;
                    }
                    else
                        return false;
                }
            }

            set
            {
                if (IgnoreInterlock == false && InterlockManager.Instance.IsSystemInterlock() == true)
                {
                    return;
                }

                if (SimulationMode)
                {
                    //SimulationValue = value;

                    Device.SetInputIOValue(Index, value);

                    NotifyPropertyChanged("Value");
                    NotifyPropertyChanged("CorrectionValue");
                }
            }
        }

        public override bool SimulationMode
        {
            get { return _simulationMode; }
            set
            {
                if (_simulationMode == value) return;

                _simulationMode = value;
                NotifyPropertyChanged("SimulationMode");
                try
                {
                    Value = Device.GetInputIOValue(Index);
                }
                catch (Exception e)
                {
                    throw new Exception(string.Format("Raise exception when Set SimulationMode. Name={0}, Index={1}\n{2}", Name, Index, e.ToString()));
                }
            }
        }
    }

    public abstract class FAMemoryBasePart : FAPart
    {
        private List<FAPartInputIOInfo> _inputIO = new List<FAPartInputIOInfo>();
        private List<FAPartOutputIOInfo> _outputIO = new List<FAPartOutputIOInfo>();

        private FAMemoryBaseDevice _device = null;

        public FAMemoryBasePart()
        {
            SimulationModeChanged += SimulationModeChangedEventHandler;
        }

        protected FAMemoryBaseDevice Device
        {
            get { return _device; }
            private set { _device = value; }
        }

        [FAAttribute("Status")]
        public List<FAPartInputIOInfo> InputIO
        {
            get { return _inputIO; }
        }

        [FAAttribute("Status")]
        public List<FAPartOutputIOInfo> OutputIO
        {
            get { return _outputIO; }
        }
        
        [FAAttribute("")]
        [FAPropertyAttribute]
        public override bool IgnoreInterlock
        {
            get { return base.IgnoreInterlock; }
            set
            {
                if (base.IgnoreInterlock == value) return;

                SetIgnoreInterlockOfIOList(value);

                base.IgnoreInterlock = value;
            }
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
                if (xml.Element("InputIOList") != null)
                    LoadInputIOList(xml.Element("InputIOList"));

                if (xml.Element("OutputIOList") != null)
                    LoadOutputIOList(xml.Element("OutputIOList"));

                SetIgnoreInterlockOfIOList(IgnoreInterlock);

                _loadedParameters = true;
            }
        }

        private void LoadInputIOList(XElement xml)
        {
            foreach (XElement item in xml.Elements())
            {
                string ioName;
                bool isInverse = false;

                if (item.HasElements)
                {
                    ioName = item.Element("IOName").Value;
                    isInverse = bool.Parse(item.Element("IsInverse").Value);
                }
                else
                {
                    ioName = item.Value;
                }
                
                FAIOInfo ioInfo = Device.GetInputIOInfo(ioName);
                if (ioInfo == null)
                    throw new Exception("Not Found IO(" + ioName + ") In IOList. Part Name is " + Name);

                var obj = new FAPartInputIOInfo(Device, ioName, ioInfo.Index, ioInfo.Description);
                obj.IsInverse = isInverse;
                this.InputIO.Add(obj);
            }
        }

        private void LoadOutputIOList(XElement xml)
        {
            foreach (XElement item in xml.Elements())
            {
                string ioName;
                bool isInverse = false;

                if (item.HasElements)
                {
                    ioName = item.Element("IOName").Value;
                    isInverse = bool.Parse(item.Element("IsInverse").Value);
                }
                else
                {
                    ioName = item.Value;
                }

                FAIOInfo ioInfo = Device.GetOutputIOInfo(ioName);
                if (ioInfo == null)
                    throw new Exception("Not Found IO(" + ioName + ") In IOList. Part Name is " + Name);

                var obj = new FAPartOutputIOInfo(Device, ioName, ioInfo.Index, ioInfo.Description);
                obj.IsInverse = isInverse;
                this.OutputIO.Add(obj);
            }
        }

        public override void Validate()
        {
            foreach (FAPartInputIOInfo item in InputIO)
                item.Validate();

            foreach (FAPartOutputIOInfo item in OutputIO)
                item.Validate();
        }

        public void AllOffOutput(object sender)
        {
            foreach (FAPartOutputIOInfo io in OutputIO)
            {
                io.Value = false;
            }

            if (SimulationMode)
            {
                foreach (FAPartInputIOInfo io in InputIO)
                {
                    io.Value = false;
                }
            }
        }

        private void SimulationModeChangedEventHandler(object sender, FAGenericEventArgs<bool> e)
        {
            foreach (FAPartInputIOInfo item in InputIO)
                item.SimulationMode = e.Value;

            foreach (FAPartOutputIOInfo item in OutputIO)
                try
                {
                    item.SimulationMode = e.Value;
                }
                catch
                {
                }
        }

        private string BoolToOnOff(bool value)
        {
            if (value == true)
                return "On";
            else
                return "Off";
        }

        public string GetInputIOStatus()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var io in InputIO)
            {
                sb.Append(io.ToString());
                if (InputIO.Last() != io)
                    sb.Append(", ");
            }

            return sb.ToString();
        }

        public string GetOutputIOStatus()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var io in OutputIO)
            {
                sb.Append(io.ToString());
                if (OutputIO.Last() != io)
                    sb.Append(", ");
            }

            return sb.ToString();
        }

        private void SetIgnoreInterlockOfIOList(bool value)
        {
            foreach (var item in InputIO)
                item.IgnoreInterlock = value;

            foreach (var item in OutputIO)
                item.IgnoreInterlock = value;
        }
    }
}
