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

namespace FAFramework.VT3500.GUI
{
    /// <summary>
    /// AlarmMainStatusControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AlarmMainStatusControl : UserControl
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public static readonly DependencyProperty AlarmRaisingStatusManagerProperty =
            DependencyProperty.Register("AlarmRaisingStatusManager", typeof(Manager.AlarmRaisingStatusManager), typeof(AlarmMainStatusControl));
        public static readonly DependencyProperty EquipmentInstanceProperty =
            DependencyProperty.Register("EquipmentInstance", typeof(Equipment.EquipmentBase), typeof(AlarmMainStatusControl));

        public Equipment.EquipmentBase EquipmentInstance
        {
            get { return (Equipment.EquipmentBase)GetValue(EquipmentInstanceProperty); }
            set
            {
                SetValue(EquipmentInstanceProperty, value);
            }
        }

        public Manager.AlarmRaisingStatusManager AlarmRaisingStatusManager
        {
            get { return (Manager.AlarmRaisingStatusManager)GetValue(AlarmRaisingStatusManagerProperty); }
            set
            {
                SetValue(AlarmRaisingStatusManagerProperty, value);
            }
        }

        public AlarmMainStatusControl()
        {
            InitializeComponent();
        }
    }
}
