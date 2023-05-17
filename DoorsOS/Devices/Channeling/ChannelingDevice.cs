using DoorsOS.Devices.HardDisks;
using DoorsOS.RealMachines.Memories;

namespace DoorsOS.Devices.Channeling
{
    public class ChannelingDevice : IChannelingDevice
    {
        private readonly IHardDisk _hardDisk;
        private readonly IRam _ram;
        // + puslapiavimo mechanizmas

        // Registers
        // nustatomi is realios masinos ir isvedami i ekrana?

        /// <summary>
        /// Track, FROM which we will copy, number.
        /// </summary>
        public char[] SB { get; set; }
        /// <summary>
        /// Track, TO which we will copy, number.
        /// </summary>
        public char[] DB { get; set; }
        /// <summary>
        /// Object, FROM which we will copy, number.
        /// </summary>
        public char[] ST { get; set; }
        /// <summary>
        /// Object, TO which we will copy, number.
        /// </summary>
        public char[] DT { get; set; }

        // Command, which channeling device itself cannot perform (does not have processor)?
        public int XCHG { get; set; }

        public ChannelingDevice(IRam ram)
        {
            _hardDisk = new HardDisk();
            _ram = ram;
        }

        public void Exchange()
        {
            // konvertuojam abu i intus

            // st switch

            // dt switch

        }
    }
}
