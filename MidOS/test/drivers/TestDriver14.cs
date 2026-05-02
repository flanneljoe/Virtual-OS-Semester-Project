// Name: Joseph Feltz
// zID: z2048486

using MidOS.src.classes;

namespace MidOS.test.drivers
{
    // Two processes compete for OS lock 0. Each prints its ID (1 or 2) twelve times
    // while holding the lock. With a time quantum of 10, Process A's first quantum
    // expires while it is still in the critical section, forcing Process B to block on
    // AcquireLockI rather than interleave. Correct output is all 1s followed by all 2s
    // (or all 2s then all 1s if B somehow schedules first), never mixed.
    internal static class TestDriver14
    {
        internal static void Run()
        {
            Console.WriteLine("=== Test 14: Lock Mutual Exclusion ===");
            Console.WriteLine("Expected: 12 lines of '1' followed by 12 lines of '2' (no interleaving)");
            Console.WriteLine("--- Output ---");
            new CPU(-1, ["test/MidAsm/test14a.txt", "test/MidAsm/test14b.txt"]);
            Console.WriteLine();
            Console.WriteLine("---");
        }
    }
}
