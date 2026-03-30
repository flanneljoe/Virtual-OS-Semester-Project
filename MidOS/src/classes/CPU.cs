using System;
using System.Text.Json;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using MidOS.src.interfaces;
using MidOS.src.models;
using System.Threading.Tasks.Sources;
using System.Runtime.CompilerServices;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Security.AccessControl;

namespace MidOS.src.classes
{  
    internal class CPU : ICPU
    {
        // Instruction flags, true when set, false when clear
        private bool ZERO_FLAG, SIGN_FLAG = false;

        private uint INSN_SIZE = 4;

        private ulong clock = 0;
        public ulong GetClock() => clock;

        private uint[] regs;

        private bool IsHalted = false;
        private PCB? currentProc = null;

        private MemManager mem;
        private List<string> programFiles = [];
        private OSConfig? config;

        public CPU(int virtualMemSize, List<string> programFiles)
        {
            ZERO_FLAG = false;
            SIGN_FLAG = false;

            regs = new uint[15];

            this.programFiles = programFiles;

            try
            {
                string jsonString = File.ReadAllText("osconfig.json");
                config = JsonSerializer.Deserialize<OSConfig>(jsonString) ?? new OSConfig {
                    GlobalDataSize = 0, HeapSize = 0, StackSize = 4
                };
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Error: The file 'osconfig.json' was not found.");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }

            mem = new MemManager(virtualMemSize, config ?? new OSConfig());

            Run();
        }

        public void Run()
        {
            if (config == null) 
                return;

            Scheduler scheduler = new Scheduler();

            // Phase 1: Load all programs into processes and enqueue them
            foreach (string file in programFiles)
            {
                const uint PROG_BASE = 0;

                IProgram p = new DefaultProgram(file, PROG_BASE);

                // Create the PCB early with a placeholder AddressSpace so that
                // ProgramLoader can write directly into proc.WorkingSetPages
                PCB proc = new PCB(p, new AddressSpace(0, 0, 0, 0, 0, 0, 0, 0), config.TimeQuantum);

                // Point the MMU at this process's page table so the
                // loader writes to physical frames that belong to this process
                mem.SetPageTable(proc.WorkingSetPages);

                ProgramLoader loader = new ProgramLoader(p, mem, proc.WorkingSetPages);
                loader.LoadProgram();

                // Compute the full virtual address layout now that code size is known
                uint codeBase  = PROG_BASE;
                uint codeLimit = codeBase + p.GetSize();

                uint dataBase  = codeLimit;
                uint dataLimit = dataBase + config.GlobalDataSize;

                uint heapBase  = dataLimit;
                uint heapLimit = heapBase + config.HeapSize;

                uint stackBase  = heapLimit + config.StackSize;
                uint stackLimit = heapLimit;

                // Replace the placeholder with the real AddressSpace
                proc.SetAddressSpace(new AddressSpace(
                    codeBase,  codeLimit,
                    dataBase,  dataLimit,
                    heapBase,  heapLimit,
                    stackBase, stackLimit
                ));

                // Set initial register state:
                // r11 (IP) = 0  (already 0 from zero-init)
                // r13 (SP) = top of stack
                // r14 = start of global data (fixes previously always-0 bug)
                proc.GetRegisters()[13] = stackBase;
                proc.GetRegisters()[14] = dataBase;

                proc.State = ProcessState.Ready;
                scheduler.Enqueue(proc);
            }

            // Scheduler loop, runs until all processes have terminated
            while (!scheduler.AllTerminated())
            {
                scheduler.WakeExpired(clock);

                PCB? next = scheduler.SelectNext();
                if (next == null)
                {
                    Tick();   // all live processes are sleeping; advance time
                    continue;
                }

                next.State = ProcessState.Running;
                mem.SetPageTable(next.WorkingSetPages);
                mem.SetContext(next.GetAddressSpace());

                uint remaining = next.TimeQuantum;
                while (remaining > 0 && next.State == ProcessState.Running)
                {
                    RunProcess(next);
                    next.ClockCyclesUsed++;
                    remaining--;
                }

                // If the process is still Running after the quantum, context-switch it out
                if (next.State == ProcessState.Running)
                {
                    next.State = ProcessState.Ready;
                    next.ContextSwitchCount++;
                }
            }

            // Print per-process statistics
            Console.WriteLine("\n--- Process Statistics ---");
            foreach (PCB proc in scheduler.GetAll())
            {
                Console.WriteLine(
                    $"Process {proc.ProcessId}: " +
                    $"{proc.ClockCyclesUsed} cycles, {proc.ContextSwitchCount} context switches");
            }
        }

        public uint GetRegAddr(uint reg)
        {
            return mem.GetAddr(regs[reg]);
            //return regs[reg];
        }

        public uint GetRegVal(uint reg)
        {
            return regs[reg];
        }

        public bool GetSign()
        {
            return SIGN_FLAG;
        }
        public bool GetZero()
        {
            return ZERO_FLAG;
        }

        public uint GetSP()
        {
            return regs[13];
        }   
        public void SetSP(uint val)
        {
            regs[13] = val;
        }

        public uint GetIP()
        {
            return regs[11];
        }

        public void SetIP(uint val)
        {
            regs[11] = val;
        }

        private void AdvanceIP()
        {
            SetIP(GetIP() + INSN_SIZE);
        }

        public uint GetGlobalMemoryStart()
        {
            return regs[14];
        }

        public void SetRegAddr(uint reg, uint addr)
        {
            try
            {
                if (reg < 11)
                {
                    regs[reg] = mem.GetAddr(addr);
                }
                else
                {
                    throw new ArgumentException("CPU: Attempted to set the value of a reserved register.", "reg");
                }
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException("CPU: Attempted to read the value stored at an invalid address.", "addr", e);
            }
        }

        public void SetRegVal(uint reg, uint val)
        {
            regs[reg] = val;
        }

        public void RunProcess(PCB proc)
        {
            currentProc = proc;

            // Restore CPU state from process, registers and flags
            regs = proc.GetRegisters();
            ZERO_FLAG = proc.ZeroFlag;
            SIGN_FLAG = proc.SignFlag;
            mem.SetContext(proc.GetAddressSpace());

            // Fetch, Decode, Execute
            (uint opCode, uint p1, uint p2) = mem.GetInsn(GetIP());
            Action<uint, uint> Exec = TryDecode(opCode);
            Exec(p1, p2);

            // Save CPU state back to process, registers and flags
            proc.SetRegisters(regs);
            proc.ZeroFlag = ZERO_FLAG;
            proc.SignFlag  = SIGN_FLAG;

            currentProc = null;
            Tick();
        }
        
        public void Tick()
        {
            clock++;
        }

        public Action<uint, uint> TryDecode(uint opCode)
        {
            string insnName = OpCodes.GetOpName(opCode);
            switch (insnName)
            {
                case "incr":
                    return Exec_Incr;

                case "addi":
                    return Exec_Addi;
                case "addr":
                    return Exec_Addr;

                case "pushr":
                    return Exec_Pushr;
                case "pushi":
                    return Exec_Pushi;

                case "movi":
                    return Exec_Movi;
                case "movr":
                    return Exec_Movr;
                case "movmr":
                    return Exec_Movmr;
                case "movrm":
                    return Exec_Movrm;
                case "movmm":
                    return Exec_Movmm;

                case "printr":
                    return Exec_Printr;
                case "printm":
                    return Exec_Printm;                    
                case "printcr":
                    return Exec_Printcr;
                case "printcm":
                    return Exec_Printcm;

                case "jmp":
                    return Exec_Jmp;
                    
                case "jmpi":
                    return Exec_Jmpi;
                    
                case "jmpa":
                    return Exec_Jmpa;
                    

                case "cmpi":
                    return Exec_Cmpi;
                    
                case "cmpr":
                    return Exec_Cmpr;
                    

                case "jlt":
                    return Exec_Jlt;
                    
                case "jlti":
                    return Exec_Jlti;
                    
                case "jlta":
                    return Exec_Jlta;
                    

                case "jgt":
                    return Exec_Jgt;
                    
                case "jgti":
                    return Exec_Jgti;
                    
                case "jgta":
                    return Exec_Jgta;
                    

                case "je":
                    return Exec_Je;
                    
                case "jei":
                    return Exec_Jei;
                    
                case "jea":
                    return Exec_Jea;
                    

                case "call":
                    return Exec_Call;
                    
                case "callm":
                    return Exec_Callm;
                    

                case "ret":
                    return Exec_Ret;
                    
                case "exit":
                    return Exec_Exit;
                    

                case "popr":
                    return Exec_Popr;
                    
                case "popm":
                    return Exec_Popm;
                    

                case "sleep":
                    return Exec_Sleep;
                    

                case "input":
                    return Exec_Input;
                    
                case "inputc":
                    return Exec_Inputc;
                    

                case "setPriority":
                    return Exec_SetPriority;
                    
                case "setPriorityI":
                    return Exec_SetpriorityI;
                    

                default:
                    return Exec_InvalidInsn;
                    
            }
        }

        #region **** MidAsm Delegates ****

        private void Exec_InvalidInsn(uint p1, uint p2)
        {
            IsHalted = true;
        }

        private void Exec_Incr(uint p1, uint p2)
        {
            uint setVal = GetRegVal(p1) + 1;
            SetRegVal(p1, setVal);
            AdvanceIP();
        }

        private void Exec_Addi(uint p1, uint p2)
        {
            uint setVal = GetRegVal(p1) + p2;
            SetRegVal(p1, setVal);
            AdvanceIP();
        }

        private void Exec_Addr(uint p1, uint p2)
        {
            uint setVal = GetRegVal(p1) + GetRegVal(p2);
            SetRegVal(p1, setVal);
            AdvanceIP();
        }

        // Shared stack helpers used by push/pop instructions and call/ret
        private bool TryPush(uint value)
        {
            uint newSP = GetSP() - 1;
            if (currentProc == null || !currentProc.GetAddressSpace().IsStackAddr(newSP, 1))
                return false;
            SetSP(newSP);
            mem.WriteAddr(newSP, value);
            return true;
        }

        private bool TryPop(out uint value)
        {
            uint sp = GetSP();
            if (currentProc == null || !currentProc.GetAddressSpace().IsStackAddr(sp, 1))
            {
                value = 0;
                return false;
            }
            value = mem.ReadAddr(sp);
            SetSP(sp + 1);
            return true;
        }

        private void Exec_Pushr(uint p1, uint p2)
        {
            if (!TryPush(GetRegVal(p1))) { IsHalted = true; return; }
            AdvanceIP();
        }

        private void Exec_Pushi(uint p1, uint p2)
        {
            if (!TryPush(p1)) { IsHalted = true; return; }
            AdvanceIP();
        }

        private void Exec_Movi(uint p1, uint p2)
        {
            SetRegVal(p1, p2);
            AdvanceIP();
        }

        private void Exec_Movr(uint p1, uint p2)
        {
            SetRegVal(p1, GetRegVal(p2));
            AdvanceIP();
        }

        private void Exec_Movmr(uint p1, uint p2)
        {
            SetRegVal(p1, mem.GetAddr(GetRegVal(p2)));
            AdvanceIP();
        }

        private void Exec_Movrm(uint p1, uint p2)
        {
            mem.WriteAddr(GetRegVal(p1), GetRegVal(p2));
            AdvanceIP();
        }

        private void Exec_Movmm(uint p1, uint p2)
        {
            uint addr1 = GetRegVal(p1);
            uint addr2 = GetRegVal(p2);

            mem.WriteAddr(addr1, mem.ReadAddr(addr2));
            AdvanceIP();
        }

        private void Exec_Printr(uint p1, uint p2)
        {
            Console.Write($"{GetRegVal(p1)}");
            AdvanceIP();
        }

        private void Exec_Printm(uint p1, uint p2)
        {
            uint addr = GetRegAddr(p1);
            Console.Write($"{addr:X8}");
            AdvanceIP();
        }

        private void Exec_Printcr(uint p1, uint p2)
        {
            Console.Write($"{Convert.ToChar(GetRegVal(p1))}");
            AdvanceIP();
        }

        private void Exec_Printcm(uint p1, uint p2)
        {
            uint addr = GetRegAddr(p1);
            Console.Write($"{Convert.ToChar(addr)}");
            AdvanceIP();
        }

        private void Exec_Jmp(uint p1, uint p2)
        {
            int relBytes = (int)GetRegVal(p1);
            if (mem.ValidAddr(GetIP() + (uint)relBytes))
            {
                SetIP(GetIP() + (uint)relBytes);
            }
            else
            {
                IsHalted = true;
            }
        }

        private void Exec_Jmpi(uint p1, uint p2)
        {
            int relBytes = (int)p1;
            if (mem.ValidAddr(GetIP() + (uint)relBytes))
            {
                SetIP(GetIP() + (uint)relBytes);
            }
            else
            {
                IsHalted = true;
            }
        }

        private void Exec_Jmpa(uint p1, uint p2)
        {
            if (mem.ValidAddr(p1))
            {
                SetIP(p1);
            }
            else
            {
                IsHalted = true;
            }
        }

        private void Exec_Cmpi(uint p1, uint p2)
        {
            int x = (int)(GetRegVal(p1));
            int y = (int)(p2);
            if (x == y)
            {
                ZERO_FLAG = true;
            }
            else
            {
                ZERO_FLAG = false;
                if (x < y)
                {
                    SIGN_FLAG = true;
                }
                else
                {
                    SIGN_FLAG = false;
                }
            }
            AdvanceIP();
        }

        private void Exec_Cmpr(uint p1, uint p2)
        {
            int x = Convert.ToInt32(GetRegVal(p1));
            int y = Convert.ToInt32(GetRegVal(p2));

            if (x == y)
            {
                ZERO_FLAG = true;
            }
            else
            {
                ZERO_FLAG = false;
                if (x < y)
                {
                    SIGN_FLAG = true;
                }
                else
                {
                    SIGN_FLAG = false;
                }
            }
            AdvanceIP();
        }

        private void Exec_Jlt(uint p1, uint p2)
        {
            if (SIGN_FLAG && !ZERO_FLAG)
            {
                Exec_Jmp(p1, p2);
            }
            else
            {
                AdvanceIP();
            }
        }

        private void Exec_Jlti(uint p1, uint p2)
        {
            if (SIGN_FLAG && !ZERO_FLAG)
            {

                Exec_Jmpi(p1, p2);
            }
            else
            {
                AdvanceIP();
            }
        }

        private void Exec_Jlta(uint p1, uint p2)
        {
            if (SIGN_FLAG && !ZERO_FLAG)
            {
                Exec_Jmpa(p1, p2);
            }
            else
            {
                AdvanceIP();
            }
        }

        private void Exec_Jgt(uint p1, uint p2)
        {
            if (!SIGN_FLAG && !ZERO_FLAG)
            {
                Exec_Jmp(p1, p2);
            }
            else
            {
                AdvanceIP();
            }
        }

        private void Exec_Jgti(uint p1, uint p2)
        {
            if (!SIGN_FLAG && !ZERO_FLAG)
            {
                Exec_Jmpi(p1, p2);
            }
            else
            {
                AdvanceIP();
            }
        }

        private void Exec_Jgta(uint p1, uint p2)
        {
            if (!SIGN_FLAG && !ZERO_FLAG)
            {
                Exec_Jmpa(p1, p2);
            }
            else
            {
                AdvanceIP();
            }
        }

        private void Exec_Je(uint p1, uint p2)
        {
            if (!ZERO_FLAG)
            {
                Exec_Jmp(p1, p2);
            }
            else
            {
                AdvanceIP();
            }
        }

        private void Exec_Jei(uint p1, uint p2)
        {
            if (!ZERO_FLAG)
            {
                Exec_Jmpi(p1, p2);
            }
            else
            {
                AdvanceIP();
            }
        }

        private void Exec_Jea(uint p1, uint p2)
        {
            if (!ZERO_FLAG)
            {
                Exec_Jmpa(p1, p2);
            }
            else
            {
                AdvanceIP();
            }
        }

        private void Exec_Call(uint p1, uint p2)
        {
            // Capture call site IP before anything changes
            uint callIP = GetIP();

            // Push the return address — the instruction immediately after this call
            if (!TryPush(callIP + INSN_SIZE)) 
            { 
                IsHalted = true;
                return;
            }

            // Jump relative to the call instruction using the offset in register p1
            SetIP(callIP + GetRegVal(p1));
        }

        private void Exec_Callm(uint p1, uint p2)
        {
            // Capture call site IP before anything changes
            uint callIP = GetIP();

            // Push the return address — the instruction immediately after this call
            if (!TryPush(callIP + INSN_SIZE)) { IsHalted = true; return; }

            // Jump relative to the call instruction using the offset stored at the address in register p1
            SetIP(callIP + mem.ReadAddr(GetRegVal(p1)));
        }

        private void Exec_Ret(uint p1, uint p2)
        {
            // Pop the saved return address and resume execution there
            if (!TryPop(out uint returnAddr)) { IsHalted = true; return; }
            SetIP(returnAddr);
        }

        private void Exec_Exit(uint p1, uint p2)
        {
            if (currentProc != null)
                currentProc.State = ProcessState.Terminated;
        }

        private void Exec_Popr(uint p1, uint p2)
        {
            if (!TryPop(out uint value)) { IsHalted = true; return; }
            SetRegVal(p1, value);
            AdvanceIP();
        }

        private void Exec_Popm(uint p1, uint p2)
        {
            if (!TryPop(out uint value)) { IsHalted = true; return; }
            mem.WriteAddr(GetRegVal(p1), value);
            AdvanceIP();
        }

        private void Exec_Sleep(uint p1, uint p2)
        {
            if (currentProc != null)
            {
                currentProc.SleepUntil = (p1 == 0) ? ulong.MaxValue : clock + p1;
                currentProc.State = ProcessState.WaitingAsleep;
            }
            AdvanceIP();
        }

        private void Exec_Input(uint p1, uint p2)
        {
            // A 64-bit value doesn't fit in a single 32-bit register.
            // Convention: high 32 bits go into r[p1], low 32 bits into r[p1+1].
            if (p1 + 1 >= regs.Length)
            {
                Console.WriteLine($"input: register r{p1} has no adjacent register for the low word.");
                IsHalted = true;
                return;
            }

            Console.Write("input: ");
            string? line = Console.ReadLine();

            if (!ulong.TryParse(line, out ulong value))
            {
                Console.WriteLine($"input: '{line}' is not a valid 64-bit integer.");
                IsHalted = true;
                return;
            }

            SetRegVal(p1,     (uint)(value >> 32));        // high word
            SetRegVal(p1 + 1, (uint)(value & 0xFFFFFFFF)); // low word
            AdvanceIP();
            // Yield after I/O so other processes can run while this one re-enters the ready queue
            if (currentProc != null)
                currentProc.State = ProcessState.Ready;
        }

        private void Exec_Inputc(uint p1, uint p2)
        {
            // Read a single character and store its ASCII value in r[p1]
            Console.Write("inputc: ");
            ConsoleKeyInfo key = Console.ReadKey();
            Console.WriteLine();
            SetRegVal(p1, (uint)key.KeyChar);
            AdvanceIP();
            // Yield after I/O so other processes can run while this one re-enters the ready queue
            if (currentProc != null)
                currentProc.State = ProcessState.Ready;
        }

        private void Exec_SetPriority(uint p1, uint p2)
        {
            if (currentProc != null)
                currentProc.Priority = GetRegVal(p1);
            AdvanceIP();
        }

        private void Exec_SetpriorityI(uint p1, uint p2)
        {
            if (currentProc != null)
                currentProc.Priority = p1;
            AdvanceIP();
        }
        #endregion
    }

