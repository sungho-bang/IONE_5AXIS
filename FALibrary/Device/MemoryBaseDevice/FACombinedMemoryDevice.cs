using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection;

namespace FALibrary.Device.MemoryBaseDevice
{
    public class FACombinedMemoryDevice : FAMemoryBaseDevice
    {
        private class CombinedIOInfo
        {
            public FAMemoryBaseDevice Device { get; private set; }
            public int Index { get; private set; }

            public CombinedIOInfo(FAMemoryBaseDevice device, int index)
            {
                Device = device;
                Index = index;
            }
        }

        private Dictionary<int, CombinedIOInfo> _combinedInputIOList = new Dictionary<int, CombinedIOInfo>();
        private Dictionary<int, CombinedIOInfo> _combinedOutputIOList = new Dictionary<int, CombinedIOInfo>();
        private Dictionary<string, FAMemoryBaseDevice> _subDeviceList = new Dictionary<string, FAMemoryBaseDevice>();
        
        public Dictionary<string, FAMemoryBaseDevice> SubDeviceList
        {
            get { return _subDeviceList; }
        }

        private class DeviceIOInfo
        {

        }

        public override void Open()
        {
            foreach (var device in _subDeviceList.Values)
                device.Open();
        }

        public override void Close()
        {
            foreach (var device in _subDeviceList.Values)
                device.Close();
        }

        public override void ReadWrite()
        {
            foreach (var device in _subDeviceList.Values)
                device.ReadWrite();
        }

        public override bool GetInputIOValue(int index)
        {
            try
            {
                return _combinedInputIOList[index].Device.GetInputIOValue(_combinedInputIOList[index].Index);
            }
            catch
            {
                return false;
            }
        }

        public override void SetInputIOValue(int index, bool value) //Simulation에서만 사용
        {
            try
            {
                _combinedInputIOList[index].Device.SetInputIOValue(_combinedInputIOList[index].Index, value);
            }
            catch
            {
            }
        }

        public override bool GetOutputIOValue(int index)
        {
            try
            {
                return _combinedOutputIOList[index].Device.GetOutputIOValue(_combinedOutputIOList[index].Index);
            }
            catch
            {
                return false;
            }
        }

        public override void SetOutputIOValue(int index, bool value)
        {
            try
            {
                _combinedOutputIOList[index].Device.SetOutputIOValue(_combinedOutputIOList[index].Index, value);
            }
            catch
            {                
            }
        }

        public override void GetInputIOBytes(int index, byte[] bytes)
        {
        }

        public override void SetOutputIOBytes(int index, byte[] bytes)
        {
        }

        public override void LoadParameters(XElement xml)
        {
            try
            {
                LoadSubDeviceList(xml);
                AdjustIOIndex();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private void LoadSubDeviceList(XElement xml)
        {
            Assembly assembly = Assembly.Load("FALibrary");

            foreach (XElement item in xml.Elements())
            {
                string name = item.Element("Name").Value.Trim();
                string deviceType = item.Element("DeviceType").Value.Trim();
                string description = item.Element("Description").Value.Trim();

                Type type = System.Type.GetType("FALibrary.Device." + deviceType);
                object obj = assembly.CreateInstance("FALibrary.Device." + deviceType);
                FADevice device = obj as FADevice;
                device.Name = name;
                device.Description = description;

                try
                {
                    device.LoadParameters(item.Element("Parameters"));
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }

                SubDeviceList.Add(name, (FAMemoryBaseDevice)device);
            }            
        }

        private void AdjustIOIndex()
        {
            Dictionary<string, int> maxValues = new Dictionary<string, int>();
            foreach (var device in SubDeviceList.Values)
                maxValues.Add(device.Name, GetMaxIndex(device));

            int max = maxValues.Values.Max();
            int deviceIndexLength = max.ToString().Length + 1;

            int deviceCount = 1;
            foreach (var device in SubDeviceList.Values)
            {
                AddIOInfo(device, deviceCount * deviceIndexLength);
            }
        }

        private void AddIOInfo(FAMemoryBaseDevice device, int deviceIndexValue)
        {
            foreach (var item in device.GetInputIOInfoList().Values)
            {
                int index = deviceIndexValue + item.Index;
                _combinedInputIOList.Add(index, new CombinedIOInfo(device, item.Index));
                FAIOInfo ioInfo = device.GetInputIOInfo(item.Index);
                InputIOInfoList.Add(index, new FAIOInfo(ioInfo.Name, index, ioInfo.Description));
            }

            foreach (var item in device.GetOutputIOInfoList().Values)
            {
                int index = deviceIndexValue + item.Index;
                _combinedOutputIOList.Add(deviceIndexValue + item.Index, new CombinedIOInfo(device, item.Index));
                FAIOInfo ioInfo = device.GetOutputIOInfo(item.Index);
                OutputIOInfoList.Add(index, new FAIOInfo(ioInfo.Name, index, ioInfo.Description));
            }
        }

        private int GetMaxIndex(FAMemoryBaseDevice device)
        {
            int max = 0;
            bool first = false;

            foreach (var info in device.GetInputIOInfoList())
            {
                if (first == false)
                {
                    first = true;
                    max = info.Value.Index;
                }
                else
                {
                    if (max < info.Value.Index)
                        max = info.Value.Index;
                }
            }

            return max;
        }        
    }
}
