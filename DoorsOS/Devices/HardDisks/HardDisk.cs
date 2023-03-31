using DoorsOS.OS.Constants;

namespace DoorsOS.Devices.HardDisks
{
    public class HardDisk : IHardDisk
    {
        public string Path { get; } = OsConstants.HardDisk;

        public void Setup()
        {
            if(!File.Exists(Path))
            {
                File.Create(Path);
            }
        }
    }
}
