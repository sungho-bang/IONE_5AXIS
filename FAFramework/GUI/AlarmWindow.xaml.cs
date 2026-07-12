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
using System.Windows.Shapes;
using System.ComponentModel;

namespace FAFramework.GUI
{
    /// <summary>
    /// AlarmWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AlarmWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private Manager.AlarmRaisingStatusManager _alarmRaisingStatusManager;
        public Manager.AlarmRaisingStatusManager AlarmRaisingStatusManager
        {
            get { return _alarmRaisingStatusManager; }
            set
            {
                if (_alarmRaisingStatusManager == value) return;
                _alarmRaisingStatusManager = value;
                NotifyPropertyChanged("AlarmRaisingStatusManager");
            }
        }

        public AlarmWindow()
        {
            InitializeComponent();
        }
    }
}
