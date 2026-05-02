// Name: Joseph Feltz
// zID: z2048486

using MidOS.src.interfaces;

namespace MidOS.src.classes
{
    public class MemPage : IMemPage
    {
        public uint PhysicalBase { get; set; }
        public bool IsOccupied { get; set; }
        public bool IsShared { get; set; } = false;
        public bool IsValid { get; set; } = true;
        public bool IsDirty { get; set; } = false;
        public ulong LastUsed { get; set; } = 0;

        public MemPage(uint physicalBase, bool isOccupied = false)
        {
            PhysicalBase = physicalBase;
            IsOccupied = isOccupied;
        }
    }
}
