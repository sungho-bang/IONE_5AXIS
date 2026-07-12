using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FAFramework.Utility.PackingLog
{
    public class MaterialLog : LogBase
    {
        public string MaterialName { get; set; }
        public int RemainQuantity { get; set; }
        public int UseQuantity { get; set; }

        public override string ToString()
        {
            AppendElement(MaterialName);
            AppendElement(UseQuantity.ToString());
            AppendElement(RemainQuantity.ToString());

            return ToLog();
        }
    }
}
