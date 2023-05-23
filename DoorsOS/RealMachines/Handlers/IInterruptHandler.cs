namespace DoorsOS.RealMachines.Handlers
{
    public interface IInterruptHandler
    {
        bool HasInterrupted();

        void HandleInterrupt();
    }
}
