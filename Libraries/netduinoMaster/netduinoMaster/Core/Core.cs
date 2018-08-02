namespace netduinoMaster
{
    #region Class

    public class SVendor : IVendor
    {
        private string brand = null;
        public string Brand
        {
            get
            {
                return brand;
            }

            set
            {
                brand = value;
            }
        }

        private string model = null;
        public string Model
        {
            get
            {
                return model;
            }

            set
            {
                model = value;
            }
        }

        private string version = null;
        public string Version
        {
            get
            {
                return version;
            }

            set
            {
                version = value;
            }
        }

        #region Constructor

        public SVendor() { }

        public SVendor(string brand, string model,string version)
        {
            Brand = brand;
            Model = model;
            Version = version;
        }

        #endregion

        #region Overload operator

        // Overload == operator
        public static bool operator ==(SVendor source, SVendor target)
        {
            if (source.Brand != target.Brand)
                return false;

            if (source.Model != target.Model)
                return false;

            if (source.Version != target.Version)
                return false;

            return true;
        }

        // Overload != operator
        public static bool operator !=(SVendor source, SVendor target)
        {
            if (source.Brand != target.Brand)
                return true;

            if (source.Model != target.Model)
                return true;

            if (source.Version != target.Version)
                return true;

            return false;
        }

        #endregion
    };

    public class SFunction : IFunction
    {
        private string name = null;
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        private bool request = false;
        public bool Request
        {
            get
            {
                return request;
            }

            set
            {
                request = value;
            }
        }

        private ushort listen = 0;
        public ushort Listen
        {
            get
            {
                return listen;
            }

            set
            {
                listen = value;
            }
        }

        #region Constructor

        public SFunction() { }

        public SFunction(string name, bool request,ushort listen)
        {
            Name = name;
            Request = request;
            Listen = listen;
        }

        #endregion

        #region Overload operator

        // Overload == operator
        public static bool operator ==(SFunction source, SFunction target)
        {
            if (source.Name != target.Name)
                return false;

            if (source.Request != target.Request)
                return false;

            if (source.Listen != target.Listen)
                return false;

            return true;
        }

        // Overload != operator
        public static bool operator !=(SFunction source, SFunction target)
        {
            if (source.Name != target.Name)
                return true;

            if (source.Request != target.Request)
                return true;

            if (source.Listen != target.Listen)
                return true;

            return false;
        }

        #endregion
    };

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

    public class SMaster : IMaster
    {
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

        private string receive = null;
        public string Receive
        {
            get
            {
                return receive;
            }

            set
            {
                receive = value;
            }
        }

        private string transmit = null;
        public string Transmit
        {
            get
            {
                return transmit;
            }

            set
            {
                transmit = value;
            }
        }

        #region Constructor

        public SMaster() { }

        public SMaster(SFunction[] function, string receive, string transmit)
        {
            // Clone main data and after resize it
            Function = new SFunction[function.Length];

            for (int index = 0; index < function.Length; index++)
                EFunction.Fill(ref Function[index], ref function[index]);

            Receive = receive;
            Transmit = transmit;
        }

        #endregion
    }

    #endregion

    #region Enumeration

    public enum EHandshake { Unknown, Ready };
    public enum ECommunication { Idle, Continue, End };
    public enum ENotify { Offline, Online, Unconfirmed, Confirmed };

    #endregion
}
