// Name: Joseph Feltz
// zID: z2048486

using MidOS.src.classes;

namespace MidOS.test.drivers
{
    internal static class TestDriver6
    {
        internal static void Run()
        {
            Console.WriteLine("=== Test 6: Simple Conditional Jump ===");
            Console.WriteLine("Expected: 99");
            Console.WriteLine("--- Output ---");
            new CPU(-1, ["test/MidAsm/test6.txt"]);
            Console.WriteLine();
            Console.WriteLine("---");
        }
    }
}
