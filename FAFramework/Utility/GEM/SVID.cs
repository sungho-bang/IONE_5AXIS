using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FAFramework.Utility.GEM
{
    public class SVID
    {
        public static readonly List<SVID> ObjectList = new List<SVID>();

        public int ID { get; set; }
        public string Name { get; set; }
        public string DataFormat { get; set; }
        public string Unit { get; set; }
        public string Description { get; set; }

        public SVID()
        {
            ObjectList.Add(this);
        }

        public SVID(int id, string name, string dataFormat, string unit)
        {
            ID = id;
            Name = name;
            DataFormat = dataFormat;
            Unit = unit;

            ObjectList.Add(this);
        }
    }
}
