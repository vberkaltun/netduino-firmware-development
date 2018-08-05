namespace intelliPWR.MasterScanner
{
    public class SSlave : Constant, ISlave
    {
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

        private bool[] connectedSlavesArray = new bool[DEFAULT_DEVICE_MAX];
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

        private byte connectedSlavesCount = DEFAULT_DEVICE_MIN;
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

        #region Constructor

        public SSlave() { }

        public SSlave(byte startAddress, byte stopAddress)
        {
            // Best case. When we arrive there, that is mean all control is ok
            // And we can start to encoding operation now
            if (CheckRange(startAddress, stopAddress))
            {
                StartAddress = startAddress;
                StopAddress = stopAddress;
            }
            else
                ResetRange();
        }

        #endregion

        #region Function

        public bool CheckRange(byte startAddress, byte stopAddress)
        {
            if (startAddress > stopAddress)
                return false;

            if (startAddress < DEFAULT_START_ADDRESS)
                return false;

            if (stopAddress > DEFAULT_STOP_ADDRESS)
                return false;

            return true;
        }

        public void CleanRange(byte startAddress, byte stopAddress)
        {
            for (byte address = startAddress; address <= stopAddress; address++)
                ConnectedSlavesArray[address] = false;
        }

        public bool SetRange(byte startAddress, byte stopAddress)
        {
            bool setRangeFlag = false;

            if (CheckRange(startAddress, stopAddress))
            {
                if (startAddress > StartAddress)
                    CleanRange(StartAddress, (byte)(startAddress - 1));

                if (stopAddress < StopAddress)
                    CleanRange((byte)(stopAddress + 1), StopAddress);

                StartAddress = startAddress;
                StopAddress = stopAddress;
                setRangeFlag = true;
            }

            return setRangeFlag;
        }

        public void ResetRange()
        {
            SetRange(DEFAULT_START_ADDRESS, DEFAULT_STOP_ADDRESS);
        }

        public bool IsConnected(byte address)
        {
            return ConnectedSlavesArray[address];
        }

        #endregion
    }
}
