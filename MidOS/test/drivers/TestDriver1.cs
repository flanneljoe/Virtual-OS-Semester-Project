using MidOS.src.classes;

namespace MidOS.test.drivers
{
    internal static class TestDriver1
    {
        internal static void Run()
        {
            Console.WriteLine("=== Test 1: Move and Print ===");
            Console.WriteLine("Expected: 42");
            Console.WriteLine("--- Output ---");
            new CPU(4096, ["test/MidAsm/test1.txt"]);
            Console.WriteLine();
            Console.WriteLine("---");
        }
    }
}
