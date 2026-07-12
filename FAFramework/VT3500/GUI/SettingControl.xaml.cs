using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// SettingControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingControl : UserControl
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        //InverterModule
        public static readonly DependencyProperty EquipmentInstanceProperty =
            DependencyProperty.Register("EquipmentInstance", typeof(Equipment.EquipmentBase), typeof(SettingControl));
        public static readonly DependencyProperty ReadOnlyProperty =
           DependencyProperty.Register("ReadOnly", typeof(bool), typeof(SettingControl));
        public static readonly DependencyProperty FrontUnitProperty =
           DependencyProperty.Register("FrontUnit", typeof(object), typeof(SettingControl));
        public static readonly DependencyProperty InverterModuleProperty =
          DependencyProperty.Register("InverterModule", typeof(object), typeof(SettingControl));

        public Equipment.EquipmentBase EquipmentInstance
        {
            get { return (Equipment.EquipmentBase)GetValue(EquipmentInstanceProperty); }
            set
            {
                SetValue(EquipmentInstanceProperty, value);
            }
        }
        public bool InverterModule
        {
            get { return (bool)GetValue(InverterModuleProperty); }
            set
            {
                SetValue(InverterModuleProperty, value);
            }
        }
        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set
            {
                SetValue(ReadOnlyProperty, value);
            }
        }
        public object FrontUnit
        {
            get { return GetValue(FrontUnitProperty); }
            set
            {
                SetValue(FrontUnitProperty, value);
            }
        }
        public SettingControl()
        {
            InitializeComponent();
        }
       
    }
}
