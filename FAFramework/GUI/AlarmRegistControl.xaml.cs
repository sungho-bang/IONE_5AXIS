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
using System.Reflection;
using FALibrary;

namespace FAFramework.GUI
{
    /// <summary>
    /// AlarmRegistControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AlarmRegistControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private bool _allReset;
        public bool AllReset
        {
            get { return _allReset; }
            set
            {
                if (_allReset == value) return;
                _allReset = value;
                NotifyPropertyChanged("AllReset");
            }
        }

        public AlarmRegistControl()
        {
            InitializeComponent();
        }

        private void buttonRegist_Click(object sender, RoutedEventArgs e)
        {
            bool allReset = AllReset;
            
            Manager.MachineManager.Instance.SetAlarmIDToModules(allReset);
            Manager.MachineManager.Instance.SetAlarmIDToParts(allReset);
            Manager.MachineManager.Instance.AddAlarmOfModules();
            Manager.MachineManager.Instance.AddAlarmOfParts();
            Equipment.MainEquipment.Instance.Save();
        }
    }
}
