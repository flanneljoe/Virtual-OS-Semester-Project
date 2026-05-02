// Name: Joseph Feltz
// zID: z2048486

using MidOS.src.classes;

namespace MidOS.test.drivers
{
    internal static class TestDriver7
    {
        internal static void Run()
        {
            Console.WriteLine("=== Test 7: Simple Loop (Count to 5) ===");
            Console.WriteLine("Expected: 12345");
            Console.WriteLine("--- Output ---");
            new CPU(-1, ["test/MidAsm/test7.txt"]);
            Console.WriteLine();
            Console.WriteLine("---");
        }
    }
}
