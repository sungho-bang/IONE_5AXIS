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

namespace FAFramework.GUI.Standard
{
    /// <summary>
    /// ECCommunicationControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ECCommunicationControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public static readonly DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register("ReadOnly", typeof(bool), typeof(ECCommunicationControl));
        public static readonly DependencyProperty PartProperty =
            DependencyProperty.Register("Part", typeof(object), typeof(ECCommunicationControl));

        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set
            {
                SetValue(ReadOnlyProperty, value);
            }
        }

        public object Part
        {
            get { return (FALibrary.Part.CommunicationPart.EC.FAECPart)GetValue(PartProperty); }
            set
            {
                SetValue(PartProperty, value);
            }
        }

        private FrameworkElement _commandControl;
        public FrameworkElement CommandControl
        {
            get { return _commandControl; }
            set
            {
                _commandControl = value;
                NotifyPropertyChanged("CommandControl");
            }
        }

        public ECCommunicationControl()
        {
            InitializeComponent();
        }
    }
}
