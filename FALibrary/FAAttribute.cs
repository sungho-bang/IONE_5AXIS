using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FALibrary
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public class FAAttribute : System.Attribute
    {        
        public string GroupName { get; set; }
        public bool Visible { get; set; }
        public bool Readonly { get; set; }
        
        public FAAttribute(string groupName)
        {
            GroupName = groupName;
            Visible = true;
            Readonly = false;
        }

        public FAAttribute(string groupName, bool fReadonly, bool fVisible = true)
        {
            GroupName = groupName;
            Visible = fVisible;
            Readonly = fReadonly;
        }
    }

    public class FAIncludeSequenceAttribute : FAAttribute
    {
        public string SequenceManagerName { get; set; }

        public FAIncludeSequenceAttribute(string groupName)
            : base(groupName)
        {
        }

        public FAIncludeSequenceAttribute(string groupName, string sequenceManagerName)
            : base(groupName)
        {
            SequenceManagerName = sequenceManagerName;
        }
    }

    public class FAAttributePushButtonMethod : FAAttribute
    {
        public string UpMethodName { get; set; }
        public FAAttributePushButtonMethod(string groupname, string upMethodName)
            : base(groupname)
        {
            UpMethodName = upMethodName;
        }

        public FAAttributePushButtonMethod(string groupName, bool fReadonly, string upMethodName, bool fVisible = true)
            : base(groupName, fReadonly, fVisible)
        {
            UpMethodName = upMethodName;
        }
    }

    public class FASerializable : System.Attribute
    {
    }
}
