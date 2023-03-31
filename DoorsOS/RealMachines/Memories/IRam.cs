namespace DoorsOS.RealMachines.Memories
{
    public interface IRam
    {
        public char[] Memory { get; }

        public bool[] IsBlockUsed { get; }
        int SupervisorMemoryStart();
        void SetSupervizorMemoryByte(int block, int index, char value);
        void SetSupervizorMemoryBytes(int block, int index, string bytes);

        void SetSupervizorMemoryPage(int block, string bytes);

        void SetMemoryByte(int block, int index, char value);

        void SetMemoryBytes(int block, int index, string bytes);

        void SetMemoryWord(int block, int index, string value);

        void SetMemoryPage(int block, string value);

        void SetMemoryNumber(int block, int index, int value);

        char GetSupervizorMemoryByte(int block, int index);

        string GetSupervizorMemoryBytes(int block, int index, int numberOfBytes);

        string GetSupervizorMemoryWord(int block, int index);

        string GetSupervizoryMemoryPage(int block);

        int GetSupervizorMemoryInt(int block, int index);

        char GetMemoryByte(int block, int index);

        string GetMemoryBytes(int block, int index, int numberOfBytes);

        string GetMemoryWord(int block, int index);

        string GetMemoryPage(int block);

        int GetMemoryAsInt(int block, int index);
    }
}
