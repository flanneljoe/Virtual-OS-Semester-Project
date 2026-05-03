// Name: Joseph Feltz
// zID: z2048486

namespace MidOS.test.drivers
{
    internal static class TestRunner
    {
        internal static void RunAll()
        {
            TestDriver1.Run();
            Console.WriteLine();
            TestDriver2.Run();
            Console.WriteLine();
            TestDriver3.Run();
            Console.WriteLine();
            TestDriver4.Run();
            Console.WriteLine();
            TestDriver5.Run();
            Console.WriteLine();
            TestDriver6.Run();
            Console.WriteLine();
            TestDriver7.Run();
            Console.WriteLine();
            TestDriver8.Run();
            Console.WriteLine();
            TestDriver9.Run();
            Console.WriteLine();
            TestDriver10.Run();
            Console.WriteLine();
            TestDriver11.Run();
            Console.WriteLine();
            TestDriver12.Run();
            Console.WriteLine();
            TestDriver13.Run();
            Console.WriteLine();
            TestDriver14.Run();
            Console.WriteLine();
            TestDriver15.Run();
            Console.WriteLine();
            TestDriver16.Run();
            Console.WriteLine();
            TestDriver17.Run();
            Console.WriteLine();
            TestDriver18.Run();
            Console.WriteLine();
            TestDriver19.Run();
        }

        internal static void Run(int n)
        {
            switch (n)
            {
                case 1:  TestDriver1.Run(); break;
                case 2:  TestDriver2.Run(); break;
                case 3:  TestDriver3.Run(); break;
                case 4:  TestDriver4.Run(); break;
                case 5:  TestDriver5.Run(); break;
                case 6:  TestDriver6.Run(); break;
                case 7:  TestDriver7.Run(); break;
                case 8:  TestDriver8.Run(); break;
                case 9:  TestDriver9.Run(); break;
                case 10: TestDriver10.Run(); break;
                case 11: TestDriver11.Run(); break;
                case 12: TestDriver12.Run(); break;
                case 13: TestDriver13.Run(); break;
                case 14: TestDriver14.Run(); break;
                case 15: TestDriver15.Run(); break;
                case 16: TestDriver16.Run(); break;
                case 17: TestDriver17.Run(); break;
                case 18: TestDriver18.Run(); break;
                case 19: TestDriver19.Run(); break;
                default:
                    Console.WriteLine($"Error: No test {n}.");
                    break;
            }
        }
    }
}
