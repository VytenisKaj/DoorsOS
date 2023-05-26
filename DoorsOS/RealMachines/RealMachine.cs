using DoorsOS.Devices.Channeling;
using DoorsOS.Devices.HardDisks;
using DoorsOS.Devices.MemoryManagementUnits;
using DoorsOS.Devices.ProcessManagers;
using DoorsOS.OS.Constants;
using DoorsOS.Paginators;
using DoorsOS.RealMachines.Handlers;
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
        private readonly IChannelingDevice _channelingDevice;
        private readonly IInterruptHandler _interruptHandler;
        private readonly IProcessManager _processManager;

        public RealMachine()
        {
            _processor = new Processor();
            _ram = new Ram();
            _hardDisk = new HardDisk();
            _processManager = new ProcessManager();
            _paginator = new Paginator(_ram, _processor);
            _memoryManagementUnit = new MemoryManagementUnit(_processor, _ram);
            _channelingDevice = new ChannelingDevice(_ram, _processor);

            _interruptHandler = new InterruptHandler(_processor, _processManager, _channelingDevice);
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
                            while (!_processManager.ActiveProcess.IsFinished)
                            {
                                try
                                {
                                    if (_interruptHandler.HasInterrupted())
                                    {
                                        _interruptHandler.HandleInterrupt();
                                        if (_processor.Si == InterruptConstants.SiExec)
                                        {
                                            StartVirtualMachine();
                                            _processor.Si = InterruptConstants.SiReset;
                                            _processManager.ActiveProcess.Resume();
                                        }
                                    }
                                    else
                                    {
                                        _processManager.ActiveProcess.ExecuteInstruction();
                                        //Console.WriteLine(_processor.Ti);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    _processor.Pi = InterruptConstants.PiBadAssignment;
                                }
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
            _channelingDevice.ST = ChannelingDeviceConstants.FromHardDisk.ToCharArray();
            _channelingDevice.DT = ChannelingDeviceConstants.ToSupervizoryMemory.ToCharArray();
            _channelingDevice.CNT = 0;

            _channelingDevice.Exchange(nameToFind);
            StartVirtualMachine();
            _processManager.StartReadyProcess();
        }

        private void StartVirtualMachine()
        {
            _paginator.GetPages();
            MoveFromSupervizorMemoryToDedicatedPages();
            var newProcess = new VirtualMachine(_processor, _memoryManagementUnit, _channelingDevice);
            _processor.Ic = new char[] { '0', '0' };
            newProcess.SaveState();
            _processManager.ReadyProcesses.Add(newProcess);
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
