using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// FanControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FanControl : UserControl
    {
        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register("Angle",
            typeof(double), typeof(FanControl));

        public double Angle
        {
            get { return (double)this.GetValue(AngleProperty); }
            set
            {
                if (RotateFan)
                    this.SetValue(AngleProperty, this);
            }
        }

        public static readonly DependencyProperty RotateFanProperty = DependencyProperty.Register("RotateFan",
            typeof(bool), typeof(FanControl));

        public bool RotateFan
        {
            get { return (bool)this.GetValue(RotateFanProperty); }
            set
            {
                this.SetValue(RotateFanProperty, this);
            }
        }

        public FanControl()
        {
            InitializeComponent();
        }
    }
}
