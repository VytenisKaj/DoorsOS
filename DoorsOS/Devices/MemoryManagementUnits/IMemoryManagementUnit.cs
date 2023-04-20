using DoorsOS.OS.Constants;
using System.Text;

namespace DoorsOS.Devices.MemoryManagementUnits
{
    public interface IMemoryManagementUnit
    {
        char ReadVirtualMachineMemoryByte(int block, int index);

        string ReadVirtualMachineMemoryBytes(int block, int index, int numberOfBytes);

        string ReadVirtualMachineMemoryWord(int block, int index);

        void WriteVirtualMachineMemoryByte(int block, int index, char value);

        void WriteVirtualMachineMemoryBytes(int block, int index, string bytes);

        void WriteVirtualMachineMemoryWord(int block, int index, string word);
    }
}
