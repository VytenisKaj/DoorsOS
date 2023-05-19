using DoorsOS.Devices.ProcessManagers;
using DoorsOS.OS.Constants;
using DoorsOS.RealMachines.Processors;

namespace DoorsOS.RealMachines.Handlers
{
    public class InterruptHandler : IInterruptHandler
    {
        private readonly IProcessor _processor;
        private readonly IProcessManager _processManager;

        public InterruptHandler(IProcessor processor, IProcessManager processManager)
        {
            _processor = processor;
            _processManager = processManager;
        }
        public void HandleInterrupt()
        {
            if (_processor.Pi != InterruptConstants.PiReset)
            {
                HandlePiInterrupt();
            }

            if (_processor.Si != InterruptConstants.SiReset)
            {
                HandleSiInterrupt();
            }

            if (_processor.Ti == InterruptConstants.TiInterrupt)
            {
                HandleTiInterrupt();
            }
        }

        private void HandleSiInterrupt()
        {
            switch (_processor.Si)
            {
                case InterruptConstants.SiRdin:
                    break;
                case InterruptConstants.SiPtin:
                    break;
                case InterruptConstants.SiRdch:
                    break;
                case InterruptConstants.SiPtch:
                    break;
                case InterruptConstants.SiHalt:
                    HandleHaltInterrupt();
                    break;
                case InterruptConstants.SiExec:
                    break;
            }
        }

        private void HandleHaltInterrupt()
        {
            _processManager.StopActiveProcess();
        }

        private void HandlePiInterrupt()
        {
            switch (_processor.Pi)
            {
                case InterruptConstants.PiBadAdress:
                    break;
                case InterruptConstants.PiBadOpCode:
                    HandleBadOpInterrupt();
                    break;
                case InterruptConstants.PiBadAssignment:
                    break;
                case InterruptConstants.PiOverflow:
                    break;
            }
        }

        private void HandleBadOpInterrupt()
        {
            (string instruction, int block, int index) = _processManager.ActiveProcess().GetInstruction();
            Console.WriteLine($"Bad OP code: {instruction}"); // move to channelling device, in the future include process name, block and index
            _processManager.StopActiveProcess();
        }

        private void HandleTiInterrupt()
        {
            // For future, pick and set new active process, for now, just reset
            _processor.Ti = InterruptConstants.TiReset;
        }

        public bool HasInterrupted()
        {
            var siValue = _processor.FromHexAsCharArrayToInt(new char[] { _processor.Si });
            var piValue = _processor.FromHexAsCharArrayToInt(new char[] { _processor.Pi });
            var tiValue = _processor.FromHexAsCharArrayToInt(new char[] { _processor.Ti });

            return (siValue + piValue) > 0 || tiValue == 0;
        }
    }
}
