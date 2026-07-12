using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAFramework.GEM
{
    public struct MessageArgs
    {
        public int MsgID { get; set; }
        public short Stream { get; set; }
        public short Function { get; set; }
        public short Wbit { get; set; }
        public int Length { get; set; }
    }
}
