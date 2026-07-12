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
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace FAFramework.GUI
{
    /// <summary>
    /// UserSelectWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UserSelectWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public Equipment.UserInfo SelectedUser { get; set; }

        public static readonly DependencyProperty EquipmentInstanceProperty =
            DependencyProperty.Register("EquipmentInstance", typeof(Equipment.EquipmentBase), typeof(UserSelectWindow));

        public Equipment.EquipmentBase EquipmentInstance
        {
            get { return (Equipment.EquipmentBase)GetValue(EquipmentInstanceProperty); }
            set
            {
                SetValue(EquipmentInstanceProperty, value);
            }
        }

        #region Command
        private ICommand _clickLoginCommand;
        public ICommand ClickLoginCommand
        {
            get { return _clickLoginCommand; }
            set
            {
                _clickLoginCommand = value;
                NotifyPropertyChanged("ClickLoginCommand");
            }
        }

        public ICommand _clickCancelCommand;
        public ICommand ClickCancelCommand
        {
            get { return _clickCancelCommand; }
            set
            {
                _clickCancelCommand = value;
                NotifyPropertyChanged("ClickCancelCommand");
            }
        }

        private ICommand _clickRemoveUserCommand;
        public ICommand ClickRemoveUserCommand
        {
            get { return _clickRemoveUserCommand; }
            set
            {
                _clickRemoveUserCommand = value;
                NotifyPropertyChanged("ClickRemoveUserCommand");
            }
        }

        private ICommand _clickRegisterUserCommand;
        public ICommand ClickRegisterUserCommand
        {
            get { return _clickRegisterUserCommand; }
            set
            {
                _clickRegisterUserCommand = value;
                NotifyPropertyChanged("ClickRegisterUserCommand");
            }
        }

        private ICommand _clickChangePasswordCommand;
        public ICommand ClickChangePasswordCommand
        {
            get { return _clickChangePasswordCommand; }
            set
            {
                _clickChangePasswordCommand = value;
                NotifyPropertyChanged("ClickChangePasswordCommand");
            }
        }
        #endregion        

        #region UserPermissionObject
        public UserPermissionObject OperatorInstance { get; set; }
        public UserPermissionObject MaintenanceInstance { get; set; }
        public UserPermissionObject MasterInstance { get; set; }
        #endregion

        public UserSelectWindow()
        {
            OperatorInstance = new UserPermissionObject(Equipment.UserPermissionTypes.OPERATOR, this);
            MaintenanceInstance = new UserPermissionObject(Equipment.UserPermissionTypes.MAINTENANCE, this);
            MasterInstance = new UserPermissionObject(Equipment.UserPermissionTypes.MASTER, this);

            InitializeComponent();

            ClickLoginCommand = new Utility.CommandHandler(ClickLoginCommandHandler, true);
            ClickCancelCommand = new Utility.CommandHandler(ClickOkCancelCommandHandler, true);
            ClickRemoveUserCommand = new Utility.CommandHandler(ClickRemoveUserCommandHandler, true);
            ClickRegisterUserCommand = new Utility.CommandHandler(ClickRegisterUserCommandHandler, true);
            ClickChangePasswordCommand = new Utility.CommandHandler(ClickChangePasswordCommandHandler, true);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var userList = EquipmentInstance.Config.UserList;
            userList.CollectionChanged +=
                delegate (object aSender, NotifyCollectionChangedEventArgs args)
                {
                    OperatorInstance.SetUserList();
                    MaintenanceInstance.SetUserList();
                    MasterInstance.SetUserList();
                };

            OperatorInstance.SetUserList();
            MaintenanceInstance.SetUserList();
            MasterInstance.SetUserList();
        }

        private void ClickLoginCommandHandler(object param)
        {
            try
            {
                if ((param is object[]) == false) return;
                var parameters = param as object[];
                if (parameters.Length != 2) return;

                if (parameters[0] == null)
                {
                    string msg = Utility.UtilityClass.GetStringResource(this, "UserSelectWindow.PleaseSelectUser", "Please select user.");
                    Manager.MessageWindowManager.Instance.Show(EquipmentInstance, "UserSelectet", msg, false);
                    return;
                }

                var user = parameters[0] as Equipment.UserInfo;
                string password = (parameters[1] as PasswordBox).Password;
                if (string.IsNullOrEmpty(password)) password = string.Empty;
                if (string.IsNullOrEmpty(user.Password)) user.Password = string.Empty;

                if (user.Password != password)
                {
                    string msg = Utility.UtilityClass.GetStringResource(this, "UserSelectWindow.ThePasswordDoesNotMatch", "The password does not match");
                    Manager.MessageWindowManager.Instance.Show(EquipmentInstance, "UserSelectet", msg, false);
                    return;
                }

                Manager.LogManager.Instance.WriteTraceLog(EquipmentInstance,
                    string.Format("LOGIN : ID={0}, PERMISSION={1}", user.Name, user.Permission));
                SelectedUser = user;
                DialogResult = true;
                Close();
            }
            catch (Exception e)
            {
                Manager.MessageWindowManager.Instance.Show(EquipmentInstance, "UserSelectet",
                    string.Format("Do not login. {0}", e.ToString()), false);
            }
        }

        private void ClickOkCancelCommandHandler(object param)
        {
            DialogResult = false;
            Close();
        }

        private void ClickRemoveUserCommandHandler(object param)
        {
            try
            {
                if ((param is object[]) == false) return;
                var parameters = param as object[];
                if (parameters.Length != 3) return;

                if (parameters[0] == null) return;

                var currentUser = parameters[0] as Equipment.UserInfo;
                var selectedUser = parameters[1] as Equipment.UserInfo;
                var password = (parameters[2] as PasswordBox).Password;
                if (string.IsNullOrEmpty(password) == true)
                    password = string.Empty;

                if (selectedUser == null) return;
                if (selectedUser.Name == "admin")
                {
                    string msg = Utility.UtilityClass.GetStringResource(this, "UserSelectWindow.CannotRemoveTheAdminUser", "Can not remove the admin user.");
                    Manager.MessageWindowManager.Instance.Show(EquipmentInstance, "UserSelectet", msg, false);
                    return;
                }

                if (selectedUser == currentUser)
                {
                    string msg = Utility.UtilityClass.GetStringResource(this, "UserSelectWindow.CannotRemoveTheCurrentLoginedUser", "Can not remove the current logined user.");
                    Manager.MessageWindowManager.Instance.Show(EquipmentInstance, "UserSelectet", msg, false);
                    return;
                }

                if (string.IsNullOrEmpty(selectedUser.Password) == true)
                    selectedUser.Password = string.Empty;

                if (selectedUser.Password != password)
                {
                    string msg = Utility.UtilityClass.GetStringResource(this, "UserSelectWindow.ThePasswordDoesNotMatch", "The password does not match");
                    Manager.MessageWindowManager.Instance.Show(EquipmentInstance, "UserSelectet", msg, false);
                    return;
                }

                if (currentUser == null || currentUser.Permission < selectedUser.Permission)
                {
                    string msg = Utility.UtilityClass.GetStringResource(this, "UserSelectWindow.YourPermissionsDoNotAllowYouRemoveTheSelectedUser", "Your permissions do not allow you remove the selected user");
                    Manager.MessageWindowManager.Instance.Show(EquipmentInstance, "UserSelectet", msg, false);
                    return;
                }

                var removedUserName = selectedUser.Name;
                var removedUserPermission = selectedUser.Permission;

                if (currentUser == selectedUser)
                {
                    EquipmentInstance.Config.UserList.Remove(selectedUser);
                    EquipmentInstance.CurrentUser = null;
                }
                else
                    EquipmentInstance.Config.UserList.Remove(selectedUser);

                Manager.LogManager.Instance.WriteTraceLog(EquipmentInstance,
                    string.Format("REMOVE USER ID={0}, PERMISSION={1}. BY USER ID={2}, PERMISSION={3}",
                        removedUserName, removedUserPermission, currentUser.Name, currentUser.Permission));
            }
            catch (Exception e)
            {
                Manager.MessageWindowManager.Instance.Show(EquipmentInstance, "UserSelectet",
                    string.Format("Do not remove user. {0}", e.ToString()), false);
            }
        }

        private void ClickRegisterUserCommandHandler(object param)
        {
            try
            {
                if ((param is object[]) == false) return;
                var parameters = param as object[];
                if (parameters.Length != 5) return;

                var currentUser = parameters[0] as Equipment.UserInfo;
                var permission = (Equipment.UserPermissionTypes)parameters[1];
                var userName = parameters[2] as string;
                if (string.IsNullOrEmpty(userName) == true)
                {
                    string msg = Utility.UtilityClass.GetStringResource(this, "UserSelectWindow.PleaseEnterUserName", "Please enter a user name");
                    Manager.MessageWindowManager.Instance.Show(EquipmentInstance, "UserSelectet", msg, false);
                    return;
                }

                if (ExistUser(userName) == true)
                {
                    string msg = Utility.UtilityClass.GetStringResource(this, "UserSelectWindow.UsernameAlreadyExists", "Username already exists.");
                    Manager.MessageWindowManager.Instance.Show(EquipmentInstance, "UserSelectet", msg, false);
                    return;
                }

                var password = (parameters[3] as PasswordBox).Password;
                var confirmNewPassword = (parameters[4] as PasswordBox).Password;

                if (string.IsNullOrEmpty(password) == true)
                    password = string.Empty;

                if (string.IsNullOrEmpty(confirmNewPassword) == true)
                    confirmNewPassword = string.Empty;

                if (password != confirmNewPassword)
                {
                    string msg = Utility.UtilityClass.GetStringResource(this, "UserSelectWindow.NewPasswordDoesNotMatchNewConfirmPassword", "New password does not match new confirm password.");
                    Manager.MessageWindowManager.Instance.Show(EquipmentInstance, "UserSelectet", msg, false);
                    return;
                }

                if (currentUser == null || currentUser.Permission < permission)
                {
                    string msg = Utility.UtilityClass.GetStringResource(this, "UserSelectWindow.YourPermissionsDoNotAllowYouRegisterTheUser", "Your permissions do not allow you register the user");
                    Manager.MessageWindowManager.Instance.Show(EquipmentInstance, "UserSelectet", msg, false);
                    return;
                }

                Equipment.UserInfo user = new Equipment.UserInfo();
                user.Name = userName;
                user.Permission = permission;
                user.Password = password;
                EquipmentInstance.Config.UserList.Add(user);

                Manager.LogManager.Instance.WriteTraceLog(EquipmentInstance,
                    string.Format("REGISTER USER ID={0}, PERMISSION={1}. BY USER ID={2}, PERMISSION={3}",
                        user.Name, user.Permission, currentUser.Name, currentUser.Permission));
            }
            catch (Exception e)
            {
                Manager.MessageWindowManager.Instance.Show(EquipmentInstance, "UserSelectet",
                    string.Format("Do not register user. {0}", e.ToString()), false);
            }
        }

        private void ClickChangePasswordCommandHandler(object param)
        {
            try
            {
                if ((param is object[]) == false) return;
                var parameters = param as object[];
                if (parameters.Length != 5) return;

                var currentUser = parameters[0] as Equipment.UserInfo;
                var selectedUser = parameters[1] as Equipment.UserInfo;

                if (selectedUser == null)
                {
                    string msg = Utility.UtilityClass.GetStringResource(this, "UserSelectWindow.PleaseSelectAUser", "Please select a user.");
                    Manager.MessageWindowManager.Instance.Show(EquipmentInstance, "UserSelectet", msg, false);
                    return;
                }

                if (currentUser.Permission < selectedUser.Permission)
                {
                    string msg = Utility.UtilityClass.GetStringResource(this, "UserSelectWindow.YourPermissionsDoNotAllowYouChangePasswordOfTheUser", "Your permissions do not allow you change password of the user");
                    Manager.MessageWindowManager.Instance.Show(EquipmentInstance, "UserSelectet", msg, false);
                    return;
                }

                var password = (parameters[2] as PasswordBox).Password;
                if (password == null) password = string.Empty;

                if (currentUser.Password != password)
                {
                    string msg = Utility.UtilityClass.GetStringResource(this, "UserSelectWindow.ThePasswordDoesNotMatch", "The password does not match");
                    Manager.MessageWindowManager.Instance.Show(EquipmentInstance, "UserSelectet", msg, false);
                    return;
                }

                var newPassword = (parameters[3] as PasswordBox).Password;
                var confirmNewPassword = (parameters[4] as PasswordBox).Password;

                if (newPassword == null) newPassword = string.Empty;
                if (confirmNewPassword == null) confirmNewPassword = string.Empty;

                if (newPassword != confirmNewPassword)
                {
                    string msg = Utility.UtilityClass.GetStringResource(this, "UserSelectWindow.NewPasswordDoesNotMatchNewConfirmPassword", "New password does not match new confirm password.");
                    Manager.MessageWindowManager.Instance.Show(EquipmentInstance, "UserSelectet", msg, false);
                    return;
                }

                selectedUser.Password = newPassword;

                string successMsg = Utility.UtilityClass.GetStringResource(this, "UserSelectWindow.PasswordChangeSuccessful", "Password change successful.");
                Manager.MessageWindowManager.Instance.Show(EquipmentInstance, "UserSelectet", successMsg, false);
            }
            catch (Exception e)
            {
                Manager.MessageWindowManager.Instance.Show(EquipmentInstance, "UserSelectet",
                    string.Format("Do not change password. {0}", e.ToString()), false);
            }
        }

        private bool ExistUser(string userName)
        {
            if (EquipmentInstance == null) return false;
            if (EquipmentInstance.Config == null) return false;
            if (EquipmentInstance.Config.UserList == null) return false;

            foreach (var user in EquipmentInstance.Config.UserList)
            {
                if (user.Name == userName) return true;
            }

            return false;
        }
    }

    public class UserInfoToArrayConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return values.ToArray();
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class UserPermissionObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public Equipment.UserPermissionTypes PermissionType { get; set; }
        public UserSelectWindow WindowInstance { get; set; }

        private FAFramework.Utility.ThreadSafeObservableCollection<Equipment.UserInfo> _userList = new FAFramework.Utility.ThreadSafeObservableCollection<Equipment.UserInfo>();
        public FAFramework.Utility.ThreadSafeObservableCollection<Equipment.UserInfo> UserList
        {
            get { return _userList; }
            set
            {
                _userList = value;
                NotifyPropertyChanged("UserList");
            }
        }

        public UserPermissionObject(Equipment.UserPermissionTypes permission, UserSelectWindow owner)
        {
            PermissionType = permission;
            WindowInstance = owner;
        }

        public void SetUserList()
        {
            var userList = WindowInstance.EquipmentInstance.Config.UserList;
            UserList = new FAFramework.Utility.ThreadSafeObservableCollection<Equipment.UserInfo>(userList.Where(w => IsMatchType(w, PermissionType)).Select(s => s));
        }

        private bool IsMatchType(Equipment.UserInfo userInfo, params Equipment.UserPermissionTypes[] types)
        {
            if (userInfo == null) return false;
            if (types == null) return false;

            foreach (var permission in types)
            {
                if (userInfo.Permission == permission)
                    return true;
            }

            return false;
        }
    }
}

