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
            bool foundAmj = false;
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

                    if (line.StartsWith("$$$$"))
                    {
                        foundAmj = false;
                        nameFound = false;
                    }
                    else if (line == nameToFind)
                    {
                        nameFound = true;
                    }
                    else if (line == "$AMJ" && nameFound)
                    {
                        foundAmj = true;
                    }
                    else if (foundAmj && line != "$END")
                    {
                        line = string.Join("", line.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                        if (line == "CODE")
                        {
                            codeSegment = supervizorCurrentByte;
                        }
                        else if (line == "DATA")
                        {
                            dataSegment = supervizorCurrentByte;
                        }
                        else
                        {
                            _ram.SetSupervizorMemoryBytes(supervizorMemoryCurrentBlock, supervizorCurrentByte, line);
                            supervizorCurrentByte += line.Length;
                            if(supervizorCurrentByte >= OsConstants.BlockSize)
                            {
                                int numberOfBlocks = supervizorCurrentByte / OsConstants.BlockSize;
                                supervizorMemoryCurrentBlock += numberOfBlocks;
                                supervizorCurrentByte = numberOfBlocks * OsConstants.BlockSize;
                            }
                        }

                    }
                    else if (foundAmj && line == "$END")
                    {
                        break;
                    }
                    else
                    {
                        nameFound = false;
                    }
                }
            }
            StartVirtualMachine(dataSegment, codeSegment);
        }

        private void StartVirtualMachine(int dataSegment, int codeSegment)
        {
            _paginator.GetPages();
            MoveFromSupervizorMemoryToDedicatedPages();
            _processor.Cs = _processor.FromIntToHexNumberTwoBytes(codeSegment);
            _processor.Ds = _processor.FromIntToHexNumberTwoBytes(dataSegment);
            _processor.Ti = 'F';
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

        public char ReadVirtualMachineMemoryByte(int block, int index)
        {
            var ptrValue = _processor.FromHexAsCharArrayToInt(_processor.Ptr);
            var realBlock = _ram.GetMemoryAsInt(ptrValue + block * OsConstants.WordLenghtInBytes, 0);
            return _ram.GetMemoryByte(realBlock, index);
        }

        public string ReadVirtualMachineMemoryBytes(int block, int index, int numberOfBytes)
        {
            var sb = new StringBuilder(numberOfBytes);
            for (int i = 0; i < numberOfBytes; i++)
            {
                sb.Append(ReadVirtualMachineMemoryByte(block, index + i));
            }
            return sb.ToString();
        }

        public string ReadVirtualMachineMemoryWord(int block, int index)
        {
            return ReadVirtualMachineMemoryBytes(block, index, OsConstants.WordLenghtInBytes);
        }

        public void WriteVirtualMachineMemoryByte(int block, int index)
        {
            throw new NotImplementedException();
        }

        public void WriteVirtualMachineMemoryBytes(int block, int index, int numberOfBytes)
        {
            throw new NotImplementedException();
        }

        public void WriteVirtualMachineMemoryWord(int block, int index)
        {
            throw new NotImplementedException();
        }
    }
}
