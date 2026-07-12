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
    /// SubUnitRetryTimeSetting.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SubUnitRetryTimeSettingControl : UserControl, INotifyPropertyChanged
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
        public static readonly DependencyProperty SubUnitProperty =
            DependencyProperty.Register("SubUnit", typeof(Equipment.SubUnitBase), typeof(SubUnitRetryTimeSettingControl));
        public static readonly DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register("ReadOnly", typeof(bool), typeof(SubUnitRetryTimeSettingControl));
        #endregion

        public Equipment.SubUnitBase SubUnit
        {
            get { return (Equipment.SubUnitBase)GetValue(SubUnitProperty); }
            set
            {
                SetValue(SubUnitProperty, value);
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

        public SubUnitRetryTimeSettingControl()
        {
            InitializeComponent();
        }
    }
}
