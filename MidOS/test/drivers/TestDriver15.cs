// Name: Joseph Feltz
// zID: z2048486

using MidOS.src.classes;

namespace MidOS.test.drivers
{
    // Process A loops 15 times then calls SignalEventI #0.
    // Process B calls WaitEventI #0 immediately and blocks, then prints 99 once woken.
    // '99' must appear in the output, and only after A has completed its loop —
    // proving B was blocked until A signaled event 0.
    internal static class TestDriver15
    {
        internal static void Run()
        {
            Console.WriteLine("=== Test 15: Event Signal/Wait ===");
            Console.WriteLine("Expected: 99 (printed by Process B only after Process A signals event 0)");
            Console.WriteLine("--- Output ---");
            new CPU(-1, ["test/MidAsm/test15a.txt", "test/MidAsm/test15b.txt"]);
            Console.WriteLine();
            Console.WriteLine("---");
        }
    }
}
