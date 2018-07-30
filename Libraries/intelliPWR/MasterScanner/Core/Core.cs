using Microsoft.SPOT.Hardware;

namespace intelliPWR.MasterScanner
{
    public class Core : IConfig, ISlave
    {
        #region Constant

        protected const byte DEFAULT_START_ADDRESS = 0x03;
        protected const byte DEFAULT_STOP_ADDRESS = 0x77;
        protected const byte DEFAULT_ARRAY_SIZE = 0x7F;
        protected const byte DEFAULT_ARRAY_COUNT = 0x00;

        protected const ushort DEFAULT_DEVICE_CLOCK = 100;
        protected const ushort DEFAULT_DEVICE_TIMEOUT = 10;
        protected const ushort DEFAULT_DEVICE_RETRY = 1;

        #endregion

        #region Variable

        protected static I2CDevice.Configuration Configuration;
        protected static I2CDevice Device;

        public ConnectedEventHandler OnConnected;
        public DisconnectedEventHandler OnDisconnected;

        #endregion

        #region Encapsulation

        private byte startAddress = DEFAULT_START_ADDRESS;
        public byte StartAddress
        {
            get
            {
                return startAddress;
            }

            set
            {
                startAddress = value;
            }
        }

        private byte stopAddress = DEFAULT_STOP_ADDRESS;
        public byte StopAddress
        {
            get
            {
                return stopAddress;
            }

            set
            {
                stopAddress = value;
            }
        }

        private bool[] connectedSlavesArray = new bool[DEFAULT_ARRAY_SIZE];
        public bool[] ConnectedSlavesArray
        {
            get
            {
                return connectedSlavesArray;
            }

            set
            {
                connectedSlavesArray = value;
            }
        }

        private byte connectedSlavesCount = DEFAULT_ARRAY_COUNT;
        public byte ConnectedSlavesCount
        {
            get
            {
                return connectedSlavesCount;
            }

            set
            {
                connectedSlavesCount = value;
            }
        }

        // -----

        private ushort clockSpeed = DEFAULT_DEVICE_CLOCK;
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

        private ushort retryCount = DEFAULT_DEVICE_RETRY;
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

        private ushort timeout = DEFAULT_DEVICE_TIMEOUT;
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

        #endregion

        #region Delegate

        /// <summary>
        /// Delegate that defines event handler for connected.
        /// </summary>
        /// <param name="array">An array of connected device list.</param>
        /// <param name="count">Size of connected device.</param>
        public delegate void ConnectedEventHandler(string[] array, byte count);

        /// <summary>
        /// Delegate that defines event handler for disconnected.
        /// </summary>
        /// <param name="array">An array of disconnected device list.</param>
        /// <param name="count">Size of disconnected device.</param>
        public delegate void DisconnectedEventHandler(string[] array, byte count);

        #endregion
    }
}
