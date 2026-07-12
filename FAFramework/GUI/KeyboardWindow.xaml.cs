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
using System.Windows.Shapes;
using System.ComponentModel;
using System.Globalization;

namespace FAFramework.GUI
{
    /// <summary>
    /// KeyboardWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class KeyboardWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private bool _passwordMode;
        public bool PasswordMode
        {
            get { return _passwordMode; }
            set
            {
                if (_passwordMode == value) return;
                _passwordMode = value;
                NotifyPropertyChanged("PasswordMode");
            }
        }

        private string _inputString = string.Empty;
        public string InputString
        {
            get { return _inputString; }
            set
            {
                if (_inputString == value) return;
                _inputString = value;
                NotifyPropertyChanged("InputString");
            }
        }

        private bool _isShift;
        public bool IsShift
        {
            get { return _isShift; }
            set
            {
                if (_isShift == value) return;
                _isShift = value;
                NotifyPropertyChanged("IsShift");
            }
        }

        public KeyboardWindow()
        {
            InitializeComponent();
        }

        private void RaiseKeydownEvent(string keyText)
        {
            var eventArgs = new TextCompositionEventArgs(Keyboard.PrimaryDevice,
                new TextComposition(InputManager.Current, Keyboard.FocusedElement, keyText));

            eventArgs.RoutedEvent = TextInputEvent;

            InputManager.Current.ProcessInput(eventArgs);
        }

        private void RaiseKeydownEvent(Key key)
        {
            var keyEventArgs = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, key);
            keyEventArgs.RoutedEvent = Keyboard.KeyDownEvent;
            InputManager.Current.ProcessInput(keyEventArgs);
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordMode)
                InputString = passwordBox.Password;

            DialogResult = true;
            Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ExecuteCapsLockClickCommand(object sender, ExecutedRoutedEventArgs e)
        {
            IsShift = Keyboard.IsKeyToggled(Key.CapsLock);
        }

        private void ClickShift(object sender, RoutedEventArgs e)
        {
            IsShift = !IsShift;
        }

        private void ClickCharacterButton(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            RaiseKeydownEvent(button.Content.ToString());
        }

        private void ClickSpaceButton(object sender, RoutedEventArgs e)
        {
            RaiseKeydownEvent(Key.Space);
        }

        private void ClickEnterButton(object sender, RoutedEventArgs e)
        {
            RaiseKeydownEvent(Key.Enter);
        }

        private void ClickBackspace(object sender, RoutedEventArgs e)
        {
            RaiseKeydownEvent(Key.Back);
        }

        private void ClickClearButton(object sender, RoutedEventArgs e)
        {
            InputString = string.Empty;
        }

        private void ClickTabButton(object sender, RoutedEventArgs e)
        {
            RaiseKeydownEvent(Key.Tab);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (PasswordMode)
            {
                passwordBox.Focus();
            }
            else
            {
                textBoxInput.Focus();
                textBoxInput.CaretIndex = textBoxInput.Text.Length;
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }

    public static class KeyboardCommands
    {
        private static RoutedCommand _leftShiftCommand = new RoutedCommand();
        public static RoutedCommand LeftShiftCommand
        {
            get { return _leftShiftCommand; }
            set { _leftShiftCommand = value; }
        }
    }

    public static class CharacterPairs
    {
        public class CharacterPair
        {
            public string Normal { get; set; }
            public string Shift { get; set; }

            public CharacterPair(string normal, string shift)
            {
                Normal = normal;
                Shift = shift;
            }
        }

        public static CharacterPair[] Items = new CharacterPair[] { new CharacterPair("`", "~"),
                                                                    new CharacterPair("1", "!"),
                                                                    new CharacterPair("2", "@"),
                                                                    new CharacterPair("3", "#"),
                                                                    new CharacterPair("4", "$"),
                                                                    new CharacterPair("5", "%"),
                                                                    new CharacterPair("6", "^"),
                                                                    new CharacterPair("7", "&"),
                                                                    new CharacterPair("8", "*"),
                                                                    new CharacterPair("9", "("),
                                                                    new CharacterPair("0", ")"),
                                                                    new CharacterPair("-", "_"),
                                                                    new CharacterPair("=", "+"),
                                                                    new CharacterPair("[", "{"),
                                                                    new CharacterPair("]", "}"),
                                                                    new CharacterPair("\\", "|"),
                                                                    new CharacterPair(";", ":"),
                                                                    new CharacterPair("'", "\""),
                                                                    new CharacterPair(",", "<"),
                                                                    new CharacterPair(".", ">"),
                                                                    new CharacterPair("/", "?") };
    }

    public class CharacterKeyShiftConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length < 2) return values;

            var value = values[0];
            var param = values[1];
            if ((value is string) == false) return value;

            var str = value as string;
            if (string.IsNullOrEmpty(str) == true) return value;
            if (str.Length <= 0) return value;

            if ((param is bool) == false) return value;
            var isShift = (bool)param;

            var chr = str[0];
            if (Char.IsLetter(chr) == true)
            {
                if (isShift == true)
                    return chr.ToString().ToUpper();
                else
                    return chr.ToString().ToLower();
            }
            else
            {
                string firstChar = chr.ToString();
                var characterPair = CharacterPairs.Items.Select(x => x).Where(x => x.Normal == firstChar || x.Shift == firstChar).First();

                if (isShift == true)
                    return characterPair.Shift;
                else
                    return characterPair.Normal;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("No two way conversion, one way binding only.");
        }
    }
}
