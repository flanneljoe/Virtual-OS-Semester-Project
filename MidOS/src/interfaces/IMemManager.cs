using MidOS.src.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidOS.src.interfaces
{
    internal interface IMemManager
    {
        /// <summary>
        /// Gets the value of the specified address if it's a valid and accessable address.
        /// </summary>
        /// <param name="addr">The address to get the value of.</param>
        /// <returns>Returns an <seealso cref="uint"/> representing the value at the specified address.</returns>
        uint GetAddr(uint addr);

        /// <summary>
        /// Sets the value of the specified address to the provided value if it's a valid and accessable address.
        /// </summary>
        /// <param name="addr">The address to set the value of.</param>
        /// <param name="val">The value to be set.</param>
        void SetAddr(uint addr, uint val);

        /// <summary>
        /// Gets the value of the specified address from memory. Not to be called directly by user programs.
        /// </summary>
        /// <param name="addr">The address to get the value of.</param>
        /// <returns>Returns an <seealso cref="uint"/> representing the value at the specified address.</returns>
        uint ReadAddr(uint addr);

        /// <summary>
        /// Sets the value of the specified address to the provided value if it's a valid and accessable address. Not to be called directly by user programs.
        /// </summary>
        /// <param name="addr">Address to set the value of.</param>
        /// <param name="val">Value to be set.</param>
        void WriteAddr(uint addr, uint val);

        /// <summary>
        /// Gets an instruction stored at the specified address.
        /// </summary>
        /// <param name="addr">The address containing the first byte of the instruction.</param>
        /// <returns>Returns a three valued tuple containing <seealso cref="uint"/> representing the byte containing the instruction, the byte containing the value of the first paremeter, and the byte containing the value of the second parameter.</returns>
        (uint insn, uint p1, uint p2) GetInsn(uint addr);

        /// <summary>
        /// Checks if the given address is valid in the context of the curent <seealso cref="AddressSpace"/>.
        /// </summary>
        /// <param name="addr">The address to be checked.</param>
        /// <returns>Returns a <seealso cref="bool"/> representing if the given address is valid in the current <seealso cref="AddressSpace"/>. True if valid, false otherwise.</returns>
        bool ValidAddr(uint addr);

        /// <summary>
        /// Sets the current address space to the given <seealso cref="AddressSpace"/>.
        /// </summary>
        /// <param name="ctx"><seealso cref="AddressSpace"/> to set the current address space to.</param>
        void SetContext(AddressSpace ctx);

        /// <summary>
        /// Allocates the next free physical page frame and marks it as occupied.
        /// If no free frames exist, evicts the least recently used page to make room.
        /// </summary>
        /// <returns>The physical base address of the allocated page frame.</returns>
        /// <exception cref="OutOfMemoryException">Thrown when no page can be evicted.</exception>
        uint AllocatePhysicalPage();

        /// <summary>
        /// Maps a logical page number to a specific physical base address in the page table.
        /// </summary>
        /// <param name="logicalPage">The logical page number to map.</param>
        /// <param name="physicalBase">The physical base address to map it to.</param>
        void MapLogicalPage(uint logicalPage, uint physicalBase);

        /// <summary>
        /// Returns the size of each page in bytes as configured in OSConfig.
        /// </summary>
        /// <returns>The page size in bytes.</returns>
        uint GetPageSize();

        /// <summary>
        /// Replaces the active page translation table with the provided per-process table.
        /// Call this on every context switch so the MMU uses the incoming process's mappings.
        /// </summary>
        /// <param name="table">The process's page table (WorkingSetPages from its PCB).</param>
        void SetPageTable(List<IMemPage> table);

        /// <summary>
        /// Marks all page frames as unoccupied, resetting the allocator for the next process.
        /// </summary>
        void FreeAllPages();

        /// <summary>
        /// Marks the page frame at the given physical base address as unoccupied.
        /// </summary>
        /// <param name="physicalBase">The physical base address of the page frame to free.</param>
        void FreePage(uint physicalBase);

        /// <summary>
        /// Returns the physical base address of the shared frame with the given region ID.
        /// </summary>
        /// <param name="regionId">Zero-based index into the shared frame pool.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when regionId is out of range.</exception>
        uint GetSharedFrameBase(uint regionId);

        /// <summary>
        /// Registers a process's page table with the memory manager for LRU eviction scanning.
        /// Must be called before the process executes any instructions.
        /// </summary>
        /// <param name="pid">The process ID.</param>
        /// <param name="workingSet">The process's per-process page table.</param>
        void RegisterProcess(uint pid, List<IMemPage> workingSet);

        /// <summary>
        /// Removes a process from the eviction candidate pool and frees all of its physical frames.
        /// Call this when a process terminates.
        /// </summary>
        /// <param name="pid">The process ID to unregister.</param>
        void UnregisterProcess(uint pid);

        /// <summary>
        /// Sets the currently executing process so LRU eviction prefers not to evict its pages.
        /// </summary>
        /// <param name="pid">The process ID of the running process.</param>
        void SetCurrentProcess(uint pid);

        /// <summary>
        /// Advances the memory manager's internal clock, used to timestamp LRU page accesses.
        /// Should be called once per CPU clock tick.
        /// </summary>
        void Tick();

        /// <summary>
        /// Total number of page faults that have occurred across all processes.
        /// </summary>
        ulong PageFaultCount { get; }
    }
}
