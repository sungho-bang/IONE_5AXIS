using System.Runtime.Serialization;

namespace FAFramework.GEM
{
    [DataContract]
    public class SVIDContainer
    {
        [DataMember]
        public SVIDDefine[] SVIDs { get; private set; }
    }
}
