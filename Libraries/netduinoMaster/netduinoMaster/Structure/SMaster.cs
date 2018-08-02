namespace netduinoMaster
{
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
}
