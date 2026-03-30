using MidOS.src.classes;

namespace MidOS.test.drivers
{
    internal static class TestDriver5
    {
        internal static void Run()
        {
            Console.WriteLine("=== Test 5: Stack Operations ===");
            Console.WriteLine("Expected: 100");
            Console.WriteLine("--- Output ---");
            new CPU(4096, ["test/MidAsm/test5.txt"]);
            Console.WriteLine();
            Console.WriteLine("---");
        }
    }
}
