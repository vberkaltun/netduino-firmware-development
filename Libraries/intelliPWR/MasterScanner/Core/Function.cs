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
        /// <param name="startAddress">The start address of I2C scanner bus.</param>
        /// <param name="stopAddress">The stop address of I2C scanner bus.</param>
        /// <param name="clockSpeed">The clock speed of master scanner lib.</param>
        /// <param name="timeout">Time delay after an one clock hertz.</param>
        /// <param name="retryCount">Retry count for worst case operations.</param>
        protected void Initialize(byte startAddress, byte stopAddress, ushort clockSpeed, ushort timeout, ushort retryCount)
        {
            Configuration = new I2CDevice.Configuration(0, clockSpeed);
            Device = new I2CDevice(Configuration);

            Slave = new SSlave();
            Config = new SConfig();

            if (CheckRange(startAddress, stopAddress))
            {
                Slave.StartAddress = startAddress;
                Slave.StopAddress = stopAddress;
            }
            else
                ResetRange();

            Slave.ConnectedSlavesArray = new bool[DEFAULT_ARRAY_SIZE];
            Slave.ConnectedSlavesCount = DEFAULT_ARRAY_COUNT;

            Config.ClockSpeed = clockSpeed;
            Config.RetryCount = retryCount;
            Config.Timeout = timeout;
        }

        /// <summary>
        /// Check if user did not register an event delegaterary.
        /// </summary>
        /// <param name="array">An array of connected device list.</param>
        /// <param name="count">Size of connected device.</param>
        protected void OnTriggeredConnected(string data)
        {
            if (data != null)
            {
                // We are passing here our data as a string type data but that 
                // Is not acceptable for end-user. So we will trim it with space
                // And after that will store this data in an string array
                string[] connectedOutput = data.Trim().Split(' ');

                // don't bother if user hasn't registered a callback
                if (OnConnected == null)
                    return;

                OnConnected(connectedOutput, (byte)connectedOutput.Length);

                //Debug.Print("=== OnConnected");

                //for (int i = 0; i < connectedOutput.Length; i++)
                //    Debug.Print(connectedOutput[i]);
            }
        }

        /// <summary>
        /// Check if user did not register an event delegaterary.
        /// </summary>
        /// <param name="array">An array of disconnected device list.</param>
        /// <param name="count">Size of disconnected device.</param>
        protected void OnTriggeredDisconnected(string data)
        {
            if (data != null)
            {
                // We are passing here our data as a string type data but that 
                // Is not acceptable for end-user. So we will trim it with space
                // And after that will store this data in an string array
                string[] disconnectedOutput = data.Trim().Split(' ');

                // don't bother if user hasn't registered a callback
                if (OnDisconnected == null)
                    return;

                OnDisconnected(disconnectedOutput, (byte)disconnectedOutput.Length);

                //Debug.Print("=== OnDisconnected");

                //for (int i = 0; i < disconnectedOutput.Length; i++)
                //    Debug.Print(disconnectedOutput[i]);
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
                Slave.ConnectedSlavesArray[address] = false;
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
                if (startAddress > Slave.StartAddress)
                    CleanRange(Slave.StartAddress, (byte)(startAddress - 1));

                if (stopAddress < Slave.StopAddress)
                    CleanRange((byte)(stopAddress + 1), Slave.StopAddress);

                Slave.StartAddress = startAddress;
                Slave.StopAddress = stopAddress;
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
            return Slave.ConnectedSlavesArray[address];
        }

        /// <summary>
        /// Scan slaves, must be static cause of thread process.
        /// </summary>
        public void ScanSlaves()
        {
            // That is looking worst but it is very easy solution for up-to-date
            // Scanning process. In scanning step, we will detect last changes
            // On I2C bus and will put last changed to here as connected or not.
            // After that, we will trim and split it with space delimiter.
            // So, we can generate up-to-date output I2C data's
            string currentConnectedSlavesArray = null;
            string currentDisconnectedSlavesArray = null;

            // Start to scanning slave device on I2C bus
            for (byte address = Slave.StartAddress; address < Slave.StopAddress; address++)
            {
                // Reinitialize config data of an I2C device
                Device.Config = new I2CDevice.Configuration(address, Config.ClockSpeed);
                byte[] handshake = new byte[] { address };

                try
                {
                    // Generate a transaction for testing an connected device
                    I2CDevice.I2CTransaction[] transaction = { I2CDevice.CreateWriteTransaction(handshake) };
                    ushort retryCount = 0;

                    // When retry is overflowed, abort it to worst case
                    while (Device.Execute(transaction, Config.Timeout) != handshake.Length)
                        if (retryCount++ >= Config.RetryCount)
                            throw new Exception();

                    // Check that is it connected as before or not
                    if (Slave.ConnectedSlavesArray[address] == false)
                    {
                        currentConnectedSlavesArray += " " + address.ToString();
                        Slave.ConnectedSlavesArray[address] = true;
                    }
                }
                catch (Exception)
                {
                    // Check that is it disconnected as before or not
                    if (Slave.ConnectedSlavesArray[address] == true)
                    {
                        currentDisconnectedSlavesArray += " " + address.ToString();
                        Slave.ConnectedSlavesArray[address] = false;
                    }
                }
            }

            // Notify end user with delegate method
            OnTriggeredConnected(currentConnectedSlavesArray);
            OnTriggeredDisconnected(currentDisconnectedSlavesArray);

            // Issue 53 - Recommendation by NevynUK. At the here, we are 
            // Disposing our device for next process and reinitializing it. When
            // We choose to not to do that all process about scanning will stop 
            // In next step. We experienced this situation at before
            Device.Dispose();
            Configuration = new I2CDevice.Configuration(0, Config.ClockSpeed);
            Device = new I2CDevice(Configuration);

            //Debug.EnableGCMessages(true);
            //Debug.Print(Debug.GC(false).ToString());
        }

        #endregion
    }
}
