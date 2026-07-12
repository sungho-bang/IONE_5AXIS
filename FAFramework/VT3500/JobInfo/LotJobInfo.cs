using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Linq;

namespace FAFramework.VT3500.JobInfo
{
    public class FALotJobInfo : FAObject
    {
        private string _name;
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

        private MoveJobInfo _moveJobInfo = new MoveJobInfo();
        [FAAttribute("")]
        public MoveJobInfo MoveJobInfo
        {
            get { return _moveJobInfo; }
            set
            {
                if (_moveJobInfo == value) return;
                _moveJobInfo = value;
                NotifyPropertyChanged("MoveJobInfo");
            }

        }
        public FALotJobInfo()
        {

        }

        public void CopyTo(FALotJobInfo obj)
        {
            obj.Name = Name;

            MoveJobInfo.CopyTo(obj.MoveJobInfo);
        }

        //public bool ToINIFormat(out string result)
        //{
        //    List<string> list = new List<string>();
        //    list.AddRange(MoveJobInfo.ToKeyValueArray("MoveJobInfo."));

        //    result = string.Join("\n", list);
        //    return true;
        //}

        //public void Parsing(string str)
        //{
        //    XElement xml = new XElement("Root");
        //    foreach (var line in str.Split('\n'))
        //    {
        //        var keyValue = line.Split('=');
        //        if (keyValue.Length >= 2)
        //            ParsingLine(xml, keyValue[0], keyValue[1]);
        //    }

        //    if (xml.Element("MoveJobInfo") != null)
        //        MoveJobInfo.Parsing(xml.Element("MoveJobInfo"));
        //}

        public void ParsingLine(XElement xml, string key, string value)
        {
            var splitData = key.Split('.');
            XElement temp = xml;

            foreach (var item in splitData)
            {
                if (temp.Element(item) == null)
                {
                    XElement subElement = new XElement(item);
                    temp.Add(subElement);
                    temp = subElement;
                }
                else
                {
                    temp = temp.Element(item);
                }
            }

            temp.Value = value;
        }
    }
}
