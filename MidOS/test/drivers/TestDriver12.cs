// Name: Joseph Feltz
// zID: z2048486

using MidOS.src.classes;

namespace MidOS.test.drivers
{
    internal static class TestDriver12
    {
        internal static void Run()
        {
            Console.WriteLine("=== Test 12: Memory-to-Memory Copy ===");
            Console.WriteLine("Expected: 55");
            Console.WriteLine("--- Output ---");
            new CPU(-1, ["test/MidAsm/test12.txt"]);
            Console.WriteLine();
            Console.WriteLine("---");
        }
    }
}
