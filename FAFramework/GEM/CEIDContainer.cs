using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;

namespace FAFramework.GEM
{
    [DataContract]
    public class CEIDContainer
    {
        [DataMember]
        public CEIDDefine[] CEIDs { get; private set; }
    }
}
