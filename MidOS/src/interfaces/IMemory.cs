// Name: Joseph Feltz
// zID: z2048486

namespace MidOS.src.interfaces
{
    public interface IMemory
    {
        int GetSize();

        uint GetByte(uint addr);

        void SetByte(uint addr, uint value);
        
    }
}
