using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FALibrary.Sequence;

namespace FAFramework.Module
{
    public interface StackerPart
    {
        FASequence Initialize { get; set; }
        bool Lockable(object owner);
        bool Lock(object owner);
        bool Release(object owner);
        void RequestCharging();
        bool IsChargingStandby();
        FASequence MoveToBottomPos { get; set; }
        FASequence Process { get; set; }
    }
}
