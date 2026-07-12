using System.Runtime.Serialization;
using System.ComponentModel;

namespace FAFramework.GEM
{
    [DataContract]
    public class EventReport : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        [DataMember]
        public CEIDDefine CEID { get; set; }

        [DataMember]
        public SVID[] SVIDDefine { get; set; }
    }
}
