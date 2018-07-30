using Microsoft.SPOT.Hardware;
using System;

namespace intelliPWR.MasterScanner
{
    public class Function : Core
    {
        #region Protected

        /// <summary>
        /// Default constructor initializer.
        /// </summary>
        protected void Initialize()
        {
            Configuration = new I2CDevice.Configuration(DEFAULT_START_ADDRESS, DEFAULT_DEVICE_CLOCK);
            Device = new I2CDevice(Configuration);
        }

        /// <summary>
        /// Check if user did not register an event delegaterary.
        /// </summary>
        /// <param name="array">An array of connected device list.</param>
        /// <param name="count">Size of connected device.</param>
        protected void OnTriggeredConnected(string[] array, byte count)
        {

            if (count != 0)
            {
                // don't bother if user hasn't registered a callback
                if (OnConnected == null)
                    return;

                OnConnected(array, count);
            }
        }

        /// <summary>
        /// Check if user did not register an event delegaterary.
        /// </summary>
        /// <param name="array">An array of disconnected device list.</param>
        /// <param name="count">Size of disconnected device.</param>
        protected void OnTriggeredDisconnected(string[] array, byte count)
        {
            if (count != 0)
            {
                // don't bother if user hasn't registered a callback
                if (OnDisconnected == null)
                    return;

                OnDisconnected(array, count);
            }
        }

        /// <summary>
        /// Check that is given range is correct or not.
        /// </summary>
        /// <param name="startAddress">A given start address of range.</param>
        /// <param name="stopAddress">A given stop address of range.</param>
        /// <returns></returns>
        protected bool CheckRange(byte startAddress, byte stopAddress)
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
        protected void CleanRange(byte startAddress, byte stopAddress)
        {
            for (byte address = startAddress; address <= stopAddress; address++)
                ConnectedSlavesArray[address] = false;
        }

        #endregion

        #region Public

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

        /// <summary>
        /// Scan slaves, must be static cause of thread process.
        /// </summary>
        public void ScanSlaves()
        {
            string currentConnectedSlavesArray = null;
            string currentDisconnectedSlavesArray = null;

            // Loop till related thread is not suspended
            while (true)
            {
                // Start to scanning slave device on I2C bus
                for (byte address = StartAddress; address < StopAddress; address++)
                {
                    // Reinitialize config data of an I2C device
                    Device.Config = new I2CDevice.Configuration(address, ClockSpeed);
                    byte[] handshake = new byte[] { address };

                    try
                    {
                        // Generate a transaction for testing an connected device
                        I2CDevice.I2CTransaction[] transaction = { I2CDevice.CreateWriteTransaction(handshake) };
                        ushort retryCount = 0;

                        // When retry is overflowed, abort it to worst case
                        while (Device.Execute(transaction, Timeout) != handshake.Length)
                            if (retryCount++ > RetryCount)
                                throw new Exception();

                        currentConnectedSlavesArray += " " + address.ToString();
                    }
                    catch (Exception)
                    {
                        currentDisconnectedSlavesArray += " " + address.ToString();
                    }
                }

                string[] connectedOutput = currentConnectedSlavesArray.Trim().Split(' ');
                OnConnected(connectedOutput, (byte)connectedOutput.Length);

                string[] disconnectedOutput = currentDisconnectedSlavesArray.Trim().Split(' ');
                OnDisconnected(disconnectedOutput, (byte)disconnectedOutput.Length);
            }
        }

        #endregion
    }
}
