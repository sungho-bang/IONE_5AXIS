using System.ComponentModel;

namespace FAFramework.GEM
{
    public class CEIDDefine : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
