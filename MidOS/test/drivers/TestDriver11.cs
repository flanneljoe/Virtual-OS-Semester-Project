// Name: Joseph Feltz
// zID: z2048486

using MidOS.src.classes;

namespace MidOS.test.drivers
{
    internal static class TestDriver11
    {
        internal static void Run()
        {
            Console.WriteLine("=== Test 11: Factorial ===");
            Console.WriteLine("Expected: 120");
            Console.WriteLine("--- Output ---");
            new CPU(-1, ["test/MidAsm/test11.txt"]);
            Console.WriteLine();
            Console.WriteLine("---");
        }
    }
}
