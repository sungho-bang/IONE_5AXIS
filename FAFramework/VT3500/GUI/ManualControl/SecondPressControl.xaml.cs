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
    /// SecondPressControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SecondPressControl : UserControl, INotifyPropertyChanged
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
           DependencyProperty.Register("ReadOnly", typeof(bool), typeof(SecondPressControl));
        public static readonly DependencyProperty SubUnitProperty =
           DependencyProperty.Register("SubUnit", typeof(object), typeof(SecondPressControl));
        public static readonly DependencyProperty SecondUnitProperty =
            DependencyProperty.Register("SecondUnit", typeof(object), typeof(SecondPressControl));
        public static readonly DependencyProperty EquipmentInstanceProperty =
        DependencyProperty.Register("EquipmentInstance", typeof(Equipment.EquipmentBase), typeof(SecondPressControl));

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
        public object SecondUnit
        {
            get { return GetValue(SecondUnitProperty); }
            set
            {
                SetValue(SecondUnitProperty, value);
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

        public SecondPressControl()
        {
            InitializeComponent();
        }

        // 하강
        private void BtnDescentDown(Button btn)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.UpButtonColor = "White";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.DownButtonColor = "Red";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.MotorRun.On.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.Closing.On.Execute(this);
        }
        private void BtnDescentUp(Button btn)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.UpButtonColor = "Red";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.DownButtonColor = "White";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.MotorRun.Off.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.Closing.Off.Execute(this);
        }

        // 상승
        private void BtnIncreaseDown(Button btn)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.UpButtonColor = "White";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.DownButtonColor = "Red";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.MotorRun.On.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.Opening.On.Execute(this);
        }
        private void BtnIncreaseUp(Button btn)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.UpButtonColor = "Red";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.DownButtonColor = "White";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.MotorRun.Off.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.Opening.Off.Execute(this);
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
