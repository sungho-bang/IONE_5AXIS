using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace FAFramework.VT3500.GUI.ManualControl
{
    /// <summary>
    /// PressControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InverterMotorControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public static readonly DependencyProperty ReadOnlyProperty =
           DependencyProperty.Register("ReadOnly", typeof(bool), typeof(InverterMotorControl));
        public static readonly DependencyProperty SubUnitProperty =
           DependencyProperty.Register("SubUnit", typeof(object), typeof(InverterMotorControl));
        public static readonly DependencyProperty EquipmentInstanceProperty =
        DependencyProperty.Register("EquipmentInstance", typeof(Equipment.EquipmentBase), typeof(InverterMotorControl));

        private string _subject;
        public string Subject
        {
            get { return _subject; }
            set
            {
                if (_subject == value) return;
                _subject = value;
                NotifyPropertyChanged("Subject");
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
        public object SubUnit
        {
            get { return GetValue(SubUnitProperty); }
            set
            {
                SetValue(SubUnitProperty, value);
            }
        }

        public Equipment.EquipmentBase EquipmentInstance
        {
            get { return (Equipment.EquipmentBase)GetValue(EquipmentInstanceProperty); }
            set
            {
                SetValue(EquipmentInstanceProperty, value);
            }
        }

        public InverterMotorControl()
        {
            InitializeComponent();
        }

        private void Inverter_Run_Click(object sender, RoutedEventArgs e)
        {
            var eqp = Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500;
            eqp.FrontLoadingUnit.InverterMotor.Run();
        }
        private void Inverter_ReverseRun_Click(object sender, RoutedEventArgs e)
        {
            var eqp = Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500;
            eqp.FrontLoadingUnit.InverterMotor.ReverseRun();
        }
        private void Inverter_Stop_Click(object sender, RoutedEventArgs e)
        {
            var eqp = Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500;
            eqp.FrontLoadingUnit.InverterMotor.Stop();
        }
        private void Inverter_Param_Write_Click(object sender, RoutedEventArgs e)
        {
            //var eqp = Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500;
            //eqp.FrontLoadingUnit.InverterMotor.Write_SetSpeed = 10;
        }
    }
}
