using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidOS.src.interfaces
{
    public interface IMemory
    {
        int GetSize();

        uint GetByte(uint addr);

        void SetByte(uint addr, uint value);
        
    }
}
