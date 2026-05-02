// Name: Joseph Feltz
// zID: z2048486

namespace MidOS.src.interfaces
{
    public interface IMemPage
    {
        /// <summary>
        /// The physical base address of this page frame in physical memory.
        /// </summary>
        uint PhysicalBase { get; set; }

        /// <summary>
        /// Whether this page frame is currently allocated to a process.
        /// </summary>
        bool IsOccupied { get; set; }

        /// <summary>
        /// Whether this page frame is a shared memory region reserved at OS startup.
        /// Shared frames are never handed to individual processes by AllocatePhysicalPage.
        /// </summary>
        bool IsShared { get; set; }

        /// <summary>
        /// Whether this virtual page is currently loaded in physical memory.
        /// False means the page has been swapped out to disk and must be faulted back in.
        /// </summary>
        bool IsValid { get; set; }

        /// <summary>
        /// Whether this page has been written to since it was last loaded into physical memory.
        /// Clean pages (IsDirty = false) that already have a swap copy can be evicted without a disk write.
        /// </summary>
        bool IsDirty { get; set; }

        /// <summary>
        /// The clock tick of the most recent access to this page. Used by the LRU eviction algorithm.
        /// </summary>
        ulong LastUsed { get; set; }
    }
}
