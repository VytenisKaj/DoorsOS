using DoorsOS.VirtualMachines;

namespace DoorsOS.Devices.ProcessManagers
{
    public class ProcessManager : IProcessManager
    {
        public List<IVirtualMachine> Processes { get; } = new();

        public IVirtualMachine ActiveProcess()
        {
            return Processes.Where(p => p.IsActive).Single();
        }

        public void StopActiveProcess()
        {
            var activeProcess = ActiveProcess();
            activeProcess.IsActive = false;
            activeProcess.IsFinished = true;
        }
    }
}
