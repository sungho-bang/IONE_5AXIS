using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using FALibrary.Part.MMCPart;

namespace FAFramework.GUI.Standard
{
    /// <summary>
    /// MotorConfigControl.xaml 에 대한 상호작용 로직
    /// </summary>
    public partial class MotorConfigControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Motor 설정을 실제 파일에 저장할 때 호출되는 델리게이트.
        /// 상위 Window(Form) 쪽에서 이 델리게이트에 파일 저장 함수를 연결해 줍니다.
        /// </summary>
        public Action<FAMMCPart> SaveToFileAction { get; set; }

        #region === INotifyPropertyChanged ===
        private void NotifyPropertyChanged(string propertyName = "")
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region === Limit 체크용 포커스/복원/깜빡임 ===

        /// <summary>
        /// TextBox 포커스 진입 시 기존 값을 Tag 에 백업
        /// </summary>
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;

            tb.Tag = tb.Text;
        }

        /// <summary>
        /// Limit 오류 시 TextBox 배경을 빨간색으로 깜빡이기 (XAML 의 BlinkRedStoryboard 사용)
        /// </summary>
        private void BlinkRed(TextBox tb)
        {
            try
            {
                var sb = this.Resources["BlinkRedStoryboard"] as Storyboard;
                if (sb == null) return;

                Storyboard.SetTarget(sb, tb);
                sb.Begin();
            }
            catch
            {
                // 리소스가 없거나 실패하더라도 프로그램이 죽지 않도록 무시
            }
        }

        /// <summary>
        /// 엔터 입력 시 Limit 체크만 수행.
        /// - 허용 범위 밖이면: 경고 + 깜빡임 + 이전 값 복원
        /// - 허용 범위 안이면: 그대로 둠 (실제 반영은 SaveAllWithConfirm 에서 UpdateSource 할 때)
        /// </summary>
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter && e.Key != Key.Return) return;

            e.Handled = true; // 다른 Key 처리 방지

            var tb = sender as TextBox;
            if (tb == null) return;

            double newValue;
            if (!double.TryParse(tb.Text, out newValue))
            {
                MessageBox.Show("숫자만 입력 가능합니다.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                RestoreText(tb);
                return;
            }

            var pos = tb.DataContext as FAMMCPosition;
            if (pos == null)
            {
                // 바인딩이 Position 이 아닌 경우는 Limit 체크 안 함
                return;
            }

            string posName = pos.Name ?? string.Empty;

            // 이름에 "Torque" 가 포함되면 토크 파라미터로 간주
            bool isTorque = posName.IndexOf("torque", StringComparison.OrdinalIgnoreCase) >= 0;

            // 그 외에서 이름에 Home/pos/position 이 들어가면 위치 파라미터로 간주
            bool isPosition =
                !isTorque &&
                (
                    posName.IndexOf("home", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    posName.IndexOf("pos", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    posName.IndexOf("position", StringComparison.OrdinalIgnoreCase) >= 0
                );

            if (isPosition)
            {
                if (!ValidatePositionValue(newValue, pos))
                {
                    BlinkRed(tb);
                    RestoreText(tb);
                }
                return;
            }

            if (isTorque)
            {
                if (!ValidateTorqueValue(newValue, pos))
                {
                    BlinkRed(tb);
                    RestoreText(tb);
                }
                return;
            }

            // 위치/토크 외 항목(속도 등)은 여기서 별도 체크 없이 통과
        }

        /// <summary>
        /// 포커스 진입 시 Tag 로 백업해 둔 값 복원
        /// </summary>
        private void RestoreText(TextBox tb)
        {
            if (tb == null) return;
            if (tb.Tag != null)
                tb.Text = tb.Tag.ToString();
        }

        /// <summary>
        /// 위치값 Limit 검사 (LwLimit ~ UpLimit)
        /// </summary>
        private bool ValidatePositionValue(double value, FAMMCPosition pos)
        {
            double min = pos.LwLimit;
            double max = pos.UpLimit;

            if (value < min || value > max)
            {
                MessageBox.Show(
                    $"{pos.Name} 값은 {min} ~ {max} 범위만 허용됩니다.",
                    "Position Limit Check",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 토크값 Limit 검사 (LwLimit ~ UpLimit 사용)
        /// </summary>
        private bool ValidateTorqueValue(double value, FAMMCPosition pos)
        {
            double min = pos.LwLimit;
            double max = pos.UpLimit;

            if (value < min || value > max)
            {
                MessageBox.Show(
                    $"{pos.Name} 값은 {min} ~ {max} 범위에서만 설정 가능합니다.",
                    "Torque Limit Check",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        #endregion

        #region === DependencyProperty 정의 ===

        public static readonly DependencyProperty PartProperty =
            DependencyProperty.Register(
                "Part",
                typeof(object),
                typeof(MotorConfigControl),
                new PropertyMetadata(new PropertyChangedCallback(PartChanged)));

        public static readonly DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register(
                "ReadOnly",
                typeof(bool),
                typeof(MotorConfigControl));

        public static readonly DependencyProperty UserProperty =
            DependencyProperty.Register(
                "User",
                typeof(Equipment.UserInfo),
                typeof(MotorConfigControl));

        public FAMMCPart Part
        {
            get { return (FAMMCPart)GetValue(PartProperty); }
            set { SetValue(PartProperty, value); }
        }

        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set { SetValue(ReadOnlyProperty, value); }
        }

        public Equipment.UserInfo User
        {
            get { return (Equipment.UserInfo)GetValue(UserProperty); }
            set { SetValue(UserProperty, value); }
        }

        private FAMMCPosition _selectedPosition = new FAMMCPosition();
        public FAMMCPosition SelectedPosition
        {
            get { return _selectedPosition; }
            set
            {
                if (_selectedPosition == value || value == null) return;
                _selectedPosition = value;
                NotifyPropertyChanged("SelectedPosition");
            }
        }

        private FAFramework.Utility.ThreadSafeObservableCollection<FAMMCPosition> _positions =
            new FAFramework.Utility.ThreadSafeObservableCollection<FAMMCPosition>();
        private FAFramework.Utility.ThreadSafeObservableCollection<FAMMCPosition> _positions2 =
            new FAFramework.Utility.ThreadSafeObservableCollection<FAMMCPosition>();

        /// <summary>
        /// 일반 포지션 리스트
        /// </summary>
        public FAFramework.Utility.ThreadSafeObservableCollection<FAMMCPosition> Positions
        {
            get { return _positions; }
            set
            {
                if (_positions == value) return;
                _positions = value;
                NotifyPropertyChanged("Positions");
            }
        }

        /// <summary>
        /// Torque 관련 포지션 리스트
        /// </summary>
        public FAFramework.Utility.ThreadSafeObservableCollection<FAMMCPosition> Positions2
        {
            get { return _positions2; }
            set
            {
                if (_positions2 == value) return;
                _positions2 = value;
                NotifyPropertyChanged("Positions2");
            }
        }

        #endregion

        #region === 생성자 / Part 변경 처리 ===

        public MotorConfigControl()
        {
            InitializeComponent();
        }

        public static void PartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as MotorConfigControl;
            if (ctrl == null) return;

            ctrl.Part = (FAMMCPart)e.NewValue;
            ctrl.ExtractPositions();
        }

        /// <summary>
        /// 숨겨야 하는 Position 이름 목록을 파일에서 읽어온다.
        /// </summary>
        public List<string> GetHiddlePositionNameList()
        {
            var list = new List<string>();

            if (Part == null) return list;
            if (string.IsNullOrEmpty(Part.FullName)) return list;

            string filepath = System.IO.Path.Combine(
                FAFramework.ConfigClasses.GlobalConst.CONFIG_PATH,
                Part.FullName);

            if (!System.IO.File.Exists(filepath))
                return list;

            list.AddRange(System.IO.File.ReadAllLines(filepath));
            return list;
        }

        /// <summary>
        /// Part 내부의 FAMMCPosition 프로퍼티들을 추출하여 Positions / Positions2 에 채운다.
        /// </summary>
        public void ExtractPositions()
        {
            Positions.Clear();
            Positions2.Clear();

            if (Part == null) return;

            PropertyInfo[] propList = Part.GetType().GetProperties();
            List<FAMMCPosition> positionList = new List<FAMMCPosition>();

            foreach (PropertyInfo info in propList)
            {
                if (info.PropertyType == typeof(FAMMCPosition))
                {
                    var p = info.GetValue(Part, null) as FAMMCPosition;
                    if (p != null)
                        positionList.Add(p);
                }
            }

            var hiddenPositionNameList = GetHiddlePositionNameList();

            foreach (var item in positionList)
            {
                if (hiddenPositionNameList.Contains(item.Name))
                    continue;

                // Torque Limit 관련은 별도의 리스트로
                if (item.Name == "TorqueLimitParams" || item.Name == "GetTorqueLimitParams")
                {
                    Positions2.Add(item);
                }
                else
                {
                    // 특정 AxisNo 에 대해 PlacePos / TargetPosition 제외
                    if (Part.AxisNo == 8 || Part.AxisNo == 9 || Part.AxisNo == 10 ||
                        Part.AxisNo == 11 || Part.AxisNo == 12)
                    {
                        if (item.Name == "PlacePos" || item.Name == "TargetPosition")
                            continue;
                    }

                    Positions.Add(item);
                }
            }
        }

        #endregion

        #region === 모터 동작 버튼 이벤트 ===

        private void buttonHome_Click(object sender, RoutedEventArgs e)
        {
            Part?.MoveHome.Execute(sender);
        }

        private void buttonMove_Click(object sender, RoutedEventArgs e)
        {
            if (Part == null || SelectedPosition == null) return;

            SelectedPosition.CopyTo(Part.TargetPosition);
            Part.MoveToPos.Execute(sender);
        }

        private void buttonOn_Click(object sender, RoutedEventArgs e)
        {
            if (Part == null) return;

            if (Part.ServoOn)
                Part.ServoOffAction.Execute(sender);
            else
                Part.ServoOnAction.Execute(sender);
        }

        private void buttonStop_Click(object sender, RoutedEventArgs e)
        {
            if (Part == null) return;

            Part.Stop.Execute(sender);
            Part.AlarmResetAction.Execute(sender);
        }

        private void buttonReset_Click(object sender, RoutedEventArgs e)
        {
            Part?.AlarmResetAction.Execute(sender);
        }

        private void buttonJogNegative_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Part?.MoveJogNegative.Execute(sender);
        }

        private void buttonJogNegative_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Part?.Stop.Execute(sender);
        }

        private void buttonJogPositive_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Part?.MoveJogPositive.Execute(sender);
        }

        private void buttonJogPositive_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Part?.Stop.Execute(sender);
        }

        #endregion

        #region === Torque Limit 적용 버튼 ===

        private void buttonTorqueMv_Click(object sender, RoutedEventArgs e)
        {
            if (Part == null) return;
            if (Positions2 == null || Positions2.Count < 2) return;

            // Positions2[1] : 설정용, Positions2[0] : 읽기용으로 가정
            if (Positions2[1].Position > 100 || Positions2[1].DriveSpeed > 100)
            {
                MessageBox.Show("100% 이상 사용할 수 없습니다.", "Torque Limit", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            double maxLimit = Positions2[1].Position * 2.94;
            double minLimit = Positions2[1].DriveSpeed * 2.94;

            Part.TorqMaxParamter = maxLimit;
            Part.TorqMinParamter = minLimit;

            Part.SetTorqueLimitParams.Execute(sender);
            Part.GetTorqueLimitParams.Execute(sender);

            Positions2[0].Position = Part.GetTorqMaxParamter;
            Positions2[0].DriveSpeed = Part.GetTorqMinParamter;
        }

        #endregion

        #region === 기존 TextChanged 제약 (축별 제한) ===

        // Position 리스트 상단용 제한
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Part == null) return;

            /*
            int axisNo = Part.AxisNo;
            string text = ((TextBox)sender).Text;
            int max_11 = 98;
            int max = 55;

            try
            {
                double val = double.Parse(text);

                if (axisNo == 11)
                {
                    if (val >= max_11)
                    {
                        MessageBox.Show(max_11 + " 사용할 수 없습니다.");
                        ((TextBox)sender).Text = "0";
                    }
                }
                else if (axisNo == 8 || axisNo == 9 || axisNo == 10 || axisNo == 12)
                {
                    if (val >= max)
                    {
                        MessageBox.Show(max + "이상 사용할 수 없습니다.");
                        ((TextBox)sender).Text = "0";
                    }
                }
            }
            catch
            {
                // 숫자 변환 실패시 무시 (저장 시 한 번 더 체크)
            }
            */
            int Axiso = Part.AxisNo;
            String text = ((TextBox)sender).Text;
            int max_11 = 98;
            int max = 55;


            try
            {

                if (Axiso == 11)
                {

                    if (double.Parse(text) >= max_11)
                    {
                        MessageBox.Show(max_11.ToString() + " 사용할 수 없습니다.");

                        ((TextBox)sender).Text = "0";


                    }
                }
                else if (Axiso == 8 || Axiso == 9 || Axiso == 10 || Axiso == 12)
                {
                    if (double.Parse(text) >= max)
                    {
                        MessageBox.Show(max.ToString() + "이상 사용할 수 없습니다.");

                        ((TextBox)sender).Text = "0";


                    }
                }

            }
            catch (Exception ex)
            {


            }
        }

        private void TextBox_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // 필요시 사용
        }

        // Torque 리스트 하단 Position(TextChanged1) 제한
        private void TextBox_TextChanged1(object sender, TextChangedEventArgs e)
        {
            if (Part == null) return;

            /*
            int axisNo = Part.AxisNo;
            string text = ((TextBox)sender).Text;
            double max_11 = 33;
            double max = 101;

            try
            {
                double val = double.Parse(text);

                if (axisNo == 8)
                {
                    if (val >= max_11)
                    {
                        MessageBox.Show(max_11 + " 사용할 수 없습니다.");
                        ((TextBox)sender).Text = "0";
                    }
                }
                else
                {
                    if (val >= max)
                    {
                        MessageBox.Show(max + " 사용할 수 없습니다.");
                        ((TextBox)sender).Text = "0";
                    }
                }
            }
            catch
            {
                // 무시
            }
            */

            int Axiso = Part.AxisNo;
            String text = ((TextBox)sender).Text;
            double max_11 = 33;
            double max = 101;

            try
            {
                if (Axiso == 8)
                {
                    if (double.Parse(text) >= max_11)
                    {
                        MessageBox.Show(max_11.ToString() + " 사용할 수 없습니다.");

                        ((TextBox)sender).Text = "0";


                    }

                }
                else
                {
                    if (double.Parse(text) >= max)
                    {
                        MessageBox.Show(max.ToString() + " 사용할 수 없습니다.");

                        ((TextBox)sender).Text = "0";

                    }

                }
            }
            catch (Exception)
            {


            }

        }

        // Torque 리스트 하단 DriveSpeed(TextChanged2) 제한
        private void TextBox_TextChanged2(object sender, TextChangedEventArgs e)
        {
            if (Part == null) return;

            /*
            int axisNo = Part.AxisNo;
            string text = ((TextBox)sender).Text;
            double max_11 = 33;
            double max = 101;

            try
            {
                double val = double.Parse(text);

                if (axisNo == 8)
                {
                    if (val >= max_11)
                    {
                        MessageBox.Show(max_11 + " 사용할 수 없습니다.");
                        ((TextBox)sender).Text = "0";
                    }
                }
                else
                {
                    if (val >= max)
                    {
                        MessageBox.Show(max + " 사용할 수 없습니다.");
                        ((TextBox)sender).Text = "0";
                    }
                }
            }
            catch
            {
                // 무시
            }
            */
            double Axiso = Part.AxisNo;
            String text = ((TextBox)sender).Text;
            double max_11 = 33;
            double max = 101;


            try
            {
                if (Axiso == 8)
                {
                    if (double.Parse(text) >= max_11)
                    {
                        MessageBox.Show(max_11.ToString() + " 사용할 수 없습니다.");

                        ((TextBox)sender).Text = "0";


                    }

                }
                else
                {

                    if (double.Parse(text) >= max)
                    {
                        MessageBox.Show(max.ToString() + " 사용할 수 없습니다.");

                        ((TextBox)sender).Text = "0";

                    }


                }
            }
            catch (Exception)
            {


            }
        }

        #endregion

        #region === 저장 버튼에서 호출: 모든 편집 내용을 Part 로 반영 ===

        /// <summary>
        /// 상위 폼에서 "저장" 버튼 누를 때 호출.
        /// 1) 모든 TextBox 의 숫자 여부를 검사
        /// 2) 문제가 없으면 각 TextBox.Text 바인딩에 대해 UpdateSource() 호출
        ///    (XAML 에서 UpdateSourceTrigger=Explicit 일 때만 실제 Part/FAMMCPosition 에 값이 들어감)
        /// 파일에 쓰는 부분(XML/INI 저장)은 이 함수가 true 를 리턴한 뒤,
        /// 상위 폼에서 기존 Config 저장 함수를 그대로 호출하면 됨.
        /// </summary>
        public bool SaveAllWithConfirm()
        {
            var result = MessageBox.Show(
                "모터 설정을 저장하시겠습니까?",
                "저장 확인",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return false;

            // 1. 숫자 유효성 검사
            foreach (var tb in FindVisualChildren<TextBox>(this))
            {
                var be = tb.GetBindingExpression(TextBox.TextProperty);
                if (be == null) continue; // 바인딩 없는 TextBox 는 패스

                var txt = tb.Text;
                if (string.IsNullOrWhiteSpace(txt))
                    continue; // 빈 값은 허용 (필요시 정책 변경)

                double dummy;
                if (!double.TryParse(txt, out dummy))
                {
                    MessageBox.Show(
                        "숫자만 입력 가능한 항목이 있습니다.\r\n해당 값을 다시 확인해 주세요.",
                        "저장 오류",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    tb.Focus();
                    return false;
                }
            }

            // 2. 실제 바인딩 소스(FAMMCPosition, Part 등)에 값 밀어넣기
            foreach (var tb in FindVisualChildren<TextBox>(this))
            {
                var be = tb.GetBindingExpression(TextBox.TextProperty);
                if (be != null)
                    be.UpdateSource();
            }

            // 여기까지 오면 Part / FAMMCPosition 안에는 최신 값이 설정됨.
            // 실제 파일(XML/INI) 저장은 상위 폼에서
            // motorConfigControl.SaveAllWithConfirm() == true 인 경우에만
            // 기존 저장 루틴(예: ConfigManager.SaveMotorConfig(Part)) 을 호출하면 됨.


            // 3. 실제 파일 저장은 상위에서 제공하는 SaveToFileAction 에 위임
            try
            {
                if (SaveToFileAction != null && Part != null)
                {
                    SaveToFileAction(Part);   // ← 여기서 XML/INI 저장 함수가 호출됨
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "모터 설정을 파일에 저장하는 중 오류가 발생했습니다.\r\n" +
                    ex.Message,
                    "저장 오류",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        /*
        /// <summary>
        /// VisualTree 에서 자식 TextBox 들을 재귀적으로 찾아주는 유틸
        /// </summary>
        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T)
                    yield return (T)child;

                foreach (var childOfChild in FindVisualChildren<T>(child))
                    yield return childOfChild;
            }
        }
        */
        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null)
                yield break;

            int count = VisualTreeHelper.GetChildrenCount(depObj);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T tChild)
                    yield return tChild;

                foreach (T childOfChild in FindVisualChildren<T>(child))
                    yield return childOfChild;
            }
        }

        /// <summary>
        /// 화면(TextBox)에 입력되어 있는 값을 한 번에 바인딩 소스(FAMMCPosition/Part)에 반영
        /// - 엔터 입력 시에는 절대 저장/반영하지 않고
        /// - 저장 버튼에서만 이 함수를 호출하도록 한다.
        /// </summary>
        /// <returns>
        /// 전부 정상 적용되면 true, 중간에 오류가 나면 false
        /// </returns>
        public bool ApplyEdits()
        {
            try
            {
                // 이 컨트롤 내부의 모든 TextBox에 대해
                foreach (var tb in FindVisualChildren<TextBox>(this))
                {
                    // Text 바인딩이 걸려 있는 것만 대상
                    var be = tb.GetBindingExpression(TextBox.TextProperty);
                    if (be != null)
                    {
                        // 현재 TextBox에 보이는 값을 바인딩 소스(FAMMCPosition/Part)로 반영
                        be.UpdateSource();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "모터 설정 값을 적용하는 중 오류가 발생했습니다.\r\n" + ex.Message,
                    "MotorConfigControl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }
        }


        #endregion
    }
}
