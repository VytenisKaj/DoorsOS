using DoorsOS.RealMachines;

namespace DoorsOS
{
    public class Program
    {
        private readonly IRealMachine _operatingSystem = new RealMachine();
        static void Main()
        {
            new Program().Run();
        }

        private void Run()
        {
            _operatingSystem.Run();
        }
    }
}