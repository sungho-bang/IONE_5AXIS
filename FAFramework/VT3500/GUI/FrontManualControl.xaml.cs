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
    public partial class FrontManualControl : UserControl, INotifyPropertyChanged
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
           DependencyProperty.Register("ReadOnly", typeof(bool), typeof(FrontManualControl));
        public static readonly DependencyProperty FrontUnitProperty =
           DependencyProperty.Register("FrontUnit", typeof(object), typeof(FrontManualControl));        
        public static readonly DependencyProperty FirstUnitProperty =
           DependencyProperty.Register("FirstUnit", typeof(object), typeof(FrontManualControl));
        public static readonly DependencyProperty SecondUnitProperty =
           DependencyProperty.Register("SeconddUnit", typeof(object), typeof(FrontManualControl));
        public static readonly DependencyProperty ThirdUnitProperty =
            DependencyProperty.Register("ThirdUnit", typeof(object), typeof(FrontManualControl));
        public static readonly DependencyProperty EquipmentInstanceProperty =
        DependencyProperty.Register("EquipmentInstance", typeof(Equipment.EquipmentBase), typeof(FrontManualControl));

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

        public object FirstdUnit
        {
            get { return GetValue(FirstUnitProperty); }
            set
            {
                SetValue(FirstUnitProperty, value);
            }
        }

        public object SeconddUnit
        {
            get { return GetValue(SecondUnitProperty); }
            set
            {
                SetValue(SecondUnitProperty, value);
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

        public FrontManualControl()
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
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.LoadingStep = true;
        }
        
        private void Loading_Start_Button_Click(object sender, RoutedEventArgs e)
        {
            if(Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualLoading.IsStartable() ||
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualLoading.State == FALibrary.Sequence.SequenceState.Aborted)
            {
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualLoading.Start();
            }
            else if(Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualLoading.IsRestartable())
            {
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualLoading.Resume();
            }
        }

        private void Loading_Stop_Button_Click(object sender, RoutedEventArgs e)
        {
            if(Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualLoading.State == FALibrary.Sequence.SequenceState.Running)
            {
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualLoading.Suspend();
            }
            else
            {
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualLoading.Stop();
            }
        }

        // Press -------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Press_NextStepMove_Button_Click(object sender, RoutedEventArgs e)
        {
          //  Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.PressStep = true;
        }

        private void Press_Start_Button_Click(object sender, RoutedEventArgs e)
        {
            //if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualPress.IsStartable() ||
            //    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualPress.State == FALibrary.Sequence.SequenceState.Aborted)
            //{
            //    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualPress.Start();
            //}
            //else if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualPress.IsRestartable())
            //{
            //    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualPress.Resume();
            //}
        }

        private void Press_Stop_Button_Click(object sender, RoutedEventArgs e)
        {
            //if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualPress.State == FALibrary.Sequence.SequenceState.Running)
            //{
            //    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualPress.Suspend();
            //}
            //else
            //{
            //    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualPress.Stop();
            //}
        }

        // Pull ---------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Pull_Start_Button_Click(object sender, RoutedEventArgs e)
        {
            //if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkFrontPullManual.IsStartable() ||
            //    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkFrontPullManual.State == FALibrary.Sequence.SequenceState.Aborted)
            //{
            //    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkFrontPullManual.Start();
            //}
            //else if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkFrontPullManual.IsRestartable())
            //{
            //    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkFrontPullManual.Resume();
            //}
        }

        private void Pull_Stop_Button_Click(object sender, RoutedEventArgs e)
        {
            //if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkFrontPullManual.State == FALibrary.Sequence.SequenceState.Running)
            //{
            //    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkFrontPullManual.Suspend();
            //}
            //else
            //{
            //    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkFrontPullManual.Stop();
            //}
        }
    }
}
