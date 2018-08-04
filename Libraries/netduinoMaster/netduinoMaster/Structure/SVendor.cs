namespace netduinoMaster
{
    public class SVendor : IVendor
    {
        private string brand = null;
        public string Brand
        {
            get
            {
                return brand;
            }
        }

        private string model = null;
        public string Model
        {
            get
            {
                return model;
            }
        }

        private string version = null;
        public string Version
        {
            get
            {
                return version;
            }
        }

        #region Constructor

        public SVendor() { }

        public SVendor(string brand, string model, string version)
        {
            this.brand = brand;
            this.model = model;
            this.version = version;
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
}
