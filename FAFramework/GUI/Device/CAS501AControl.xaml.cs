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
using System.Globalization;
using FAFramework.Utility;

namespace FAFramework.GUI.Device
{
    /// <summary>
    /// CAS501AControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CAS501AControl : UserControl
    {
        public static readonly DependencyProperty PartProperty =
            DependencyProperty.Register("Part", typeof(FALibrary.Part.LoadCell.FACI501A), typeof(CAS501AControl));

        public FALibrary.Part.LoadCell.FACI501A Part
        {
            get { return (FALibrary.Part.LoadCell.FACI501A)GetValue(PartProperty); }
            set
            {
                SetValue(PartProperty, value);
            }
        }

        public ICommand SetZeroCommand { get; set; }

        public CAS501AControl()
        {
            SetZeroCommand = new CommandHandler(
                delegate (object param)
                {
                    if (Part == null) return;

                    try
                    {
                        Part.SetZero();
                    }
                    catch (Exception e)
                    {
                        Manager.LogManager.Instance.WriteSystemLog(e.ToString());
                    }
                },
                true);

            InitializeComponent();
        }
    }
}
