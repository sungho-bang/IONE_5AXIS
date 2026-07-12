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
    /// ThirdPressControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ThirdPressControl : UserControl, INotifyPropertyChanged
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
           DependencyProperty.Register("ReadOnly", typeof(bool), typeof(ThirdPressControl));
        public static readonly DependencyProperty SubUnitProperty =
           DependencyProperty.Register("SubUnit", typeof(object), typeof(ThirdPressControl));
        public static readonly DependencyProperty ThirdUnitProperty =
            DependencyProperty.Register("ThirdUnit", typeof(object), typeof(ThirdPressControl));
        public static readonly DependencyProperty EquipmentInstanceProperty =
        DependencyProperty.Register("EquipmentInstance", typeof(Equipment.EquipmentBase), typeof(ThirdPressControl));

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
        public object ThirdUnit
        {
            get { return GetValue(ThirdUnitProperty); }
            set
            {
                SetValue(ThirdUnitProperty, value);
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

        public ThirdPressControl()
        {
            InitializeComponent();
        }
        // 하강
        private void BtnDescentDown(Button btn)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.UpButtonColor = "White";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.DownButtonColor = "Red";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.MotorRun.On.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.Closing.On.Execute(this);
        }
        private void BtnDescentUp(Button btn)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.UpButtonColor = "Red";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.DownButtonColor = "White";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.MotorRun.Off.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.Closing.Off.Execute(this);
        }

        // 상승
        private void BtnIncreaseDown(Button btn)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.UpButtonColor = "White";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.DownButtonColor = "Red";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.MotorRun.On.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.Opening.On.Execute(this);
        }
        private void BtnIncreaseUp(Button btn)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.UpButtonColor = "Red";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.DownButtonColor = "White";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.MotorRun.Off.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.Opening.Off.Execute(this);
        }

        private void Downbutton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            BtnDescentDown((Button)sender);
        }

        private void Downbutton_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            BtnDescentUp((Button)sender);
        }

        private void Downbutton_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            BtnDescentDown((Button)sender);
        }

        private void Downbutton_PreviewTouchUp(object sender, TouchEventArgs e)
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

        private void Downbutton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
