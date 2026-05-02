// Name: Joseph Feltz
// zID: z2048486

using MidOS.src.classes;

namespace MidOS.test.drivers
{
    // A single process prints 1, sleeps for 5 cycles, then prints 2.
    // During the 5-cycle sleep window the idle process runs 4 times (sleep #N gives N-1 idle cycles).
    // The statistics line "Idle: 4 cycles" confirms the idle process was actually swapped to.
    internal static class TestDriver18
    {
        internal static void Run()
        {
            Console.WriteLine("=== Test 18: Idle Process Swap ===");
            Console.WriteLine("Expected: 12  |  Idle: 4 cycles in statistics (idle ran during sleep)");
            Console.WriteLine("--- Output ---");
            new CPU(-1, ["test/MidAsm/test18.txt"]);
            Console.WriteLine();
            Console.WriteLine("---");
        }
    }
}
