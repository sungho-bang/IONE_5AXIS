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

namespace FAFramework.GUI.Standard
{
    /// <summary>
    /// MainOperationControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainOperationControl : UserControl
    {
        public static readonly DependencyProperty EquipmentInstanceProperty =
            DependencyProperty.Register("EquipmentInstance", typeof(Equipment.EquipmentBase), typeof(MainOperationControl));
        
        public static readonly DependencyProperty AdditionalButtonsControlProperty =
            DependencyProperty.Register("AdditionalButtonsControl", typeof(Control), typeof(MainOperationControl));

        public static readonly DependencyProperty SubEquipmentInstanceProperty =
            DependencyProperty.Register("SubEquipmentInstance", typeof(FAFramework.VT3500.SubEquipment), typeof(MainOperationControl));

        public FAFramework.VT3500.SubEquipment SubEquipmentInstance
        {
            get { return (FAFramework.VT3500.SubEquipment)GetValue(SubEquipmentInstanceProperty); }
            set
            {
                SetValue(SubEquipmentInstanceProperty, value);
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

        public Control AdditionalButtonsControl
        {
            get { return (Control)GetValue(AdditionalButtonsControlProperty); }
            set
            {
                SetValue(AdditionalButtonsControlProperty, value);
            }
        }

        public MainOperationControl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EquipmentInstance.RequestStart();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            EquipmentInstance.RequestStop();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            EquipmentInstance.RequestInitialize();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            EquipmentInstance.AlarmRaisingStatusManager.ClearCurrentAlarm();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            EquipmentInstance.TurnOffSound();
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.CommonUnit.SignalTowerBuzzer.DoTurnOff(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.CommonUnit.SignalTowerBuzzer.DoTurnOff(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.CommonUnit.SignalTowerBuzzer.DoTurnOff(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.CommonUnit.SignalTowerBuzzer.DoTurnOff(this);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Style_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void UniformGrid_IsStylusCaptureWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
        
    }
}
