using MidOS.src.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidOS.src.classes
{
    public class MemPage : IMemPage
    {
        public uint PhysicalBase { get; set; }
        public bool IsOccupied { get; set; }

        public MemPage(uint physicalBase, bool isOccupied = false)
        {
            PhysicalBase = physicalBase;
            IsOccupied = isOccupied;
        }
    }
}
