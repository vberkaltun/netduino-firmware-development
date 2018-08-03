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
            Vendor.Brand = vendor.Brand;
            Vendor.Model = vendor.Model;
            Vendor.Version = vendor.Version;

            // Clone main data and after resize it
            Function = function;

            //for (int index = 0; index < function.Length; index++)
            //    Function[index].Fill(function[index]);

            Handshake = handshake;
            Address = address;
        }

        #endregion
    };
}
