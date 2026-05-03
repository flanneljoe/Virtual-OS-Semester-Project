using MidOS.src.classes;

namespace MidOS.test.drivers
{
    // Process A maps shared region 0, writes 42 into it, then signals event 1.
    // Process B maps shared region 0, waits on event 1, then reads and prints the value.
    // Correct output (42) proves MapSharedMem wires both processes to the same physical frame.
    internal static class TestDriver19
    {
        internal static void Run()
        {
            Console.WriteLine("=== Test 19: Shared Memory Read/Write ===");
            Console.WriteLine("Expected: 42");
            Console.WriteLine("--- Output ---");
            new CPU(-1, ["test/MidAsm/test19a.txt", "test/MidAsm/test19b.txt"]);
            Console.WriteLine();
            Console.WriteLine("---");
        }
    }
}
