/*
 * I2C SCANNER - 20.07.2018
 * 
 * =============================================================================
 *
 * The MIT License (MIT)
 * 
 * Copyright (c) 2018 Berk Altun - vberkaltun.com
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sub license, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 *
 * =============================================================================
 */

namespace intelliPWR.MasterScanner
{
    public class MasterScanner : Function, IMasterScanner
    {
        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MasterScanner()
        {
            // Initialize for first run
            Initialize();
        }

        /// <summary>
        /// Default constructor with start and stop parameters.
        /// </summary>
        /// <param name="startAddress">The start address of I2C scanner bus.</param>
        /// <param name="stopAddress">The stop address of I2C scanner bus.</param>
        public MasterScanner(byte startAddress, byte stopAddress)
        {
            StartAddress = startAddress;
            StopAddress = stopAddress;

            // Initialize for first run
            Initialize();
        }

        /// <summary>
        /// Default constructor with clockSpeed and timeout parameters.
        /// </summary>
        /// <param name="clockSpeed">The clock speed of master scanner lib.</param>
        /// <param name="timeout">Time delay after an one clock hertz.</param>
        public MasterScanner(ushort clockSpeed, ushort timeout)
        {
            ClockSpeed = clockSpeed;
            Timeout = timeout;

            // Initialize for first run
            Initialize();
        }

        /// <summary>
        /// Default constructor with clockSpeed, timeout and retryCount parameters.
        /// </summary>
        /// <param name="clockSpeed">The clock speed of master scanner lib.</param>
        /// <param name="timeout">Time delay after an one clock hertz.</param>
        /// <param name="retryCount">Retry count for worst case operations.</param>
        public MasterScanner(ushort clockSpeed, ushort timeout, ushort retryCount)
        {
            ClockSpeed = clockSpeed;
            Timeout = timeout;
            RetryCount = retryCount;

            // Initialize for first run
            Initialize();
        }

        /// <summary>
        /// Default constructor with startAddress, stopAddress, clockSpeed, timeout and retryCount parameters.
        /// </summary>
        /// <param name="startAddress">The start address of I2C scanner bus.</param>
        /// <param name="stopAddress">The stop address of I2C scanner bus.</param>
        /// <param name="clockSpeed">The clock speed of master scanner lib.</param>
        /// <param name="timeout">Time delay after an one clock hertz.</param>
        /// <param name="retryCount">Retry count for worst case operations.</param>
        public MasterScanner(byte startAddress, byte stopAddress, ushort clockSpeed, ushort timeout, ushort retryCount)
        {
            StartAddress = startAddress;
            StopAddress = stopAddress;

            ClockSpeed = clockSpeed;
            Timeout = timeout;
            RetryCount = retryCount;

            // Initialize for first run
            Initialize();
        }

        #endregion
    }
}