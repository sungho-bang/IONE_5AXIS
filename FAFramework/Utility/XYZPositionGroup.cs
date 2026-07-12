using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary;
using FALibrary.Utility;
using FALibrary.Part.MemoryBasePart;
using FALibrary.Sequence;
using FALibrary.Part.MMCPart;
using FAFramework.Utility;

namespace FAFramework.Utility
{
    public class XYZPositionGroup : FAObject
    {
        private FAMMCPosition _positionX;
        [FAAttribute("")]
        public FAMMCPosition PositionX
        {
            get { return _positionX; }
            set
            {
                if (_positionX == value) return;
                _positionX = value;
                NotifyPropertyChanged("PositionX");
            }
        }

        private FAMMCPosition _positionY;
        [FAAttribute("")]
        public FAMMCPosition PositionY
        {
            get { return _positionY; }
            set
            {
                if (_positionY == value) return;
                _positionY = value;
                NotifyPropertyChanged("PositionY");
            }
        }

        private FAMMCPosition _positionZ;
        [FAAttribute("")]
        public FAMMCPosition PositionZ
        {
            get { return _positionZ; }
            set
            {
                if (_positionZ == value) return;
                _positionZ = value;
                NotifyPropertyChanged("PositionZ");
            }
        }

        public XYZPositionGroup()
        {
            PositionX = new FAMMCPosition();
            PositionY = new FAMMCPosition();
            PositionZ = new FAMMCPosition();
        }

        public void CopyTo(XYZPositionGroup position)
        {
            PositionX.CopyTo(position.PositionX);
            PositionY.CopyTo(position.PositionY);
            PositionZ.CopyTo(position.PositionZ);
        }

        public override string ToString()
        {
            return PositionX.ToString() + ", " + PositionY.ToString() + ", " + PositionZ.ToString();
        }

        public string[] ToKeyValueArray(string prefix)
        {
            List<string> list = new List<string>();
            list.AddRange(PositionX.ToKeyValueArray(string.Format("{0}PositionX.", prefix)));
            list.AddRange(PositionY.ToKeyValueArray(string.Format("{0}PositionY.", prefix)));
            list.AddRange(PositionZ.ToKeyValueArray(string.Format("{0}PositionZ.", prefix)));

            return list.ToArray();
        }

        public void Parsing(System.Xml.Linq.XElement xml)
        {
            if (xml.Element("PositionX") != null)
                PositionX.Parsing(xml.Element("PositionX"));

            if (xml.Element("PositionY") != null)
                PositionY.Parsing(xml.Element("PositionY"));

            if (xml.Element("PositionZ") != null)
                PositionZ.Parsing(xml.Element("PositionZ"));
        }
    }
}
