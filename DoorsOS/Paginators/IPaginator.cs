namespace DoorsOS.Paginators
{
    public interface IPaginator
    {
        public void GetPages();

        public int FindUnusedPage();
    }
}
