using DoorsOS.OS.Constants;
using System.Text;

namespace DoorsOS.RealMachines.Memories
{
    public class Ram : IRam
    {
        public char[] Memory { get; } = new char[OsConstants.TotalMemorySize];
        public bool[] IsBlockUsed { get; } = new bool[OsConstants.TotalMemorySize / OsConstants.PageSize];

        public int SupervisorMemoryStart()
        {
            return OsConstants.TotalMemorySize - OsConstants.BlockSize * OsConstants.VirtualMachineBlocksCount;
        }
        public void SetSupervizorMemoryByte(int block, int index, char value)
        {
            this.Memory[SupervisorMemoryStart() + block * OsConstants.BlockSize + index] = value;
        }
        
        public void SetSupervizorMemoryBytes(int block, int index, string bytes)
        {
            foreach (char value in bytes)
            {
                SetSupervizorMemoryByte(block, index, value);
                index++;
            }
        }

        public char GetMemoryByte(int block, int index)
        {
            return this.Memory[block * OsConstants.BlockSize + index];
        }

        public string GetMemoryBytes(int block, int index, int numberOfBytes)
        {
            var sb = new StringBuilder(numberOfBytes);
            for (int i = 0; i < numberOfBytes; i++)
            {
                sb.Append(GetMemoryByte(block, index + i));
            }
            return sb.ToString();
        }

        public string GetMemoryWord(int block, int index)
        {
            return GetMemoryBytes(block, index, OsConstants.WordLenghtInBytes);
        }

        public int GetMemoryAsInt(int block, int index)
        {
            return Int32.Parse(GetMemoryWord(block, index), System.Globalization.NumberStyles.HexNumber);
        }

        public void SetMemoryByte(int block, int index, char value)
        {
            this.Memory[block * OsConstants.BlockSize + index] = value;
        }

        public void SetMemoryBytes(int block, int index, string bytes)
        {
            foreach (char value in bytes)
            {
                SetMemoryByte(block, index, value);
                index++;
            }
        }

        public void SetMemoryWord(int block, int index, string value)
        {
            SetMemoryBytes(block, index, value);
        }

        public void SetMemoryNumber(int block, int word, int value)
        {
            SetMemoryWord(block, word * OsConstants.WordLenghtInBytes, value.ToString("X4"));
        }

        public char GetSupervizorMemoryByte(int block, int index)
        {
            return this.Memory[SupervisorMemoryStart() + block * OsConstants.BlockSize + index];
        }

        public string GetSupervizorMemoryBytes(int block, int index, int numberOfBytes)
        {
            var sb = new StringBuilder(numberOfBytes);
            for (int i = 0; i < numberOfBytes; i++)
            {
                sb.Append(GetSupervizorMemoryByte(block, index + i));
            }
            return sb.ToString();
        }

        public string GetSupervizorMemoryWord(int block, int index)
        {
            return GetSupervizorMemoryBytes(block, index, OsConstants.WordLenghtInBytes);
        }

        public int GetSupervizorMemoryInt(int block, int index)
        {
            throw new NotImplementedException();
        }

        public void SetSupervizorMemoryPage(int block, string bytes)
        {
            //SetMemoryPage(SupervisorMemoryStart() + block, bytes);
            SetSupervizorMemoryBytes(block, 0, bytes);
        }

        public void SetMemoryPage(int block, string value)
        {
            for (int i = 0; i < OsConstants.BlockSize; i++)
            {
                char charToSet = i < value.Length? value[i] : '\0';
                SetMemoryByte(block, i, charToSet);
            }
        }

        public string GetSupervizoryMemoryPage(int block)
        {
            //return GetMemoryPage(SupervisorMemoryStart() + block);
            var sb = new StringBuilder(OsConstants.BlockSize);
            for (int i = 0; i < OsConstants.PageSize; i++)
            {
                sb.Append(GetSupervizorMemoryWord(block, i * OsConstants.WordLenghtInBytes));
            }
            return sb.ToString();
        }

        public string GetMemoryPage(int block)
        {
            var sb = new StringBuilder(OsConstants.BlockSize);
            for (int i = 0; i < OsConstants.PageSize; i++)
            {
                sb.Append(GetMemoryWord(block, i * OsConstants.WordLenghtInBytes));
            }
            return sb.ToString();
        }
    }
}
