using DoorsOS.OS.Constants;

namespace DoorsOS.RealMachines.Processors
{
    public class Processor : IProcessor
    {
        public char[] Ptr { get; set; } = new char[OsConstants.WordLenghtInBytes];
        public char[] R1 { get; set; } = new char[OsConstants.WordLenghtInBytes];
        public char[] R2 { get; set; } = new char[OsConstants.WordLenghtInBytes];
        public char[] Ic { get; set; } = new char[OsConstants.WordLenghtInBytes / 2];
        public char[] C { get; set; } = new char[OsConstants.WordLenghtInBytes];
        public char Mode { get; set; } = new();
        public char Pi { get; set; } = new();
        public char Si { get; set; } = new();
        public char Ti { get; set; } = new();
        public char[] Cs { get; set; } = new char[OsConstants.WordLenghtInBytes / 2];
        public char[] Ds { get; set; } = new char[OsConstants.WordLenghtInBytes / 2];

        public char[] FromIntToHexNumber(int value) // test nagative
        {
            return value.ToString("X4").ToCharArray();
        }

        public char[] FromIntToHexNumberTwoBytes(int value)
        {
            return value.ToString("X2").ToCharArray();
        }

        public char FromIntToHexNumberByte(int value)
        {
            return value.ToString("X")[0];
        }

        public int FromHexAsCharArrayToInt(char[] hex)
        {
            return Int32.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        }

        public void SetOverflowFlag(bool toFalse = false)
        {
            C[1] = toFalse ? '0' : '1';
        }

        public void SetZeroFlag(bool toFalse = false)
        {
            C[3] = toFalse ? '0' : '1';
        }

        public void SetCarryFlag(bool toFalse = false)
        {
            C[2] = toFalse ? '0' : '1';
        }
    }
}
