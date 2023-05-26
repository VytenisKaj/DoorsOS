namespace DoorsOS.Devices.Channeling
{
    public interface IChannelingDevice
    {
        char[] SB_block { get; set; }
        char[] SB_index { get; set; }
        char[] DB_block { get; set; }
        char[] DB_index { get; set; }
        char[] ST { get; set; }
        char[] DT { get; set; }
        int CNT { get; set; }

        void Exchange(string nameToFind);
        void Exchange();
    }
}
