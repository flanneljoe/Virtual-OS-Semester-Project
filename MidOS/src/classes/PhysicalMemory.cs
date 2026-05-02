// Name: Joseph Feltz
// zID: z2048486

using MidOS.src.interfaces;

namespace MidOS.src.classes
{
    internal class PhysicalMemory : IMemory
    {
        private uint[] mem;

        public PhysicalMemory(int memorySize)
        {
            mem = new uint[memorySize];
            // Fill with 'F' for debug printouts
            Array.Fill(mem, (uint)0x46);
        }
        public int GetSize()
        {
            return (int)mem.Length;
        }

        public uint GetByte(uint addr)
        {
            return mem[addr];
        }        

        public void SetByte(uint addr, uint value)
        {
            mem[addr] = value;
        }
    }
}
