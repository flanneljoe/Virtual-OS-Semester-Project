using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidOS.src.models
{
    public enum ProcessState
    {
        New,            // just created, not yet queued
        Ready,          // in the ready queue, waiting for CPU time
        Running,        // currently executing on the CPU
        WaitingAsleep,  // sleeping after a sleep instruction
        Terminated      // finished executing, waiting to be cleaned up
        // WaitingOnLock and WaitingOnEvent are reserved for a future module
    }
}
