namespace DoorsOS.RealMachines
{
    public interface IRealMachine
    {
        void Run();
        char ReadVirtualMachineMemoryByte(int block, int index);
        string ReadVirtualMachineMemoryBytes(int block, int index, int numberOfBytes);
        string ReadVirtualMachineMemoryWord(int block, int index);
        void WriteVirtualMachineMemoryByte(int block, int index);
        void WriteVirtualMachineMemoryBytes(int block, int index, int numberOfBytes);
        void WriteVirtualMachineMemoryWord(int block, int index);
    }
}
