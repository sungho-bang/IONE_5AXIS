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

namespace FAFramework.GUI.Standard
{
    /// <summary>
    /// StandardMainControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class StandardMainControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region DependencyProperty
        public static readonly DependencyProperty EquipmentInstanceProperty =
            DependencyProperty.Register("EquipmentInstance", typeof(Equipment.EquipmentBase), typeof(StandardMainControl));
        #endregion

        public ImageSource _imageLogo;
        public ImageSource ImageLogo
        {
            get { return _imageLogo; }
            set
            {
                _imageLogo = value;
                NotifyPropertyChanged("ImageLogo");
            }
        }

        private Page _pageLeftSide;
        public Page PageLeftSide
        {
            get { return _pageLeftSide; }
            set
            {
                _pageLeftSide = value;
                NotifyPropertyChanged("PageLeftSide");
            }
        }

        private FrameworkElement _centerControl;
        public FrameworkElement CenterControl
        {
            get { return _centerControl; }
            set
            {
                _centerControl = value;
                NotifyPropertyChanged("CenterControl");
            }
        }

        private FrameworkElement _buttonsControl;
        public FrameworkElement ButtonsControl
        {
            get { return _buttonsControl; }
            set
            {
                _buttonsControl = value;
                NotifyPropertyChanged("ButtonsControl");
            }
        }

        private AlarmControl _alarmControlInstance;
        public AlarmControl AlarmControlInstance
        {
            get { return _alarmControlInstance; }
            set
            {
                if (_alarmControlInstance == value) return;
                _alarmControlInstance = value;
                NotifyPropertyChanged("AlarmControlInstance");
            }
        }

        private string _traceLog;
        public string TraceLog
        {
            get { return _traceLog; }
            set
            {
                _traceLog = value;
                NotifyPropertyChanged("TraceLog");
            }
        }

        private Control _alarmMainImage;
        public Control AlarmMainImage
        {
            get { return _alarmMainImage; }
            set
            {
                if (_alarmMainImage == value) return;
                _alarmMainImage = value;
                NotifyPropertyChanged("AlarmMainImage");
            }
        }

        private DateTime _currentTime;
        public DateTime CurrentTime
        {
            get { return _currentTime; }
            set
            {
                _currentTime = value;
                NotifyPropertyChanged("CurrentTime");
            }
        }

        private LinkedList<string> _traceLogList = new LinkedList<string>();

        private DateTime _statusLabelDownTime = DateTime.Now;

        public ICommand UserIDClick { get; set; }

        public Equipment.EquipmentBase EquipmentInstance
        {
            get { return (Equipment.EquipmentBase)GetValue(EquipmentInstanceProperty); }
            set
            {
                SetValue(EquipmentInstanceProperty, value);
            }
        }
        
        public StandardMainControl()
        {
           
            InitializeComponent();

            UserIDClick = new Utility.CommandHandler(
              delegate (object sender)
              {
                  UserSelectWindow win = new UserSelectWindow();
                  win.EquipmentInstance = EquipmentInstance;
                  win.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);

                  if ((bool)win.ShowDialog() == true)
                  {
                      EquipmentInstance.CurrentUser = win.SelectedUser;
                  }
              }, true);

            Manager.LogManager.Instance.OnWriteTraceLog += WriteTraceLog;

            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timer.Tick +=
                delegate (object sender, EventArgs e)
                {
                    CurrentTime = DateTime.Now;
                };

            timer.Start();
        }

        private void WriteTraceLog(object sender, Manager.LogEventArgs e)
        {
            Dispatcher.Invoke(
                new Action(
                    delegate
                    {
                        if (e.Equipment == EquipmentInstance)
                        {
                            var log = e.Date.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + e.Log;
                            if (_traceLogList.Count > 200)
                                _traceLogList.Clear();

                            _traceLogList.AddFirst(log);
                            TraceLog = string.Join("\n", _traceLogList);
                        }
                    }));
        }

        private void labelState_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _statusLabelDownTime = DateTime.Now;
        }

        private void labelState_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if ((DateTime.Now - _statusLabelDownTime).TotalSeconds > 3)
            {
                Equipment.MainEquipment.Instance.CreateDumpFileInOtherThread();
            }

            _statusLabelDownTime = DateTime.Now;
        }

        private void TextBoxTraceLog_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                _traceLogList.Clear();
                TraceLog = "";
            }
        }
        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var file = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "readme.txt");
                if (System.IO.File.Exists(file))
                {
                    System.Diagnostics.Process.Start("notepad.exe", file);
                }
                else
                {
                    MessageBox.Show($"버전 정보 파일({file})이 존재하지 않습니다.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"버전 정보를 보여줄 수 없습니다.\n{ex.ToString()}");
            }
        }

        private void labelSWVersion_MouseDown(object sender, MouseButtonEventArgs e)
        {
            WindowVersionInfo dlg = new WindowVersionInfo();
            dlg.Show();
        }
    }
}
