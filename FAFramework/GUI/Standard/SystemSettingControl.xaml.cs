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
using System.Collections.ObjectModel;

namespace FAFramework.GUI.Standard
{
    /// <summary>
    /// SystemSettingControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SystemSettingControl : UserControl, INotifyPropertyChanged
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
            DependencyProperty.Register("ReadOnly", typeof(bool), typeof(SystemSettingControl));
        public static readonly DependencyProperty ConfigProperty =
            DependencyProperty.Register("Config", typeof(object), typeof(SystemSettingControl), new PropertyMetadata(new PropertyChangedCallback(ConfigChanged)));
        public static readonly DependencyProperty AdditionalControlProperty =
            DependencyProperty.Register("AdditionalControl", typeof(Control), typeof(SystemSettingControl));
        #endregion

        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set
            {
                SetValue(ReadOnlyProperty, value);
            }
        }

        public object Config
        {
            get { return GetValue(ConfigProperty); }
            set
            {
                SetValue(ConfigProperty, value);
            }
        }

        public Control AdditionalControl
        {
            get { return (Control)GetValue(AdditionalControlProperty); }
            set
            {
                SetValue(AdditionalControlProperty, value);
            }
        }

        private object _extractedConfig;
        public object ExtractedConfig
        {
            get { return _extractedConfig; }
            set
            {
                _extractedConfig = value;
                NotifyPropertyChanged("ExtractedConfig");
            }
        }

        public SystemSettingControl()
        {
            InitializeComponent();
        }

        public void ExtractConfig(object config)
        {
            if (config == null) return;

            Type[] exceptType =
                new Type[] { typeof(FALibrary.Utility.SerializableDictionary<string, FALibrary.FARange>) };

            Type[] exceptAttributeType =
                new Type[] { typeof(Utility.ExceptExtractProperty) };

            ExtractedConfig = Utility.ObjectElementExtractor.ExtractElement(config,
                string.Empty,
                false,
                exceptType,
                exceptAttributeType,
                null);
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            //Utility.UtilityClass.TextBox_GotFocus(sender, e);
        }

        public static void ConfigChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            dynamic obj = d;
            obj.Config = e.NewValue;
            obj.ExtractConfig(e.NewValue);
        }
    }
}
