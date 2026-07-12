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

using FAFramework.Utility;

namespace FAFramework.GUI
{
    /// <summary>
    /// PartActionWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PartActionWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private PartDefineForManualOperation _partDefine;
        public PartDefineForManualOperation PartDefine
        {
            get { return _partDefine; }
            set
            {
                _partDefine = value;
                NotifyPropertyChanged("PartDefine");
            }
        }

        public PartActionWindow()
        {
            InitializeComponent();
        }

        public void CheckedUseRepeatCheckBox(object sender, RoutedEventArgs e)
        {
            CheckBox button = sender as CheckBox;
            var partAction = button.DataContext as AliasPartAction;

            PartDefine.AddRepeatAction(partAction);
        }

        public void UncheckedUseRepeatCheckBox(object sender, RoutedEventArgs e)
        {
            CheckBox button = sender as CheckBox;
            var partAction = button.DataContext as AliasPartAction;

            PartDefine.RemoveRepeatAction(partAction);
        }

        public void ClickAction(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            var partAction = button.DataContext as AliasPartAction;

            partAction.ActionMethod(sender);
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            PartDefine.StopRepeatAction();
            Close();
        }

        private void buttonExecuteRepeatActions_Click(object sender, RoutedEventArgs e)
        {
            if (PartDefine.IsStopedRepeatAction)
                PartDefine.StartRepeatAction();
            else
                PartDefine.StopRepeatAction();
        }
    }
}
