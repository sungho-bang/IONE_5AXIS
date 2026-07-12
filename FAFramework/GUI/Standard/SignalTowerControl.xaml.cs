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
    /// SignalTowerControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SignalTowerControl : UserControl, INotifyPropertyChanged
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
        public static readonly DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register("ReadOnly", typeof(bool), typeof(SignalTowerControl));
        public static readonly DependencyProperty StateConfigProperty =
            DependencyProperty.Register("StateConfig", typeof(FAFramework.ConfigClasses.EquipmentStateConfigGroup), typeof(SignalTowerControl));
        #endregion

        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set
            {
                SetValue(ReadOnlyProperty, value);
            }
        }

        public FAFramework.ConfigClasses.EquipmentStateConfigGroup StateConfig
        {
            get { return (FAFramework.ConfigClasses.EquipmentStateConfigGroup)GetValue(StateConfigProperty); }
            set
            {
                SetValue(StateConfigProperty, value);
            }
        }

        public SignalTowerControl()
        {
            InitializeComponent();
        }
    }
}
