using MidOS.src.interfaces;
using MidOS.src.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidOS.src.classes
{
    public class PCB : IPCB
    {
        private static uint nextProcessId = 0;

        private AddressSpace ctx;
        private IProgram pgm;
        private uint[] regs = new uint[15];

        // Id and Scheduling Info
        public uint ProcessId { get; }
        public ProcessState State { get; set; } = ProcessState.New;
        public uint TimeQuantum { get; set; }
        public uint Priority { get; set; } = 1;

        // Statistics
        public ulong ClockCyclesUsed { get; set; } = 0;
        public uint ContextSwitchCount { get; set; } = 0;

        // CPU Flag State
        public bool ZeroFlag { get; set; } = false;
        public bool SignFlag { get; set; } = false;

        // Sleep
        public ulong SleepUntil { get; set; } = 0;

        // Per-Process Page Table
        public List<IMemPage> WorkingSetPages { get; } = [];

        public PCB(IProgram p, AddressSpace context, uint timeQuantum)
        {
            ProcessId = nextProcessId++;
            ctx = context;
            pgm = p;
            TimeQuantum = timeQuantum;
        }

        public AddressSpace GetAddressSpace()
        {
            return this.ctx;
        }

        public void SetAddressSpace(AddressSpace ctx)
        {
            this.ctx = ctx;
        }

        public uint[] GetRegisters()
        {
            return this.regs;
        }

        public void SetRegisters(uint[] regs)
        {
            this.regs = regs;
        }
    }
}
