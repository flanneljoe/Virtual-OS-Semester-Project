using MidOS.src.classes;

namespace MidOS.test.drivers
{
    internal static class TestDriver8
    {
        internal static void Run()
        {
            Console.WriteLine("=== Test 8: Subroutine Call ===");
            Console.WriteLine("Expected: 84");
            Console.WriteLine("--- Output ---");
            new CPU(4096, ["test/MidAsm/test8.txt"]);
            Console.WriteLine();
            Console.WriteLine("---");
        }
    }
}
