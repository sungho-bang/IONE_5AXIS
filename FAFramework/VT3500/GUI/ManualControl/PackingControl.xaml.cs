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
    /// PackingControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PackingControl : UserControl, INotifyPropertyChanged
    {
        Control _selectedControl;

        public event PropertyChangedEventHandler PropertyChanged;
        public bool GobackButtonHide2 { get; set; }
        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public static readonly DependencyProperty ReadOnlyProperty =
           DependencyProperty.Register("ReadOnly", typeof(bool), typeof(PackingControl));
        public static readonly DependencyProperty FrontUnitProperty =
            DependencyProperty.Register("FrontUnit", typeof(object), typeof(PackingControl));
        public static readonly DependencyProperty RearUnitProperty =
            DependencyProperty.Register("RearUnit", typeof(object), typeof(PackingControl));
        public static readonly DependencyProperty ThirdUnitProperty =
            DependencyProperty.Register("ThirdUnit", typeof(object), typeof(PackingControl));
        public static readonly DependencyProperty EquipmentInstanceProperty =
        DependencyProperty.Register("EquipmentInstance", typeof(Equipment.EquipmentBase), typeof(PackingControl));
        
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

        public object FrontUnit
        {
            get { return GetValue(FrontUnitProperty); }
            set
            {
                SetValue(FrontUnitProperty, value);
            }
        }
        public object RearUnit
        {
            get { return GetValue(RearUnitProperty); }
            set
            {
                SetValue(RearUnitProperty, value);
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
        public CommandHandler ShowControlCommand { get; set; }
        private void ShowControl(Control control)
        {
            _selectedControl = control;
            MainControlView = false;
            contentControlSelectedControl.Content = _selectedControl;
        }
        public PackingControl()
        {
            ShowControlCommand = new CommandHandler(
                delegate (object param)
                {
                    var args = param as object[];
                    Button button = (Button)args[0];
                    dynamic control = args[1];

                    GobackButtonHide2 = true;
                    control.Subject = (string)button.ToolTip;
                    ShowControl(control as Control);
                },
                true);

            InitializeComponent();
        }
        private void Closebutton_Click(object sender, RoutedEventArgs e)
        {
            GobackButtonHide2 = false;
            MainControlView = true;
        }
    }
}
