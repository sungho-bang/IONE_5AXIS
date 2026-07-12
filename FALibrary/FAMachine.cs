using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary.Alarm;
using FALibrary.Sequence;
using System.Reflection;
using FALibrary.Utility;
using FALibrary;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace FALibrary
{  
    public abstract class FAMachine : FAObject
    {
        private string _name;
        private string _fullName;
        private string _description;
        private int _machineID;

        protected event EventHandler<CreatePropertyEventArgs> OnCreateProperty = null;
        public Dictionary<string, FARetryInfo> RetryInfoList { get; private set; }
        public Dictionary<string, FATime> TimeList { get; private set; }

        [FAAttribute("Info", true)]
        [FAPropertyAttribute]
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
        [FAAttribute("Info", true)]
        public string FullName
        {
            get
            {
                return _fullName;
            }

            set
            {
                if (_fullName == value) return;

                _fullName = value;
                NotifyPropertyChanged("FullName");
            }
        }
        [FAAttribute("Info", true)]
        [FAPropertyAttribute]        
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
        [FAAttribute("Info", true)]
        [FAPropertyAttribute]
        public int MachineID
        {
            get { return _machineID; }
            set
            {
                if (_machineID == value) return;

                _machineID = value;
                NotifyPropertyChanged("MachineID");
            }
        }

        public FAMachine()
        {
            TimeList = new Dictionary<string, FATime>();
            RetryInfoList = new Dictionary<string, FARetryInfo>();
            SetPropertyCreateMethodList();
            CreateProperty();
        }        

        private void LoadTimers(XElement xml)
        {
            foreach (XElement item in xml.Elements())
            {
                string name = "";
                string type = "second";
                double value = 0;
                string description = "";

                if (item.Element("Name") != null)
                    name = item.Element("Name").Value.Trim();

                if (item.Element("Type") != null)
                    type = item.Element("Type").Value.Trim();

                if (item.Element("Description") != null)
                    description = item.Element("Description").Value.Trim();
                else
                    description = "";

                if (type != "millisecond" &&
                    type != "second" &&
                    type != "minute" &&
                    type != "hour")
                {
                    throw new Exception("Time type is not correct. " + "Time Name : " + name + "Time Type : " + type);
                }

                if (double.TryParse(item.Element("TypeValue").Value.Trim(), out value) == false)
                    throw new Exception("Timer Value is not digit" +
                        "Timer Name : " + name + "Timer Value : " + item.Element("Value").Value.Trim());

                FATime time = (FATime)FAReflection.GetPropertyValue(this, name);
                if (type == "millisecond")
                    time.Type = FATimeType.millisecond;
                else if (type == "second")
                    time.Type = FATimeType.second;
                else if (type == "minute")
                    time.Type = FATimeType.minute;
                else if (type == "hour")
                    time.Type = FATimeType.hour;

                time.Description = description;
                time.TypeValue = value;                
            }
        }

        private void SaveTime(XElement xml)
        {
            if (xml == null) return;

            XElement timeList = xml.Element("Time");
            if (timeList == null)
                timeList = new XElement("Time");

            PropertyInfo[] propList;
            propList = this.GetType().GetProperties();

            foreach (PropertyInfo info in propList)
            {
                if (info.PropertyType == typeof(FATime))
                {
                    string name = info.Name;
                    FATime value = (FATime)info.GetValue(this, null);
                    if (value == null) continue;

                    XElement item = FAUtility.GetElement(timeList, "Name", name);
                    if (item == null)
                    {
                        item = new XElement("Item");
                        timeList.Add(item);
                    }

                    if (item.Element("Name") != null)
                        item.Element("Name").SetValue(info.Name);
                    else
                        item.Add(new XElement("Name", info.Name));

                    if (item.Element("Type") != null)
                        item.Element("Type").SetValue(value.Type);
                    else
                        item.Add(new XElement("Type", value.Type));

                    if (item.Element("TypeValue") != null)
                        item.Element("TypeValue").SetValue(value.TypeValue);
                    else
                        item.Add(new XElement("TypeValue", value.TypeValue));

                    if (item.Element("Description") != null)
                        item.Element("Description").SetValue(value.Description);
                    else
                        item.Add(new XElement("Description", value.Description));
                }
            }

            if (xml.Element("Time") == null && !timeList.IsEmpty)
                xml.Add(timeList);
        }

        private void LoadRetryInfo(XElement xml)
        {
            foreach (XElement item in xml.Elements())
            {
                string name = "";
                uint retryLimit = 3;
                string description = "";

                if (item.Element("Name") != null)
                    name = item.Element("Name").Value.Trim();

                if (item.Element("RetryLimit") != null)
                {
                    uint result;
                    if (uint.TryParse(item.Element("RetryLimit").Value, out result) == true)
                    {
                        retryLimit = result;
                    }
                }

                if (item.Element("Description") != null)
                    description = item.Element("Description").Value.Trim();
                else
                    description = "";

                FARetryInfo retryInfo = (FARetryInfo)FAReflection.GetPropertyValue(this, name);
                retryInfo.Description = description;
                retryInfo.Name = name;
                retryInfo.RetryLimit = retryLimit;
            }
        }

        private void SaveRetryInfo(XElement xml)
        {
            if (xml == null) return;

            XElement retryInfoList = xml.Element("RetryInfo");
            if (retryInfoList == null)
                retryInfoList = new XElement("RetryInfo");
            
            PropertyInfo[] propList;
            propList = this.GetType().GetProperties();

            foreach (PropertyInfo info in propList)
            {
                if (info.PropertyType == typeof(FARetryInfo))
                {
                    string name = info.Name;
                    FARetryInfo value = (FARetryInfo)info.GetValue(this, null);
                    if (value == null) continue;

                    XElement item = FAUtility.GetElement(retryInfoList, "Name", name);
                    if (item == null)
                    {
                        item = new XElement("Item");
                        retryInfoList.Add(item);
                    }

                    if (item.Element("Name") != null)
                        item.Element("Name").SetValue(info.Name);
                    else
                        item.Add(new XElement("Name", info.Name));

                    if (item.Element("RetryLimit") != null)
                        item.Element("RetryLimit").SetValue(value.RetryLimit);
                    else
                        item.Add(new XElement("RetryLimit", value.RetryLimit));

                    if (item.Element("Description") != null)
                        item.Element("Description").SetValue(value.Description);
                    else
                        item.Add(new XElement("Description", value.Description));
                }

                if (xml.Element("RetryInfo") == null && !retryInfoList.IsEmpty)
                    xml.Add(retryInfoList);
            }
        }

        public virtual void LoadParameters(XElement xml)
        {
            try
            {
                if (xml.Element("Property") != null)
                    LoadProperty(xml.Element("Property"));                

                if (xml.Element("Time") != null)
                    LoadTimers(xml.Element("Time"));

                if (xml.Element("RetryInfo") != null)
                    LoadRetryInfo(xml.Element("RetryInfo"));
            }
            catch (Exception e)
            {
                throw new Exception("Name : " + Name + ", " + e.Message);
            }
        }

        public virtual void SaveParameters(XElement xml)
        {
            try
            {
                SaveTime(xml);

                if (xml.Element("Property") != null)
                    SaveProperty(xml.Element("Property"));
                else
                {
                    xml.Add(new XElement("Property"));
                    SaveProperty(xml.Element("Property"));
                }

                SaveRetryInfo(xml);
            }
            catch (Exception e)
            {
                throw new Exception("Name : " + Name + ", " + e.Message);
            }
        }
        
        private void LoadProperty(XElement xml)
        {
            foreach (XElement item in xml.Elements())
            {
                string name = "";
                string strValue = "";
                object value = 0;

                if (item.Element("Name") != null)
                    name = item.Element("Name").Value.Trim();
                else
                    continue;

                if (item.Element("Value") != null)
                    strValue = item.Element("Value").Value.Trim();
                else
                    continue;                

                Type propertyType = FAReflection.GetPropertyType(this, name);
                if (propertyType == null) continue;

                if (propertyType == typeof(bool))
                {
                    bool result;
                    if (bool.TryParse(strValue, out result) == false) continue;
                    value = result;
                }
                else if (propertyType == typeof(byte))
                {
                    byte result;
                    if (byte.TryParse(strValue, out result) == false) continue;
                    value = result;
                }
                else if (propertyType == typeof(short))
                {
                    short result;
                    if (short.TryParse(strValue, out result) == false) continue;
                    value = result;
                }
                else if (propertyType == typeof(ushort))
                {
                    ushort result;
                    if (ushort.TryParse(strValue, out result) == false) continue;
                    value = result;
                }
                else if (propertyType == typeof(uint))
                {
                    uint result;
                    if (uint.TryParse(strValue, out result) == false) continue;
                    value = result;
                }
                else if (propertyType == typeof(int))
                {
                    int result;
                    if (int.TryParse(strValue, out result) == false) continue;
                    value = result;
                }
                else if (propertyType == typeof(long))
                {
                    long result;
                    if (long.TryParse(strValue, out result) == false) continue;
                    value = result;
                }
                else if (propertyType == typeof(ulong))
                {
                    ulong result;
                    if (ulong.TryParse(strValue, out result) == false) continue;
                    value = result;
                }
                else if (propertyType == typeof(string))
                {
                    value = strValue;
                }
                else if (propertyType == typeof(float))
                {
                    float result;
                    if (float.TryParse(strValue, out result) == false) continue;
                    value = result;
                }
                else if (propertyType == typeof(double))
                {
                    double result;
                    if (double.TryParse(strValue, out result) == false) continue;
                    value = result;
                }
                else
                {
                    PropertyInfo info = this.GetType().GetProperty(name);
                    if (IsHaveFASerializable(Attribute.GetCustomAttributes(info)))
                    {
                        XDocument doc = new XDocument();
                        doc.Add(item.Element("Value").Elements());
                        value = Utility.FAUtility.Deserialize(doc, info.PropertyType);
                    }
                    else
                        return;
                }
                
                FAReflection.SetPropertyValue(this, name, value);
            }
        }

        private void SaveProperty(XElement xml)
        {
            if (xml == null) return;

            PropertyInfo[] propList;
            propList = this.GetType().GetProperties();

            foreach (PropertyInfo info in propList)
            {
                if (info.GetSetMethod(false) != null &&
                    IsProperty(info.PropertyType) &&
                    IsHaveSerializeAttribute(Attribute.GetCustomAttributes(info)))
                {
                    string name = info.Name;
                    object value = info.GetValue(this, null);
                    if (value == null) continue;

                    XElement item = FAUtility.GetElement(xml, "Name", name);
                    if (item == null)
                    {
                        item = new XElement("Item");
                        xml.Add(item);
                    }

                    if (item.Element("Name") != null)
                        item.Element("Name").SetValue(info.Name);
                    else
                        item.Add(new XElement("Name", info.Name));

                    if (item.Element("Value") != null)
                        item.Element("Value").SetValue(value);
                    else
                        item.Add(new XElement("Value", value));
                }
                else if (IsHaveFASerializable(Attribute.GetCustomAttributes(info)))
                {
                    string name = info.Name;
                    object value = info.GetValue(this, null);
                    if (value != null)
                    {
                        XDocument doc = Utility.FAUtility.Serialize(value);
                        XElement item = FAUtility.GetElement(xml, "Name", name);
                        if (item == null)
                        {
                            item = new XElement("Item");
                            xml.Add(item);
                        }

                        if (item.Element("Name") != null)
                            item.Element("Name").SetValue(info.Name);
                        else
                            item.Add(new XElement("Name", info.Name));

                        if (item.Element("Value") != null)
                        {
                            item.Element("Value").Remove();
                            item.Add(new XElement("Value"));
                            item.Element("Value").Add(doc.Root);
                        }
                        else
                        {
                            item.Add(new XElement("Value"));
                            item.Element("Value").Add(doc.Root);
                        }
                    }
                }
            }
        }

        protected virtual void SetPropertyCreateMethodList()
        {
            OnCreateProperty += CreateTime;
            OnCreateProperty += CreateRetryInfo;
        }

        private void CreateProperty()
        {
            PropertyInfo[] propList;
            propList = this.GetType().GetProperties();

            foreach (PropertyInfo info in propList)
            {
                if (OnCreateProperty != null)
                {
                    OnCreateProperty(this, new CreatePropertyEventArgs(info));
                }
            }
        }

        private void CreateTime(object sender, CreatePropertyEventArgs e)
        {
            if (e.PropertyInfo.PropertyType == typeof(FATime))
            {
                FATime time = new FATime(FATimeType.second, 10);
                time.Name = e.PropertyInfo.Name;
                e.PropertyInfo.SetValue(this, time, null);
                TimeList.Add(e.PropertyInfo.Name, time);
            }
        }

        private void CreateRetryInfo(object sender, CreatePropertyEventArgs e)
        {
            if (e.PropertyInfo.PropertyType == typeof(FARetryInfo))
            {
                FARetryInfo obj = new FARetryInfo(3);
                obj.Name = e.PropertyInfo.Name;
                e.PropertyInfo.SetValue(this, obj, null);
                RetryInfoList.Add(e.PropertyInfo.Name, obj);
            }
        }

        private bool IsProperty(Type type)
        {
            if (type == typeof(bool))
                return true;
            else if (type == typeof(byte))
                return true;
            else if (type == typeof(short))
                return true;
            else if (type == typeof(ushort))
                return true;
            else if (type == typeof(int))
                return true;
            else if (type == typeof(uint))
                return true;
            else if (type == typeof(long))
                return true;
            else if (type == typeof(ulong))
                return true;
            else if (type == typeof(float))
                return true;
            else if (type == typeof(double))
                return true;
            else if (type == typeof(string))
                return true;
            else
                return false;            
        }

        private bool IsHaveSerializeAttribute(Attribute[] attrs)
        {
            foreach (Attribute at in attrs)
            {
                if (at is FAPropertyAttribute)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsHaveFASerializable(Attribute[] attrs)
        {
            foreach (Attribute at in attrs)
            {
                if (at is FASerializable)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
