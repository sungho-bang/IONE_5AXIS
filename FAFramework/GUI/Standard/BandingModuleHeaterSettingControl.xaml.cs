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

namespace FAFramework.GUI.Standard
{
    /// <summary>
    /// BandingMachineHeaterSettingControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class BandingModuleHeaterSettingControl : UserControl
    {
        public static readonly DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register("ReadOnly", typeof(bool), typeof(BandingModuleHeaterSettingControl));
        public static readonly DependencyProperty BandingModuleProperty =
            DependencyProperty.Register("BandingModule", typeof(FAFramework.Module.FABandingModule), typeof(BandingModuleHeaterSettingControl));

        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set
            {
                SetValue(ReadOnlyProperty, value);
            }
        }

        public FAFramework.Module.FABandingModule BandingModule
        {
            get { return (FAFramework.Module.FABandingModule)GetValue(BandingModuleProperty); }
            set
            {
                SetValue(BandingModuleProperty, value);
            }
        }

        public BandingModuleHeaterSettingControl()
        {
            InitializeComponent();
        }
    }
}
