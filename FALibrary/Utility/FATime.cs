using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace FALibrary.Utility
{
    public enum FATimeType
    {
        millisecond,
        second,
        minute,
        hour
    }

    [Serializable]
    public class FATime : FALibrary.FAObject
    {
        public FATime()
        {
        }

        private string _name;
        private string _description = "";
        private FATimeType _type = FATimeType.second;
        private double _typeValue = 0;

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
        public FATimeType Type
        {
            get
            {
                return _type;
            }

            set
            {
                if (_type == value) return;

                _type = value;
                double temp = TypeValue;
                TypeValue = 0;
                TypeValue = temp;
                NotifyPropertyChanged("Type");
            }
        }

        [FAAttribute("")]
        public double TypeValue
        {
            get { return _typeValue; }
            set
            {
                if (_typeValue == value) return;

                _typeValue = value;
                switch (Type)
                {
                    case FATimeType.millisecond :                        
                        Time = new TimeSpan((long)(value * 10000));
                        break;
                    case FATimeType.second :
                        Time = new TimeSpan((long)(value * 10000 * 1000));
                        break;
                    case FATimeType.minute :
                        Time = new TimeSpan((long)(value * 10000 * 1000 * 60));
                        break;
                    case FATimeType.hour :
                        Time = new TimeSpan((long)(value * 10000 * 1000 * 60 * 60));
                        break;
                }
                
                NotifyPropertyChanged("TypeValue");
                NotifyPropertyChanged("Time");
            }
        }
       
        public FATime(FATimeType type, double value)
        {
            Type = type;
            TypeValue = value;
        }

        public FATime(string type, double value)
        {
            if (type == "millisecond")
                Type = FATimeType.millisecond;
            else if (type == "second")
                Type = FATimeType.second;
            else if (type == "minute")
                Type = FATimeType.minute;
            else if (type == "hour")
                Type = FATimeType.hour;

            TypeValue = value;
        }

        [FAAttribute("")]
        [XmlIgnoreAttribute]
        public TimeSpan Time { get; set; }
        [FAAttribute("")]
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description == value) return;

                _description = value;
                NotifyPropertyChanged("Description");
            }
        }
    }
}
