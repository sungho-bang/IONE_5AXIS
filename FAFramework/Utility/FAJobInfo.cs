using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary;

namespace FAFramework.Utility
{
    [Serializable]
    public abstract class FAJobInfo : FAObject
    {
        public abstract void CopyTo(FAJobInfo obj);
        public abstract FAJobInfo Clone();
    }
}