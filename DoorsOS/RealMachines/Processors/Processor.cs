using DoorsOS.OS.Constants;

namespace DoorsOS.RealMachines.Processors
{
    public class Processor : IProcessor
    {
        public char[] Ptr { get; set; } = new char[OsConstants.WordLenghtInBytes];
        public char[] R1 { get; set; } = new char[OsConstants.WordLenghtInBytes];
        public char[] R2 { get; set; } = new char[OsConstants.WordLenghtInBytes];
        public char[] Ic { get; set; } = new char[OsConstants.WordLenghtInBytes / 2];
        public char C { get; set; } = new();
        public char Mode { get; set; } = new();
        public char Pi { get; set; } = new();
        public char Si { get; set; } = new();
        public char Ti { get; set; } = new();

        public char[] FromIntToHexNumber(int value) // test nagative
        {
            return value.ToString("X4").ToCharArray();
        }

        public int FromHexAsCharArrayToInt(char[] hex)
        {
            return Int32.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        }
    }
}
