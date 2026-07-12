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
using System.Reflection;
using FAFramework.Utility;
using FALibrary;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace FAFramework.GUI
{
    /// <summary>
    /// DebugWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DebugWindow : Window, INotifyPropertyChanged
    {
        private static readonly string PASSWORD = "vestek5539";

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool AllowClose { get; set; }

        private object _items;
        public object Items
        {
            get { return _items; }
            set
            {
                _items = value;
                NotifyPropertyChanged("Items");
            }
        }

        private ICommand _clickLockCommand;
        public ICommand ClickLockCommand
        {
            get { return _clickLockCommand; }
            set
            {
                _clickLockCommand = value;
                NotifyPropertyChanged("ClickLockCommand");
            }
        }

        private ICommand _clickUnlockCommand;
        public ICommand ClickUnlockCommand
        {
            get { return _clickUnlockCommand; }
            set
            {
                _clickUnlockCommand = value;
                NotifyPropertyChanged("ClickUnlockCommand");
            }
        }

        public DebugWindow()
        {
            ClickLockCommand = new Utility.CommandHandler(ClickLockCommandHandler, true);
            ClickUnlockCommand = new Utility.CommandHandler(ClickUnlockCommandHandler, true);

            InitializeComponent();

#if DEBUG
            tabControl.Visibility = System.Windows.Visibility.Visible;
#endif
        }

        public void Initialize(FAFramework.Equipment.EquipmentManager em)
        {
            Items = Equipment.MainEquipment.Instance.DebugList;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ClickAction tag = ((Button)sender).Tag as ClickAction;
            if (tag == null) return;

            if (ClickAction.Items.ContainsKey(tag) == false) return;

            if (ClickAction.Items[tag].ClickMethod != null)
                ClickAction.Items[tag].ClickMethod(sender, e);
        }

        private void Button_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UpDownAction tag = ((Button)sender).Tag as UpDownAction;
            if (tag == null) return;

            if (UpDownAction.Items.ContainsKey(tag) == false) return;

            if (UpDownAction.Items[tag].MouseUpMethod != null)
                UpDownAction.Items[tag].MouseUpMethod(sender, e);
        }

        private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UpDownAction tag = ((Button)sender).Tag as UpDownAction;
            if (tag == null) return;

            if (UpDownAction.Items.ContainsKey(tag) == false) return;

            if (UpDownAction.Items[tag].MouseDownMethod != null)
                UpDownAction.Items[tag].MouseDownMethod(sender, e);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
#if DEBUG
                ClickLockCommandHandler(new object[] { password, tabControl });
#endif
            }
            catch
            {
            }

            if (AllowClose == false)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void buttonSaveAll_Click(object sender, RoutedEventArgs e)
        {
            Equipment.MainEquipment.Instance.Save();
        }

        private void ClickLockCommandHandler(object param)
        {
            try
            {
                if ((param is object[]) == false) return;
                var parameters = param as object[];
                if (parameters.Length != 2) return;

                var passwordBox = parameters[0] as PasswordBox;
                var control = parameters[1] as Control;
                if (control == null) return;

                passwordBox.Password = string.Empty;
                control.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (Exception e)
            {
                Manager.LogManager.Instance.WriteSystemLog(e.ToString());
            }
        }

        private void ClickUnlockCommandHandler(object param)
        {
            try
            {
                if ((param is object[]) == false) return;
                var parameters = param as object[];
                if (parameters.Length != 2) return;

                var passwordBox = parameters[0] as PasswordBox;
                var control = parameters[1] as Control;
                if (control == null) return;

                if (passwordBox.Password != PASSWORD)
                {
                    passwordBox.Password = string.Empty;
                    MessageBox.Show("Not correct password");
                    return;
                }

                passwordBox.Password = string.Empty;
                control.Visibility = System.Windows.Visibility.Visible;
            }
            catch (Exception e)
            {
                Manager.LogManager.Instance.WriteSystemLog(e.ToString());
            }
        }
    }
}
