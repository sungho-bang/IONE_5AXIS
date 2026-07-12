using System.Runtime.Serialization;
using System.ComponentModel;

namespace FAFramework.GEM
{
    [DataContract]
    public class EventReportContainer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        [DataMember]
        public EventReport[] Items { get; private set; }
    }
}
