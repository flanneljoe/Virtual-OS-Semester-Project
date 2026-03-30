using MidOS.src.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidOS.src.interfaces
{
    internal interface IScheduler
    {
        /// <summary>
        /// Adds a process to the scheduler's process list.
        /// </summary>
        /// <param name="proc">The PCB of the process to enqueue.</param>
        void Enqueue(PCB proc);

        /// <summary>
        /// Selects the next process to run. Returns the highest-priority Ready process,
        /// or null if no process is currently Ready.
        /// </summary>
        /// <returns>The next PCB to execute, or null.</returns>
        PCB? SelectNext();

        /// <summary>
        /// Checks all sleeping processes and transitions any whose SleepUntil has passed
        /// back to the Ready state.
        /// </summary>
        /// <param name="clock">The current CPU clock cycle.</param>
        void WakeExpired(ulong clock);

        /// <summary>
        /// Returns true when every process in the scheduler has reached the Terminated state.
        /// </summary>
        bool AllTerminated();

        /// <summary>
        /// Returns a read-only view of all processes (for statistics reporting).
        /// </summary>
        IReadOnlyList<PCB> GetAll();
    }
}
