namespace DoorsOS.Devices.HardDisks
{
    public interface IHardDisk
    {
        string Path { get; }
        void Setup();
    }
}
