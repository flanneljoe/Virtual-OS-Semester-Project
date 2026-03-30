using MidOS.src.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidOS.src.interfaces
{
    public interface IPCB
    {
        public AddressSpace GetAddressSpace();

        public void SetRegisters(uint[] regs);

        public uint[] GetRegisters();
    }
}
