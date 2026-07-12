using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FALibrary
{
    public class FAStatus
    {
        private string _name = "";
        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string DisplayName
        {
            get;
            set;
        }

        public FAStatus(string name)
        {
            _name = name;
            DisplayName = name;
        }

        public void SetName(string name)
        {
            _name = name;
        }

        public override string ToString()
        {
            return Name;
        }        
    }
}
