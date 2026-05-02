// Name: Joseph Feltz
// zID: z2048486

using MidOS.src.interfaces;
using MidOS.src.models;

namespace MidOS.src.classes
{
    internal class Scheduler : IScheduler
    {
        private List<PCB> processes = [];

        public void Enqueue(PCB proc)
        {
            processes.Add(proc);
        }

        public PCB? SelectNext()
        {
            // Pick the highest-priority Ready process.
            // Stable list order provides round-robin behaviour among equal-priority processes.
            return processes
                .Where(p => p.State == ProcessState.Ready)
                .OrderByDescending(p => p.Priority)
                .FirstOrDefault();
        }

        public void WakeExpired(ulong clock)
        {
            foreach (PCB proc in processes)
            {
                if (proc.State == ProcessState.WaitingAsleep && clock >= proc.SleepUntil)
                {
                    proc.State = ProcessState.Ready;
                }
            }
        }

        public bool AllTerminated()
        {
            return processes.All(p => p.IsIdleProcess || p.State == ProcessState.Terminated);
        }

        public IReadOnlyList<PCB> GetAll()
        {
            return processes.AsReadOnly();
        }
    }
}
