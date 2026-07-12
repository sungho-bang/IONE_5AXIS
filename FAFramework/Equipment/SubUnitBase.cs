using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection;
using FALibrary.Utility;
using FALibrary.Part;
using FALibrary.Part.MemoryBasePart;
using FALibrary.Sequence;
using FAFramework.Utility;
using FALibrary;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace FAFramework.Equipment
{
    public abstract class SubUnitBase : FAObject
    {
        public class PartInfo
        {
            public bool ShowRetryInfoToScreen { get; set; }
            public bool ShowTimeToScreen { get; set; }
            public FAPart Part { get; set; }
            public XElement PartXMLInfo { get; set; }
        }

        public List<PartInfo> PartList
        {
            get;
            private set;
        }

        public string Name { get; set; }

        public FASequenceManager SequenceManager { get; private set; }

        private string PartListFilePath { get; set; }

        private static Type[] _constructorIncludeSequenceManager = { typeof(FASequenceManager) };

        public SubUnitBase()
        {
            PartList = new List<PartInfo>();
        }

        public void ValidatePart()
        {
            foreach (var part in PartList)
            {
                part.Part.Validate();
            }
        }

        public void Initialize(FASequenceManager sequenceManager, string partListFilePath)
        {
            SequenceManager = sequenceManager;
            PartListFilePath = partListFilePath;
            CreateParts(sequenceManager);

            if (File.Exists(PartListFilePath) == true)
            {
                var xml = XElement.Load(PartListFilePath);
                LoadPartList(xml);
            }
        }

        [FAAttribute("Operation")]
        public void Load()
        {
            try
            {
                if (File.Exists(PartListFilePath) == true)
                {
                    var xml = XElement.Load(PartListFilePath);
                    ReloadPartList(xml);
                }
            }
            catch
            {
            }
        }

        [FAAttribute("Operation")]
        public void LoadBackup(string path)
        {
            try
            {
                if (File.Exists(path) == true)
                {
                    var xml = XElement.Load(path);
                    ReloadPartList(xml);
                }
            }
            catch
            {
            }
        }

        [FAAttribute("Operation")]
        public void Save()
        {
            try
            {
                var xml = new XElement("PartList");
                SavePart(xml);
                var doc = new XDocument();
                doc.Add(xml);
                doc.Save(PartListFilePath);
            }
            catch
            {
            }
        }

        private void CreateParts(FASequenceManager sequenceManager)
        {
            PropertyInfo[] propList = this.GetType().GetProperties();
            foreach (PropertyInfo info in propList)
            {
                if (info.PropertyType.IsSubclassOf(typeof(FAPart)) == false) continue;

                var constructor = info.PropertyType.GetConstructor(_constructorIncludeSequenceManager);

                object part;
                if (constructor != null)
                {
                    part = Activator.CreateInstance(info.PropertyType,
                                    FAReflection.GetPropertyValue(this, "SequenceManager"));
                }
                else
                {
                    part = Activator.CreateInstance(info.PropertyType);
                }

                info.SetValue(this, part, null);
                foreach (var partAction in (part as FAPart).GetAllPartAction().ToArray())
                    partAction.Part = part as FAPart;
                Manager.MachineManager.Instance.AddPart(((FAMachine)part));
            }
        }

        private void LoadPartList(XElement xml)
        {
            foreach (XElement item in xml.Elements())
            {
                string name = item.Element("Name").Value.Trim();
                string description = item.Element("Description").Value.Trim();
                string deviceName = item.Element("DeviceName").Value.Trim();
                object obj = FAReflection.GetPropertyValue(this, name);
                if ((obj is FAPart) == false)
                {
                    throw new Exception($"{this.Name}.{name} +  is not FAPart. Description={description}");
                }

                FAPart part = obj as FAPart;
                part.Description = description;
                part.Name = name;
                part.FullName = this.Name + "." + part.Name;
                part.DeviceName = deviceName;
                if (string.IsNullOrEmpty(deviceName) &&
                    ConfigClasses.GlobalConst.CHECK_EXIST_DEVICE_IN_DEVICELIST)
                    throw new Exception($"SubUnit[{ this.Name }].Part[{ name }]'s device name is empty.");

                if (MainEquipment.Instance.DeviceManager.Items.ContainsKey(deviceName))
                    part.SetDevice(MainEquipment.Instance.DeviceManager.Items[deviceName]);
                else
                {
                    if (ConfigClasses.GlobalConst.CHECK_EXIST_DEVICE_IN_DEVICELIST)
                        throw new Exception($"SubUnit[{ this.Name }].Part[{ name }]. Can not found device[{deviceName}] in DeviceList");
                }

                try
                {
                    if (item.Element("Parameters") != null)
                        part.LoadParameters(item.Element("Parameters"));

                    int cnt = this.GetType().GetProperty(name).GetCustomAttributes(typeof(HideOnScreenProperty), true).Count();

                    PartInfo partInfo = new PartInfo();
                    partInfo.Part = part;
                    partInfo.PartXMLInfo = item;
                    if (cnt == 0)
                    {
                        if (part.RetryInfoList.Count > 0)
                            partInfo.ShowRetryInfoToScreen = true;

                        if (part.TimeList.Count > 0)
                            partInfo.ShowTimeToScreen = true;
                    }

                    PartList.Add(partInfo);
                }
                catch (Exception e)
                {
                    throw new Exception($"{this.Name}.{name}" + ".\n" + e.Message);
                }
            }
        }

        private void ReloadPartList(XElement xml)
        {
            foreach (XElement item in xml.Elements())
            {
                string name = item.Element("Name").Value.Trim();
                string description = item.Element("Description").Value.Trim();
                string deviceName = item.Element("DeviceName").Value.Trim();
                object obj = FAReflection.GetPropertyValue(this, name);
                if ((obj is FAPart) == false)
                {
                    throw new Exception(name + " is not FAPart");
                }

                FAPart part = obj as FAPart;
                part.Description = description;
                part.Name = name;
                part.FullName = this.Name + "." + part.Name;
                part.DeviceName = deviceName;

                try
                {
                    if (item.Element("Parameters") != null)
                        part.LoadParameters(item.Element("Parameters"));
                }
                catch (Exception e)
                {
                    throw new Exception("Part Name is " + name + ".\n" + e.Message);
                }
            }
        }

        private void SavePart(XElement xml)
        {
            foreach (PartInfo partInfo in PartList)
            {
                if (partInfo.PartXMLInfo.Element("Parameters") == null)
                    partInfo.PartXMLInfo.Add(new XElement("Parameters"));

                partInfo.Part.SaveParameters(partInfo.PartXMLInfo.Element("Parameters"));

                xml.Add(partInfo.PartXMLInfo);
            }
        }
    }
}
