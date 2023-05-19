namespace DoorsOS.VirtualMachines
{
    public interface IVirtualMachine
    {
        bool IsActive { get; set; }
        bool IsFinished { get; set; }
        bool IsStopped { get; set; }
        void ExecuteInstruction();
        (string instruction, int block, int index) GetInstruction();
        void SaveState();

    }
}
