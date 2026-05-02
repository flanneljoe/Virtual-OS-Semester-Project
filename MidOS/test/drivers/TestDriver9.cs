// Name: Joseph Feltz
// zID: z2048486

using MidOS.src.classes;

namespace MidOS.test.drivers
{
    internal static class TestDriver9
    {
        internal static void Run()
        {
            Console.WriteLine("=== Test 9: Memory Operations ===");
            Console.WriteLine("Expected: 77");
            Console.WriteLine("--- Output ---");
            new CPU(-1, ["test/MidAsm/test9.txt"]);
            Console.WriteLine();
            Console.WriteLine("---");
        }
    }
}
