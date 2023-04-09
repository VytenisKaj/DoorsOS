namespace DoorsOS.VirtualMachines
{
    public interface IVirtualMachine
    {
        public bool IsActive { get; set; }
        public bool IsFinished { get; set; }
        void ExecuteInstruction();

    }
}
