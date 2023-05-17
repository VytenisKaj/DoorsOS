using DoorsOS.Devices.Channeling;
using DoorsOS.Devices.HardDisks;
using DoorsOS.Devices.MemoryManagementUnits;
using DoorsOS.OS.Constants;
using DoorsOS.Paginators;
using DoorsOS.RealMachines.Memories;
using DoorsOS.RealMachines.Processors;
using DoorsOS.VirtualMachines;
using System.Text;

namespace DoorsOS.RealMachines
{
    public class RealMachine : IRealMachine
    {
        private readonly IProcessor _processor;
        private readonly IRam _ram;
        private readonly IHardDisk _hardDisk;
        private readonly IPaginator _paginator;
        private readonly IMemoryManagementUnit _memoryManagementUnit;
        private readonly List<IVirtualMachine> _virtualMachines = new();

        public RealMachine()
        {
            _processor = new Processor();
            _ram = new Ram();
            _hardDisk = new HardDisk();
            _paginator = new Paginator(_ram, _processor);
            _memoryManagementUnit = new MemoryManagementUnit(_processor, _ram);

            /*_ram.IsBlockUsed[1] = true;
            _ram.IsBlockUsed[6] = true; // For testing paginator, simulating used pages
            _ram.IsBlockUsed[9] = true;
            _ram.IsBlockUsed[15] = true;*/
        }

        public void Run()
        {
            _hardDisk.Setup();
            bool isRunning = true;
            while (isRunning)
            {
                var comamndWithPamameters = Console.ReadLine();
                var commandAndParameters = comamndWithPamameters?.Split(' ');
                var command = commandAndParameters?[0].Trim().ToLower();
                switch (command)
                {
                    case Commands.Shutdown:
                        isRunning = false; 
                        break;
                    case Commands.Run:
                        if (commandAndParameters?.Length > 1)
                        {
                            ExecuteRun(commandAndParameters[1].Trim());
                            while (!_virtualMachines[0].IsFinished)
                            {
                                _virtualMachines[0].ExecuteInstruction();
                                Console.WriteLine(_processor.Ti);
                            }
                        }
                        else
                        {
                            Console.WriteLine("RUN command missing parameter");
                        }
                        break;
                    default:
                        Console.WriteLine($"'{command}' is not a valid commnd");
                        break;
                }
            }
        }

        private void ExecuteRun(string nameToFind)
        {
            var segments = ReadCommandsAndSetBytes(nameToFind);
            StartVirtualMachine(segments.DataSegment, segments.CodeSegment);
        }

        private VirtualMachineSegments ReadCommandsAndSetBytes(string nameToFind)
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

        private void StartVirtualMachine(int dataSegment, int codeSegment)
        {
            _paginator.GetPages();
            MoveFromSupervizorMemoryToDedicatedPages();
            _processor.Cs = _processor.FromIntToHexNumberTwoBytes(codeSegment);
            _processor.Ds = _processor.FromIntToHexNumberTwoBytes(dataSegment);
            _virtualMachines.Add(new VirtualMachine(_processor, _memoryManagementUnit));
        }

        private void MoveFromSupervizorMemoryToDedicatedPages()
        {
            for (int i = 0; i < OsConstants.VirtualMachineBlocksCount; i++)
            {
                var ptrValue = _processor.FromHexAsCharArrayToInt(_processor.Ptr);
                var pageLocation = _ram.GetMemoryAsInt(ptrValue, i * OsConstants.WordLenghtInBytes);
                _ram.SetMemoryPage(pageLocation, _ram.GetSupervizoryMemoryPage(i));
            }
        }
    }
}
