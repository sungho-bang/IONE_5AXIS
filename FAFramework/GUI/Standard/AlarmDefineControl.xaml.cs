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
using System.Globalization;
using System.ComponentModel;
using FALibrary.Alarm;

namespace FAFramework.GUI.Standard
{
    /// <summary>
    /// AlarmDefineControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AlarmDefineControl : UserControl, INotifyPropertyChanged
    {
        public sealed class ImageConverter : IValueConverter
        {
            public object Convert(object value, Type targetType,
                                  object parameter, CultureInfo culture)
            {
                try
                {
                    return new BitmapImage(new Uri((string)value));
                }
                catch
                {
                    return new BitmapImage();
                }
            }

            public object ConvertBack(object value, Type targetType,
                                      object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public static readonly DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register("ReadOnly", typeof(bool), typeof(AlarmDefineControl));

        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set
            {
                SetValue(ReadOnlyProperty, value);
            }
        }

        private FAAlarm _selectedItem;
        public FAAlarm SelectedItem
        {
            get { return _selectedItem; }

            set
            {
                _selectedItem = value;

                if (value != null && Alarm != null)
                    value.CopyTo(Alarm);
            }
        }

        private FAAlarm _alarm;
        public FAAlarm Alarm
        {
            get { return _alarm; }
            set
            {
                if (_alarm == value) return;

                _alarm = value;
                NotifyPropertyChanged("Alarm");
            }
        }

        private SortedDictionary<int, FAAlarm> _alarmList;
        public SortedDictionary<int, FAAlarm> AlarmList
        {
            get { return _alarmList; }
            set
            {
                _alarmList = value;
                NotifyPropertyChanged("AlarmList");
            }
        }

        public AlarmDefineControl()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this) == false)
            {
                Equipment.MainEquipment.Instance.AlarmResourceManager.OnLoadAlarmList +=
                    delegate
                    {
                        AlarmList = FALibrary.Alarm.FAAlarmManager.Instance.Items;
                    };

                Alarm = new FAAlarm();

                AlarmList = FALibrary.Alarm.FAAlarmManager.Instance.Items;
            }
        }

        private bool AddAlarm(FAAlarm alarm)
        {
            if (FAAlarmManager.Instance.Items.ContainsKey(alarm.AlarmNo) == true)
            {
                var msg = Utility.UtilityClass.GetStringResource(this, "TheAlarmCodeYouHaveEnteredAlreadyExists", "The Alarm Code You Have Entered Already Exists");

                MessageBox.Show(msg);
                return false;
            }

            FAAlarm copiedAlarm = new FAAlarm();
            alarm.CopyTo(copiedAlarm);
            FAAlarmManager.Instance.Items.Add(alarm.AlarmNo, copiedAlarm);
            return true;
        }

        private bool RemoveAlarm(FAAlarm alarm)
        {
            if (SelectedItem == null) return false;

            if (AlarmList.ContainsKey(SelectedItem.AlarmNo) == true)
            {
                AlarmList.Remove(SelectedItem.AlarmNo);
                return true;
            }

            return false;
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            AddAlarm(Alarm);
            UpdateAlarmList();
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null) return;

            if (Alarm.AlarmNo == SelectedItem.AlarmNo)
                Alarm.CopyTo(SelectedItem);
            else
            {
                if (AddAlarm(Alarm) == true)
                {
                    RemoveAlarm(SelectedItem);
                }
            }

            UpdateAlarmList();
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            RemoveAlarm(SelectedItem);
            UpdateAlarmList();
        }

        private void buttonSetImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Filter = "All Image Files | *.*";
            if ((bool)dlg.ShowDialog())
            {
                Alarm.ImagePath = dlg.FileName;
            }
        }

        private void buttonClearImage_Click(object sender, RoutedEventArgs e)
        {
            Alarm.ImagePath = "";
        }

        private void UpdateAlarmList()
        {
            AlarmList = null;
            AlarmList = FALibrary.Alarm.FAAlarmManager.Instance.Items;
            Equipment.MainEquipment.Instance.AlarmResourceManager.Save();
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
