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

        /// <summary>
        /// Extends the shared segment by one page and returns the virtual base address of the new page.
        /// </summary>
        public uint MapSharedPage(uint pageSize);
    }
}
