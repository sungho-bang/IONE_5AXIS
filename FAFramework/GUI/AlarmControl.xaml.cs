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

namespace FAFramework.GUI
{
    /// <summary>
    /// AlarmControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AlarmControl : UserControl, INotifyPropertyChanged
    {

        private Control _alarmMainImageControl;
        public Control AlarmMainImageControl
        {
            get { return _alarmMainImageControl; }
            set
            {
                _alarmMainImageControl = value;
                NotifyPropertyChanged("AlarmMainImageControl");
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

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public static readonly DependencyProperty AlarmRaisingStatusManagerProperty =
            DependencyProperty.Register("AlarmRaisingStatusManager", typeof(Manager.AlarmRaisingStatusManager), typeof(AlarmControl));
        public static readonly DependencyProperty AlarmMainImageControlProperty =
            DependencyProperty.Register("AlarmMainImageControl", typeof(Control), typeof(AlarmControl));

        public Manager.AlarmRaisingStatusManager AlarmRaisingStatusManager
        {
            get { return (Manager.AlarmRaisingStatusManager)GetValue(AlarmRaisingStatusManagerProperty); }
            set
            {
                SetValue(AlarmRaisingStatusManagerProperty, value);
            }
        }

        public AlarmControl()
        {
            InitializeComponent();
        }

        private void imageAlarmImage_MediaEnded(object sender, RoutedEventArgs e)
        {
            try
            {
                var mediaElement = sender as MediaElement;
                var source = mediaElement.Source;
                mediaElement.Source = null;
                mediaElement.Source = source;
            }
            catch
            {
            }
        }
    }
}
