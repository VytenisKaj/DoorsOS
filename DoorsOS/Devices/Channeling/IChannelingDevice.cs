namespace DoorsOS.Devices.Channeling
{
    public interface IChannelingDevice
    {
        char[] SB { get; set; }
        char[] DB { get; set; }
        char[] ST { get; set; }
        char[] DT { get; set; }
        int XCHG { get; set; }
        (string command, string[] commandAndParameters) ReadAndFormatInput();
        void WriteToConsole(string message);
        VirtualMachineSegments Channnel(string nameToFind);
    }
}
