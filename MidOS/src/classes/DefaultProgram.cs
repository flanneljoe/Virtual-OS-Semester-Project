// Name: Joseph Feltz
// zID: z2048486

using MidOS.src.interfaces;

namespace MidOS.src.classes
{
    internal class DefaultProgram : IProgram
    {
        private string Name = "";
        private string Path;
        private uint Size;
        private uint ProgramBase;

        public DefaultProgram(string path, uint addr)
        {
            this.Path = path;
            this.Size = 0;
            this.ProgramBase = addr;
        }

        public uint GetProgramBase()
        {
            return this.ProgramBase;
        }

        public string GetName()
        {
            return (string)this.Name;
        }

        public uint GetSize()
        {
            return this.Size;
        }

        public void SetSize(uint size)
        {
            this.Size = size;
        }

        public string GetPath()
        {
            return this.Path;
        }

        public void SetName(string name)
        {
            this.Name = name;
        }
    }
}
