namespace DoorsOS.RealMachines.Processors
{
    public interface IProcessor
    {
        public char[] Ptr { get; set; }
        public char[] R1 { get; set; }
        public char[] R2 { get; set; }
        public char[] Ic { get; set; }
        public char[] C { get; set; }
        public char Mode { get; set; }
        public char Pi { get; set; }
        public char Si { get; set; }
        public char Ti { get; set; }
        public char[] Cs { get; set; }
        public char[] Ds { get; set; }


        char[] FromIntToHexNumber(int value);
        char FromIntToHexNumberByte(int value);
        char[] FromIntToHexNumberTwoBytes(int value);
        int FromHexAsCharArrayToInt(char[] hex);
        void SetOverflowFlag(bool toFalse = false);
        void SetZeroFlag(bool toFalse = false);
        void SetCarryFlag(bool toFalse = false);
    }
}
