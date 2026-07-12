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
    /// IOListControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class IOListControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private Orientation _gridOrientation;
        public Orientation GridOrientation
        {
            get { return _gridOrientation; }
            set
            {
                _gridOrientation = value;
                NotifyPropertyChanged("GridOrientation");
            }
        }

        public static readonly DependencyProperty PartProperty =
            DependencyProperty.Register("Part", typeof(FALibrary.Part.MemoryBasePart.FAMemoryBasePart), typeof(IOListControl));

        public static readonly DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register("ReadOnly", typeof(bool), typeof(IOListControl));

        public FALibrary.Part.MemoryBasePart.FAMemoryBasePart Part
        {
            get { return (FALibrary.Part.MemoryBasePart.FAMemoryBasePart)GetValue(PartProperty); }
            set
            {
                SetValue(PartProperty, value);
            }
        }

        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set
            {
                SetValue(ReadOnlyProperty, value);
            }
        }

        public IOListControl()
        {

            GridOrientation = Orientation.Vertical;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (GridOrientation == Orientation.Horizontal)
                GridOrientation = Orientation.Vertical;
            else
                GridOrientation = Orientation.Horizontal;
        }
    }
}