    internal class ProgramLoader(IProgram program, MemManager m, List<IMemPage> workingSetPages)
    {
        internal IProgram p = program;
        internal MemManager mem = m;
        // The per-process page table this loader populates during loading
        internal List<IMemPage> WorkingSetPages = workingSetPages;

        internal void LoadProgram()
        {
            try
            {
                IEnumerable<string> lines = File.ReadLines(p.GetPath());
                p.SetName(lines.First());

                uint currentAddr = p.GetProgramBase();
                uint pageSize = mem.GetPageSize();
                uint currentLogicalPage = currentAddr / pageSize;

                // Allocate the first physical frame for this process and add it to its page table
                uint physBase = mem.AllocatePhysicalPage();
                WorkingSetPages.Add(new MemPage(physBase) { IsOccupied = true });
                mem.SetPageTable(WorkingSetPages);

                foreach (string line in lines.Skip(1))
                {
                    // get rid of line comments
                    string insn = line.IndexOf(';') > -1 ? line.Substring(0, line.IndexOf(";")) : line;

                    // skip blank lines and comment-only lines
                    if (string.IsNullOrWhiteSpace(insn)) continue;

                    // separate instruction and parameters into tokens
                    string[] tokens = insn.Split([' ', ',']);

                    // trim any whitespace off of instruction
                    tokens[0] = tokens[0].Trim();

                    // Look up the instruction code in the OpCode struct's dictonary
                    if (!OpCodes.IsValidCode(tokens[0]))
                        throw new Exception("Found invalid instruction: " +  tokens[0]);

                    uint opCode = OpCodes.GetOpCode(tokens[0]);

                    // Write value of the OpCode to memory
                    WritePagedByte(currentAddr++, opCode, ref currentLogicalPage, pageSize);

                    // Track how many parameter bytes have been written
                    int paramBytes = 0;

                    if (tokens.Length > 1)
                    {
                        // If there are parameter values, parse them
                        for (int i = 1; i < tokens.Length; i++)
                        {
                            string parm = tokens[i];
                            if (parm.Length > 1)
                            {
                                // switch on the first char of the parm to determine what type of parameter it is
                                switch (parm[0])
                                {
                                    case 'r':
                                        WritePagedByte(currentAddr++, (uint)(int.Parse(parm[1..parm.Length])), ref currentLogicalPage, pageSize);
                                        paramBytes++;
                                        break;
                                    case '#':
                                        WritePagedByte(currentAddr++, (uint)int.Parse(parm[1..parm.Length]), ref currentLogicalPage, pageSize);
                                        paramBytes++;
                                        break;
                                    case '@':
                                        WritePagedByte(currentAddr++, (uint)parm[1], ref currentLogicalPage, pageSize);
                                        paramBytes++;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }

                    // Pad to exactly 3 parameter bytes so every instruction is 4 bytes wide
                    while (paramBytes < 3)
                    {
                        WritePagedByte(currentAddr++, 0, ref currentLogicalPage, pageSize);
                        paramBytes++;
                    }

                    p.SetSize(p.GetSize() + 4);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine($"Error occured while decoding program file: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"An uxpected error occured while decoding program file: {e.Message}");
            }
        }

        // Writes a byte to the given logical address, allocating a new per-process page frame
        // when the address crosses into a new logical page.
        private void WritePagedByte(uint addr, uint value, ref uint currentLogicalPage, uint pageSize)
        {
            uint page = addr / pageSize;

            if (page != currentLogicalPage)
            {
                // Crossed into a new logical page
                // allocate a physical frame and add it to this process's page table
                uint physBase = mem.AllocatePhysicalPage();
                WorkingSetPages.Add(new MemPage(physBase) { IsOccupied = true });
                mem.SetPageTable(WorkingSetPages);
                currentLogicalPage = page;
            }

            mem.WriteAddr(addr, value);
        }
    }

}
