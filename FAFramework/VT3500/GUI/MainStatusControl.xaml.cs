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
using FAFramework.GUI;
using FAFramework.Utility;

namespace FAFramework.VT3500.GUI
{
    /// <summary>
    /// MainStatusControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainStatusControl : UserControl, INotifyPropertyChanged
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
            DependencyProperty.Register("EquipmentInstance", typeof(Equipment.EquipmentBase), typeof(MainStatusControl));

        public static readonly DependencyProperty JobManagerInstanceProperty =
            DependencyProperty.Register("JobManagerInstance", typeof(JobInfo.JobManager), typeof(MainStatusControl));
        
        public static readonly DependencyProperty FirstUnitProperty =
           DependencyProperty.Register("FirstUnit", typeof(object), typeof(MainStatusControl));

        public static readonly DependencyProperty FrontUnitProperty =
         DependencyProperty.Register("FrontUnit", typeof(object), typeof(MainStatusControl));
        public JobInfo.JobManager JobManagerInstance
        {
            get { return (JobInfo.JobManager)GetValue(JobManagerInstanceProperty); }
            set
            {
                SetValue(JobManagerInstanceProperty, value);
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
        public object FrontUnit
        {
            get { return GetValue(FrontUnitProperty); }
            set
            {
                SetValue(FrontUnitProperty, value);
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
        public CommandHandler MaintenanceModeClick { get; set; }

        public MainStatusControl()
        {

            MaintenanceModeClick = new Utility.CommandHandler(
                delegate (object sender)
                {
                    UserSelectWindow win = new UserSelectWindow();
                    win.EquipmentInstance = EquipmentInstance;
                    win.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);

                    if ((bool)win.ShowDialog() == true)
                    {
                        if (win.SelectedUser.Permission == Equipment.UserPermissionTypes.OPERATOR)
                        {
                            MessageBox.Show("Permission of Selected id not allow this work.");
                        }
                        else
                        {
                            EquipmentInstance.MaintenanceMode = true;
                        }
                    }
                }, true);

            InitializeComponent();
        }
        private void receipe_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            receipe.ItemsSource = JobManagerInstance.LotJobInstance.LotJobInfoList;

            receipe.SelectedItem = JobManagerInstance.LotJobInstance.LotJobInfoList.Select(x => x.Name);
        }

        private void receipe_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            var param = e.AddedItems[0];
            var getparam = param as JobInfo.FALotJobInfo;
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.MainLoopModule.SelectJob = getparam.Name;
        }
        
        private void ZeroAmount(object sender, MouseButtonEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.UICuttingCount = 0;
        }
        

        private void ZeroLength(object sender, MouseButtonEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.TapeLoadingServoUsedLength = 0;
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            //if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.State == Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.StateStop)
            //{
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.UsePress = true;
            //}
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            //if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.State == Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.StateStop)
            //{
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.UsePress = false;
            //}
        }

        private void ToggleButton_Checked_1(object sender, RoutedEventArgs e)
        {
            //if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.State == Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.StateStop)
            //{
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.UsePress = true;
            //}
        }

        private void ToggleButton_Unchecked_1(object sender, RoutedEventArgs e)
        {
            //if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.State == Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.StateStop)
            //{
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.UsePress = false;
            //}
        }

        private void ToggleButton_Checked_2(object sender, RoutedEventArgs e) // 사용
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.UsePressResumeSequence = false;
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.UsePressStopSequence = true;

            if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.State ==
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.StateRun &&
               !Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.UsePress)
            {
                //Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.ReturnPlaceSkip = true;
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.PlaceSkip = true;
            }
            else
            {
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.PlaceSkip = false;
            }
        }

        private void ToggleButton_Unchecked_2(object sender, RoutedEventArgs e) //미사용
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.UsePressStopSequence = false;
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.UsePressResumeSequence = true;
        }

        private void ToggleButton_Checked_3(object sender, RoutedEventArgs e)
        {
            //if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.State == Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.StateStop)
            //{
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FourthPressModule.UsePress = true;
            //}
        }

        private void ToggleButton_Unchecked_3(object sender, RoutedEventArgs e)
        {
            //if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.State == Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.StateStop)
            //{
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FourthPressModule.UsePress = false;
            //}
        }
        private void ToggleButton_Checked_4(object sender, RoutedEventArgs e)
        {
            //if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.State == Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.StateStop)
            //{
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.UsePress = true;
            //}
        }

        private void ToggleButton_Unchecked_4(object sender, RoutedEventArgs e)
        {
            //if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.State == Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.StateStop)
            //{
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.UsePress = false;
            //}
        }

        private void Upbutton_Copy_Checked(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.FirstTopHeater = "Red";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.FirstTopHeater1 = "White";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SafetyFirstTopModule.HeaterPowerOn.On.Execute(this);
        }
        private void Upbutton_Copy_Unchecked(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.FirstTopHeater1 = "Blue";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.FirstTopHeater = "White";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SafetyFirstTopModule.HeaterPowerOn.Off.Execute(this);
        }
        private void Downbutton_Copy_Checked(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.FirstBottomHeater = "Red";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.FirstBottomHeater1 = "White";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SafetyFirstBottomModule.HeaterPowerOn.On.Execute(this);
        }
        private void Downbutton_Copy_Unchecked(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.FirstBottomHeater = "White";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.FirstBottomHeater1 = "Blue";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SafetyFirstBottomModule.HeaterPowerOn.Off.Execute(this);
        }
        private void Upbutton_Copy1_Click_1(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FourthPressModule.FourthTopHeater = "Red";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FourthPressModule.FourthTopHeater1 = "White";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SafetyFourthBottomModule.HeaterPowerOn.On.Execute(this);
        }

        private void Upbutton_Copy2_Click(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FourthPressModule.FourthTopHeater = "White";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FourthPressModule.FourthTopHeater1 = "Blue";
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SafetyFourthBottomModule.HeaterPowerOn.Off.Execute(this);
        }

        private void FirstPress(object sender, RoutedEventArgs e)
        {
            if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.State == Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.StateStop)
            {

                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkFirstManualPressServo.Start();

                //Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.FirstPressModule.WorkPress2.Start();


                if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkFirstPressServo.State ==
                FALibrary.Sequence.SequenceState.Suspended)
                {
                    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkFirstPressServo.Stop();
                    //Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkUpPress.Stop();
                }
                else if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkFirstPressServo.State ==
                    FALibrary.Sequence.SequenceState.Suspending)
                {
                    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkFirstPressServo.Stop();
                    //Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.FirstPressModule.WorkUpPress.Stop();
                }
            }
        }
        private void SecondPress(object sender, RoutedEventArgs e)
        {
            if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.State ==
                   Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.StateStop)
            {
                // Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.SecondPressModule.WorkPress2.Start();


                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkSecondManualPressServo.Start();


                if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.WorkPress2.State ==
               FALibrary.Sequence.SequenceState.Suspended)
                {
                    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.SecondPressModule.WorkPress2.Stop();
                    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.SecondPressModule.WorkUpPress.Stop();
                }
                else if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.WorkPress2.State ==
                    FALibrary.Sequence.SequenceState.Suspending)
                {
                    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.SecondPressModule.WorkPress2.Stop();
                    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.SecondPressModule.WorkUpPress.Stop();
                }
            }
        }
        private void ThirdPress(object sender, RoutedEventArgs e) //
        {
            if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.State ==
                     Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.StateStop)
            {
                //Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.ThirdPressModule.WorkPress2.Start();

                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkThridManualPressServo.Start();


                if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.WorkPress2.State ==
               FALibrary.Sequence.SequenceState.Suspended)
                {
                    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.ThirdPressModule.WorkPress2.Stop();
                    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.ThirdPressModule.WorkUpPress.Stop();
                }
                else if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.WorkPress2.State ==
                    FALibrary.Sequence.SequenceState.Suspending)
                {
                    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.ThirdPressModule.WorkPress2.Stop();
                    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.ThirdPressModule.WorkUpPress.Stop();
                }

            }
        }
        private void FourthPress(object sender, RoutedEventArgs e)
        {
            if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.State ==
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.StateStop)
            {
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.WorkFourthManualPressServo.Start();

                if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FourthPressModule.WorkPress2.State ==
              FALibrary.Sequence.SequenceState.Suspended)
                {
                    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.WorkFourthManualPressServo.Stop();
                  //  Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.FourthPressModule.WorkUpPress.Stop();
                }
                else if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FourthPressModule.WorkPress2.State ==
                    FALibrary.Sequence.SequenceState.Suspending)
                {
                    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.WorkFourthManualPressServo.Stop();
                   // Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.FourthPressModule.WorkUpPress.Stop();
                }
            }
        }
        private void OptionPress(object sender, RoutedEventArgs e)
        {
            if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.State ==
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.StateStop)
            {
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkOptionManualPressServo.Start();
               
                if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.WorkPress2.State ==
              FALibrary.Sequence.SequenceState.Suspended)
                {
                    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkOptionManualPressServo.Stop();
                    //Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.OptionPressModule.WorkUpPress.Stop();
                }
                else if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.WorkPress2.State ==
                    FALibrary.Sequence.SequenceState.Suspending)
                {
                    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkOptionManualPressServo.Stop();
                   // Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.OptionPressModule.WorkUpPress.Stop();
                }
            }
        }
        private void OnePitchMove(object sender, RoutedEventArgs e)
        {
            if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.State ==
               Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.StateStop)
            {
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualLoading.Start();

                if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualLoading.State ==
              FALibrary.Sequence.SequenceState.Suspended)
                {
                    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualLoading.Stop();
                }
                else if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualLoading.State ==
                    FALibrary.Sequence.SequenceState.Suspending)
                {
                    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualLoading.Stop();
                }
            }
        }

        private void BandPitchChange(object sender, RoutedEventArgs e)
        {
            if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.State ==
              Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.StateStop)
            {
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualMoving.Start();

                if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualMoving.State ==
              FALibrary.Sequence.SequenceState.Suspended)
                {
                    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualMoving.Stop();
                }
                else if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualMoving.State ==
                    FALibrary.Sequence.SequenceState.Suspending)
                {
                    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualMoving.Stop();
                }
            }
        }
        private void PackingOneCycle(object sender, RoutedEventArgs e)
        {
            if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.State ==
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.StateStop)
            {
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.WorkRearPullManual.Start(); //

                if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.WorkRearPullManual.State ==
           FALibrary.Sequence.SequenceState.Suspended)
                {
                    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.WorkRearPullManual.Stop();
                }
                else if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.WorkRearPullManual.State ==
                    FALibrary.Sequence.SequenceState.Suspending)
                {
                    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.WorkRearPullManual.Stop();
                }
            }
        }
        private void SearchingImark(object sender, RoutedEventArgs e)
        {
            if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.State ==
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.StateStop)
            {
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.SearchingImark.Start(); //

                if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.SearchingImark.State ==
          FALibrary.Sequence.SequenceState.Suspended)
                {
                    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.SearchingImark.Stop();
                }
                else if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.SearchingImark.State ==
                    FALibrary.Sequence.SequenceState.Suspending)
                {
                    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.SearchingImark.Stop();
                }
            }
        }

        private void OneAllCycle(object sender, RoutedEventArgs e)
        {
            if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.State ==
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.StateStop)
            {
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualOnceOneCycle.Start(); //

                if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualOnceOneCycle.State ==
          FALibrary.Sequence.SequenceState.Suspended)
                {
                    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualOnceOneCycle.Stop();
                }
                else if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualOnceOneCycle.State ==
                    FALibrary.Sequence.SequenceState.Suspending)
                {
                    Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontModule.WorkManualOnceOneCycle.Stop();
                }
            }
        }

        private void Check(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.UseIMark = true;
        }
        private void UnCheck(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearModule.UseIMark = false;
        }

        private void FirstPress_MotorRun_Button_Check(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.MotorRun.On.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontLoadingUnit.FirstPressServo.ServoOnAction.Execute(this);

        }

        private void FirstPress_MotorRun_Button_UnCheck(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.MotorRun.Off.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontLoadingUnit.FirstPressServo.ServoOffAction.Execute(this);

        }

        private void SecondPress_MotorRun_Button_Check(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.MotorRun.On.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontLoadingUnit.SecondPressServo.ServoOnAction.Execute(this);

        }

        private void SecondPress_MotorRun_Button_UnCheck(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.MotorRun.Off.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontLoadingUnit.SecondPressServo.ServoOffAction.Execute(this);
        }
        
        private void OptionPress_MotorRun_Button_Check(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.MotorRun.On.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontLoadingUnit.OptionServo.ServoOnAction.Execute(this);
        }

        private void OptionPress_MotorRun_Button_UnCheck(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.MotorRun.Off.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontLoadingUnit.OptionServo.ServoOffAction.Execute(this);
        }

        private void ThirdPress_MotorRun_Button_Check(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.MotorRun.On.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontLoadingUnit.ThirdPressServo.ServoOnAction.Execute(this);
        }

        private void ThirdPress_MotorRun_Button_UnCheck(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.MotorRun.Off.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FrontLoadingUnit.ThirdPressServo.ServoOffAction.Execute(this);
        }

        private void FourthPress_MotorRun_Button_Check(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FourthPressModule.MotorRun.On.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearLoadingUnit.FourthPressServo.ServoOnAction.Execute(this);
        }

        private void FourthPress_MotorRun_Button_UnCheck(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FourthPressModule.MotorRun.Off.Execute(this);
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.RearLoadingUnit.FourthPressServo.ServoOffAction.Execute(this);
        }
        

        private void UseManualFirstPress(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.ManualUsePress = true;
        }

        private void UnUseManualFirstPress(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FirstPressModule.ManualUsePress = false;
        }

        private void UseManualSecondPress(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.ManualUsePress = true;
        }

        private void UnUseManualSecondPress(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.SecondPressModule.ManualUsePress = false;
        }

        private void UseManualThirdPress(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.ManualUsePress = true;
        }

        private void UnUseManualThirdPress(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.ManualUsePress = false ;
        }

        private void UseManualFourthPress(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FourthPressModule.ManualUsePress = true;
        }

        private void UnUseManualFourthPress(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.FourthPressModule.ManualUsePress = false;
        }

        private void UseThird(object sender, RoutedEventArgs e)
        {
            if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.State ==
          Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.StateStop)
            {
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.UsePress = false;
            }
        }

        private void UnUseThird(object sender, RoutedEventArgs e)
        {
            if (Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.State ==
          Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.StateStop)
            {
                Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ThirdPressModule.UsePress = false;
            }
        }

        private void UseManualOptionPress(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.ManualUsePress = true;
        }

        private void UnUseManualOptionPress(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.OptionPressModule.ManualUsePress = false;
        }
        
    }
}
