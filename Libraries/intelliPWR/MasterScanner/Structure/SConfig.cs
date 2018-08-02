namespace intelliPWR.MasterScanner
{
    public class SConfig : IConfig
    {
        private ushort clockSpeed = 0;
        public ushort ClockSpeed
        {
            get
            {
                return clockSpeed;
            }

            set
            {
                clockSpeed = value;
            }
        }

        private ushort retryCount = 0;
        public ushort RetryCount
        {
            get
            {
                return retryCount;
            }

            set
            {
                retryCount = value;
            }
        }

        private ushort timeout = 0;
        public ushort Timeout
        {
            get
            {
                return timeout;
            }

            set
            {
                timeout = value;
            }
        }

        #region Constructor

        public SConfig() { }

        public SConfig(ushort clockSpeed, ushort retryCount, ushort timeout)
        {
            // Best case. When we arrive there, that is mean all control is ok
            // And we can start to encoding operation now
            ClockSpeed = clockSpeed;
            RetryCount = retryCount;
            Timeout = timeout;
        }

        #endregion
    }
}
