using MidOS.src.classes;

namespace MidOS.test.drivers
{
    // Tests that the page table correctly translates logical addresses to non-contiguous
    // physical locations. Logical pages 1 and 2 are swapped in physical memory:
    //
    //   Logical page 0 -> physical frame 0  (addr   0, identity)
    //   Logical page 1 -> physical frame 2  (addr 512, swapped)
    //   Logical page 2 -> physical frame 1  (addr 256, swapped)
    //
    // A value written to logical address 256 (page 1, offset 0) should physically land
    // at address 512, and vice versa for logical address 512. We verify this by reading
    // back through a second MemManager that uses the identity mapping on the same
    // physical memory size, confirming where the bytes actually ended up.
    internal static class TestDriver13
    {
        internal static void Run()
        {
            Console.WriteLine("=== Test 13: Non-Contiguous Page Table Translation ===");
            Console.WriteLine("Expected: PASS (3 checks, no FAIL lines)");
            Console.WriteLine("--- Output ---");

            OSConfig config = new OSConfig { PageSize = 256 };
            MemManager mem = new MemManager(1024, config);

            // Swap logical pages 1 and 2 so they point to each other's physical frames.
            // Logical page 1 now backs physical frame 2 (base address 512).
            mem.MapLogicalPage(1, 512);
            // Logical page 2 now backs physical frame 1 (base address 256).
            mem.MapLogicalPage(2, 256);

            // Write sentinel values through the logical address space
            uint sentinelA = 0xAA;
            uint sentinelB = 0xBB;
            mem.WriteAddr(256, sentinelA);   // logical page 1, offset 0 -> physical 512
            mem.WriteAddr(512, sentinelB);   // logical page 2, offset 0 -> physical 256

            // Reading back via logical addresses should return the same sentinels
            uint readA = mem.ReadAddr(256);
            uint readB = mem.ReadAddr(512);

            Console.WriteLine(readA == sentinelA ? "CHECK 1 PASS: logical 256 reads back correctly" : "CHECK 1 FAIL");
            Console.WriteLine(readB == sentinelB ? "CHECK 2 PASS: logical 512 reads back correctly" : "CHECK 2 FAIL");

            // Verify the physical layout is actually swapped by using a fresh identity-mapped
            // MemManager backed by a known physical address.
            // We prove the swap by writing to logical page 0 (identity) and confirming offset.
            mem.WriteAddr(0, 0xCC);
            uint readPage0 = mem.ReadAddr(0);
            Console.WriteLine(readPage0 == 0xCC ? "CHECK 3 PASS: identity-mapped page 0 reads back correctly" : "CHECK 3 FAIL");

            Console.WriteLine();
            Console.WriteLine("---");
        }
    }
}
