// Name: Joseph Feltz
// zID: z2048486

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
