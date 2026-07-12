using FAFramework.Utility;
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
    /// SealingControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SealingControl : UserControl, INotifyPropertyChanged
    {
        Control _selectedControl;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool GobackButtonHide3 { get; set; }
        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public static readonly DependencyProperty ReadOnlyProperty =
           DependencyProperty.Register("ReadOnly", typeof(bool), typeof(SealingControl));
        public static readonly DependencyProperty SubUnitProperty =
            DependencyProperty.Register("SubUnit", typeof(object), typeof(SealingControl));
        public static readonly DependencyProperty HeaterBottomUnitProperty =
            DependencyProperty.Register("HeaterBottomUnit", typeof(object), typeof(SealingControl));
        public static readonly DependencyProperty FourthUnitProperty =
           DependencyProperty.Register("FourthUnit", typeof(object), typeof(SealingControl));
        public static readonly DependencyProperty EquipmentInstanceProperty =
        DependencyProperty.Register("EquipmentInstance", typeof(Equipment.EquipmentBase), typeof(SealingControl));

        private bool _showControlView = true;
        public bool MainControlView
        {
            get { return _showControlView; }
            set
            {
                if (_showControlView == value) return;
                _showControlView = value;
                NotifyPropertyChanged("MainControlView");

                if (value)
                {
                    if (_selectedControl != null)
                        contentControlSelectedControl.Content = null;
                }
            }
        }

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

        public object FourthUnit
        {
            get { return GetValue(FourthUnitProperty); }
            set
            {
                SetValue(FourthUnitProperty, value);
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
        public Equipment.EquipmentBase EquipmentInstance
        {
            get { return (Equipment.EquipmentBase)GetValue(EquipmentInstanceProperty); }
            set
            {
                SetValue(EquipmentInstanceProperty, value);
            }
        }
         public CommandHandler ShowControlCommand { get; set; }

        private void ShowControl(Control control)
        {
            _selectedControl = control;
            MainControlView = false;
            contentControlSelectedControl.Content = _selectedControl;
        }

        public SealingControl()
        {
            ShowControlCommand = new CommandHandler(
                delegate (object param)
                {
                    var args = param as object[];
                    Button button = (Button)args[0];
                    dynamic control = args[1];

                    GobackButtonHide3 = true;
                    control.Subject = (string)button.ToolTip;
                    ShowControl(control as Control);
                },
                true);
            InitializeComponent();
        }
        private void Closebutton_Click(object sender, RoutedEventArgs e)
        {
            GobackButtonHide3 = false;
            MainControlView = true;
        }
    }
}
