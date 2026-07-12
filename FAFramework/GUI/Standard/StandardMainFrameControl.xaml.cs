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
using System.Globalization;
using FAFramework.Utility;

namespace FAFramework.GUI.Standard
{
    /// <summary>
    /// StandardMainFrameControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class StandardMainFrameControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region DependencyProperty
        public static readonly DependencyProperty EquipmentInstanceProperty =
            DependencyProperty.Register("EquipmentInstance", typeof(Equipment.EquipmentBase), typeof(StandardMainFrameControl));
        #endregion

        #region Pages
        private UIElement _pageMain;
        public UIElement PageMain
        {
            get { return _pageMain; }
            set
            {
                _pageMain = value;
                NotifyPropertyChanged("PageMain");
            }
        }

        private UIElement _pageManual;
        public UIElement PageManual
        {
            get { return _pageManual; }
            set
            {
                _pageManual = value;
                NotifyPropertyChanged("PageManual");
            }
        }

        private UIElement _pageConfig;
        public UIElement PageConfig
        {
            get { return _pageConfig; }
            set
            {
                _pageConfig = value;
                NotifyPropertyChanged("PageConfig");
            }
        }

        private UIElement _pageLog;
        public UIElement PageLog
        {
            get { return _pageLog; }
            set
            {
                _pageLog = value;
                NotifyPropertyChanged("PageLog");
            }
        }
        #endregion

        private UIElement _currentPage;
        public UIElement CurrentPage
        {
            get { return _currentPage; }
            set
            {
                if (_currentPage == value) return;

                //if (EquipmentInstance.State == EquipmentInstance.StateStop)
                //{
                //    if (_currentPage != PageConfig)
                //        Equipment.MainEquipment.Instance.Save();
                //    else if (_currentPage == PageConfig)
                //        Equipment.MainEquipment.Instance.LoadBackup();
                //}

                _currentPage = value;
                NotifyPropertyChanged("CurrentPage");

                if (value == PageMain)
                    Manager.LogManager.Instance.WriteTraceLog(EquipmentInstance, "OPEN MAIN PAGE");
                else if (value == PageManual)
                    Manager.LogManager.Instance.WriteTraceLog(EquipmentInstance, "OPEN MANUAL PAGE");
                else if (value == PageConfig)
                    Manager.LogManager.Instance.WriteTraceLog(EquipmentInstance, "OPEN CONFIG PAGE");
                else if (value == PageLog)
                    Manager.LogManager.Instance.WriteTraceLog(EquipmentInstance, "OPEN LOG PAGE");
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

        public CommandHandler ClickMenu { get; set; }

        public CommandHandler ClickOpenDevice { get; set; }

        public StandardMainFrameControl()
        {
            ClickMenu = new CommandHandler(
                delegate (object obj)
                {
                    if (obj is UIElement)
                    {
                        if (obj == null) return;
                        ShowPage(obj as UIElement);
                    }
                }, true);

            ClickOpenDevice = new CommandHandler(
                delegate (object obj)
                {
                    Equipment.MainEquipment.Instance.DeviceOpenRequest = true;
                }, true);

            InitializeComponent();
        }

        public void ShowPage(UIElement control)
        {
            if (control != null)
                CurrentPage = control;
        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string msg = Utility.UtilityClass.GetStringResource(this, "AreYouSureYouWantToExit", "Are You Sure You Want To Exit");

                var result = (MessageBox.Show(msg, "", MessageBoxButton.YesNo));
                if (result == MessageBoxResult.No) return;

                if (EquipmentInstance != null)
                {
                    Equipment.MainEquipment.Instance.Stop();
                    Equipment.MainEquipment.Instance.Save();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Application.Current.Shutdown();
        }

        private void buttonReset_Click(object sender, RoutedEventArgs e)
        {
            if (EquipmentInstance != null)
                EquipmentInstance.ReleaseEmergency();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Application.Current.MainWindow != null)
                ShowPage(PageMain);
        }
    }
}
