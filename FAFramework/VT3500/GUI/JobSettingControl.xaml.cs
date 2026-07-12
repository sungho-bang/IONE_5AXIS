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
using System.ComponentModel;

namespace FAFramework.VT3500.GUI
{
    /// <summary>
    /// JobSettingControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class JobSettingControl : UserControl, INotifyPropertyChanged
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
            DependencyProperty.Register("EquipmentInstance", typeof(Equipment.EquipmentBase), typeof(JobSettingControl));

        public static readonly DependencyProperty JobManagerInstanceProperty =
            DependencyProperty.Register("JobManagerInstance", typeof(JobInfo.JobManager), typeof(JobSettingControl));

        public Equipment.EquipmentBase EquipmentInstance
        {
            get { return (Equipment.EquipmentBase)GetValue(EquipmentInstanceProperty); }
            set
            {
                SetValue(EquipmentInstanceProperty, value);
            }
        }

        public JobInfo.JobManager JobManagerInstance
        {
            get { return (JobInfo.JobManager)GetValue(JobManagerInstanceProperty); }
            set
            {
                SetValue(JobManagerInstanceProperty, value);
            }
        }

        private JobInfo.FALotJobInfo _selectedJob;
        public JobInfo.FALotJobInfo SelectedJob
        {
            get { return _selectedJob; }
            set
            {
                if (_selectedJob == value) return;
                _selectedJob = value;
                NotifyPropertyChanged("SelectedJob");
            }
        }

        private string _selectedPartID;
        public string SelectedPartID
        {
            get { return _selectedPartID; }
            set
            {
                if (_selectedPartID == value) return;
                _selectedPartID = value;
                NotifyPropertyChanged("SelectedPartID");
            }
        }

        public CommandHandler AddJobCommand { get; set; }
        public CommandHandler RemoveJobCommand { get; set; }
        public CommandHandler RenameJobCommand { get; set; }
        public CommandHandler CopyJobCommand { get; set; }
        public CommandHandler AddPartIDCommand { get; set; }
        public CommandHandler RemovePartIDCommand { get; set; }
        public CommandHandler ApplyImmediately { get; set; }

        public JobSettingControl()
        {
            AddJobCommand = new CommandHandler(
                obj =>
                {
                    var newJob = new JobInfo.FALotJobInfo { Name = obj as string };
                    JobManagerInstance.LotJobInstance.LotJobInfoList.Add(newJob);
                    SelectedJob = JobManagerInstance.LotJobInstance.LotJobInfoList.Last();
                }, true);

            RemoveJobCommand = new CommandHandler(
                obj =>
                {
                    JobManagerInstance.LotJobInstance.LotJobInfoList.Remove(obj as JobInfo.FALotJobInfo);

                    if (JobManagerInstance.LotJobInstance.LotJobInfoList.Count > 0)
                        SelectedJob = JobManagerInstance.LotJobInstance.LotJobInfoList.Last();
                    else
                        SelectedJob = null;
                }, true);

            RenameJobCommand = new CommandHandler(
                obj =>
                {
                    var paramArray = (object[])obj;
                    var selectedJob = (JobInfo.FALotJobInfo)paramArray[0];
                    var jobName = paramArray[1] as string;
                    selectedJob.Name = jobName;

                }, true);

            CopyJobCommand = new CommandHandler(
                obj =>
                {
                    var paramArray = (object[])obj;
                    var selectedJob = (JobInfo.FALotJobInfo)paramArray[0];
                    var jobName = paramArray[1] as string;

                    var newLotJob = new JobInfo.FALotJobInfo();
                    selectedJob.CopyTo(newLotJob);
                    newLotJob.Name = jobName;
                    JobManagerInstance.LotJobInstance.LotJobInfoList.Add(newLotJob);
                    SelectedJob = JobManagerInstance.LotJobInstance.LotJobInfoList.Last();
                }, true);

            //ApplyImmediately = new CommandHandler(
            //    obj =>
            //    {
            //        var paramArray = (object[])obj;
            //        var selectedJob = (JobInfo.FALotJobInfo)paramArray[0];
            //        var jobName = paramArray[1] as string;
            //        selectedJob.Name = jobName;

            //        var module = Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule;
            //        selectedJob.MoveJobInfo.CopyTo(module.MoveJobInfo);
            //    }, true);

            InitializeComponent();
        }

        private void ListViewJobList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            textBoxJobName.Text = SelectedJob.Name;
        }
    }
}
