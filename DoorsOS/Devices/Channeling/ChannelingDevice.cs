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
        public char[] SB { get; set; }
        public char[] DB { get; set; }
        public char[] ST { get; set; }
        public char[] DT { get; set; }

        // Commands?
        public int XCHG { get; set; }

        public ChannelingDevice(IRam ram)
        {
            _hardDisk = new HardDisk();
            _ram = ram;
        }

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
