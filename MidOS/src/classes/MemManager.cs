using MidOS.src.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidOS.src.classes
{
    internal class MemManager : IMemManager
    {
        private IMemory memory;

        private AddressSpace? ctx;

        public MemManager(int memSize)
        {
            memory = new PhysicalMemory(memSize);
        }

        public void WriteAddr(uint addr, uint val)
        {
            // Sets the address in memory to the given value
            // This function shouldn't be called directly by the user
            memory.SetByte(addr, val);
        }

        public uint ReadAddr(uint addr)
        {
            // Gets te address in memory
            // This function should be called direclty by the user
            return memory.GetByte(addr);
        }

        public uint GetAddr(uint addr)
        {
            // Checks if the address is valid
            // Returns the value if it is, throws an exception if not
            if (!ValidAddr(addr))
                throw new ArgumentException("MemManager: Attempted to get the value of an invalid memory address.", "addr");
            return ReadAddr(addr);
        }

        public void SetAddr(uint addr, uint val)
        {
            // Checks if the address is valid
            // Writes the value in memory if it is, throws an exception if not
            if (!ValidAddr(addr))
            {
                throw new ArgumentException("MemManager: Attempted to set the value of an invalid memory address.", $"addr: {addr.ToString()}");
            }
            WriteAddr(addr, val);
        }

        public (uint insn, uint p1, uint p2) GetInsn(uint addr)
        {
            // Attempts to get all three instruction values
            // Throws an exception on the first invalid instruction
            uint insn, p1, p2;
            try
            {
                insn = GetAddr(addr);
            }
            catch (ArgumentException e) 
            {
                throw new ArgumentException("MemManager: Invalid instruction address.", "addr", e);
            }

            try
            {
                p1 = GetAddr(addr + 1);
                p2 = GetAddr(addr + 2);
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException($"MemManager: Invalid instruction parameter address.", $"addr: {addr.ToString()}", e);
            }

            return (insn, p1, p2);
        }       
        public void SetContext(AddressSpace ctx)
        {
            this.ctx = ctx;
        }

        public bool ValidAddr(uint addr)
        {
            if (ctx != null)
            {
                return ctx.ContainsAddr(addr, 1);
            }
            else
            {
                return 0 <= addr && addr < memory.GetSize();
            }
        }
    }
}
