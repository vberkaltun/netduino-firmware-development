using intelliPWR.MasterScanner.Constant;
using intelliPWR.MasterScanner.Interface;

namespace intelliPWR.MasterScanner.Structure
{
    public class SConfig : CConfig
    {
        #region Variable

        public ushort ClockSpeed = DEFAULT_DEVICE_CLOCK;
        public ushort RetryCount = DEFAULT_DEVICE_RETRY;
        public ushort Timeout = DEFAULT_DEVICE_TIMEOUT;

        #endregion

        #region Constructor

        public SConfig(ushort clockSpeed, ushort timeout, ushort retryCount)
        {
            ClockSpeed = clockSpeed;
            RetryCount = retryCount;
            Timeout = timeout;
        }

        #endregion
    }
}
