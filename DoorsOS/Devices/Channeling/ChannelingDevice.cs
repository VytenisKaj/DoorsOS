using DoorsOS.Devices.HardDisks;
using DoorsOS.OS.Constants;
using DoorsOS.RealMachines.Memories;

namespace DoorsOS.Devices.Channeling
{
    public class ChannelingDevice
    {
        private readonly IHardDisk _hardDisk;
        private readonly IRam _ram;

        // Registers
        // nustatomi is realios masinos ir isvedami i ekrana?

        /// <summary>
        /// Track, FROM which we will copy, number.
        /// </summary>
        public char[] SB { get; set; }
        /// <summary>
        /// Track, TO which we will copy, number.
        /// </summary>
        public char[] DB { get; set; }
        /// <summary>
        /// Object, FROM which we will copy, number.
        /// </summary>
        public char[] ST { get; set; }
        /// <summary>
        /// Object, TO which we will copy, number.
        /// </summary>
        public char[] DT { get; set; }

        // Command, which channeling device itself cannot perform (does not have processor)?
        public int XCHG { get; set; }

        public ChannelingDevice(IRam ram)
        {
            _hardDisk = new HardDisk();
            _ram = ram;
        }

        public (string command, string[] commandAndParameters) ReadAndFormatInput()
        {
            var commandWithParameters = Console.ReadLine();
            var commandAndParameters = commandWithParameters?.Split(' ');
            var command = commandAndParameters?[0].Trim().ToLower();

            return (command ?? string.Empty, commandAndParameters ?? new string[] {});
        }

        public void WriteToConsole(string message) => Console.WriteLine(message);

        public VirtualMachineSegments Channnel(string nameToFind)
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
                    case ChannelingConstants.Start:
                        foundAmj = false;
                        nameFound = false;
                        break;
                    case var s when s == nameToFind:
                        nameFound = true;
                        break;
                    case ChannelingConstants.Amj when nameFound:
                        foundAmj = true;
                        break;
                    case var s when line.Replace(" ", "") == ChannelingConstants.Code && foundAmj && line != ChannelingConstants.End:
                        codeSegment = supervizorCurrentByte;
                        break;
                    case var s when line.Replace(" ", "") == ChannelingConstants.Data && foundAmj && line != ChannelingConstants.End:
                        dataSegment = supervizorCurrentByte;
                        break;
                    case ChannelingConstants.End when foundAmj:
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
                                supervizorMemoryCurrentBlock += supervizorCurrentByte / OsConstants.BlockSize;
                                supervizorCurrentByte %= OsConstants.BlockSize;
                            }
                        }
                        break;
                }
            }
        
            return new VirtualMachineSegments() { DataSegment = dataSegment, CodeSegment = codeSegment };
        }
    }
}
