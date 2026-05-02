// Name: Joseph Feltz
// zID: z2048486

namespace MidOS.src.models
{
    public class OpCodes
    {
        internal static Dictionary<string, uint> opCodes = new Dictionary<string, uint>
        {
            { "incr", 1 },

            { "addi", 2 },
            { "addr", 3 },

            { "pushr", 4 },
            { "pushi", 5 },

            { "movi", 6 },
            { "movr", 7 },
            { "movmr", 8 },
            { "movrm", 9 },
            { "movmm", 10 },

            { "printr", 11 },
            { "printm", 12 },
            { "printcr", 13 },

            { "jmp", 14 },
            { "jmpi", 15 },
            { "jmpa", 16 },

            { "cmpi", 17 },
            { "cmpr", 18 },

            { "jlt", 19 },
            { "jlti", 20 },
            { "jlta", 21 },

            { "jgt", 22 },
            { "jgti", 23 },
            { "jgta", 24 },

            { "je", 25 },
            { "jei", 26 },
            { "jea", 27 },

            { "call", 28 },
            { "callm", 29 },

            { "ret", 30 },
            { "exit", 31 },

            { "popr", 32 },
            { "popm", 33 },

            { "sleep", 34 },

            { "input", 35 },
            { "inputc", 42 },

            { "setPriority", 43 },
            { "setPriorityI", 44 },

            { "MapSharedMem", 45 },

            { "AcquireLock", 46 },
            { "AcquireLockI", 47 },
            { "ReleaseLock", 48},
            { "ReleaseLockI", 49 },

            { "SignalEvent", 50 },
            { "WaitEvent", 51 },
            { "SignalEventI", 52 },
            { "WaitEventI", 53 },

            { "Alloc",      54 },
            { "FreeMemory", 55 },
        };

        public static bool IsValidCode(string insn)
        {
            return opCodes.ContainsKey(insn);
        }

        public static uint GetOpCode(string insn)
        {
            return opCodes[insn];   
        }

        public static string GetOpName(uint opCode)
        {
            return opCodes.FirstOrDefault(pair => pair.Value == opCode).Key;
        }
    }
}
