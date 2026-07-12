using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using FALibrary;

namespace FAFramework.ConfigClasses
{
    [Serializable]
    public class EquipmentConfig : FAObject
    {
        private string _systemID;
        [FAAttribute("")]
        public string SystemID
        {
            get { return _systemID; }
            set
            {
                if (_systemID == value) return;

                _systemID = value;
                NotifyPropertyChanged("SystemID");
            }
        }

        private int _jamDelay;
        [FAAttribute("")]
        public int JamDelay
        {
            get { return _jamDelay; }
            set
            {
                if (_jamDelay == value) return;

                _jamDelay = value;
                NotifyPropertyChanged("JamDelay");
            }
        }

        private string _currentUser;
        [FAAttribute("")]
        [Utility.ExceptExtractProperty]
        public string CurrentUser
        {
            get { return _currentUser; }
            set
            {
                if (_currentUser == value) return;

                _currentUser = value;
                NotifyPropertyChanged("CurrentUser");
            }
        }

        private FAFramework.Utility.ThreadSafeObservableCollection<Equipment.UserInfo> _userList = new FAFramework.Utility.ThreadSafeObservableCollection<Equipment.UserInfo>();
        [FAAttribute("")]
        [Utility.ExceptExtractProperty]
        public FAFramework.Utility.ThreadSafeObservableCollection<Equipment.UserInfo> UserList
        {
            get { return _userList; }
            set
            {
                _userList = value;
                NotifyPropertyChanged("UserList");
            }
        }
    }
}
