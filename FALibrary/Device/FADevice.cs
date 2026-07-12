using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using FALibrary.Utility;

namespace FALibrary.Device
{
    public abstract class FADevice : FAObject
    {
        private string _name;
        private string _description;
        [FAAttribute("")]
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value) return;

                _name = value;
                NotifyPropertyChanged("Name");
            }
        }
        [FAAttribute("")]
        public string Description
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
        public bool IsOpened { get; protected set; }

        public FADevice()
        {
            IsOpened = false;
        }

        public virtual void LoadParameters(XElement xml)
        {
            if (xml.Element("Property") != null)
                LoadProperty(xml.Element("Property"));
        }

        public virtual void Open() { }
        public virtual void Close() { }
        public virtual void ReadWrite() { }

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

                if (propertyType.IsEnum)
                {
                    value = Enum.Parse(propertyType, strValue);                    
                }
                else if (propertyType == typeof(bool))
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
                    return;

                FAReflection.SetPropertyValue(this, name, value);
            }
        }
    }
}
