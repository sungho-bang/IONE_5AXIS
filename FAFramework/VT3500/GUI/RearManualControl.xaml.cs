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

namespace FAFramework.VT3500.GUI
{
    /// <summary>
    /// PackingControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RearManualControl : UserControl, INotifyPropertyChanged
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
           DependencyProperty.Register("ReadOnly", typeof(bool), typeof(RearManualControl));
        public static readonly DependencyProperty RearUnitProperty =
            DependencyProperty.Register("RearUnit", typeof(object), typeof(RearManualControl));
        public static readonly DependencyProperty FourthUnitProperty =
           DependencyProperty.Register("FourthUnit", typeof(object), typeof(RearManualControl));
        public static readonly DependencyProperty EquipmentInstanceProperty =
        DependencyProperty.Register("EquipmentInstance", typeof(Equipment.EquipmentBase), typeof(RearManualControl));

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

        public object RearUnit
        {
            get { return GetValue(RearUnitProperty); }
            set
            {
                SetValue(RearUnitProperty, value);
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

        public RearManualControl()
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

        // Loading ----------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Loading_NextStepMove_Button_Click(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.LoadingStep = true;
        }
        
        private void Loading_Start_Button_Click(object sender, RoutedEventArgs e)
        {
            if(Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.WorkManualLoading.IsStartable() ||
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.WorkManualLoading.State == FALibrary.Sequence.SequenceState.Aborted)
            {
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.WorkManualLoading.Start();
            }
            else if(Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.WorkManualLoading.IsRestartable())
            {
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.WorkManualLoading.Resume();
            }
        }

        private void Loading_Stop_Button_Click(object sender, RoutedEventArgs e)
        {
            if(Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.WorkManualLoading.State == FALibrary.Sequence.SequenceState.Running)
            {
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.WorkManualLoading.Suspend();
            }
            else
            {
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.WorkManualLoading.Stop();
            }
        }

        // Press -------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Press_NextStepMove_Button_Click(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.PressStep = true;
        }

        private void Press_Start_Button_Click(object sender, RoutedEventArgs e)
        {
            //if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.WorkManualPress.IsStartable() ||
            //    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.WorkManualPress.State == FALibrary.Sequence.SequenceState.Aborted)
            //{
            //    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.WorkManualPress.Start();
            //}
            //else if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.WorkManualPress.IsRestartable())
            //{
            //    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.WorkManualPress.Resume();
            //}
        }

        private void Press_Stop_Button_Click(object sender, RoutedEventArgs e)
        {
            //if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.WorkManualPress.State == FALibrary.Sequence.SequenceState.Running)
            //{
            //    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.WorkManualPress.Suspend();
            //}
            //else
            //{
            //    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.WorkManualPress.Stop();
            //}
        }

        // Pull ---------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Pull_Start_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.WorkRearPullManual.IsStartable() ||
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.WorkRearPullManual.State == FALibrary.Sequence.SequenceState.Aborted)
            {
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.WorkRearPullManual.Start();
            }
            else if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.WorkRearPullManual.IsRestartable())
            {
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.WorkRearPullManual.Resume();
            }
        }

        private void Pull_Stop_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.WorkRearPullManual.State == FALibrary.Sequence.SequenceState.Running)
            {
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.WorkRearPullManual.Suspend();
            }
            else
            {
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.WorkRearPullManual.Stop();
            }
        }
    }
}
