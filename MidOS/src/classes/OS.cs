using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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

            if (args.Length == 0)
            {
                Console.WriteLine("Error: Missing Virtual Memory Size");
                usage();
                return;
            }

            virtualMemSize = (int)Math.Ceiling(int.Parse(args[0]) / 8.0);

            Console.WriteLine("Memory Size: " +  virtualMemSize);

            pgmFiles = [.. args[1..args.Length]];
            Console.WriteLine("Program Files Provided: " + pgmFiles.Count);

            CPU c = new CPU(virtualMemSize, pgmFiles);
            
        }

        static void usage()
        {
            Console.WriteLine("OS <size of virtual memory in bytes> <program1.txt> <program2.txt> ...");
        }
    }
}
