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

        private uint pageSize;

        // Active translation table
        // Used by Translate() to resolve logial to physical addresses
        // Swapped per context switch via SetPageTable()
        private List<IMemPage> pageTable;

        // Global physical frame pool that tracks which frames are free/occupied.
        // AllocatePhysicalPage/FreePage operate on this pool, not on pageTable.
        private List<IMemPage> physicalFramePool;

        public uint GetPageSize()
        {
            return pageSize;
        }

        public void SetPageTable(List<IMemPage> table)
        {
            pageTable = table;
        }

        public MemManager(int memSize, OSConfig config)
        {
            memory = new PhysicalMemory(memSize);
            pageSize = config.PageSize;
            physicalFramePool = [];
            pageTable = [];
            InitPageTable(memSize);
        }

        private void InitPageTable(int memSize)
        {
            // Populate the global physical frame pool with an identity mapping.
            // Programs with less than pageSize memory still need a frame, so use ceiling division.
            int totalPages = (memSize + (int)pageSize - 1) / (int)pageSize;
            for (int i = 0; i < totalPages; i++)
            {
                physicalFramePool.Add(new MemPage((uint)(i * pageSize)));
            }
        }

        private uint Translate(uint virtualAddr)
        {
            uint page = virtualAddr / pageSize;
            uint offset = virtualAddr % pageSize;

            // If full on pages, try to get another page 
            if (page >= (uint)pageTable.Count)
            {
                IMemPage? free = physicalFramePool.FirstOrDefault(p => !p.IsOccupied);
                if (free == null)
                    throw new OutOfMemoryException($"MemManager: No free physical frames for virtual address {virtualAddr} (page {page}).");

                free.IsOccupied = true;
                while (pageTable.Count <= (int)page)
                    pageTable.Add(new MemPage(0));
                pageTable[(int)page].PhysicalBase = free.PhysicalBase;
            }

            return pageTable[(int)page].PhysicalBase + offset;
        }

        public void WriteAddr(uint addr, uint val)
        {
            memory.SetByte(Translate(addr), val);
        }

        public uint ReadAddr(uint addr)
        {
            return memory.GetByte(Translate(addr));
        }

        public uint GetAddr(uint addr)
        {
            if (!ValidAddr(addr))
                throw new ArgumentException("MemManager: Attempted to get the value of an invalid memory address.", "addr");
            return ReadAddr(addr);
        }

        public void SetAddr(uint addr, uint val)
        {
            if (!ValidAddr(addr))
            {
                throw new ArgumentException("MemManager: Attempted to set the value of an invalid memory address.", $"addr: {addr.ToString()}");
            }
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

        public void FreeAllPages()
        {
            foreach (IMemPage frame in physicalFramePool)
            {
                frame.IsOccupied = false;
            }
        }

        public void FreePage(uint physicalBase)
        {
            IMemPage? frame = physicalFramePool.FirstOrDefault(p => p.PhysicalBase == physicalBase);
            if (frame != null)
                frame.IsOccupied = false;
        }

        public uint AllocatePhysicalPage()
        {
            IMemPage? free = physicalFramePool.FirstOrDefault(p => !p.IsOccupied);
            if (free == null)
                throw new OutOfMemoryException("MemManager: No free physical page frames available.");

            free.IsOccupied = true;
            return free.PhysicalBase;
        }

        public void MapLogicalPage(uint logicalPage, uint physicalBase)
        {
            // Grow the active translation table if needed
            while (pageTable.Count <= (int)logicalPage)
            {
                pageTable.Add(new MemPage(0));
            }

            pageTable[(int)logicalPage].PhysicalBase = physicalBase;

            // Mark the frame as occupied in the global pool
            IMemPage? frame = physicalFramePool.FirstOrDefault(p => p.PhysicalBase == physicalBase);
            if (frame != null)
                frame.IsOccupied = true;
        }
    }
}
