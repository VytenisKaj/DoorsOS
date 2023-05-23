using DoorsOS.VirtualMachines;

namespace DoorsOS.Devices.ProcessManagers
{
    public interface IProcessManager
    {
        List<IVirtualMachine> Processes { get; }
        IVirtualMachine ActiveProcess();
        void StopActiveProcess();
    }
}
