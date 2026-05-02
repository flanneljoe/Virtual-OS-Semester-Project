// Name: Joseph Feltz
// zID: z2048486

using MidOS.src.classes;

namespace MidOS.test.drivers
{
    // Two processes each write a sentinel to their global data page 0, then fill their
    // remaining data pages so the combined working set (~18 pages) exceeds the physical
    // frame limit (14 usable frames). The OS must evict pages via LRU and swap them to
    // its simulated disk. When each process reads back its sentinel, correct output proves
    // the full evict -> swap-write -> page-fault -> swap-read pipeline works.
    // Expected: 42 and 99 each printed once; order may vary with scheduling.
    internal static class TestDriver16
    {
        internal static void Run()
        {
            Console.WriteLine("=== Test 16: Virtual Memory Page Swap ===");
            Console.WriteLine("Expected: 42 and 99 each printed once (order may vary)");
            Console.WriteLine("--- Output ---");
            new CPU(-1, ["test/MidAsm/test16a.txt", "test/MidAsm/test16b.txt"]);
            Console.WriteLine();
            Console.WriteLine("---");
        }
    }
}
