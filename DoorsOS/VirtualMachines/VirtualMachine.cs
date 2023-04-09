using DoorsOS.OS.Constants;
using DoorsOS.RealMachines;
using DoorsOS.RealMachines.Processors;

namespace DoorsOS.VirtualMachines
{
    public class VirtualMachine : IVirtualMachine
    {
        public bool IsActive { get; set; } = true;
        public bool IsFinished { get; set; } = false;
        private readonly IProcessor _processor;
        private readonly IRealMachine _realMachine;
        private readonly int dataSegmentStart;
        private readonly int codeSegmentStart;
        public VirtualMachine(IProcessor processor, IRealMachine realMachine)
        {
            _processor = processor;
            _processor.Ic = _processor.FromIntToHexNumberTwoBytes(0);
            _realMachine = realMachine;
            dataSegmentStart = _processor.FromHexAsCharArrayToInt(_processor.Ds);
            codeSegmentStart = _processor.FromHexAsCharArrayToInt(_processor.Cs);
        }

        public void ExecuteInstruction()
        {
            int icValue = _processor.FromHexAsCharArrayToInt(_processor.Ic);
            int instructionIndex = codeSegmentStart + icValue;
            int block = instructionIndex / OsConstants.BlockSize;
            int index = instructionIndex % OsConstants.BlockSize;
            string instruction = _realMachine.ReadVirtualMachineMemoryWord(block, index).ToUpper();
            switch (instruction)
            {
                case Instructions.Comp:
                    break;
                case Instructions.Jmpa:
                    break;
                case Instructions.Jmpb:
                    break;
                case Instructions.Jmpe:
                    break;
                case Instructions.Jmne:
                    break;
                case Instructions.Jump:
                    ExecuteJumpInstructon();
                    break;
                case Instructions.Addi:
                    break;
                case Instructions.Subs:
                    break;
                case Instructions.Divi:
                    break;
                case Instructions.Mult:
                    break;
                case Instructions.Move:
                    ExecuteMoveInstruction(block, index);
                    break;
                case Instructions.Mvtr:
                    break;
                case Instructions.Mvtm:
                    break;
                case Instructions.Mvch:
                    break;
                case Instructions.Halt:
                    ExecuteHaltInstruction();
                    break;
                case Instructions.Exec:
                    break;
                case Instructions.Rdin:
                    break;
                case Instructions.Ptin:
                    break;
                case Instructions.Rdch:
                    break;
                case Instructions.Ptch:
                    break;
                default:
                    throw new NotSupportedException(instruction);
            }
        }

        private void ExecuteHaltInstruction()
        {
            IsFinished = true;
            _processor.Ti = _processor.FromIntToHexNumberByte(0);
        }

        private void ExecuteJumpInstructon()
        {
            int r1Value = _processor.FromHexAsCharArrayToInt(_processor.R1);
            _processor.Ic = _processor.FromIntToHexNumberTwoBytes(r1Value);
            DecrementTi();
        }

        private void ExecuteMoveInstruction(int block, int index)
        {
            string extraByte = _realMachine.ReadVirtualMachineMemoryWord(block, index + OsConstants.WordLenghtInBytes);
            _processor.R1 = extraByte.ToCharArray();
            int icValue = _processor.FromHexAsCharArrayToInt(_processor.Ic);
            _processor.Ic = _processor.FromIntToHexNumberTwoBytes(icValue + 2 * OsConstants.WordLenghtInBytes);
            DecrementTi();
        }

        private void DecrementTi()
        {
            var tiValue = Int32.Parse(_processor.Ti.ToString(), System.Globalization.NumberStyles.HexNumber);
            _processor.Ti = _processor.FromIntToHexNumberByte(--tiValue);
        }
    }
}
