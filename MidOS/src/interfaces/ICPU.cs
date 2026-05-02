// Name: Joseph Feltz
// zID: z2048486

using MidOS.src.classes;

namespace MidOS.src.interfaces
{
    public interface ICPU
    {
        /// <summary>
        /// Gets the address held in the given register.
        /// </summary>
        /// <param name="reg">The register containing the address to get.</param>
        /// <returns></returns>
        uint GetRegAddr(uint reg);

        /// <summary>
        /// Sets the value of the given register to the value at the specified address.
        /// </summary>
        /// <param name="reg">The register value to set.</param>
        /// <param name="addr">The address containing the value to set to.</param>
        void SetRegAddr(uint reg, uint addr);


        /// <summary>
        /// Gets the value in the specified register.
        /// </summary>
        /// <param name="reg">The register value to get.</param>
        /// <returns></returns>
        uint GetRegVal(uint reg);

        /// <summary>
        /// Sets the value of the specified register to the given value.
        /// </summary>
        /// <param name="reg">The register to set.</param>
        /// <param name="val">The value to set the reigster to.</param>
        void SetRegVal(uint reg, uint val);

        /// <summary>
        /// Gets the value of the instruction pointer.
        /// </summary>
        /// <returns>Returns a <seealso cref="uint"/> representing the value of the instruction pointer.</returns>
        uint GetIP();

        /// <summary>
        /// Sets the value of the instruction pointer.
        /// </summary>
        /// <param name="val">Sets the instruction pointer to the given value.</param>
        void SetIP(uint val);

        /// <summary>
        /// Gets the value of the stack pointer.
        /// </summary>
        /// <returns>Value of the Stack Pointer</returns>
        uint GetSP();

        /// <summary>
        /// Sets the value of the stack pointer.
        /// </summary>
        /// <param name="val">Value to set the stack pointer to.</param>
        void SetSP(uint val);

        /// <summary>
        /// Gets the address of the Global Memory starting address.
        /// </summary>
        /// <returns>Returns a <seealso cref="uint"/> representing the Global Memory starting address.</returns>
        uint GetGlobalMemoryStart();

        /// <summary>
        /// Starts the CPU after setup has completed.
        /// Only intended to be called once at the end of the CPU constructor.
        /// </summary>
        void Run();

        /// <summary>
        /// Runs the given process.
        /// </summary>
        /// <param name="proc">PCB for the process to be run.</param>
        void RunProcess(PCB proc);


        /// <summary>
        /// Attempts to decode a uint representing a MidOS OpCode
        /// </summary>
        /// <param name="opCode">The OpCode to decode.</param>
        /// <returns>Returns an Action<uint, uint> that is to be invoked by the provided OpCode.</returns>
        Action<uint, uint> TryDecode(uint opCode);

        /// <summary>
        /// Ticks the CPU.
        /// </summary>
        void Tick();

        /// <summary>
        /// Gets the current value of the system clock (number of cycles elapsed).
        /// </summary>
        /// <returns>Returns a <seealso cref="ulong"/> representing the cycle count.</returns>
        ulong GetClock();

        /// <summary>
        /// Gets the sign flag.
        /// </summary>
        /// <returns>Returns the current value of the sign flag.</returns>
        bool GetSign();

        /// <summary>
        /// Gets the zero flag.
        /// </summary>
        /// <returns>Returns the current value of the zero flag.</returns>
        bool GetZero();
    }
}
