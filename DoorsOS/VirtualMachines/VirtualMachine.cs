using DoorsOS.Devices.Channeling;
using DoorsOS.Devices.MemoryManagementUnits;
using DoorsOS.OS.Constants;
using DoorsOS.RealMachines.Processors;
using System.Globalization;

namespace DoorsOS.VirtualMachines
{
    public class VirtualMachine : IVirtualMachine
    {
        public bool IsActive { get; set; } = true;
        public bool IsFinished { get; set; } = false;
        public bool IsStopped { get; set; } = false;
        private readonly IProcessor _processor;
        private readonly IMemoryManagementUnit _memoryManagementUnit;
        private readonly IChannelingDevice _channelingDevice;

        private readonly int dataSegmentStart;
        private readonly int codeSegmentStart;
        private ProcessorState _processorState;

        public VirtualMachine(IProcessor processor, IMemoryManagementUnit memoryManagementUnit, IChannelingDevice channelingDevice)
        {
            _processor = processor;
            _processor.Ic = _processor.FromIntToHexNumberTwoBytes(0);
            _channelingDevice = channelingDevice;
            _processor.Pi = InterruptConstants.PiReset;
            _processor.Si = InterruptConstants.SiReset;
            _processor.Ti = InterruptConstants.TiReset;
            _memoryManagementUnit = memoryManagementUnit;
            dataSegmentStart = _processor.FromHexAsCharArrayToInt(_processor.Ds);
            codeSegmentStart = _processor.FromHexAsCharArrayToInt(_processor.Cs);
            _processorState = new ProcessorState(processor);
        }

        public void ExecuteInstruction()
        {
            int icValue = _processor.FromHexAsCharArrayToInt(_processor.Ic);
            int instructionIndex = codeSegmentStart + icValue;
            int block = instructionIndex / OsConstants.BlockSize;
            int index = instructionIndex % OsConstants.BlockSize;
            var instruction = _memoryManagementUnit.ReadVirtualMachineMemoryWord(block, index).ToUpper();
            //(string instruction, int block, int index) = GetInstruction();
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
                    ExecuteJumpInstruction();
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
                    ExecuteMultInstruction();
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
                    ExecuteMvchInstruction();
                    break;
                case Instructions.Mmem:
                    ExecuteMmemInstruction(block, index);
                    break;
                case Instructions.Halt:
                    ExecuteHaltInstruction();
                    break;
                case Instructions.Exec:
                    ExecuteExecInstruction();
                    break;
                case Instructions.Rdin:
                    ExecuteRdinInstruction();
                    break;
                case Instructions.Ptin:
                    ExecutePtinInstruction();
                    break;
                case Instructions.Rdch:
                    ExecuuteRdchInstruction();
                    break;
                case Instructions.Ptch:
                    ExecutePtchInstruction();
                    break;
                default:
                    _processor.Pi = InterruptConstants.PiBadOpCode;
                    break;
            }
        }

        public (string instruction, int block, int index) GetInstruction()
        {
            int icValue = _processor.FromHexAsCharArrayToInt(_processor.Ic);
            int instructionIndex = codeSegmentStart + icValue;
            int block = instructionIndex / OsConstants.BlockSize;
            int index = instructionIndex % OsConstants.BlockSize;
            return  (_memoryManagementUnit.ReadVirtualMachineMemoryWord(block, index).ToUpper(), block, index);
        }

        private void ExecuteExecInstruction()
        {
            _processor.Si = InterruptConstants.SiExec;
            DoTiAndIcDefaultBehaviour();
        }

        private void ExecuteHaltInstruction()
        {
            IsFinished = true;
            IsActive = false;
            IsStopped = false;
            _processor.Si = InterruptConstants.SiHalt;
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
            DoTiAndIcDefaultBehaviour();
        }

        private void ExecuteMvtmInstruction()
        {
            var block = _processor.FromHexAsCharArrayToInt(new char[] { _processor.R2[2] });
            var index = _processor.FromHexAsCharArrayToInt(new char[] { _processor.R2[3] });
            
            _memoryManagementUnit.WriteVirtualMachineMemoryWord(block, index + dataSegmentStart, new string(_processor.R1));
            DoTiAndIcDefaultBehaviour();
        }

        private void ExecuteMvchInstruction()
        {
            (_processor.R2, _processor.R1) = (_processor.R1, _processor.R2);
            DoTiAndIcDefaultBehaviour();
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

            DoTiAndIcDefaultBehaviour();
        }

        private void ExecuteJumpInstruction()
        {
            int r1Value = _processor.FromHexAsCharArrayToInt(_processor.R1);
            _processor.Ic = _processor.FromIntToHexNumberTwoBytes(r1Value);
            DecrementTi();
        }

        private void ExecuteJmpaInstruction()
        {
            if (_processor.C[2] == '0' && _processor.C[3] == '0')
            {
                ExecuteJumpInstruction();
            }
            else
            {
                DoTiAndIcDefaultBehaviour();
            }
        }
        private void ExecuteJmpbInstruction()
        {
            if (_processor.C[2] == '1' && _processor.C[3] == '0')
            {
                ExecuteJumpInstruction();
            }
            else
            {
                DoTiAndIcDefaultBehaviour();
            }
        }

