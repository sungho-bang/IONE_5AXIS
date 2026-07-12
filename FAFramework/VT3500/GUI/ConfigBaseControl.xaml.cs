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
using FAFramework.Utility;
using FAFramework.GUI.Standard;   // ★ 이 줄 추가

namespace FAFramework.VT3500.GUI
{
    /// <summary>
    /// ConfigBaseControl.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 
    public partial class ConfigBaseControl : UserControl
    {

        public static readonly DependencyProperty EquipmentInstanceProperty =
       DependencyProperty.Register("EquipmentInstance", typeof(Equipment.EquipmentBase), typeof(ConfigBaseControl));
        public static readonly DependencyProperty ReadOnlyProperty =
           DependencyProperty.Register("ReadOnly", typeof(bool), typeof(ConfigBaseControl));

        public Equipment.EquipmentBase EquipmentInstance
        {
            get { return (Equipment.EquipmentBase)GetValue(EquipmentInstanceProperty); }
            set
            {
                SetValue(EquipmentInstanceProperty, value);
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


        public CommandHandler SaveCommand
        {
            get;
            set;
        }

        // ★ 추가: 이 ConfigBaseControl 안에 로드된 MotorConfigControl 들을 모아두는 리스트
        private readonly List<MotorConfigControl> _motorControls = new List<MotorConfigControl>();
        public ConfigBaseControl()
        {

            SaveCommand = new CommandHandler(
             delegate (object param)
             {
                 try
                 {
                     if (MessageBox.Show("저장하시겠습니까?", "저장",
                                         MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                     {
                          // ★ 1) 먼저 MotorConfigControl 쪽에 입력된 값을
                          //     전부 바인딩 소스(FAMMCPosition/Part)로 반영
                          bool ok = true;

                         foreach (var mc in _motorControls.ToList())
                         {
                             if (mc == null)
                                 continue;

                             if (!mc.ApplyEdits())
                             {
                                 ok = false;
                                 break;
                             }
                         }

                          // ApplyEdits 중 오류가 나면 저장 중단
                          if (!ok)
                             return;

                          // ★ 2) 모든 값이 정상 반영되었으면 기존처럼 실제 저장 실행
                          EquipmentInstance.Save();
                     }
                 }
                 catch (Exception e)
                 {
                     Manager.LogManager.Instance.WriteSystemLog(e.ToString());
                 }
             },
             true);


            InitializeComponent();
        }

        private void MotorConfigControl_Loaded(object sender, RoutedEventArgs e)
        {
            var mc = sender as MotorConfigControl;
            if (mc == null)
                return;

            if (!_motorControls.Contains(mc))
            {
                _motorControls.Add(mc);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    public class AllZeroToBoolConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            foreach (var item in values)
            {
                try
                {
                    int v = (int)System.Convert.ChangeType(item, typeof(int));
                    if (v != 0)
                        return true;
                }
                catch
                {
                }
            }

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("No two way conversion, one way binding only.");
        }
    }

    public class VisiblePartsExistToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                dynamic partList = value;

                foreach (var part in partList)
                {
                    if (part.ShowRetryInfoToScreen || part.ShowTimeToScreen)
                        return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
