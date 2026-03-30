using MidOS.src.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidOS.src.interfaces
{
    public interface IProgram
    {
        string GetName();

        void SetName(string name);

        string GetPath();

        uint GetSize();

        uint GetProgramBase();

        void SetSize(uint size);


    }
}
