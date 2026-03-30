using MidOS.src.classes;
using MidOS.src.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidOS.src.interfaces
{
    public interface IPCB
    {
        /// <summary>Returns the unique process identifier.</summary>
        uint ProcessId { get; }

        /// <summary>Gets or sets the current lifecycle state of the process.</summary>
        ProcessState State { get; set; }

        /// <summary>Gets or sets the maximum number of instructions this process may run before a context switch.</summary>
        uint TimeQuantum { get; set; }

        /// <summary>Gets or sets the process scheduling priority (1–32, higher = more urgent).</summary>
        uint Priority { get; set; }

        /// <summary>Gets or sets the total number of clock cycles this process has executed.</summary>
        ulong ClockCyclesUsed { get; set; }

        /// <summary>Gets or sets how many times this process has been context-switched out.</summary>
        uint ContextSwitchCount { get; set; }

        /// <summary>Gets or sets the saved value of the CPU zero flag.</summary>
        bool ZeroFlag { get; set; }

        /// <summary>Gets or sets the saved value of the CPU sign flag.</summary>
        bool SignFlag { get; set; }

        /// <summary>Gets or sets the clock cycle at which a sleeping process should wake up.</summary>
        ulong SleepUntil { get; set; }

        /// <summary>The set of page table entries (logical→physical mappings) owned by this process.</summary>
        List<IMemPage> WorkingSetPages { get; }

        /// <summary>Returns the process's virtual address space descriptor.</summary>
        AddressSpace GetAddressSpace();

        /// <summary>Replaces the process's virtual address space descriptor.</summary>
        void SetAddressSpace(AddressSpace ctx);

        /// <summary>Overwrites the saved register file with the provided array.</summary>
        void SetRegisters(uint[] regs);

        /// <summary>Returns a reference to the saved register file.</summary>
        uint[] GetRegisters();
    }
}
