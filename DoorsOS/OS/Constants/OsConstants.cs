namespace DoorsOS.OS.Constants
{
    public static class OsConstants
    {
        public static readonly string HardDisk = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "Devices/HardDisks/hdd.txt");
        public const int MinimalVirtualMachineCount = 4;
        public const int WordLenghtInBytes = 4;
        public const int VirtualMachineBlocksCount = 16;
        public const int BlockSize = PageSize * WordLenghtInBytes;
        public const int BlockForPagging = 1;
        public const int TotalMemorySize = 65536;
        public const int PageSize = 16;
    }
}
