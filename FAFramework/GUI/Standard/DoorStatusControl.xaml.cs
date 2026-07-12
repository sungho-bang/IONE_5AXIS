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
using FALibrary.Part.MemoryBasePart;

namespace FAFramework.GUI.Standard
{
    public enum DoorOpenDirection
    {
        Left, // 힌지가 왼쪽
        Right, // 힌지가 오른쪽
        FrontSlide // 정면으로 잡아당김
    }

    /// <summary>
    /// DoorStatusControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DoorStatusControl : UserControl
    {
        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle",
                typeof(double),
                typeof(DoorStatusControl));

        public double Angle
        {
            get { return (double)this.GetValue(AngleProperty); }
            set { this.SetValue(AngleProperty, this); }
        }

        public static readonly DependencyProperty DoorOpenDirectionProperty =
            DependencyProperty.Register("DoorOpenDirection",
                typeof(DoorOpenDirection),
                typeof(DoorStatusControl));

        public DoorOpenDirection DoorOpenDirection
        {
            get { return (DoorOpenDirection)this.GetValue(DoorOpenDirectionProperty); }
            set { this.SetValue(DoorOpenDirectionProperty, this); }
        }

        public static readonly DependencyProperty DoorPartProperty =
            DependencyProperty.Register("DoorPart",
                typeof(FAPartDoor),
                typeof(DoorStatusControl));

        public FAPartDoor DoorPart
        {
            get { return (FAPartDoor)this.GetValue(DoorPartProperty); }
            set { this.SetValue(DoorPartProperty, this); }
        }

        public DoorStatusControl()
        {
            InitializeComponent();
        }
    }
}
