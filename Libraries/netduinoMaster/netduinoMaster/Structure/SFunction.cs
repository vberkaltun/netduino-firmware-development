namespace netduinoMaster
{
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

        public SFunction(string name, bool request, ushort listen)
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
}
