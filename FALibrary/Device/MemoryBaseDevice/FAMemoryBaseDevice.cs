using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace FALibrary.Device.MemoryBaseDevice
{
    public class FAIOInfo
    {
        public string Name { get; private set; }
        public int Index { get; private set; }
        public string Description { get; private set; }

        public FAIOInfo(string aName, int aIndex, string aDescription)
        {
            Name = aName;
            Index = aIndex;
            Description = aDescription;
        }        
    }

    public abstract class FAMemoryBaseDevice : FADevice
    {
        private Dictionary<int, FAIOInfo> _inputIOInfoList = new Dictionary<int, FAIOInfo>();
        private Dictionary<int, FAIOInfo> _outputIOInfoList = new Dictionary<int, FAIOInfo>();

        protected Dictionary<int, FAIOInfo> InputIOInfoList
        {
            get { return _inputIOInfoList; }
        }

        protected Dictionary<int, FAIOInfo> OutputIOInfoList
        {
            get { return _outputIOInfoList; }
        }

        public Dictionary<int, FAIOInfo> GetInputIOInfoList()
        {
            Dictionary<int, FAIOInfo> temp = new Dictionary<int, FAIOInfo>();

            foreach (var item in InputIOInfoList)
                temp.Add(item.Key, new FAIOInfo(item.Value.Name, item.Value.Index, item.Value.Description));

            return temp;
        }

        public Dictionary<int, FAIOInfo> GetOutputIOInfoList()
        {
            Dictionary<int, FAIOInfo> temp = new Dictionary<int, FAIOInfo>();

            foreach (var item in OutputIOInfoList)
                temp.Add(item.Key, new FAIOInfo(item.Value.Name, item.Value.Index, item.Value.Description));

            return temp;
        }

        public FAIOInfo GetInputIOInfo(int index)
        {
            if (InputIOInfoList.ContainsKey(index) == true)
                return InputIOInfoList[index];

            return null;
        }

        public FAIOInfo GetInputIOInfo(string name)
        {
            foreach (KeyValuePair<int, FAIOInfo> item in InputIOInfoList)
            {
                if (item.Value.Name == name)
                {
                    return item.Value;
                }
            }

            return null;
        }

        public FAIOInfo GetOutputIOInfo(int index)
        {
            if (OutputIOInfoList.ContainsKey(index) == true)
                return OutputIOInfoList[index];
            
            return null;
        }

        public FAIOInfo GetOutputIOInfo(string name)
        {
            foreach (KeyValuePair<int, FAIOInfo> item in OutputIOInfoList)
            {
                if (item.Value.Name == name)
                {
                    return item.Value;
                }
            }

            return null;
        }

        public abstract bool GetInputIOValue(int index);

        public abstract void SetInputIOValue(int index, bool value); //Simulation에서만 사용

        public abstract bool GetOutputIOValue(int index);

        public abstract void SetOutputIOValue(int index, bool value);

        public abstract void GetInputIOBytes(int index, byte[] bytes);
        
        public abstract void SetOutputIOBytes(int index, byte[] bytes);

        public override void LoadParameters(XElement xml)
        {
            try
            {
                base.LoadParameters(xml);

                if (xml.Element("InputIOList") != null)
                    LoadIOList(xml.Element("InputIOList"), InputIOInfoList);

                if (xml.Element("OutputIOList") != null)
                    LoadIOList(xml.Element("OutputIOList"), OutputIOInfoList);
            }
            catch(Exception e)
            {                
                throw new Exception(e.Message);
            }
        }

        private void LoadIOList(XElement xml, Dictionary<int, FAIOInfo> aIOInfoList)
        {
            foreach (XElement item in xml.Elements())
            {                
                string name = item.Element("Name").Value.Trim();
                int index = 0;
                if (int.TryParse(item.Element("Index").Value.Trim(), out index) == false)                    
                    throw new Exception(item.Element("Index").Value.Trim() + " can not convert to integer");

                string description = item.Element("Description").Value.Trim();

                if (aIOInfoList.ContainsKey(index) == false)
                    aIOInfoList.Add(index, new FAIOInfo(name, index, description));
                else
                    throw new Exception("Reregistered IO. " + "IO Name : " + name + ", IO Index : " + index);
            }
        }
    }
}
