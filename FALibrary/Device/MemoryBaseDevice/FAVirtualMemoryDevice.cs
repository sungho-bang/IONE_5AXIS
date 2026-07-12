using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace FALibrary.Device.MemoryBaseDevice
{
    public class FAVirtualMemoryDevice : FAMemoryBaseDevice
    {
        protected Dictionary<int, bool> IOList { get; set; }

        public FAVirtualMemoryDevice()
        {
            IOList = new Dictionary<int, bool>();
        }

        public override void LoadParameters(XElement xml)
        {
            base.LoadParameters(xml);

            foreach (var item in InputIOInfoList)
            {
                if (IOList.ContainsKey(item.Key) == false)
                    IOList.Add(item.Key, false);
            }

            foreach (var item in OutputIOInfoList)
            {
                if (IOList.ContainsKey(item.Key) == false)
                    IOList.Add(item.Key, false);
            }
        }

        public override bool GetInputIOValue(int index)
        {
            return IOList[index];
        }

        public override void SetInputIOValue(int index, bool value) //Simulation에서만 사용
        {
            IOList[index] = value;
        }

        public override bool GetOutputIOValue(int index)
        {
            return IOList[index];
        }

        public override void SetOutputIOValue(int index, bool value)
        {
            IOList[index] = value;
        }

        public override void GetInputIOBytes(int index, byte[] bytes)
        {
        }

        public override void SetOutputIOBytes(int index, byte[] bytes)
        {
        }
    }
}
