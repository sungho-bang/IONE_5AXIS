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
using System.IO;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Linq;
using FALibrary;
using FALibrary.Sequence;
using FAFramework.Utility;

namespace FAFramework.VT3500.GUI
{
    /// <summary>
    /// IntializeControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class IntializeControl : UserControl
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
            DependencyProperty.Register("EquipmentInstance", typeof(Equipment.EquipmentBase), typeof(IntializeControl));

        public Equipment.EquipmentBase EquipmentInstance
        {
            get { return (Equipment.EquipmentBase)GetValue(EquipmentInstanceProperty); }
            set
            {
                SetValue(EquipmentInstanceProperty, value);
            }
        }

        public IntializeControl()
        {
            InitializeComponent();
        }

        private void button1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

    }
}
