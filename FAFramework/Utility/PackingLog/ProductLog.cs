using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FAFramework.Utility.PackingLog
{
    public class ProductLog : LogBase
    {
        public string ProductID { get; set; }
        public DateTime CarryInTime { get; set; }
        public DateTime CarryOutTime { get; set; }
        public int CarryInQuantity { get; set; }
        public int CarryOutQuantity { get; set; }

        public override string ToString()
        {
            AppendElement(ProductID);
            AppendElement(CarryInTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            AppendElement(CarryOutTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            AppendElement(CarryInQuantity.ToString());
            AppendElement(CarryOutQuantity.ToString());

            return ToLog();
        }
    }
}
