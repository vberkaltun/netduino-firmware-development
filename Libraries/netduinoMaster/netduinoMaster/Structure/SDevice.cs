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

            set
            {
                vendor = value;
            }
        }

        private SFunctionArray function = new SFunctionArray();
        public SFunctionArray Function
        {
            get
            {
                return function;
            }

            set
            {
                function = value;
            }
        }

        private EHandshake handshake = EHandshake.Unknown;
        public EHandshake Handshake
        {
            get
            {
                return handshake;
            }

            set
            {
                handshake = value;
            }
        }

        private byte address = 0;
        public byte Address
        {
            get
            {
                return address;
            }

            set
            {
                address = value;
            }
        }

        #region Constructor

        public SDevice() { }

        public SDevice(SVendor vendor, SFunctionArray function, EHandshake handshake, byte address)
        {
            this.Vendor = new SVendor(vendor.Brand, vendor.Model, vendor.Version);
            this.Function = function;
            this.Handshake = handshake;
            this.Address = address;
        }

        #endregion
    };
}
