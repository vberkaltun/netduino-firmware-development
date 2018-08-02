using intelliPWR.MasterScanner.Constant;
using intelliPWR.MasterScanner.Interface;

namespace intelliPWR.MasterScanner.Structure
{
    public class SSlave : CSlave, ISlave
    {
        #region Variable

        public byte StartAddress = DEFAULT_START_ADDRESS;
        public byte StopAddress = DEFAULT_STOP_ADDRESS;
        public bool[] ConnectedSlavesArray = DEFAULT_ARRAY_SIZE;
        public byte ConnectedSlavesCount = DEFAULT_ARRAY_COUNT;

        #endregion

        #region Constructor

        public SSlave(byte startAddress, byte stopAddress)
        {
            if (CheckRange(startAddress, stopAddress))
            {
                StartAddress = startAddress;
                StopAddress = stopAddress;
            }
            else
                ResetRange();

            ConnectedSlavesArray = new bool[DEFAULT_ARRAY_COUNT];
            ConnectedSlavesCount = DEFAULT_ARRAY_COUNT;
        }

        #endregion

        #region Function

        /// <summary>
        /// Check that is given range is correct or not.
        /// </summary>
        /// <param name="startAddress">A given start address of range.</param>
        /// <param name="stopAddress">A given stop address of range.</param>
        /// <returns></returns>
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

        /// <summary>
        /// When default addresses range were changed, clear last stored address data. 
        /// </summary>
        /// <param name="startAddress">Start address of range.</param>
        /// <param name="stopAddress">Stop address of range.</param>
        public void CleanRange(byte startAddress, byte stopAddress)
        {
            for (byte address = startAddress; address <= stopAddress; address++)
                ConnectedSlavesArray[address] = false;
        }

        /// <summary>
        /// Changes default scanning range with new range.
        /// </summary>
        /// <param name="startAddress">Start and stop address of range.</param>
        /// <param name="stopAddress">Stop and stop address of range.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Resets current scanning range with default range.
        /// </summary>
        public void ResetRange()
        {
            SetRange(DEFAULT_START_ADDRESS, DEFAULT_STOP_ADDRESS);
        }

        /// <summary>
        /// Checks if specified address is online on bus.
        /// </summary>
        /// <param name="address">Address of specified device.</param>
        /// <returns>Connected or not Connected.</returns>
        public bool IsConnected(byte address)
        {
            return ConnectedSlavesArray[address];
        }

        #endregion
    }
}
