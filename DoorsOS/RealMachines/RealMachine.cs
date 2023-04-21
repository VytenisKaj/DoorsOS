using DoorsOS.Devices.Channeling;
using DoorsOS.Devices.HardDisks;
using DoorsOS.Devices.MemoryManagementUnits;
using DoorsOS.OS.Constants;
using DoorsOS.Paginators;
using DoorsOS.RealMachines.Memories;
using DoorsOS.RealMachines.Processors;
using DoorsOS.VirtualMachines;

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
        private readonly IChannelingDevice _channelingDevice;

        public RealMachine()
        {
            _processor = new Processor();
            _ram = new Ram();
            _hardDisk = new HardDisk();
            _paginator = new Paginator(_ram, _processor);
            _memoryManagementUnit = new MemoryManagementUnit(_processor, _ram);
            _channelingDevice = new ChannelingDevice(_ram);

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
                var (command, commandAndParameters) = _channelingDevice.ReadAndFormatInput();

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
                                _channelingDevice.WriteToConsole(_processor.Ti.ToString());
                            }
                        }
                        else
                        {
                            _channelingDevice.WriteToConsole("RUN command missing parameter");
                        }
                        break;
                    default:
                        _channelingDevice.WriteToConsole($"'{command}' is not a valid commnd");
                        break;
                }
            }
        }

        private void ExecuteRun(string nameToFind)
        {
            var segmentsForVM = _channelingDevice.Channnel(nameToFind);
            StartVirtualMachine(segmentsForVM.DataSegment, segmentsForVM.CodeSegment);
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
    }
}
