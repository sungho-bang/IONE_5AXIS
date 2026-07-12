using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FALibrary.Sequence
{
    public class FARetryInfo : FAObject
    {
        private string _name;
        private uint _retryLimit = 3;
        private uint _retryCount = 0;
        private string _description = "";

        [FAAttribute("")]
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value) return;

                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        [FAAttribute("")]
        public uint RetryLimit
        {
            get { return _retryLimit; }
            set
            {
                if (_retryLimit == value) return;

                _retryLimit = value;
                NotifyPropertyChanged("RetryLimit");
            }
        }

        [FAAttribute("")]
        public uint RetryCount
        {
            get { return _retryCount; }
            set
            {
                if (_retryCount == value) return;

                _retryCount = value;
                NotifyPropertyChanged("RetryCount");
            }
        }

        [FAAttribute("")]
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description == value) return;

                _description = value;
                NotifyPropertyChanged("Description");
            }
        }

        public FARetryInfo()
        {
            RetryLimit = 3;
            RetryCount = 0;
        }

        public FARetryInfo(uint retryLimit)
        {
            RetryLimit = retryLimit;
            RetryCount = 0;
        }

        public void ClearCount()
        {
            RetryCount = 0;
        }

        public bool IncreaseCount()
        {
            if (RetryCount + 1 >= RetryLimit)
            {
                RetryCount = 0;
                return false;
            }
            else
            {
                RetryCount++;
                return true;
            }
        }
    }
}