        private void ExecuteJmpeInstruction()
        {
            if (_processor.C[3] == '1')
            {
                ExecuteJumpInstruction();
            }
            else
            {
                DoTiAndIcDefaultBehaviour();
            }
        }

        private void ExecuteJmneInstruction()
        {
            if (_processor.C[3] == '0')
            {
                ExecuteJumpInstruction();
            }
            else
            {
                DoTiAndIcDefaultBehaviour();
            }
        }

        private int HexToIntTwosComplement(char[] hex)
        {
            int temp = int.Parse(hex, NumberStyles.HexNumber);
            return (temp > 32767) ? (int)(temp - 65536) : (int)temp;
        }

        private void ExecuteAddiInstruction()
        {
            var r1Value = HexToIntTwosComplement(_processor.R1);
            var r2Value = HexToIntTwosComplement(_processor.R2);

            short result = (short)(r1Value + r2Value);
            int intResult = r1Value + r2Value;

            //SetFlags(result, uintResult);

            _processor.R1 = _processor.FromIntToHexNumber(result);

            DoTiAndIcDefaultBehaviour();
        }

        private void ExecuteMultInstruction()
        {
            var r1Value = HexToIntTwosComplement(_processor.R1);
            var r2Value = HexToIntTwosComplement(_processor.R2);

            short result = (short)(r1Value * r2Value);
            int intResult = r1Value + r2Value;

            //SetFlags(result, uintResult);

            _processor.R1 = _processor.FromIntToHexNumber(result);

            DoTiAndIcDefaultBehaviour();
        }

        private void ExecuteSubsInstruction()
        {
            var r1Value = HexToIntTwosComplement(_processor.R1);
            var r2Value = HexToIntTwosComplement(_processor.R2);

            short result = (short)(r1Value - r2Value);
            int intResult = r1Value + r2Value;

            //SetFlags(result, uintResult);

            _processor.R1 = _processor.FromIntToHexNumber(result);

            DoTiAndIcDefaultBehaviour();
        }

        private void ExecuteDiviInstruction()
        {
            var r1Value = HexToIntTwosComplement(_processor.R1);
            var r2Value = HexToIntTwosComplement(_processor.R2);

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

            DoTiAndIcDefaultBehaviour();
        }

        private void ExecutePtinInstruction()
        {
            _channelingDevice.ST = ChannelingDeviceConstants.FromUserMemory.ToArray();
            _channelingDevice.DT = ChannelingDeviceConstants.ToConsole.ToArray();
            var realBlock = _memoryManagementUnit.RealMemoryBlock(_processor.R1[2]);
            _channelingDevice.SB_block = realBlock;
            _channelingDevice.SB_index = new char[] { '0', '0', '0', _processor.R1[3] };

            _processor.Si = InterruptConstants.SiPtin;
            DoTiAndIcDefaultBehaviour();
        }

        private void ExecuteRdinInstruction()
        {
            _channelingDevice.ST = ChannelingDeviceConstants.FromConsole.ToArray();
            _channelingDevice.DT = ChannelingDeviceConstants.ToUserMemory.ToArray();
            _channelingDevice.DB_block = _memoryManagementUnit.RealMemoryBlock(_processor.R1[2]);
            _channelingDevice.DB_index = new char[] { '0', '0', '0', _processor.R1[3] };
            _processor.R2 = _processor.FromIntToHexNumber(OsConstants.WordLenghtInBytes);

            _processor.Si = InterruptConstants.SiRdin;
            DoTiAndIcDefaultBehaviour();
        }

        private void ExecutePtchInstruction()
        {
            _channelingDevice.ST = ChannelingDeviceConstants.FromUserMemory.ToArray();
            _channelingDevice.DT = ChannelingDeviceConstants.ToConsole.ToArray();
            var realBlock = _memoryManagementUnit.RealMemoryBlock(_processor.R1[2]);
            _channelingDevice.SB_block = realBlock;
            _channelingDevice.SB_index = new char[] {'0', '0', '0', _processor.R1[3] };

            _processor.Si = InterruptConstants.SiPtch;
            DoTiAndIcDefaultBehaviour();
        }

        private void ExecuuteRdchInstruction()
        {
            
            _channelingDevice.ST = ChannelingDeviceConstants.FromConsole.ToArray();
            _channelingDevice.DT = ChannelingDeviceConstants.ToUserMemory.ToArray();
            _channelingDevice.DB_block = _memoryManagementUnit.RealMemoryBlock(_processor.R1[2]);
            _channelingDevice.DB_index = new char[] { '0', '0', '0', _processor.R1[3] };

            _processor.Si = InterruptConstants.SiRdch;
            DoTiAndIcDefaultBehaviour();
        }

        private void SetFlags(int result, uint intResult)
        {
            if (result == 0)
            {
                _processor.SetZeroFlag();
            }

            if (result > Int32.MaxValue || result < Int32.MinValue)
            {
                _processor.SetOverflowFlag();
            }

            if (intResult > uint.MaxValue)
            {
                _processor.SetCarryFlag();
            }
        }

        private void DoTiAndIcDefaultBehaviour()
        {
            DecrementTi();
            IncrementIc();
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

        public void SaveState()
        {
            _processorState = new ProcessorState(_processor);
        }

        public void Resume()
        {
            IsActive = true;
            IsStopped = false;
            _processorState.Load(_processor);
        }
    }
}
