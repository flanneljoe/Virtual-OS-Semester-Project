using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidOS.src.classes
{
    public class OSConfig
    {
        public uint GlobalDataSize { get; set; } = 0;
        public uint HeapSize { get; set; } = 0;
        public uint StackSize { get; set; } = 4;
        public uint PageSize { get; set; } = 256;
        public uint TimeQuantum { get; set; } = 10;
        public uint SharedMemoryCount { get; set; } = 2;
    }
}
