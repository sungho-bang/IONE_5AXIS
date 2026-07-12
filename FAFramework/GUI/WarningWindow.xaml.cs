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
using System.Windows.Shapes;
using System.ComponentModel;
using System.Globalization;
using FAFramework.Utility;

namespace FAFramework.GUI
{
    /// <summary>
    /// WarningWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WarningWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private ImageSource _imageSource;
        public ImageSource ImageSource
        {
            get { return _imageSource; }
            set
            {
                _imageSource = value;
                NotifyPropertyChanged("ImageSource");
            }
        }

        private bool _useSound;
        public bool UseSound
        {
            get { return _useSound; }
            set
            {
                _useSound = value;
                NotifyPropertyChanged("UseSound");
            }
        }

        private bool _useCustomSound;
        public bool UseCustomSound
        {
            get { return _useCustomSound; }
            set
            {
                _useCustomSound = value;
                NotifyPropertyChanged("UseCustomSound");
            }

        }
        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                NotifyPropertyChanged("Message");
            }
        }

        private string _caption;
        public string Caption
        {
            get { return _caption; }
            set
            {
                if (_caption == value) return;

                _caption = value;
                NotifyPropertyChanged("Caption");
            }
        }

        private DateTime _raisedTime;
        public DateTime RaisedTime
        {
            get { return _raisedTime; }
            set
            {
                if (_raisedTime == value) return;

                _raisedTime = value;
                NotifyPropertyChanged("RaisedTime");
            }
        }

        private Equipment.EquipmentBase _equipmentInstance;
        public Equipment.EquipmentBase EquipmentInstance
        {
            get { return _equipmentInstance; }
            set
            {
                _equipmentInstance = value;
                NotifyPropertyChanged("EquipmentInstance");
            }
        }

        public event EventHandler OnCloseWindow = delegate { };

        public ICommand CloseCommand { get; set; }
        public ICommand ClearSoundCommand { get; set; }

        public WarningWindow()
        {

            CloseCommand = new CommandHandler(
                   delegate (object sender)
                   {
                       if (EquipmentInstance != null)
                           EquipmentInstance.TurnOffSound();

                       this.Close();
                   },
                   true);

            ClearSoundCommand = new CommandHandler(
                delegate (object sender)
                {
                    if (EquipmentInstance != null)
                        EquipmentInstance.TurnOffSound();
                },
                true);

            InitializeComponent();

        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (EquipmentInstance != null)
            {
                if (UseSound && UseCustomSound == false)
                    EquipmentInstance.TurnOnSound();
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            OnCloseWindow(sender, EventArgs.Empty);

            if (EquipmentInstance != null)
            {
                if (UseSound || UseCustomSound)
                    EquipmentInstance.TurnOffSound();
            }
        }
    }
}
