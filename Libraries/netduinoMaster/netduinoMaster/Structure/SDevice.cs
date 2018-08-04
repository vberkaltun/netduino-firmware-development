namespace netduinoMaster
{
    public class SDevice : IDevice
    {
        private SVendor vendor = new SVendor();
        public SVendor Vendor
        {
            get
            {
                return vendor;
            }
        }

        private SFunctionArray function = new SFunctionArray();
        public SFunctionArray Function
        {
            get
            {
                return function;
            }
        }

        private EHandshake handshake = EHandshake.Unknown;
        public EHandshake Handshake
        {
            get
            {
                return handshake;
            }
        }

        private byte address = 0;
        public byte Address
        {
            get
            {
                return address;
            }
        }

        #region Constructor

        public SDevice() { }

        public SDevice(SVendor vendor, SFunctionArray function, EHandshake handshake, byte address)
        {
            this.vendor = new SVendor(vendor.Brand, vendor.Model, vendor.Version);
            this.function = function;
            this.handshake = handshake;
            this.address = address;
        }

        #endregion
    };
}
