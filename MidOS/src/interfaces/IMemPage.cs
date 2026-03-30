using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidOS.src.interfaces
{
    internal interface IMemPage
    {
        /// <summary>
        /// The physical base address of this page frame in physical memory.
        /// </summary>
        uint PhysicalBase { get; set; }

        /// <summary>
        /// Whether this page frame is currently allocated to a process.
        /// </summary>
        bool IsOccupied { get; set; }
    }
}
