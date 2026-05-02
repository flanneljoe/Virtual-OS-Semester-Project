// Name: Joseph Feltz
// zID: z2048486

namespace MidOS.src.classes
{
    /// <summary>
    /// Holds the configuration values for the OS.
    /// Values are set by the CPU on startup, read from osconfig.json.
    /// Default values here serve as a fallback should the json fail to deserialize.
    /// </summary>
    public class OSConfig
    {
        public uint GlobalDataSize { get; set; } = 0;
        public uint HeapSize { get; set; } = 0;
        public uint StackSize { get; set; } = 4;
        public uint PageSize { get; set; } = 256;
        public uint TimeQuantum { get; set; } = 10;
        public uint SharedMemoryCount { get; set; } = 2;
        public uint PhysicalMemorySize { get; set; } = 4096;
    }
}
