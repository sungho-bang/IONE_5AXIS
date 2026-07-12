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
    /// OptionPressControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class OptionPressControl : UserControl, INotifyPropertyChanged
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
           DependencyProperty.Register("ReadOnly", typeof(bool), typeof(OptionPressControl));
        public static readonly DependencyProperty SubUnitProperty =
           DependencyProperty.Register("SubUnit", typeof(object), typeof(OptionPressControl));
        public static readonly DependencyProperty OptionUnitProperty =
            DependencyProperty.Register("OptionUnit", typeof(object), typeof(OptionPressControl));
        public static readonly DependencyProperty EquipmentInstanceProperty =
        DependencyProperty.Register("EquipmentInstance", typeof(Equipment.EquipmentBase), typeof(OptionPressControl));

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
        public object OptionUnit
        {
            get { return GetValue(OptionUnitProperty); }
            set
            {
                SetValue(OptionUnitProperty, value);
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

        public OptionPressControl()
        {
            InitializeComponent();
        }

        // 하강
        private void BtnDescentDown(Button btn)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.UpButtonColor = "White";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.DownButtonColor = "Red";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.MotorRun.On.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.Closing.On.Execute(this);
        }
        private void BtnDescentUp(Button btn)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.UpButtonColor = "Red";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.DownButtonColor = "White";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.MotorRun.Off.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.Closing.Off.Execute(this);
        }

        // 상승
        private void BtnIncreaseDown(Button btn)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.UpButtonColor = "White";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.DownButtonColor = "Red";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.MotorRun.On.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.Opening.On.Execute(this);
        }
        private void BtnIncreaseUp(Button btn)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.UpButtonColor = "Red";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.DownButtonColor = "White";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.MotorRun.Off.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.Opening.Off.Execute(this);
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
    }
}
