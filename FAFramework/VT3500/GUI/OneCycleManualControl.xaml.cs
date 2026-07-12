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
    /// OneCycleManualControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class OneCycleManualControl : UserControl
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
            DependencyProperty.Register("EquipmentInstance", typeof(Equipment.EquipmentBase), typeof(OneCycleManualControl));
        public static readonly DependencyProperty ReadOnlyProperty =
           DependencyProperty.Register("ReadOnly", typeof(bool), typeof(OneCycleManualControl));
        public static readonly DependencyProperty FrontUnitProperty =
           DependencyProperty.Register("FrontUnit", typeof(object), typeof(OneCycleManualControl));
        public static readonly DependencyProperty InverterModuleProperty =
          DependencyProperty.Register("InverterModule", typeof(object), typeof(OneCycleManualControl));

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
        public OneCycleManualControl()
        {
            InitializeComponent();
        }

        private void FirstPress(object sender, RoutedEventArgs e)
        {
            if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.State ==
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.StateStop)
            {
                //Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.FirstPressModule.WorkPress2.Start();
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkFirstManualPressServo.Start();
            }
        }
        private void SecondPress(object sender, RoutedEventArgs e)
        {
            if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.State ==
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.StateStop)
            {
               // Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.SecondPressModule.WorkPress2.Start();
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkSecondManualPressServo.Start();
            }
        }
        private void OptionPress(object sender, RoutedEventArgs e)
        {
            if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.State ==
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.StateStop)
            {
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkOptionManualPressServo.Start();
            }
        }
        private void ThirdPress(object sender, RoutedEventArgs e) //
        {
            if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.State ==
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.StateStop)
            {
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkThridManualPressServo.Start();
               
            }
        }
        private void FourthPress(object sender, RoutedEventArgs e)
        {
            if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.State ==
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.StateStop)
            {
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.WorkFourthManualPressServo.Start();
            }
        }

        private void OnePitchMove(object sender, RoutedEventArgs e)
        {
            if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.State ==
               Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.StateStop)
            {
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualLoading.Start();
            }
        }

        private void BandPitchChange(object sender, RoutedEventArgs e)
        {
            if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.State ==
              Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.StateStop)
            {
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualMoving.Start();
            }
        }
        private void PackingOneCycle(object sender, RoutedEventArgs e)
        {
            if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.State ==
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.StateStop)
            {
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.WorkRearPullManual.Start(); //
            }
        }
        private void SearchingImark(object sender, RoutedEventArgs e)
        {
            if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.State ==
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.StateStop)
            {
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.SearchingImark.Start(); //
            }
        }

        private void OnceOneCycle(object sender, RoutedEventArgs e)
        {
            if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.State ==
           Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.StateStop)
            {
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualOnceOneCycle.Start(); //

                if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualOnceOneCycle.State ==
          FALibrary.Sequence.SequenceState.Suspended)
                {
                    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualOnceOneCycle.Stop();
                }
                else if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualOnceOneCycle.State ==
                    FALibrary.Sequence.SequenceState.Suspending)
                {
                    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualOnceOneCycle.Stop();
                }
            }
        }
    }
}
