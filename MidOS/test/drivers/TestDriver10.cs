using MidOS.src.classes;

namespace MidOS.test.drivers
{
    internal static class TestDriver10
    {
        internal static void Run()
        {
            Console.WriteLine("=== Test 10: Register-to-Register Move ===");
            Console.WriteLine("Expected: 123");
            Console.WriteLine("--- Output ---");
            new CPU(128, ["test/MidAsm/test10.txt"]);
            Console.WriteLine();
            Console.WriteLine("---");
        }
    }
}
