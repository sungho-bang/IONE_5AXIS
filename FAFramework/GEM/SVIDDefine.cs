using System.ComponentModel;

namespace FAFramework.GEM
{
    public class SVIDDefine : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public int ID { get; set; }
        public string Name { get; set; }
        public string DataFormat { get; set; }
        public string Unit { get; set; }
        public string Description { get; set; }
    }
}
