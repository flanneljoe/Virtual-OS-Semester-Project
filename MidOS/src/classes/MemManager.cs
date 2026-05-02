// Name: Joseph Feltz
// zID: z2048486

using MidOS.src.interfaces;

namespace MidOS.src.classes
{
    internal class MemManager : IMemManager
    {
        private readonly PhysicalMemory memory;

        private AddressSpace? ctx;

        private readonly uint pageSize;

        // Active translation table for the currently running process.
        // Swapped per context switch via SetPageTable().
        private List<IMemPage> pageTable;

        // Global physical frame pool — tracks which frames are free/occupied.
        // AllocatePhysicalPage/FreePage operate on this pool.
        private readonly List<IMemPage> physicalFramePool;

        // All registered process page tables keyed by process ID.
        // EvictLRU scans these to find the least recently used valid page.
        private readonly Dictionary<uint, List<IMemPage>> registeredProcesses = [];

        // Simulated disk swap area: maps (processId, virtualPageNumber) to saved page contents.
        private readonly Dictionary<(uint pid, int vpn), uint[]> SwapVirtualPage = [];

        private ulong tick = 0;
        private uint currentPid = 0;

        public ulong PageFaultCount { get; private set; } = 0;

        public uint GetPageSize() => pageSize;

        public void SetPageTable(List<IMemPage> table)
        {
            pageTable = table;
        }

        public MemManager(int virtualPageSize, OSConfig config)
        {
            int physicalMemSize = (int)config.PhysicalMemorySize;
            memory = new PhysicalMemory(physicalMemSize);
            pageSize = virtualPageSize > 0 ? (uint)virtualPageSize : config.PageSize / 8;
            physicalFramePool = [];
            pageTable = [];
            InitPageTable(physicalMemSize, config.SharedMemoryCount);
        }

        private void InitPageTable(int physicalMemSize, uint sharedCount)
        {
            int totalPages = (physicalMemSize + (int)pageSize - 1) / (int)pageSize;

            uint effectiveSharedCount = (uint)Math.Min((int)sharedCount, Math.Max(0, totalPages - 1));

            for (int i = 0; i < effectiveSharedCount; i++)
                physicalFramePool.Add(new MemPage((uint)(i * pageSize)) { IsShared = true, IsOccupied = true });

            for (int i = (int)effectiveSharedCount; i < totalPages; i++)
                physicalFramePool.Add(new MemPage((uint)(i * pageSize)));
        }

        private uint Translate(uint virtualAddr)
        {
            uint page = virtualAddr / pageSize;
            uint offset = virtualAddr % pageSize;

            bool needsAllocation = page >= (uint)pageTable.Count || !pageTable[(int)page].IsOccupied;

            if (needsAllocation)
            {
                uint physBase = AllocatePhysicalPage();
                while (pageTable.Count <= (int)page)
                    pageTable.Add(new MemPage(0));
                pageTable[(int)page].PhysicalBase = physBase;
                pageTable[(int)page].IsOccupied = true;
                pageTable[(int)page].IsValid = true;
                pageTable[(int)page].IsDirty = false;
                pageTable[(int)page].LastUsed = tick;
            }
            else if (!pageTable[(int)page].IsValid)
            {
                // Page fault: page is known but has been swapped out
                PageFaultCount++;

                uint physBase = AllocatePhysicalPage();
                pageTable[(int)page].PhysicalBase = physBase;
                pageTable[(int)page].IsValid = true;
                pageTable[(int)page].IsDirty = false;
                pageTable[(int)page].LastUsed = tick;

                // Load the saved page contents from swap if they exist
                if (SwapVirtualPage.TryGetValue((currentPid, (int)page), out uint[]? swapData))
                {
                    for (uint i = 0; i < pageSize; i++)
                        memory.SetByte(physBase + i, swapData[i]);
                }
            }
            else
            {
                pageTable[(int)page].LastUsed = tick;
            }

            return pageTable[(int)page].PhysicalBase + offset;
        }

        // Finds and evicts the least recently used valid, non-shared page across all processes.
        // Prefers evicting from processes other than the currently running one to reduce thrashing.
        private void EvictLRU()
        {
            IMemPage? victim = null;
            uint victimPid = 0;
            int victimVpn = -1;

            // First pass: prefer pages from non-running processes
            foreach (var (pid, table) in registeredProcesses)
            {
                if (pid == currentPid) continue;
                for (int vpn = 0; vpn < table.Count; vpn++)
                {
                    IMemPage page = table[vpn];
                    if (!page.IsValid || !page.IsOccupied || page.IsShared) continue;
                    if (victim == null || page.LastUsed < victim.LastUsed)
                    {
                        victim = page;
                        victimPid = pid;
                        victimVpn = vpn;
                    }
                }
            }

            // Fall back to current process if no other candidate exists
            if (victim == null && registeredProcesses.TryGetValue(currentPid, out var currentTable))
            {
                for (int vpn = 0; vpn < currentTable.Count; vpn++)
                {
                    IMemPage page = currentTable[vpn];
                    if (!page.IsValid || !page.IsOccupied || page.IsShared) continue;
                    if (victim == null || page.LastUsed < victim.LastUsed)
                    {
                        victim = page;
                        victimPid = currentPid;
                        victimVpn = vpn;
                    }
                }
            }

            if (victim == null)
                throw new OutOfMemoryException("MemManager: No evictable pages found during LRU eviction.");

            // Save to swap only if dirty, or if this page has never been swapped before.
            // Clean pages with an existing swap copy are already up to date — skip the write.
            bool hasSwapCopy = SwapVirtualPage.ContainsKey((victimPid, victimVpn));
            if (victim.IsDirty || !hasSwapCopy)
            {
                uint[] pageData = new uint[pageSize];
                for (uint i = 0; i < pageSize; i++)
                    pageData[i] = memory.GetByte(victim.PhysicalBase + i);
                SwapVirtualPage[(victimPid, victimVpn)] = pageData;
            }

            // Release the physical frame and invalidate the virtual page entry
            FreePage(victim.PhysicalBase);
            victim.IsValid = false;
            victim.IsDirty = false;
        }

