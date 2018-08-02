namespace netduinoMaster
{
    public class SMaster : IMaster
    {
        private SFunctionArray function = new SFunctionArray { };
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

        public SMaster(SFunctionArray function, string receive, string transmit)
        {
            // Clone main data and after resize it
            Function = function;

            //for (int index = 0; index < function.Length; index++)
            //    Function[index].Fill(function[index]);

            Receive = receive;
            Transmit = transmit;
        }

        #endregion
    }
}
