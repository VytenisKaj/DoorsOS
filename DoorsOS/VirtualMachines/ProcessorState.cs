using DoorsOS.OS.Constants;
using DoorsOS.RealMachines.Processors;

namespace DoorsOS.VirtualMachines
{
    public class ProcessorState
    {
        public char[] Ptr { get; private set; } = new char[OsConstants.WordLenghtInBytes];
        public char[] R1 { get; private set; } = new char[OsConstants.WordLenghtInBytes];
        public char[] R2 { get; private set; } = new char[OsConstants.WordLenghtInBytes];
        public char[] Ic { get; private set; } = new char[OsConstants.WordLenghtInBytes / 2];
        public char[] C { get; private set; } = new char[OsConstants.WordLenghtInBytes];
        public char[] Cs { get; private set; } = new char[OsConstants.WordLenghtInBytes / 2];
        public char[] Ds { get; private set; } = new char[OsConstants.WordLenghtInBytes / 2];

        public ProcessorState(IProcessor processor)
        {
            Ptr = processor.Ptr;
            R1 = processor.R1;
            R2 = processor.R2;
            C = processor.C;
            Cs = processor.Cs;
            Ds = processor.Ds;
            Ic = processor.Ic;
        }

        public void Load(IProcessor processor)
        {
            processor.Ptr = Ptr;
            processor.R1 = R1;
            processor.R2 = R2;
            processor.C = C;
            processor.Cs = Cs;
            processor.Ds = Ds;
            processor.Ic = Ic;
        }
    }
}
