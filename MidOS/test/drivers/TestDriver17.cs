// Name: Joseph Feltz
// zID: z2048486

using MidOS.src.classes;

namespace MidOS.test.drivers
{
    // Verifies heap allocation with Alloc and FreeMemory instructions.
    // A single process allocates 4 bytes (rounds up to one page), writes 99 to the
    // returned address and reads it back, then frees and re-allocates to confirm the
    // same address is reused. Finally it requests more bytes than HeapSize to confirm
    // the failure path returns 0.
    // Expected output (one value per line):
    //   <heap base addr>   – address of first allocation (non-zero, = codeSize + 2048)
    //   99                 – value written to and read back from the heap
    //   <heap base addr>   – same address reused after free
    //   0                  – over-allocation failure
    internal static class TestDriver17
    {
        internal static void Run()
        {
            Console.WriteLine("=== Test 17: Heap Allocation ===");
            Console.WriteLine("Expected: heap base addr, 99, heap base addr (reuse), 0 (over-alloc failure)");
            Console.WriteLine("--- Output ---");
            new CPU(-1, ["test/MidAsm/test17.txt"]);
            Console.WriteLine();
            Console.WriteLine("---");
        }
    }
}
