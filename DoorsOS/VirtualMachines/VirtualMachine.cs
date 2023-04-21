using DoorsOS.Devices.MemoryManagementUnits;
using DoorsOS.OS.Constants;
using DoorsOS.RealMachines;
using DoorsOS.RealMachines.Processors;
using System;

namespace DoorsOS.VirtualMachines
{
    public class VirtualMachine : IVirtualMachine
    {
        public bool IsActive { get; set; } = true;
        public bool IsFinished { get; set; } = false;
        private readonly IProcessor _processor;
        private readonly IMemoryManagementUnit _memoryManagementUnit;
        private readonly int dataSegmentStart;
        private readonly int codeSegmentStart;
        public VirtualMachine(IProcessor processor, IMemoryManagementUnit memoryManagementUnit)
        {
            _processor = processor;
            _processor.Ic = _processor.FromIntToHexNumberTwoBytes(0);
            _memoryManagementUnit = memoryManagementUnit;
            dataSegmentStart = _processor.FromHexAsCharArrayToInt(_processor.Ds);
            codeSegmentStart = _processor.FromHexAsCharArrayToInt(_processor.Cs);
        }

        public void ExecuteInstruction()
        {
            int icValue = _processor.FromHexAsCharArrayToInt(_processor.Ic);
            int instructionIndex = codeSegmentStart + icValue;
            int block = instructionIndex / OsConstants.BlockSize;
            int index = instructionIndex % OsConstants.BlockSize;
            string instruction = _memoryManagementUnit.ReadVirtualMachineMemoryWord(block, index).ToUpper();
            switch (instruction)
            {
                case Instructions.Comp:
                    ExecuteCompInstruction();
                    break;
                case Instructions.Jmpa:
                    ExecuteJmpaInstruction();
                    break;
                case Instructions.Jmpb:
                    ExecuteJmpbInstruction();
                    break;
                case Instructions.Jmpe:
                    ExecuteJmpeInstruction();
                    break;
                case Instructions.Jmne:
                    ExecuteJmneInstruction();
                    break;
                case Instructions.Jump:
                    ExecuteJumpInstructon();
                    break;
                case Instructions.Addi:
                    ExecuteAddiInstruction();
                    break;
                case Instructions.Subs:
                    ExecuteSubsInstruction();
                    break;
                case Instructions.Divi:
                    ExecuteDiviInstruction();
                    break;
                case Instructions.Mult:
                    ExecuteMultiInstruction();
                    break;
                case Instructions.Move:
                    ExecuteMoveInstruction(block, index);
                    break;
                case Instructions.Mvtr:
                    ExecuteMvtrInstruction();
                    break;
                case Instructions.Mvtm:
                    ExecuteMvtmInstruction();
                    break;
                case Instructions.Mvch:
                    ExecuteMvchInstruction(); // hdd line 26
                    break;
                case Instructions.Mmem:
                    ExecuteMmemInstruction(block, index);
                    break;
                case Instructions.Halt:
                    ExecuteHaltInstruction();
                    break;
                case Instructions.Exec:
                    // Implement me
                    break;
                case Instructions.Xchg:
                    // Implement here?
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

        private void ExecuteMoveInstruction(int block, int index)
        {
            string extraByte = _memoryManagementUnit.ReadVirtualMachineMemoryWord(block, index + OsConstants.WordLenghtInBytes + dataSegmentStart);
            _processor.R1 = extraByte.ToCharArray();
            int icValue = _processor.FromHexAsCharArrayToInt(_processor.Ic);
            _processor.Ic = _processor.FromIntToHexNumberTwoBytes(icValue + 2 * OsConstants.WordLenghtInBytes);
            DecrementTi();
        }

        private void ExecuteMvtrInstruction()
        {
            var block = _processor.FromHexAsCharArrayToInt(new char[] { _processor.R2[2] });
            var index = _processor.FromHexAsCharArrayToInt(new char[] { _processor.R2[3] });
            _processor.R1 = _memoryManagementUnit.ReadVirtualMachineMemoryWord(block, index + dataSegmentStart).ToArray();
            DecrementTi();
            IncrementIc();
        }

        private void ExecuteMvtmInstruction()
        {
            var block = _processor.FromHexAsCharArrayToInt(new char[] { _processor.R2[2] });
            var index = _processor.FromHexAsCharArrayToInt(new char[] { _processor.R2[3] });
            _memoryManagementUnit.WriteVirtualMachineMemoryWord(block, index + dataSegmentStart, new string(_processor.R1));
            DecrementTi();
            IncrementIc();
        }

        private void ExecuteMvchInstruction()
        {
            (_processor.R2, _processor.R1) = (_processor.R1, _processor.R2);
            DecrementTi();
            IncrementIc();
        }

        private void ExecuteMmemInstruction(int block, int index)
        {
            string extraByte = _memoryManagementUnit.ReadVirtualMachineMemoryWord(block, index + OsConstants.WordLenghtInBytes + dataSegmentStart);
            block = _processor.FromHexAsCharArrayToInt(new char[] { extraByte[2] });
            index = _processor.FromHexAsCharArrayToInt(new char[] { extraByte[3] });
            _processor.R1 = _memoryManagementUnit.ReadVirtualMachineMemoryWord(block, index + dataSegmentStart).ToArray();

            int icValue = _processor.FromHexAsCharArrayToInt(_processor.Ic);
            _processor.Ic = _processor.FromIntToHexNumberTwoBytes(icValue + 2 * OsConstants.WordLenghtInBytes);
            DecrementTi();
        }

        private void ExecuteCompInstruction()
        {
            var r1Value = _processor.FromHexAsCharArrayToInt(_processor.R1);
            var r2Value = _processor.FromHexAsCharArrayToInt(_processor.R2);
            if (r1Value == r2Value )
            {
                _processor.SetZeroFlag();
            }
            else if (r1Value < r2Value )
            {
                _processor.SetCarryFlag();
                _processor.SetZeroFlag(true);
            }
            else if (r1Value > r2Value)
            {
                _processor.SetCarryFlag(true);
                _processor.SetZeroFlag(true);
            }
            else
            {
                _processor.SetZeroFlag(true);
            }
            DecrementTi();
            IncrementIc();
        }

        private void ExecuteJumpInstructon()
        {
            int r1Value = _processor.FromHexAsCharArrayToInt(_processor.R1);
            _processor.Ic = _processor.FromIntToHexNumberTwoBytes(r1Value);
            DecrementTi();
            IncrementIc();
        }

        private void ExecuteJmpaInstruction()
        {
            if (_processor.C[2] == '0' && _processor.C[3] == '0')
            {
                ExecuteJumpInstructon();
            }
            else
            {
                DecrementTi();
                IncrementIc();
            }
        }
        private void ExecuteJmpbInstruction()
        {
            if (_processor.C[2] == '1' && _processor.C[3] == '0')
            {
                ExecuteJumpInstructon();
            }
            else
            {
                DecrementTi();
                IncrementIc();
            }
        }

        private void ExecuteJmpeInstruction()
        {
            if (_processor.C[3] == '1')
            {
                ExecuteJumpInstructon();
            }
            else
            {
                DecrementTi();
                IncrementIc();
            }
        }

        private void ExecuteJmneInstruction()
        {
            if (_processor.C[3] == '0')
            {
                ExecuteJumpInstructon();
            }
            else
            {
                DecrementTi();
                IncrementIc();
            }
        }

        private void ExecuteAddiInstruction()
        {
            var r1Value = _processor.FromHexAsCharArrayToInt(_processor.R1);
            var r2Value = _processor.FromHexAsCharArrayToInt(_processor.R2);

            var result = r1Value + r2Value;
            uint uintResult = (uint)r1Value + (uint)r2Value;

            SetFlags(result, uintResult);

            _processor.R1 = _processor.FromIntToHexNumber(result);

            DecrementTi();
            IncrementIc();
        }

        private void ExecuteMultiInstruction()
        {
            var r1Value = _processor.FromHexAsCharArrayToInt(_processor.R1);
            var r2Value = _processor.FromHexAsCharArrayToInt(_processor.R2);

            var result = r1Value * r2Value;
            uint uintResult = (uint)r1Value * (uint)r2Value;

            SetFlags(result, uintResult);

            _processor.R1 = _processor.FromIntToHexNumber(result);

            DecrementTi();
            IncrementIc();
        }

        private void ExecuteSubsInstruction()
        {
            var r1Value = _processor.FromHexAsCharArrayToInt(_processor.R1);
            var r2Value = _processor.FromHexAsCharArrayToInt(_processor.R2);

            var result = r1Value - r2Value;
            uint uintResult = (uint)r1Value - (uint)r2Value;

            SetFlags(result, uintResult);

            _processor.R1 = _processor.FromIntToHexNumber(result);

            DecrementTi();
            IncrementIc();
        }

        private void ExecuteDiviInstruction()
        {
            var r1Value = _processor.FromHexAsCharArrayToInt(_processor.R1);
            var r2Value = _processor.FromHexAsCharArrayToInt(_processor.R2);

            var quotient = r1Value / r2Value;
            var remainder = r1Value % r2Value;

            if (quotient == 0)
            {
                _processor.SetZeroFlag();
            }
            if (remainder != 0)
            {
                _processor.SetCarryFlag();
            }

            _processor.R1 = _processor.FromIntToHexNumber(quotient);
            _processor.R2 = _processor.FromIntToHexNumber(remainder);

            DecrementTi();
            IncrementIc();
        }

        private void SetFlags(int result, uint uintResult)
        {
            if (result == 0)
            {
                _processor.SetZeroFlag();
            }

            if (result > Int32.MaxValue || result < Int32.MinValue)
            {
                _processor.SetOverflowFlag();
            }

            if (uintResult > uint.MaxValue)
            {
                _processor.SetCarryFlag();
            }
        }

        private void DecrementTi()
        {
            var tiValue = Int32.Parse(_processor.Ti.ToString(), System.Globalization.NumberStyles.HexNumber);
            _processor.Ti = _processor.FromIntToHexNumberByte(--tiValue);
        }

        private void IncrementIc()
        {
            var icValue = _processor.FromHexAsCharArrayToInt(_processor.Ic);
            _processor.Ic = _processor.FromIntToHexNumber(icValue + OsConstants.WordLenghtInBytes);
        }
    }
}
