using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace FAFramework.GEM
{
    public class SVID : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public int ID { get; set; }
        public string Name { get; set; }
        public string DataFormat { get; set; }
        public string Unit { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
    }
}
