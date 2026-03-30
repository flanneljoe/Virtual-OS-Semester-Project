using MidOS.src.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidOS.src.interfaces
{
    public interface ICPU
    {
        /// <summary>
        /// Gets the address held in the given register.
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        uint GetRegAddr(uint reg);

        /// <summary>
        /// Sets the value of the given register to the value at the specified address.
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="addr"></param>
        void SetRegAddr(uint reg, uint addr);


        /// <summary>
        /// Gets the value in the specified register.
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        uint GetRegVal(uint reg);

        /// <summary>
        /// Sets the value of the specified register to the given value.
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="val"></param>
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
        /// <returns></returns>
        uint GetSP();

        /// <summary>
        /// Sets the value of the stack pointer.
        /// </summary>
        /// <param name="val"></param>
        void SetSP(uint val);

        /// <summary>
        /// Gets the address of the Global Memory starting address.
        /// </summary>
        /// <returns>Returns a <seealso cref="uint"/> representing the Global Memory starting address.</returns>
        uint GetGlobalMemoryStart();

        void Run();

        void RunProcess(PCB proc);



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
        /// <returns></returns>
        bool GetSign();

        /// <summary>
        /// Gets the zero flag.
        /// </summary>
        /// <returns></returns>
        bool GetZero();
    }
}
