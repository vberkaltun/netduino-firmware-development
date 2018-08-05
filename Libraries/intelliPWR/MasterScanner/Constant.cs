namespace intelliPWR.MasterScanner
{
    public class Constant
    {
        #region Constant

        protected const byte DEFAULT_START_ADDRESS = 0x03;
        protected const byte DEFAULT_STOP_ADDRESS = 0x77;

        protected const ushort DEFAULT_DEVICE_CLOCK = 100;
        protected const ushort DEFAULT_DEVICE_TIMEOUT = 100;
        protected const ushort DEFAULT_DEVICE_RETRY = 3;

        protected const byte DEFAULT_DEVICE_MAX = byte.MaxValue;
        protected const byte DEFAULT_DEVICE_MIN = byte.MinValue;

        #endregion

    }
}
