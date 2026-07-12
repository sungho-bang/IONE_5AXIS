using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace FALibrary
{
    public class InterlockManager : FAObject
    {
        private static volatile InterlockManager _instance = null;
        private static object syncRoot = new Object();

        public List<Func<bool>> SystemInterlocks { get; private set; }

        private InterlockManager()
        {
            SystemInterlocks = new List<Func<bool>>();
        }

        public static InterlockManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                            _instance = new InterlockManager();
                    }
                }

                return _instance;
            }
        }

        public bool IsSystemInterlock()
        {
            if (IgnoreInterlock == true)
                return false;

            foreach (var item in SystemInterlocks)
            {
                if (item() == true)
                    return true;
            }

            return false;
        }

        private bool _ignoreInterlock;
        [FAAttribute("")]
        public bool IgnoreInterlock
        {
            get { return _ignoreInterlock; }
            set
            {
                if (_ignoreInterlock == value) return;

                _ignoreInterlock = value;
                NotifyPropertyChanged("IgnoreInterlock");
            }
        }
    }
}
