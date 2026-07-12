using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Media.Animation;

namespace FAFramework.VT3500.GUI
{
    /// <summary>
    /// EquipmentWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EquipmentWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private Equipment.StandardEquipment _equipmentInstance;
        public Equipment.StandardEquipment EquipmentInstance
        {
            get { return _equipmentInstance; }
            set
            {
                _equipmentInstance = value;
                NotifyPropertyChanged("EquipmentInstance");
            }
        }

        public bool Test { get; set; }

        public FAFramework.GUI.DebugWindow DebugWindow { get; set; }
        public Utility.CommandHandler CloseMaintenanceModeClick { get; set; }

        public EquipmentWindow()
        {
            CloseMaintenanceModeClick = new Utility.CommandHandler(
                delegate (object sender)
                {
                    var win = new FAFramework.GUI.UserSelectWindow();
                    var equip = Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500;
                    win.EquipmentInstance = equip;
                    win.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);

                    if ((bool)win.ShowDialog() == true)
                    {
                        if (win.SelectedUser.Permission == Equipment.UserPermissionTypes.MAINTENANCE ||
                            win.SelectedUser.Permission == Equipment.UserPermissionTypes.MASTER)
                        {
                            equip.MaintenanceMode =
                                !equip.MaintenanceMode;
                        }
                        else
                        {
                            MessageBox.Show("Permission of Selected id not allow this work.");
                        }
                    }
                }, true);



            InitializeComponent();

            MouseDoubleClick += TestWindow_MouseMove;

            FAFramework.Utility.UtilityClass.BlockAltF4(this);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            // 해당 버튼으로 종료할 수 없습니다.
            e.Cancel = true;
        }

        private void SetSubWindowKeyBinding(Window window, Key key, ModifierKeys modifierKey)
        {
            EventHandler showWindow = new EventHandler(
                delegate
                {
                    try
                    {
                        if (DebugWindow != null)
                        {
                            DebugWindow.Show();
                            DebugWindow.Activate();
                        }
                        else if (DebugWindow == null || DebugWindow.IsLoaded == false)
                        {
                            DebugWindow = new FAFramework.GUI.DebugWindow();
                            DebugWindow.Closed +=
                            delegate
                            {
                                try
                                {
                                    DebugWindow = null;
                                }
                                catch
                                {
                                }
                            };

                            DebugWindow.Initialize(Equipment.MainEquipment.Instance.EquipmentManagerInstance);
                            DebugWindow.Width = 1024;
                            DebugWindow.Height = 768;
                            DebugWindow.Show();
                            if (DebugWindow.IsActive == false)
                                DebugWindow.Activate();
                        }
                    }
                    catch
                    {
                    }
                });

            RoutedCommandForSubWindow command = new RoutedCommandForSubWindow();
            command.OnExecute += showWindow;
            KeyBinding keyBinding = new KeyBinding(command, key, modifierKey);
            this.InputBindings.Add(keyBinding);
        }


        class RoutedCommandForSubWindow : ICommand
        {
            public event EventHandler OnExecute = delegate { };

            public event EventHandler CanExecuteChanged = null;

            public RoutedCommandForSubWindow()
            {
            }

            public bool CanExecute(Object parameter)
            {
                if (CanExecuteChanged != null)
                    CanExecuteChanged(this, EventArgs.Empty);

                return true;
            }

            public void Execute(Object parameter)
            {
                OnExecute(this, EventArgs.Empty);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetSubWindowKeyBinding(DebugWindow, Key.F1, ModifierKeys.Control);
        }

        private void ClickLoadButton(object sender, RoutedEventArgs e)
        {
            try
            {
                var msg = Utility.UtilityClass.GetStringResource(this, "AreYouSureYouWantToLoadData", "Are you sure you want to load data");

                var questionWindow = new FAFramework.GUI.QuestionMessageBoxWindow();
                questionWindow.Message = msg;
                questionWindow.EquipmentInstance = EquipmentInstance;
                questionWindow.ShowDialog();

                if (questionWindow.Result == FAFramework.GUI.QuestionMessageBoxWindow.QuestionResult.Yes)
                {
                    Equipment.MainEquipment.Instance.LoadBackup();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("File Load Fail\n" + ex.Message);
            }
        }

        private void ClickSaveButton(object sender, RoutedEventArgs e)
        {
            try
            {
                var msg = Utility.UtilityClass.GetStringResource(this, "AreYouSureYouWantToSaveData", "Are you sure you want to save data");

                var questionWindow = new FAFramework.GUI.QuestionMessageBoxWindow();
                questionWindow.Message = msg;
                questionWindow.EquipmentInstance = EquipmentInstance;
                questionWindow.ShowDialog();

                if (questionWindow.Result == FAFramework.GUI.QuestionMessageBoxWindow.QuestionResult.Yes)
                {
                    Equipment.MainEquipment.Instance.Save();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("File Save Fail\n" + ex.Message);
            }
        }

        #region Mouse Effect
        private Random random = new Random(DateTime.Now.Millisecond);
        private TimeSpan lastAddedTimeSpan;

        private void TestWindow_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePoint = e.GetPosition(this);
            AddAnimationEllipse(mousePoint.X, mousePoint.Y);
        }

        private void ellipseStoryboard_Completed(object sender, EventArgs e)
        {
            Storyboard storyboard = (sender as ClockGroup).Timeline as Storyboard;

            Ellipse ellipse = FindName(Storyboard.GetTargetName(storyboard)) as Ellipse;

            this.canvas.UnregisterName(ellipse.Name);

            this.canvas.Children.Remove(ellipse);
        }

        private void AddAnimationEllipse(double x, double y)
        {
            if ((DateTime.Now.TimeOfDay - this.lastAddedTimeSpan).Milliseconds < 50)
            {
                return;
            }

            this.lastAddedTimeSpan = DateTime.Now.TimeOfDay;

            Ellipse ellipse = new Ellipse();

            ellipse.Stroke = GetRandomSolidColorBrush();

            int size = (Byte)this.random.Next(5, 15);

            ellipse.Width = ellipse.Height = size;

            ellipse.Name = "_" + ellipse.GetHashCode().ToString();

            Canvas.SetLeft(ellipse, x - size / 2);
            Canvas.SetTop(ellipse, y - size / 2);

            this.canvas.RegisterName(ellipse.Name, ellipse);

            this.canvas.Children.Add(ellipse);

            Storyboard ellipseStoryboard = (Resources["EllipseStoryBoardKey"] as Storyboard).Clone();

            Storyboard.SetTargetName(ellipseStoryboard, ellipse.Name);

            ellipseStoryboard.Completed += new EventHandler(ellipseStoryboard_Completed);

            ellipse.BeginStoryboard(ellipseStoryboard);
        }

        private SolidColorBrush GetRandomSolidColorBrush()
        {
            Color color = Color.FromArgb
            (
                (Byte)this.random.Next(80, 255),
                (Byte)this.random.Next(0, 255),
                (Byte)this.random.Next(0, 255),
                (Byte)this.random.Next(0, 255)
            );

            return new SolidColorBrush(color);
        }
        #endregion
    }
}
