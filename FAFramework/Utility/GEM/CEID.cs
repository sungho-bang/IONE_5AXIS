using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FAFramework.Utility.GEM
{
    public class CEID
    {
        public static readonly List<CEID> ObjectList = new List<CEID>();

        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public CEID()
        {
            ObjectList.Add(this);
        }

        public CEID(int id, string name, string description)
        {
            ID = id;
            Name = name;
            Description = description;
            
            ObjectList.Add(this);
        }
    }
}
