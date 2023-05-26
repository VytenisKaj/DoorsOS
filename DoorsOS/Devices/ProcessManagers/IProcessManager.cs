using DoorsOS.VirtualMachines;

namespace DoorsOS.Devices.ProcessManagers
{
    public interface IProcessManager
    {
        List<IVirtualMachine> ReadyProcesses { get; }
        List<IVirtualMachine> BlockedProcesses { get; }
        List<IVirtualMachine> StoppedProcesses { get; }
        public IVirtualMachine ActiveProcess { get; }
        void StopActiveProcess();
        public void StartReadyProcess();
    }
}
