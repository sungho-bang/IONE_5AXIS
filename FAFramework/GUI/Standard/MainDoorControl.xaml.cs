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
    /// MainDoorControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainDoorControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public static readonly DependencyProperty EquipmentInstanceProperty =
            DependencyProperty.Register("EquipmentInstance", typeof(Equipment.EquipmentBase), typeof(MainDoorControl));

        public Equipment.EquipmentBase EquipmentInstance
        {
            get { return (Equipment.EquipmentBase)GetValue(EquipmentInstanceProperty); }
            set
            {
                SetValue(EquipmentInstanceProperty, value);
            }
        }

        public CommandHandler OpenDoorCommand { get; set; }
        public CommandHandler CloseDoorCommand { get; set; }

        public MainDoorControl()
        {
            OpenDoorCommand = new CommandHandler(
                obj =>
                {
                    if (EquipmentInstance != null)
                    {
                        EquipmentInstance.OpenDoor();
                    }
                }, true);

            CloseDoorCommand = new CommandHandler(
                obj =>
                {
                    if (EquipmentInstance != null)
                    {
                        EquipmentInstance.CloseDoor();
                    }
                }, true);

            InitializeComponent();
        }
    }
}
