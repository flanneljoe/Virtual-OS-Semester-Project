// Name: Joseph Feltz
// zID: z2048486

using MidOS.src.classes;

namespace MidOS.test.drivers
{
    internal static class TestDriver4
    {
        internal static void Run()
        {
            Console.WriteLine("=== Test 4: Character Output ===");
            Console.WriteLine("Expected: Hi");
            Console.WriteLine("--- Output ---");
            new CPU(-1, ["test/MidAsm/test4.txt"]);
            Console.WriteLine();
            Console.WriteLine("---");
        }
    }
}
