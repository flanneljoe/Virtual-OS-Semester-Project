using MidOS.src.interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidOS.src.classes
{
    public class AddressSpace(uint codeBase, uint codeLimit, uint globalDataBase, uint globalDataLimit, uint heapBase, uint heapLimit, uint stackBase, uint stackLimit) : IAddressSpace
    {

        public uint CodeBase { get; } = codeBase;
        public uint CodeLimit { get; } = codeLimit;
        public uint HeapBase { get; } = heapBase;
        public uint HeapLimit { get; private set; } = heapLimit;
        public uint StackBase { get; } = stackBase;
        public uint StackLimit { get; private set; } = stackLimit;

        public uint GlobalDataBase { get; } = globalDataBase;
        public uint GlobalDataLimit { get; private set; } = globalDataLimit;

        public bool ContainsAddr(uint addr, uint size)
        {
            return IsCodeAddr(addr, size) ||
                IsDataAddr(addr, size) ||
                IsHeapAddr(addr, size) ||
                IsStackAddr(addr, size);
        }

        public bool IsCodeAddr(uint addr, uint size)
        {
            return InSegment(addr, size, CodeBase, CodeLimit);
        }

        public bool IsDataAddr(uint addr, uint size) 
        { 
            return InSegment(addr, size, GlobalDataBase, GlobalDataLimit);
        }

        public bool IsStackAddr(uint addr, uint size)
        {
            return InSegment(addr, size, StackLimit, StackBase);
        }

        public bool IsHeapAddr(uint addr, uint size)
        {
            return InSegment(addr, size, HeapBase, HeapLimit);
        }

        private static bool InSegment(uint addr, uint size, uint segmentBase, uint segmentLimit)
        {
            if (size == 0)
                return false;

            ulong start = addr;
            ulong end = start + size;

            return start >= segmentBase && end <= segmentLimit;
        }

        public void GrowHeap(uint bytes)
        {
            // may need this later
            throw new NotImplementedException();
        }

        public void GrowStack(uint bytes)
        {
            // probably won't need this since SP tracks top of stack
            // keeping just in case
            throw new NotImplementedException();
        }
    }
}
