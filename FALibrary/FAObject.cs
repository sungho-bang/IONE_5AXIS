using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Reflection;
using FALibrary.Utility;

namespace FALibrary
{
    [Serializable]
    public class FAObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Dictionary<string, object> _metaProperties = new Dictionary<string, object>();
        private SerializableDictionary<string, FARange> _rangeList = new SerializableDictionary<string, FARange>();
        [FAAttribute("Info")]
        [FASerializable]
        public SerializableDictionary<string, FARange> RangeList
        {
            get { return _rangeList; }
            set
            {
                var oldItems = new SerializableDictionary<string, FARange>();
                foreach (var item in _rangeList)
                {
                    oldItems.Add(item.Key, item.Value);
                }

                _rangeList = value;
                var newAddedItems = oldItems.Where(x => _rangeList.ContainsKey(x.Key) == false);
                if (newAddedItems.Count() > 0)
                {
                    foreach (var item in newAddedItems)
                    {
                        _rangeList.Add(item.Key, item.Value);
                    }
                }
            }
        }

        public FAObject()
        {            
            PropertyInfo[] propList;
            propList = this.GetType().GetProperties();

            foreach (PropertyInfo info in propList)
            {
                if (info.PropertyType.IsPrimitive &&
                    info.PropertyType != typeof(bool) &&
                    info.PropertyType != typeof(char) &&
                    info.PropertyType != typeof(IntPtr) &&
                    info.PropertyType != typeof(UIntPtr))
                {
                    foreach (var attr in Attribute.GetCustomAttributes(info))
                    {
                        if (attr is FAAttribute)
                        {
                            var minField = info.PropertyType.GetField("MinValue");
                            var maxField = info.PropertyType.GetField("MaxValue");

                            if (minField == null) return;
                            if (maxField == null) return;
                            
                            dynamic min = minField.GetValue(minField);
                            dynamic max = maxField.GetValue(maxField);

                            Type type = min.GetType();
                            if (RangeList.ContainsKey(info.Name) == false)
                                RangeList.Add(info.Name, new FARange(min, max));
                        }
                    }
                }
            }
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public object GetMetaPropertyValue(string propertyName)
        {
            if (ContainsMetaProperty(propertyName))
                return _metaProperties[propertyName];
            else
                return null;
        }

        public void SetMetaPropertyValue(string propertyName, object value)
        {
            if (ContainsMetaProperty(propertyName))
                _metaProperties[propertyName] = value;
            else
                _metaProperties.Add(propertyName, value);
        }

        public bool ContainsMetaProperty(string propertyName)
        {
            return _metaProperties.ContainsKey(propertyName);
        }

        public void RemoveMetaProperty(string propertyName)
        {
            if (ContainsMetaProperty(propertyName))
            {
                _metaProperties.Remove(propertyName);
            }
        }

        public void ClearMetaProperties()
        {
            _metaProperties.Clear();
        }

        public IEnumerable<KeyValuePair<string, object>> GetMetaProperties()
        {
            return _metaProperties.ToArray();
        }
    }
}
