using DoorsOS.OS.Constants;
using DoorsOS.RealMachines.Memories;
using DoorsOS.RealMachines.Processors;
using System.Text;

namespace DoorsOS.Devices.MemoryManagementUnits
{
    public class MemoryManagementUnit : IMemoryManagementUnit
    {
        private readonly IProcessor _processor;
        private readonly IRam _ram;
        public MemoryManagementUnit(IProcessor processor, IRam ram)
        {
            _processor = processor;
            _ram = ram;
        }

        public char[] RealMemoryBlock(char virtualMachineMemoryBlock)
        {
            var ptrValue = _processor.FromHexAsCharArrayToInt(_processor.Ptr);
            var virtualMachineBlockValue = _processor.FromHexAsCharArrayToInt(new char[] { virtualMachineMemoryBlock });
            var realBlock = _ram.GetMemoryAsInt(ptrValue, virtualMachineBlockValue * OsConstants.WordLenghtInBytes);
            return _processor.FromIntToHexNumber(realBlock);
        }

        public char ReadVirtualMachineMemoryByte(int block, int index)
        {
            var ptrValue = _processor.FromHexAsCharArrayToInt(_processor.Ptr);
            var realBlock = _ram.GetMemoryAsInt(ptrValue, block * OsConstants.WordLenghtInBytes);
            return _ram.GetMemoryByte(realBlock, index);
        }

        public string ReadVirtualMachineMemoryBytes(int block, int index, int numberOfBytes)
        {
            var sb = new StringBuilder(numberOfBytes);
            for (int i = 0; i < numberOfBytes; i++)
            {
                sb.Append(ReadVirtualMachineMemoryByte(block, index + i));
            }
            return sb.ToString();
        }

        public string ReadVirtualMachineMemoryWord(int block, int index)
        {
            return ReadVirtualMachineMemoryBytes(block, index, OsConstants.WordLenghtInBytes);
        }

        public void WriteVirtualMachineMemoryByte(int block, int index, char value)
        {
            var ptrValue = _processor.FromHexAsCharArrayToInt(_processor.Ptr);
            var realBlock = _ram.GetMemoryAsInt(ptrValue, block * OsConstants.WordLenghtInBytes);
            _ram.SetMemoryByte(realBlock, index, value);
        }

        public void WriteVirtualMachineMemoryBytes(int block, int index, string bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                WriteVirtualMachineMemoryByte(block, index + i, bytes[i]);
            }
        }

        public void WriteVirtualMachineMemoryWord(int block, int index, string word)
        {
            WriteVirtualMachineMemoryBytes(block, index, word);
        }
    }
}
