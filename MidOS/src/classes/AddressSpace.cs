// Name: Joseph Feltz
// zID: z2048486

using MidOS.src.interfaces;

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

        // Shared segment starts at StackBase and grows upward as pages are mapped via mapSharedMem.
        public uint SharedBase { get; private set; } = stackBase;
        public uint SharedLimit { get; private set; } = stackBase;

        public bool ContainsAddr(uint addr, uint size)
        {
            return IsCodeAddr(addr, size) ||
                IsDataAddr(addr, size) ||
                IsHeapAddr(addr, size) ||
                IsStackAddr(addr, size) ||
                IsSharedAddr(addr, size);
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

        public bool IsSharedAddr(uint addr, uint size)
        {
            return InSegment(addr, size, SharedBase, SharedLimit);
        }

        // Maps one page into the shared segment and returns its virtual base address.
        public uint MapSharedPage(uint pageSize)
        {
            uint virtualBase = SharedLimit;
            SharedLimit += pageSize;
            return virtualBase;
        }

        private static bool InSegment(uint addr, uint size, uint segmentBase, uint segmentLimit)
        {
            if (size == 0)
                return false;

            ulong start = addr;
            ulong end = start + size;

            return start >= segmentBase && end <= segmentLimit;
        }
    }
}
