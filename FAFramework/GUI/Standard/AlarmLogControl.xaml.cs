using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using FAFramework.Utility;

namespace FAFramework.GUI.Standard
{
    /// <summary>
    /// AlarmLogControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AlarmLogControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private LogSearcher.LogSearcherBase _logSearcherObject;
        public LogSearcher.LogSearcherBase LogSearcherObject
        {
            get { return _logSearcherObject; }
            set
            {
                _logSearcherObject = value;
                NotifyPropertyChanged("LogSearcherObject");
            }
        }

        public AlarmLogControl()
        {
            InitializeComponent();
        }
    }
}
