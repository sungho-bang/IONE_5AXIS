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
    /// QuestionMessageBoxWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class QuestionMessageBoxWindow : Window, INotifyPropertyChanged
    {
        public enum QuestionResult
        {
            None, Yes, No, Cancel
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private bool _useSound = true;
        public bool UseSound
        {
            get { return _useSound; }
            set
            {
                _useSound = value;
                NotifyPropertyChanged("UseSound");
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

        private bool _cancelable;
        public bool Cancelable
        {
            get { return _cancelable; }
            set
            {
                if (_cancelable == value) return;
                _cancelable = value;
                NotifyPropertyChanged("Cancelable");
            }
        }

        public QuestionResult Result { get; set; }

        public ICommand YesCommand { get; set; }
        public ICommand NoCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public QuestionMessageBoxWindow()
        {
            Result = QuestionResult.None;

            YesCommand = new CommandHandler(
                delegate (object sender)
                {
                    Result = QuestionResult.Yes;
                    if (EquipmentInstance != null)
                        EquipmentInstance.TurnOffSound();

                    this.Close();
                },
                true);

            NoCommand = new CommandHandler(
                delegate (object sender)
                {
                    Result = QuestionResult.No;
                    if (EquipmentInstance != null)
                        EquipmentInstance.TurnOffSound();

                    this.Close();
                },
                true);

            CancelCommand = new CommandHandler(
                delegate (object sender)
                {
                    Result = QuestionResult.Cancel;
                    if (EquipmentInstance != null)
                        EquipmentInstance.TurnOffSound();

                    this.Close();
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
                Owner = EquipmentInstance.Window;

                if (UseSound)
                    EquipmentInstance.TurnOnSound();
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (EquipmentInstance != null)
            {
                if (UseSound)
                    EquipmentInstance.TurnOffSound();
            }
        }
    }
}
