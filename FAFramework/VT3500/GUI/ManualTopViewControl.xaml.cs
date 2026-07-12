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
using System.ComponentModel;
using FAFramework.Utility;

namespace FAFramework.VT3500.GUI
{

    public partial class ManualTopViewControl : UserControl, INotifyPropertyChanged
    {
        Control _selectedControl;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public static readonly DependencyProperty EquipmentInstanceProperty =
            DependencyProperty.Register("EquipmentInstance", typeof(Equipment.EquipmentBase), typeof(ManualTopViewControl));
        public static readonly DependencyProperty SubUnitProperty =
          DependencyProperty.Register("SubUnit", typeof(object), typeof(ManualTopViewControl));
        public static readonly DependencyProperty FirstUnitProperty =
          DependencyProperty.Register("FirstUnit", typeof(object), typeof(ManualTopViewControl));
        public static readonly DependencyProperty ReadOnlyProperty =
           DependencyProperty.Register("ReadOnly", typeof(bool), typeof(ManualTopViewControl));


        public bool GobackButtonHide { get; set; }
        public bool GobackButtonHide1 { get; set; }

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
        public object SubUnit
        {
            get { return GetValue(SubUnitProperty); }
            set
            {
                SetValue(SubUnitProperty, value);
            }
        }
        public object FirstUnit
        {
            get { return GetValue(FirstUnitProperty); }
            set
            {
                SetValue(FirstUnitProperty, value);
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


        public ManualTopViewControl()
        {
            ShowControlCommand = new CommandHandler(
                delegate (object param)
                {
                    var args = param as object[];
                    Button button = (Button)args[0];
                    dynamic control = args[1];
                    
                    GobackButtonHide = true;
                    control.Subject = (string)button.ToolTip;
                    ShowControl(control as Control);
                },
                true);

            InitializeComponent();
        }

        private void Closebutton_Click(object sender, RoutedEventArgs e)
        {
            GobackButtonHide = false;
            MainControlView = true;
        }
    }
}
