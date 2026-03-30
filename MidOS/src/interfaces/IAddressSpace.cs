using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidOS.src.interfaces
{
    public interface IAddressSpace
    {
        public bool ContainsAddr(uint addr, uint size);

        public void GrowHeap(uint bytes);

        public void GrowStack(uint bytes);
    }
}
