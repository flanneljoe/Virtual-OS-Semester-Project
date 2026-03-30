using MidOS.src.classes;

namespace MidOS.test.drivers
{
    internal static class TestDriver3
    {
        internal static void Run()
        {
            Console.WriteLine("=== Test 3: Increment and Add Immediate ===");
            Console.WriteLine("Expected: 16");
            Console.WriteLine("--- Output ---");
            new CPU(128, ["test/MidAsm/test3.txt"]);
            Console.WriteLine();
            Console.WriteLine("---");
        }
    }
}
