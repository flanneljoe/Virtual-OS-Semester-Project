using MidOS.src.classes;

namespace MidOS.test.drivers
{
    internal static class TestDriver2
    {
        internal static void Run()
        {
            Console.WriteLine("=== Test 2: Simple Arithmetic ===");
            Console.WriteLine("Expected: 30");
            Console.WriteLine("--- Output ---");
            new CPU(128, ["test/MidAsm/test2.txt"]);
            Console.WriteLine();
            Console.WriteLine("---");
        }
    }
}
