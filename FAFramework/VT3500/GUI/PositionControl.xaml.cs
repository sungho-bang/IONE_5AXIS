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
using FAFramework.Utility;
using FAFramework.VT3500.JobInfo;
using System.ComponentModel;

namespace FAFramework.VT3500.GUI
{
    /// <summary>
    /// PositionControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PositionControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public static readonly DependencyProperty EquipmentInstanceProperty =
            DependencyProperty.Register("EquipmentInstance", typeof(Equipment.EquipmentBase), typeof(PositionControl));

        public static readonly DependencyProperty JobInstanceProperty =
            DependencyProperty.Register("JobInstance", typeof(JobInfo.MoveJobInfo), typeof(PositionControl));

        public Equipment.EquipmentBase EquipmentInstance
        {
            get { return (Equipment.EquipmentBase)GetValue(EquipmentInstanceProperty); }
            set
            {
                SetValue(EquipmentInstanceProperty, value);
            }
        }

        public JobInfo.MoveJobInfo JobInstance
        {
            get { return (JobInfo.MoveJobInfo)GetValue(JobInstanceProperty); }
            set
            {
                SetValue(JobInstanceProperty, value);
            }
        }

        private object _selectedJobStep;
        public object SelectedJobStep
        {
            get { return _selectedJobStep; }
            set
            {
                if (_selectedJobStep == value) return;
                _selectedJobStep = value;
                NotifyPropertyChanged("SelectedJobStep");
            }
        }

        public CommandHandler AddCommand { get; set; }
        public CommandHandler RemoveCommand { get; set; }

        private JobInfo.MoveJobInfo _cloneJobInstance = new JobInfo.MoveJobInfo();

        public PositionControl()
        {


            AddCommand = new CommandHandler(
                obj =>
                {
                    try
                    {
                        var param = obj as JobInfo.MoveJobInfo;
                        param.CopyTo(_cloneJobInstance);

                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                        Manager.LogManager.Instance.WriteSystemLog(e.ToString());
                    }
                }, true);

            RemoveCommand = new CommandHandler(
                obj =>
                {
                    try
                    {
                        dynamic parameters = (object[])obj;
                        var list = (ThreadSafeObservableCollection<int>)parameters[0];
                        int selectedIndex = (int)parameters[1];

                        if (list.Contains(selectedIndex) == true)
                        {
                            list.Remove(selectedIndex);
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                        Manager.LogManager.Instance.WriteSystemLog(e.ToString());
                    }
                }, true);

            InitializeComponent();
        }

        private void buttonAddIndex_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            dynamic tag = button.Tag;
            var list = tag[0] as List<int>;
            int index = (int)tag[1];
            list.Add(index);
        }
    }
}
