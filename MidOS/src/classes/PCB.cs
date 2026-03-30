using MidOS.src.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace MidOS.src.classes
{
    public class PCB : IPCB
    {
        private AddressSpace ctx;
        private IProgram pgm;
        private uint[] regs = new uint[15];

        public bool IsFinished { get; set; } = false;
        public ulong SleepUntil { get; set; } = 0;
        public uint Priority { get; set; } = 0;

        public PCB(IProgram p, AddressSpace context)
        {
            ctx = context;
            pgm = p;
        }

        public AddressSpace GetAddressSpace()
        {
            return this.ctx;
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
