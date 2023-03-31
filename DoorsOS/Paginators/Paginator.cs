using DoorsOS.OS.Constants;
using DoorsOS.RealMachines.Memories;
using DoorsOS.RealMachines.Processors;

namespace DoorsOS.Paginators
{
    public class Paginator : IPaginator
    {
        private readonly IRam _ram;
        private readonly IProcessor _processor;

        public Paginator(IRam ram, IProcessor processor)
        {
            _ram = ram;
            _processor = processor;
        }

        public int FindUnusedPage()
        {
            return Array.IndexOf(_ram.IsBlockUsed, false);
        }

        public void GetPages()
        {
            SetPtr();
            for (int i = 0; i < OsConstants.VirtualMachineBlocksCount; i++)
            {
                UsePage(i, FindUnusedPage());
            }
        }

        private void SetPtr()
        {
            int paginatorPage = FindUnusedPage();
            _processor.Ptr = _processor.FromIntToHexNumber(paginatorPage);
            _ram.IsBlockUsed[paginatorPage] = true;
        }

        private void UsePage(int ptrIndex, int pageNumber)
        {
            _ram.SetMemoryNumber(_processor.FromHexAsCharArrayToInt(_processor.Ptr), ptrIndex, pageNumber);
            _ram.IsBlockUsed[pageNumber] = true;
        }
    }
}
