// Name: Joseph Feltz
// zID: z2048486

namespace MidOS.src.classes
{
    public class HeapAllocator
    {
        private readonly uint heapBase;
        private readonly uint pageSize;
        private readonly bool[] pageUsed;
        private readonly Dictionary<uint, uint> allocations = new();

        public HeapAllocator(uint heapBase, uint heapLimit, uint pageSize)
        {
            this.heapBase = heapBase;
            this.pageSize = pageSize;
            uint totalPages = (heapLimit - heapBase) / pageSize;
            pageUsed = new bool[totalPages];
        }

        // Returns the base virtual address of the allocated block, or 0 on failure.
        public uint Alloc(uint bytes)
        {
            if (bytes == 0 || pageUsed.Length == 0)
                return 0;

            uint pagesNeeded = (bytes + pageSize - 1) / pageSize;

            // First-fit: find the first run of pagesNeeded consecutive free pages.
            uint runStart = 0;
            uint runLen = 0;
            for (uint i = 0; i < (uint)pageUsed.Length; i++)
            {
                if (!pageUsed[i])
                {
                    if (runLen == 0)
                        runStart = i;
                    runLen++;
                    if (runLen == pagesNeeded)
                    {
                        for (uint j = runStart; j < runStart + pagesNeeded; j++)
                            pageUsed[j] = true;
                        uint addr = heapBase + runStart * pageSize;
                        allocations[addr] = pagesNeeded;
                        return addr;
                    }
                }
                else
                {
                    runLen = 0;
                }
            }

            return 0; // no contiguous block found
        }

        // Frees the allocation that starts at addr. No-op if addr is not a known allocation base.
        public void Free(uint addr)
        {
            if (!allocations.TryGetValue(addr, out uint pageCount))
                return;

            uint startPage = (addr - heapBase) / pageSize;
            for (uint i = startPage; i < startPage + pageCount; i++)
                pageUsed[i] = false;

            allocations.Remove(addr);
        }
    }
}
