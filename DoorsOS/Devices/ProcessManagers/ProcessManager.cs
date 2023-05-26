using DoorsOS.VirtualMachines;

namespace DoorsOS.Devices.ProcessManagers
{
    public class ProcessManager : IProcessManager
    {

        public List<IVirtualMachine> ReadyProcesses { get; } = new();
        public List<IVirtualMachine> BlockedProcesses { get; } = new();
        public List<IVirtualMachine> StoppedProcesses { get; } = new();

        public IVirtualMachine ActiveProcess { get; private set; } 

        public void StopActiveProcess()
        {
            var activeProcess = ActiveProcess;
            activeProcess.IsActive = false;
            activeProcess.IsFinished = true;
        }

        public void StartReadyProcess()
        {
            if (ReadyProcesses.Count != 0)
            {
                ActiveProcess = ReadyProcesses[0];
                ActiveProcess.IsActive = true;
                ActiveProcess.IsFinished = false;
                ActiveProcess.Resume();
            }
        }
    }
}
