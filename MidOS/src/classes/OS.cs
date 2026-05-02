// Name: Joseph Feltz
// zID: z2048486

using MidOS.test.drivers;

namespace MidOS.src.classes
{
    internal class OS
    {
        static void Main(string[] args)
        {
            int virtualMemSize = -1;
            List <string> pgmFiles;

            Console.WriteLine("Started MidOS");

            if (args.Length > 0 && args[0] == "--test")
            {
                if (args.Length > 1 && int.TryParse(args[1], out int testNum))
                    TestRunner.Run(testNum);
                else
                    TestRunner.RunAll();
                return;
            }

            if (args.Length < 1)
            {
                Console.WriteLine("Error: Missing program files to load.");
                usage();
                return;
            }

            int fileStart = 0;
            if (args[0] == "--page")
            {
                if (args.Length > 1)
                {
                    virtualMemSize = (int)Math.Ceiling(int.Parse(args[1]) / 8.0);       
                    Console.WriteLine("Memory Size (ints): " +  virtualMemSize);
                    fileStart = 2;
                }
                else
                {
                    Console.WriteLine("Error: Missing virtual page size.");
                    usage();
                    return;
                }
            }

            if (!(args.Length > fileStart))
            {
                Console.WriteLine("Error: Missing MidOS progam files.");
                usage();
                return;
            }

            pgmFiles = [.. args[fileStart..args.Length]];
            Console.WriteLine("Program Files Provided: " + pgmFiles.Count);

            CPU c = new CPU(virtualMemSize, pgmFiles);
            
        }

        static void usage()
        {
            Console.WriteLine("OS --page <size of virtual memory page in bytes> <program1.txt> <program2.txt> ...");
        }
    }
}
