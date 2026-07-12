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

namespace FAFramework.GUI.Device
{
    /// <summary>
    /// CognexDataMan.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CognexDataMan : UserControl
    {
        public static readonly DependencyProperty PartProperty =
            DependencyProperty.Register("Part", typeof(FALibrary.Part.PrinterPart.FAPrintronix5000TRPart), typeof(CognexDataMan));

        public FALibrary.Part.ScannerPart.FACognexDataMan Part
        {
            get { return (FALibrary.Part.ScannerPart.FACognexDataMan)GetValue(PartProperty); }
            set
            {
                SetValue(PartProperty, value);
            }
        }

        public CognexDataMan()
        {
            InitializeComponent();
        }
    }
}
