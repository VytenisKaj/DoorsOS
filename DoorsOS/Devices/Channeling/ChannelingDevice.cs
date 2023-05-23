using DoorsOS.Devices.HardDisks;
using DoorsOS.OS.Constants;
using DoorsOS.RealMachines.Memories;
using DoorsOS.RealMachines.Processors;

namespace DoorsOS.Devices.Channeling
{
    public class ChannelingDevice : IChannelingDevice
    {
        private readonly IHardDisk _hardDisk;
        private readonly IRam _ram;
        private readonly IProcessor _processor;

        /// <summary>
        /// Track, FROM which we will copy, number.
        /// </summary>
        public char[] SB_block { get; set; } = new char[OsConstants.WordLenghtInBytes];
        public char[] SB_index { get; set; } = new char[OsConstants.WordLenghtInBytes];
        /// <summary>
        /// Track, TO which we will copy, number.
        /// </summary>
        public char[] DB_block { get; set; } = new char[OsConstants.WordLenghtInBytes];
        public char[] DB_index { get; set; } = new char[OsConstants.WordLenghtInBytes];
        /// <summary>
        /// Object, FROM which we will copy, number.
        /// </summary>
        public char[] ST { get; set; } = new char[OsConstants.WordLenghtInBytes];
        /// <summary>
        /// Object, TO which we will copy, number.
        /// </summary>
        public char[] DT { get; set; } = new char[OsConstants.WordLenghtInBytes];

        // kiek rasysim
        public int CNT { get; set; }

        public ChannelingDevice(IRam ram, IProcessor processor)
        {
            _hardDisk = new HardDisk();
            _ram = ram;
            _processor = processor;
        }

        public void Exchange(string nameToFind)
        {
            var input = string.Empty;
            var st = new string(ST);
            var dt = new string(DT);

            // ST register
            switch (st)
            {
                case ChannelingDeviceConstants.FromUserMemory:
                    {
                        var sbBlock = FromHexAsCharArrayToInt(SB_block);
                        var sbIndex = FromHexAsCharArrayToInt(SB_index);
                        input = _ram.GetMemoryBytes(sbBlock, sbIndex, CNT);
                    }
                    break;
                case ChannelingDeviceConstants.FromSupervizoryMemory:
                    {
                        var sbBlock = FromHexAsCharArrayToInt(SB_block);
                        var sbIndex = FromHexAsCharArrayToInt(SB_index);
                        input = _ram.GetSupervizorMemoryBytes(sbBlock, sbIndex, CNT);
                    }
                    break;
                case ChannelingDeviceConstants.FromHardDisk:
                    var segments = ReadFromHardDrive(nameToFind);

                    // set data & code segments
                    _processor.Cs = _processor.FromIntToHexNumberTwoBytes(segments.CodeSegment);
                    _processor.Ds = _processor.FromIntToHexNumberTwoBytes(segments.DataSegment);

                    break;
                case ChannelingDeviceConstants.FromConsole:
                    input = Console.ReadLine();
                    break;
                default:
                    _processor.Pi = InterruptConstants.PiBadAssignment;
                    return;

            }

            if (input == null)
            {
                return;
            }

            // DT register
            switch (dt)
            {
                case ChannelingDeviceConstants.ToUserMemory:
                    {
                        var dbBlock = FromHexAsCharArrayToInt(DB_block);
                        var dbIndex = FromHexAsCharArrayToInt(DB_index);
                        _ram.SetMemoryBytes(dbBlock, dbIndex, input);
                    }
                    break;
                case ChannelingDeviceConstants.ToSupervizoryMemory:
                    {
                        _ram.SetSupervizorMemoryBytes(0, 0, input);
                    }
                    break;
                case ChannelingDeviceConstants.ToConsole:
                    Console.Write(input);
                    break;
                default:
                    _processor.Pi = InterruptConstants.PiBadAssignment;
                    return;
            }
        }

        private static int FromHexAsCharArrayToInt(char[] hex)
        {
            return Int32.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        }

