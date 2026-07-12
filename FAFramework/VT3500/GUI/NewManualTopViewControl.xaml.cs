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
    /// NewManualTopViewControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NewManualTopViewControl : UserControl
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
            DependencyProperty.Register("EquipmentInstance", typeof(Equipment.EquipmentBase), typeof(NewManualTopViewControl));
        public static readonly DependencyProperty ReadOnlyProperty =
           DependencyProperty.Register("ReadOnly", typeof(bool), typeof(NewManualTopViewControl));
        public static readonly DependencyProperty FrontUnitProperty =
           DependencyProperty.Register("FrontUnit", typeof(object), typeof(NewManualTopViewControl));
        public static readonly DependencyProperty InverterModuleProperty =
          DependencyProperty.Register("InverterModule", typeof(object), typeof(NewManualTopViewControl));
        public static readonly DependencyProperty HeaterTopUnitProperty =
          DependencyProperty.Register("HeaterTopUnit", typeof(object), typeof(NewManualTopViewControl));
        public static readonly DependencyProperty HeaterBottomUnitProperty =
            DependencyProperty.Register("HeaterBottomUnit", typeof(object), typeof(NewManualTopViewControl));

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
        public object HeaterTopUnit
        {
            get { return GetValue(HeaterTopUnitProperty); }
            set
            {
                SetValue(HeaterTopUnitProperty, value);
            }
        }
        public object HeaterBottomUnit
        {
            get { return GetValue(HeaterBottomUnitProperty); }
            set
            {
                SetValue(HeaterBottomUnitProperty, value);
            }
        }
        public NewManualTopViewControl()
        {
            InitializeComponent();
        }

        #region Inverter
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
        #endregion

        #region ShapeModule
        private void BtnIncreaseDown(Button btn)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.UpButtonColor = "White";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.DownButtonColor = "Red";
            //            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.MotorRun.On.Execute(this);
            //          Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.Opening.On.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontLoadingUnit.FirstPressServo.MoveJogNegative.Execute(this);
         }
        private void BtnIncreaseUp(Button btn)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.UpButtonColor = "Red";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.DownButtonColor = "White";
            //Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.MotorRun.Off.Execute(this);
            //Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.Opening.Off.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontLoadingUnit.FirstPressServo.Stop.Execute(this);
        }
        private void BtnDescentDown(Button btn)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.UpButtonColor = "White";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.DownButtonColor = "Red";
           // Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.MotorRun.On.Execute(this);
           // Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.Closing.On.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontLoadingUnit.FirstPressServo.MoveJogPositive.Execute(this);
        }
        
        private void BtnDescentUp(Button btn)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.UpButtonColor = "Red";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.DownButtonColor = "White";
          //  Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.MotorRun.Off.Execute(this);
          //  Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.Closing.Off.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontLoadingUnit.FirstPressServo.Stop.Execute(this);
        }
        private void Downbutton_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            BtnDescentDown((Button)sender);
        }
        private void Downbutton_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            BtnDescentUp((Button)sender);
        }
        private void Downbutton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            BtnDescentDown((Button)sender);
        }
        private void Downbutton_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            BtnDescentUp((Button)sender);
        }
        private void Upbutton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            BtnIncreaseDown((Button)sender);
        }
        private void Upbutton_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            BtnIncreaseUp((Button)sender);
        }
        private void Upbutton_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            BtnIncreaseDown((Button)sender);
        }
        private void Upbutton_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            BtnIncreaseUp((Button)sender);
        }
        private void Upbutton_Copy_Checked(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.FirstTopHeater = "Red";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.FirstTopHeater1 = "White";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SafetyFirstTopModule.HeaterPowerOn.On.Execute(this);
        }
        private void Upbutton_Copy_Unchecked(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.FirstTopHeater1 = "Blue";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.FirstTopHeater = "White";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SafetyFirstTopModule.HeaterPowerOn.Off.Execute(this);
        }
        private void Downbutton_Copy_Checked(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.FirstBottomHeater = "Red";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.FirstBottomHeater1 = "White";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SafetyFirstBottomModule.HeaterPowerOn.On.Execute(this);
        }
        private void Downbutton_Copy_Unchecked(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.FirstBottomHeater = "White";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.FirstBottomHeater1 = "Blue";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SafetyFirstBottomModule.HeaterPowerOn.Off.Execute(this);
        }
        private void BtnDescentDown1(Button btn)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.UpButtonColor = "White";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.DownButtonColor = "Red";
            // Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.MotorRun.On.Execute(this);
            //  Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.Closing.On.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontLoadingUnit.SecondPressServo.MoveJogPositive.Execute(this);

        }
        private void BtnDescentUp1(Button btn)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.UpButtonColor = "Red";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.DownButtonColor = "White";
            // Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.MotorRun.Off.Execute(this);
            //  Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.Closing.Off.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontLoadingUnit.SecondPressServo.Stop.Execute(this);

        }
        // 상승
        private void BtnIncreaseDown1(Button btn)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.UpButtonColor = "White";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.DownButtonColor = "Red";
            //  Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.MotorRun.On.Execute(this);
            // Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.Opening.On.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontLoadingUnit.SecondPressServo.MoveJogNegative.Execute(this);

        }
        private void BtnIncreaseUp1(Button btn)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.UpButtonColor = "Red";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.DownButtonColor = "White";
            // Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.MotorRun.Off.Execute(this);
            //  Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.Opening.Off.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontLoadingUnit.SecondPressServo.Stop.Execute(this);
        }
        private void Downbutton_PreviewMouseDown1(object sender, MouseButtonEventArgs e)
        {
            BtnDescentDown1((Button)sender);
        }
        private void Downbutton_PreviewMouseUp1(object sender, MouseButtonEventArgs e)
        {
            BtnDescentUp1((Button)sender);
        }
        private void Downbutton_PreviewTouchDown1(object sender, TouchEventArgs e)
        {
            BtnDescentDown1((Button)sender);
        }
        private void Downbutton_PreviewTouchUp1(object sender, TouchEventArgs e)
        {
            BtnDescentUp1((Button)sender);
        }
        private void Upbutton_PreviewMouseDown1(object sender, MouseButtonEventArgs e)
        {
            BtnIncreaseDown1((Button)sender);
        }
        private void Upbutton_PreviewMouseUp1(object sender, MouseButtonEventArgs e)
        {
            BtnIncreaseUp1((Button)sender);
        }
        private void Upbutton_PreviewTouchDown1(object sender, TouchEventArgs e)
        {
            BtnIncreaseDown1((Button)sender);
        }
        private void Upbutton_PreviewTouchUp1(object sender, TouchEventArgs e)
        {
            BtnIncreaseUp1((Button)sender);
        }

        private void BtnDescentDown5(Button btn)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.UpButtonColor = "White";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.DownButtonColor = "Red";
            // Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.MotorRun.On.Execute(this);
            // Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.Closing.On.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontLoadingUnit.OptionPressServo.MoveJogPositive.Execute(this);
        }
        private void BtnDescentUp5(Button btn)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.UpButtonColor = "Red";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.DownButtonColor = "White";
            // Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.MotorRun.Off.Execute(this);
            // Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.Closing.Off.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontLoadingUnit.OptionPressServo.Stop.Execute(this);
        }
        // 상승
        private void BtnIncreaseDown5(Button btn)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.UpButtonColor = "White";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.DownButtonColor = "Red";
            //  Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.MotorRun.On.Execute(this);
            //  Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.Opening.On.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontLoadingUnit.OptionPressServo.MoveJogNegative.Execute(this);
        }
        private void BtnIncreaseUp5(Button btn)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.UpButtonColor = "Red";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.DownButtonColor = "White";
            //  Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.MotorRun.Off.Execute(this);
            //  Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.Opening.Off.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontLoadingUnit.OptionPressServo.Stop.Execute(this);
        }
        private void Downbutton_PreviewMouseDown5(object sender, MouseButtonEventArgs e)
        {
            BtnDescentDown5((Button)sender);
        }
        private void Downbutton_PreviewMouseUp5(object sender, MouseButtonEventArgs e)
        {
            BtnDescentUp5((Button)sender);
        }
        private void Downbutton_PreviewTouchDown5(object sender, TouchEventArgs e)
        {
            BtnDescentDown5((Button)sender);
        }
        private void Downbutton_PreviewTouchUp5(object sender, TouchEventArgs e)
        {
            BtnDescentUp5((Button)sender);
        }
        private void Upbutton_PreviewMouseDown5(object sender, MouseButtonEventArgs e)
        {
            BtnIncreaseDown5((Button)sender);
        }
        private void Upbutton_PreviewMouseUp5(object sender, MouseButtonEventArgs e)
        {
            BtnIncreaseUp5((Button)sender);
        }
        private void Upbutton_PreviewTouchDown5(object sender, TouchEventArgs e)
        {
            BtnIncreaseDown5((Button)sender);
        }
        private void Upbutton_PreviewTouchUp5(object sender, TouchEventArgs e)
        {
            BtnIncreaseUp5((Button)sender);
        }
        #endregion

        #region FeedingModule
        private void MoveCylinderUp(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.TapeLoadGrip.Release.Execute(this);
        }
        private void MoveCylinderDown(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.TapeLoadGrip.Grip.Execute(this);
        }
        private void HoldCylinderUp(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.TapeHoldGrip.Release.Execute(this);
        }
        private void HoldCylinderDown(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.TapeHoldGrip.Grip.Execute(this);
        }
        #endregion

        #region SealingModule
        private void BtnDescentDown2(Button btn)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.UpButtonColor = "White";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.DownButtonColor = "Red";
            // Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.MotorRun.On.Execute(this);
            // Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.Closing.On.Execute(this);

            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontLoadingUnit.ThirdPressServo.MoveJogPositive.Execute(this);
        }
        private void BtnDescentUp2(Button btn)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.UpButtonColor = "Red";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.DownButtonColor = "White";
            //  Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.MotorRun.Off.Execute(this);
            //  Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.Closing.Off.Execute(this);


            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontLoadingUnit.ThirdPressServo.Stop.Execute(this);
        }
        // 상승
        private void BtnIncreaseDown2(Button btn)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.UpButtonColor = "White";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.DownButtonColor = "Red";
            //  Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.MotorRun.On.Execute(this);
            //  Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.Opening.On.Execute(this);


            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontLoadingUnit.ThirdPressServo.MoveJogNegative.Execute(this);
        }
        private void BtnIncreaseUp2(Button btn)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.UpButtonColor = "Red";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.DownButtonColor = "White";
            //  Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.MotorRun.Off.Execute(this);
            //  Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.Opening.Off.Execute(this);


            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontLoadingUnit.ThirdPressServo.Stop.Execute(this);
        }
        private void Downbutton_PreviewMouseDown2(object sender, MouseButtonEventArgs e)
        {
            BtnDescentDown2((Button)sender);
        }
        private void Downbutton_PreviewMouseUp2(object sender, MouseButtonEventArgs e)
        {
            BtnDescentUp2((Button)sender);
        }
        private void Downbutton_PreviewTouchDown2(object sender, TouchEventArgs e)
        {
            BtnDescentDown2((Button)sender);
        }
        private void Downbutton_PreviewTouchUp2(object sender, TouchEventArgs e)
        {
            BtnDescentUp2((Button)sender);
        }
        private void Upbutton_PreviewMouseDown2(object sender, MouseButtonEventArgs e)
        {
            BtnIncreaseDown2((Button)sender);
        }
        private void Upbutton_PreviewMouseUp2(object sender, MouseButtonEventArgs e)
        {
            BtnIncreaseUp2((Button)sender);
        }
        private void Upbutton_PreviewTouchDown2(object sender, TouchEventArgs e)
        {
            BtnIncreaseDown2((Button)sender);
        }
        private void Upbutton_PreviewTouchUp2(object sender, TouchEventArgs e)
        {
            BtnIncreaseUp2((Button)sender);
        }
        private void BandPitchChangeCylinderUp(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.BandPitchChangeCylinder.Home.Execute(this);
        }
        private void BandPitchChangeCylinderDown(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.BandPitchChangeCylinder.Push.Execute(this);
        }
        private void VacuumOn(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.BandVaccum.On.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.BandVaccumEject.Off.Execute(this);
        }
        private void VacuumOff(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.BandVaccum.Off.Execute(this);
        }
        private void VacuumEjectOn(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.BandVaccumEject.On.Execute(this);
        }
        private void VacuumEjectOff(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.BandVaccumEject.Off.Execute(this);
        }
        private void BottomMotorOn(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.PackingTapeLoadingMotor.Run.Execute(this);
        }
        private void BottomMotorOff(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.PackingTapeLoadingMotor.Stop.Execute(this);
        }
        #endregion

        #region PackingModule
        private void BtnDescentDown3(Button btn)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FourthPressModule.UpButtonColor = "White";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FourthPressModule.DownButtonColor = "Red";
            // Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FourthPressModule.MotorRun.On.Execute(this);
            //  Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FourthPressModule.Closing.On.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearLoadingUnit.FourthPressServo.MoveJogPositive.Execute(this);
        }
        private void BtnDescentUp3(Button btn)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FourthPressModule.UpButtonColor = "Red";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FourthPressModule.DownButtonColor = "White";
            ///  Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FourthPressModule.MotorRun.Off.Execute(this);
            //  Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FourthPressModule.Closing.Off.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearLoadingUnit.FourthPressServo.Stop.Execute(this);
        }
        // 상승
        private void BtnIncreaseDown3(Button btn)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FourthPressModule.UpButtonColor = "White";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FourthPressModule.DownButtonColor = "Red";
            //  Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FourthPressModule.MotorRun.On.Execute(this);
            //  Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FourthPressModule.Opening.On.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearLoadingUnit.FourthPressServo.MoveJogNegative.Execute(this);
        }
        private void BtnIncreaseUp3(Button btn)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FourthPressModule.UpButtonColor = "Red";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FourthPressModule.DownButtonColor = "White";
            //   Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FourthPressModule.MotorRun.Off.Execute(this);
            //   Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FourthPressModule.Opening.Off.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearLoadingUnit.FourthPressServo.Stop.Execute(this);
        }
        private void Downbutton_PreviewMouseDown3(object sender, MouseButtonEventArgs e)
        {
            BtnDescentDown3((Button)sender);
        }
        private void Downbutton_PreviewMouseUp3(object sender, MouseButtonEventArgs e)
        {
            BtnDescentUp3((Button)sender);
        }
        private void Downbutton_PreviewTouchDown3(object sender, TouchEventArgs e)
        {
            BtnDescentDown3((Button)sender);
        }
        private void Downbutton_PreviewTouchUp3(object sender, TouchEventArgs e)
        {
            BtnDescentUp3((Button)sender);
        }
        private void Upbutton_PreviewMouseDown3(object sender, MouseButtonEventArgs e)
        {
            BtnIncreaseDown3((Button)sender);
        }
        private void Upbutton_PreviewMouseUp3(object sender, MouseButtonEventArgs e)
        {
            BtnIncreaseUp3((Button)sender);
        }
        private void Upbutton_PreviewTouchDown3(object sender, TouchEventArgs e)
        {
            BtnIncreaseDown3((Button)sender);
        }
        private void Upbutton_PreviewTouchUp3(object sender, TouchEventArgs e)
        {
            BtnIncreaseUp3((Button)sender);
        }
        private void CuttingUp(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.SealingBandCutting.Up.Execute(this);
        }
        private void CuttingDown(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.SealingBandCutting.Down.Execute(this);
        }
        private void SealingTapeLoadingMotorRun(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.SealingTapeLoadingMotor.Run.Execute(this);
        }
        private void SealingTapeLoadingMotorStop(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.SealingTapeLoadingMotor.Stop.Execute(this);
        }
        private void Upbutton_Copy1_Click_1(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FourthPressModule.FourthTopHeater = "Red";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FourthPressModule.FourthTopHeater1 = "White";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SafetyFourthBottomModule.HeaterPowerOn.On.Execute(this);
        }

        private void Upbutton_Copy2_Click(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FourthPressModule.FourthTopHeater = "White";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FourthPressModule.FourthTopHeater1 = "Blue";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SafetyFourthBottomModule.HeaterPowerOn.Off.Execute(this);
        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
