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

namespace FAFramework.GUI
{
    /// <summary>
    /// NumericKeyboardWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NumericKeyboardWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private double _min;
        public double Min
        {
            get { return _min; }
            set
            {
                if (_min == value) return;
                _min = value;
                NotifyPropertyChanged("Min");
            }
        }

        private double _max;
        public double Max
        {
            get { return _max; }
            set
            {
                if (_max == value) return;
                _max = value;
                NotifyPropertyChanged("Max");
            }
        }

        private Type _numType;
        public Type NumType
        {
            get { return _numType; }
            set
            {
                if (_numType == value) return;
                _numType = value;
                NotifyPropertyChanged("NumType");
            }
        }

        private string _inputValueString = "0";
        public string InputValueString
        {
            get { return _inputValueString; }
            set
            {
                if (_inputValueString == value) return;
                _inputValueString = value;
                NotifyPropertyChanged("InputValueString");
            }
        }

        public object Value { get; private set; }

        public NumericKeyboardWindow()
        {
            InitializeComponent();
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            InputValueString = "0";
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            //Value = Convert.ChangeType(InputValueString,NumType);
            Value = Convert.ToDouble( InputValueString);
            this.DialogResult = true;
            this.Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void buttonDot_Click(object sender, RoutedEventArgs e)
        {
            var dotIndex = InputValueString.IndexOf('.');
            if (dotIndex < 0)
                InputValueString += '.';
        }

        private void buttonNumber_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            double value = double.Parse(InputValueString);

            string updatedValue;
            if (value == 0)
                updatedValue = button.Tag.ToString();
            else
                updatedValue = InputValueString + button.Tag.ToString();

            if (double.Parse(updatedValue) <= Max && double.Parse(updatedValue) >= Min)
                InputValueString = updatedValue;
        }

        private void buttonSign_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            double value = double.Parse(InputValueString);

            if (value != 0)
            {
                string updatedValue = InputValueString;

                var signIndex = updatedValue.IndexOf('-');
                if (signIndex >= 0)
                    updatedValue = updatedValue.Replace("-", "");
                else
                    updatedValue = '-' + updatedValue;

                if (double.Parse(updatedValue) <= Max && double.Parse(updatedValue) >= Min)
                    InputValueString = updatedValue;
            }
        }

        private void buttonZero_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            double value = double.Parse(InputValueString);

            string updatedValue = InputValueString;
            if (value != 0)
                updatedValue += button.Tag.ToString();

            if (double.Parse(updatedValue) <= Max && double.Parse(updatedValue) >= Min)
                InputValueString = updatedValue;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}