        private VirtualMachineSegments ReadFromHardDrive(string nameToFind)
        {
            bool foundAmj = false;
            bool nameFound = false;

            int supervizorMemoryCurrentBlock = 0;
            int supervizorCurrentByte = 0;
            int dataSegment = 0;
            int codeSegment = 0;

            using var reader = new StreamReader(_hardDisk.Path);
            bool breakLoop = false;

            while (reader.Peek() >= 0 && !breakLoop)
            {
                string line = reader.ReadLine();

                switch (line)
                {
                    case ChannelingDeviceConstants.Start:
                        foundAmj = false;
                        nameFound = false;
                        break;
                    case var s when s == nameToFind:
                        nameFound = true;
                        break;
                    case ChannelingDeviceConstants.Amj when nameFound:
                        foundAmj = true;
                        break;
                    case var s when line.Replace(" ", "") == ChannelingDeviceConstants.Code && foundAmj && line != ChannelingDeviceConstants.End:
                        codeSegment = supervizorCurrentByte;
                        break;
                    case var s when line.Replace(" ", "") == ChannelingDeviceConstants.Data && foundAmj && line != ChannelingDeviceConstants.End:
                        dataSegment = supervizorCurrentByte;
                        break;
                    case ChannelingDeviceConstants.End when foundAmj:
                        breakLoop = true;
                        break;
                    default:
                        if (!foundAmj)
                        {
                            nameFound = false;
                        }
                        else
                        {
                            line = line.Replace(" ", "");
                            _ram.SetSupervizorMemoryBytes(supervizorMemoryCurrentBlock, supervizorCurrentByte, line);
                            supervizorCurrentByte += line.Length;
                            if (supervizorCurrentByte >= OsConstants.BlockSize)
                            {
                                int numberOfBlocks = supervizorCurrentByte / OsConstants.BlockSize;
                                supervizorMemoryCurrentBlock += numberOfBlocks;
                                supervizorCurrentByte -= numberOfBlocks * OsConstants.BlockSize;
                            }
                        }
                        break;
                }
            }

            /*bool foundAmj = false;
            bool nameFound = false;

            int supervizorMemoryCurrentBlock = 0;
            int supervizorCurrentByte = 0;
            int dataSegment = 0;
            int codeSegment = 0;

            using (var reader = new StreamReader(_hardDisk.Path))
            {
                while (reader.Peek() >= 0)
                {
                    string line = reader.ReadLine();

                    if (line == ChannelingDeviceConstants.Start)
                    {
                        foundAmj = false;
                        nameFound = false;
                    }
                    else if (line == nameToFind)
                    {
                        nameFound = true;
                    }
                    else if (line == ChannelingDeviceConstants.Amj && nameFound)
                    {
                        foundAmj = true;
                    }
                    else if (foundAmj && line != "$END")
                    {
                        line = string.Join("", line.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                        if (line == ChannelingDeviceConstants.Code)
                        {
                            codeSegment = supervizorCurrentByte;
                        }
                        else if (line == ChannelingDeviceConstants.Data)
                        {
                            dataSegment = supervizorCurrentByte;
                        }
                        else
                        {
                            _ram.SetSupervizorMemoryBytes(supervizorMemoryCurrentBlock, supervizorCurrentByte, line);
                            supervizorCurrentByte += line.Length;
                            if (supervizorCurrentByte >= OsConstants.BlockSize)
                            {
                                int numberOfBlocks = supervizorCurrentByte / OsConstants.BlockSize;
                                supervizorMemoryCurrentBlock += numberOfBlocks;
                                supervizorCurrentByte = numberOfBlocks * OsConstants.BlockSize;
                            }
                        }

                    }
                    else if (foundAmj && line == ChannelingDeviceConstants.End)
                    {
                        break;
                    }
                    else
                    {
                        nameFound = false;
                    }
                }
            }*/

            return new VirtualMachineSegments() { DataSegment = dataSegment, CodeSegment = codeSegment };
        }
    }
}
