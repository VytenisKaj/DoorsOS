﻿using DoorsOS.Devices.Channeling;
using DoorsOS.Devices.ProcessManagers;
using DoorsOS.OS.Constants;
using DoorsOS.RealMachines.Processors;

namespace DoorsOS.RealMachines.Handlers
{
    public class InterruptHandler : IInterruptHandler
    {
        private readonly IProcessor _processor;
        private readonly IProcessManager _processManager;
        private readonly IChannelingDevice _chanellingDevice;

        public InterruptHandler(IProcessor processor, IProcessManager processManager, IChannelingDevice channelingDevice)
        {
            _processor = processor;
            _processManager = processManager;
            _chanellingDevice = channelingDevice;
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
                    HandleRdinInterrupt();
                    break;
                case InterruptConstants.SiPtin:
                    HandlePtinInterrupt();
                    break;
                case InterruptConstants.SiRdch:
                    HandleRdchInterrupt();
                    break;
                case InterruptConstants.SiPtch:
                    HandlePtchInterrupt();
                    break;
                case InterruptConstants.SiHalt:
                    HandleHaltInterrupt();
                    break;
                case InterruptConstants.SiExec:
                    HandleExecInterrupt();
                    break;
            }
        }

        private void HandleExecInterrupt()
        {
            _chanellingDevice.Exchange(new string(_processor.R2));
        }

        private void HandlePtchInterrupt()
        {
            _chanellingDevice.CNT = _processor.FromHexAsCharArrayToInt(_processor.R2);
            _chanellingDevice.Exchange();
            _processor.Si = InterruptConstants.SiReset;
        }

        private void HandleRdchInterrupt()
        {
            _chanellingDevice.CNT = _processor.FromHexAsCharArrayToInt(_processor.R2);
            _chanellingDevice.Exchange();
            _processor.Si = InterruptConstants.SiReset;
        }

        private void HandlePtinInterrupt()
        {
            _chanellingDevice.CNT = 4;
            _chanellingDevice.Exchange();
            _processor.Si = InterruptConstants.SiReset;
        }

        private void HandleRdinInterrupt()
        {
            _chanellingDevice.CNT = 4;
            _chanellingDevice.Exchange();
            _processor.Si = InterruptConstants.SiReset;
        }

        private void HandleHaltInterrupt()
        {
            _processManager.StopActiveProcess();
            _processManager.ReadyProcesses.RemoveAll(p => p.IsFinished);
            _processManager.StartReadyProcess();
            _processor.Si = InterruptConstants.SiReset;
        }

        private void HandlePiInterrupt()
        {
            switch (_processor.Pi)
            {
                case InterruptConstants.PiBadAdress:
                    HandleBadAdressInterrupt();
                    break;
                case InterruptConstants.PiBadOpCode:
                    HandleBadOpInterrupt();
                    break;
                case InterruptConstants.PiBadAssignment:
                    HandleBadAssignmentInterrupt();
                    break;
            }
        }

        private void HandleBadAssignmentInterrupt()
        {
            _processManager.StopActiveProcess();
            _processManager.ReadyProcesses.RemoveAll(p => p.IsFinished);
            _processManager.StartReadyProcess();
        }

        private void HandleBadAdressInterrupt()
        {
            _processManager.StopActiveProcess();
            _processManager.ReadyProcesses.RemoveAll(p => p.IsFinished);
            _processManager.StartReadyProcess();
        }

        private void HandleBadOpInterrupt()
        {
            (string instruction, int block, int index) = _processManager.ActiveProcess.GetInstruction();
            Console.WriteLine($"Bad OP code: {instruction}"); // move to channelling device, in the future include process name, block and index
            _processManager.StopActiveProcess();
            _processManager.ReadyProcesses.RemoveAll(p => p.IsFinished);
            _processManager.StartReadyProcess();
        }

        private void HandleTiInterrupt()
        {
            /*_processManager.StopActiveProcess();
            _processManager.ReadyProcesses.RemoveAll(p => p.IsFinished);
            _processManager.StartReadyProcess();*/
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
