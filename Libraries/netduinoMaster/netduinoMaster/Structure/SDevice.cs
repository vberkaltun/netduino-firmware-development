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

        private SFunction[] function = new SFunction[] { };
        public SFunction[] Function
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

        private char address = '\0';
        public char Address
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

        public SDevice(SVendor vendor, SFunction[] function, EHandshake handshake, char address)
        {
            Vendor.Brand = vendor.Brand;
            Vendor.Model = vendor.Model;
            Vendor.Version = vendor.Version;

            // Clone main data and after resize it
            Function = new SFunction[function.Length];

            for (int index = 0; index < function.Length; index++)
                EFunction.Fill(ref Function[index], ref function[index]);

            Handshake = handshake;
            Address = address;
        }

        #endregion
    };
}
