using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FAFramework.VT3500.JobInfo
{
    public abstract class JobStepBase : FALibrary.FAObject
    {
        public abstract void CopyTo(JobStepBase obj);
        public abstract JobStepBase Clone();
        public abstract string[] ToKeyValueArray(string prefix);
        public abstract void Parsing(System.Xml.Linq.XElement xml);
    }
}