        public void WriteAddr(uint addr, uint val)
        {
            uint phys = Translate(addr);
            memory.SetByte(phys, val);

            // Mark the page dirty so eviction knows to save it to swap
            uint page = addr / pageSize;
            if (page < (uint)pageTable.Count && pageTable[(int)page].IsOccupied)
                pageTable[(int)page].IsDirty = true;
        }

        public uint ReadAddr(uint addr)
        {
            return memory.GetByte(Translate(addr));
        }

        public uint GetAddr(uint addr)
        {
            if (!ValidAddr(addr))
                throw new ArgumentException("MemManager: Attempted to get the value of an invalid memory address.", nameof(addr));
            return ReadAddr(addr);
        }

        public void SetAddr(uint addr, uint val)
        {
            if (!ValidAddr(addr))
                throw new ArgumentException("MemManager: Attempted to set the value of an invalid memory address.", nameof(addr));
            WriteAddr(addr, val);
        }

        public (uint insn, uint p1, uint p2) GetInsn(uint addr)
        {
            uint insn, p1, p2;
            try
            {
                insn = GetAddr(addr);
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException("MemManager: Invalid instruction address.", nameof(addr), e);
            }

            try
            {
                p1 = GetAddr(addr + 1);
                p2 = GetAddr(addr + 2);
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException("MemManager: Invalid instruction parameter address.", nameof(addr), e);
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
                return ctx.ContainsAddr(addr, 1);
            else
                return addr < memory.GetSize();
        }

        public void FreeAllPages()
        {
            foreach (IMemPage frame in physicalFramePool)
                frame.IsOccupied = false;
        }

        public void FreePage(uint physicalBase)
        {
            IMemPage? frame = physicalFramePool.FirstOrDefault(p => p.PhysicalBase == physicalBase);
            if (frame != null)
                frame.IsOccupied = false;
        }

        public uint AllocatePhysicalPage()
        {
            IMemPage? free = physicalFramePool.FirstOrDefault(p => !p.IsOccupied && !p.IsShared);
            if (free == null)
            {
                EvictLRU();
                free = physicalFramePool.FirstOrDefault(p => !p.IsOccupied && !p.IsShared)
                    ?? throw new OutOfMemoryException("MemManager: No free physical page frames available even after eviction.");
            }
            free.IsOccupied = true;
            return free.PhysicalBase;
        }

        public uint GetSharedFrameBase(uint regionId)
        {
            var shared = physicalFramePool.Where(p => p.IsShared).ToList();
            if (regionId >= shared.Count)
                throw new ArgumentOutOfRangeException(nameof(regionId),
                    $"MemManager: Shared region {regionId} does not exist (only {shared.Count} shared regions).");
            return shared[(int)regionId].PhysicalBase;
        }

        public void MapLogicalPage(uint logicalPage, uint physicalBase)
        {
            while (pageTable.Count <= (int)logicalPage)
                pageTable.Add(new MemPage(0));

            pageTable[(int)logicalPage].PhysicalBase = physicalBase;
            pageTable[(int)logicalPage].IsOccupied = true;
            pageTable[(int)logicalPage].IsValid = true;

            IMemPage? frame = physicalFramePool.FirstOrDefault(p => p.PhysicalBase == physicalBase);
            if (frame != null)
                frame.IsOccupied = true;
        }

        public void RegisterProcess(uint pid, List<IMemPage> workingSet)
        {
            registeredProcesses[pid] = workingSet;
        }

        public void UnregisterProcess(uint pid)
        {
            if (!registeredProcesses.TryGetValue(pid, out var table))
                return;

            // Free all physical frames this process holds and clear its swap entries
            for (int vpn = 0; vpn < table.Count; vpn++)
            {
                IMemPage page = table[vpn];
                if (page.IsOccupied && page.IsValid && !page.IsShared)
                    FreePage(page.PhysicalBase);
                SwapVirtualPage.Remove((pid, vpn));
            }

            registeredProcesses.Remove(pid);
        }

        public void SetCurrentProcess(uint pid)
        {
            currentPid = pid;
        }

        public void Tick()
        {
            tick++;
        }
    }
}
