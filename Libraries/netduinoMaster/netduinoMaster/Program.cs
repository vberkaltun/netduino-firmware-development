using System;
using System.Text;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using intelliPWR.MasterScanner;
using intelliPWR.Serializer;

namespace netduinoMaster
{
    public class Program : Constant
    {
        #region Variable

        static PWM RGBRed = new PWM(PWMChannels.PWM_PIN_D11, 100, 0, true);
        static PWM RGBGreen = new PWM(PWMChannels.PWM_PIN_D10, 100, 0, true);
        static PWM RGBBlue = new PWM(PWMChannels.PWM_PIN_D9, 100, 0, true);

        static SColorbook ColorBlack = new SColorbook(0, 0, 0, 0, 0, 0);
        static SColorbook ColorWhite = new SColorbook(255, 255, 255, 0, 0, 1);
        static SColorbook ColorBlue = new SColorbook(0, 204, 204, 180, 1, 0.8);
        static SColorbook ColorOrange = new SColorbook(255, 128, 0, 30, 1, 1);

        static MasterScanner Scanner = new MasterScanner(I2C_START_ADDRESS, I2C_STOP_ADDRESS);
        static Serializer Serializer = new Serializer();

        static Timer TimerI2C = null;
        static Timer TimerFunction = null;
        static Timer TimerWiFi = null;
        static TimerCallback Callback = null;

        static ECommunication Communication = ECommunication.Idle;
        static ENotify Notify = ENotify.Offline;

        static SDeviceArray Device = new SDeviceArray();
        static SStringArray Transmit = new SStringArray();
        static string Receive = null;

        static I2CDevice.Configuration Configuration;
        static I2CDevice I2C;

        #endregion

        #region Uncategorized

        public static void Main()
        {
            // Start Pulse-Width Modulation of led operation
            RGBRed.Start();
            RGBGreen.Start();
            RGBBlue.Start();

            // IMPORTANT NOTICE: Due to our I2C scanner lib, When a new device
            // Connected or disconnected to master, our I2C scanner lib decides
            // Which one is to be triggered
            Scanner.OnConnected = OnConnected;
            Scanner.OnDisconnected = OnDisconnected;

            // Attach functions to lib and after run main lib
            Callback = new TimerCallback(OnCallback);
            TimerI2C = new Timer(Callback, ETimer.I2C, 0, 1000);
            TimerFunction = new Timer(Callback, ETimer.Function, 0, 500);
            TimerWiFi = new Timer(Callback, ETimer.WiFi, 0, 2000);

            // Calling the Thread.Sleep method causes the current thread to 
            // Immediately block for the number of milliseconds or the time interval
            // You pass to the method, and yields the remainder of its time 
            // Slice to another thread
            Thread.Sleep(Timeout.Infinite);
        }

        public static void UnknownEvent()
        {
            // Notify user
            Debug.Print("Error! Unexpected <" + Receive + ">[" + Receive.Length.ToString() + "] data received.");

            // IMPORTANT NOTICE: Before the calling internal functions,
            // Last stored data must be removed on memory. Otherwise, we can not sent
            // Last stored data to master device. And additional, data removing will refresh
            // the size of data in memory. This is most important thing ...
            Transmit.Clear();
            Receive = null;
        }

        public static string GenerateHexadecimal(byte data)
        {
            string result = null;
            byte division = data;

            while (division != 0)
            {
                byte remainder = (byte)(division % 16);
                division = (byte)((division - remainder) / 16);

                // At the here, we are converting 0 to 9 as directly to string, 
                // But when we calculate and find 10 to 15, we are using ASCII 
                // Table for decoding it. The default case for this process is 
                // Uppercase. So we are adding 55 (additional case 10) to result
                switch (remainder)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                        result = remainder.ToString() + result;
                        break;
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                        result = ((char)(remainder + 55)).ToString() + result;
                        break;
                    default:
                        break;
                }
            }

            // Return calculated result to caller
            return result;
        }

        #endregion

        #region Trigger

        private static void OnCallback(object state)
        {
            // IMPORTANT NOTICE: As you can see, we do not use delay function
            // In all lib. Delay function is a non-blocking function in Arduino
            // Core. So solving this, we are using MILLIS()
            switch ((ETimer)state)
            {
                case ETimer.I2C:
                    Scanner.ScanSlaves();
                    break;
                case ETimer.Function:
                    Debug.Print("Function");
                    break;
                case ETimer.WiFi:
                    Debug.Print("WiFi");
                    break;
                default:
                    Debug.Print("default");
                    break;
            }
        }

        private static void OnConnected(string[] array, byte count)
        {
            // IMPORTANT NOTICE: Before the calling internal functions,
            // Last stored data must be removed on memory. Otherwise, we can not sent
            // Last stored data to master device. And additional, data removing will refresh
            // the size of data in memory. This is most important thing ...
            Transmit.Clear();
            Receive = null;

            Debug.Print("Done! " + ((int)count).ToString() + " slave device(s) connected to I2C bus. ID(s): ");
            foreach (var item in array)
                Debug.Print("\t ID: 0x" + item);
            Debug.Print("\t ---");
        }

        private static void OnDisconnected(string[] array, byte count)
        {
            // IMPORTANT NOTICE: Before the calling internal functions,
            // Last stored data must be removed on memory. Otherwise, we can not sent
            // Last stored data to master device. And additional, data removing will refresh
            // the size of data in memory. This is most important thing ...
            Transmit.Clear();
            Receive = null;

            Debug.Print("Done! " + ((int)count).ToString() + " slave device(s) disconnected from I2C bus. ID(s): ");
            foreach (var item in array)
                Debug.Print("\t ID: 0x" + item);
            Debug.Print("\t ---");
        }

        #endregion

        #region Listen

        private static void listenFunction()
        {
            // IMPORTANT NOTICE: Device registering is more priority than others
            // Step, When new device(s) were connected to master device, firstly
            // Register these device(s) to system, after continue what you do
            for (ushort index = 0; index < Device.Length; index++)
            {
                // If current index is empty, go next
                if (Device[index].Function.Length == 0)
                    continue;

                // If handshake is not ok, that's mean probably function is also not ok
                if (Device[index].Handshake != EHandshake.Ready)
                    continue;

                // Listen functions on connected device(s)
                for (ushort subindex = 0; subindex < Device[index].Function.Length; subindex++)
                {
                    if (!Device[index].Function[subindex].Listen)
                        continue;

                    string[] data = new string[] { Device[index].Function[subindex].Name, "NULL" };
                    string result = Serializer.Encode(DATA_DELIMITER, data);

                    // Write internal function list to connected device, one-by-one
                    Write(Device[index].Address, result);

                    // We will do this till decoding will return false
                    while (true)
                    {
                        // More info was given at inside of this function
                        // Actually, that's not worst case
                        if (!Read(Device[index].Address))
                            break;

                        // IMPORTANT NOTICE: At the here, We are making output control.
                        // The code that given at above changes global flag output(s). So,
                        // For next operations, We need to check this output(s) and in this
                        // Way detect our current status
                        if (Communication != ECommunication.Continue)
                            break;
                    }

                    // If it is still IDLE, that's mean data is corrupted (Not END)
                    if (Communication == ECommunication.Idle)
                    {
                        UnknownEvent();
                        break;
                    }

                    // -----

                    // Decode last given data from slave, after we will publish it
                    string[] subBuffer = Serializer.Decode(DATA_DELIMITER, Receive);

                    // Null operator check
                    if (subBuffer == null)
                        break;

                    // -----

                    // Looking good, inline if-loop
                    data = new string[] { "0x" + GenerateHexadecimal(Device[index].Address), Device[index].Function[subindex].Name };
                    result = '/' + Serializer.Encode('/', data);

                    // Publish last received buffer to MQTT broker
                    //mqttClient.publish(result, subBuffer[1]);
                }
            }
        }

        private static void listenWiFi()
        {
        }

        #endregion

        #region I2C

        private static bool Write(byte address, string data)
        {
            // Free up out-of-date buffer data, top level clearing
            Receive = null;

            // Encode given data for bus communication
            Encode(data);

            // Send all remainder data to newly registered slave
            while (Transmit.Length != 0)
            {
                // Reinitialize config data of an I2C device
                Configuration = new I2CDevice.Configuration(address, I2C_BUS_CLOCKRATE);
                I2C = new I2CDevice(Configuration);
                byte[] handshake = Encoding.UTF8.GetBytes(Transmit.Dequeue());

                // Generate a transaction for testing an connected device
                I2CDevice.I2CTransaction[] transaction = { I2CDevice.CreateWriteTransaction(handshake) };

                // When retry is overflowed, abort it to worst case
                I2C.Execute(transaction, I2C_BUS_TIMEOUT);
                I2C.Dispose();

                // Maybe not need, right?
                Thread.Sleep(15);
            }

            // IMPORTANT NOTICE: Due to decoding of slave device, we need to Wait
            // A little bit. Otherwise, Master device will request data From slave
            // Device too early and slave cannot send it. Additional, when more
            // Devices are connected, we need to downgrade delay time. Already,
            // It will take the same time during roaming
            Thread.Sleep(100);

            // If everything goes well, we will arrive here and return true
            return true;
        }

        private static bool Read(byte address)
        {
            // Reinitialize config data of an I2C device
            Configuration = new I2CDevice.Configuration(address, I2C_BUS_CLOCKRATE);
            I2C = new I2CDevice(Configuration);
            byte[] newReceivedBuffer = new byte[BUFFER_SIZE];

            // Generate a transaction for testing an connected device
            I2CDevice.I2CTransaction[] transaction = { I2CDevice.CreateReadTransaction(newReceivedBuffer) };

            // When retry is overflowed, abort it to worst case
            while (I2C.Execute(transaction, I2C_BUS_TIMEOUT) != newReceivedBuffer.Length) { }

            // IMPORTANT NOTICE: Convert all given data to char array. At the here,
            // We are choosed stop bit from our experience at before. This stop bit 
            // Can be different on the another board. So, please be very carefully
            string output = null;
            foreach (var item in newReceivedBuffer)
            {
                if (item == I2C_BUS_ENDOFLINE)
                    break;
                output = output + (char)item;
            }

            // Maybe not need, right?
            Thread.Sleep(15);

            // Decode last given data
            if (!Decode(output))
                return false;

            // IMPORTANT NOTICE: Actually When we arrived this point, we arrived
            // Worst case point even though It was TRUE. If you came there, program will
            // Run till communication flag will be END or IDLE type. Otherwise, this
            // Point is related with CONTINUE status
            return true;
        }

        #endregion

        #region Serializer

        private static bool Decode(string data)
        {
            if (data == null)
                return false;

            // Decode given data, Calculate up-of-date and needed buffer size
            string[] newReceivedBuffer = Serializer.Decode(PROTOCOL_DELIMITERS.ToCharArray(), data);

            // Null operator check
            if (newReceivedBuffer == null)
                return false;

            switch (newReceivedBuffer[1][0])
            {
                case IDLE_SINGLE_START:
                case IDLE_MULTI_END:
                    Communication = ECommunication.End;
                    break;

                case IDLE_MULTI_START:
                    Communication = ECommunication.Continue;
                    break;

                default:
                    Communication = ECommunication.Idle;

                    // Free up out-of-date buffer data
                    Receive = null;
                    return false;
            }

            // If everything goes well, we will arrive here and return true
            Receive = Receive + newReceivedBuffer[0];
            return true;
        }

        private static bool Encode(string data)
        {
            // Firstly,check given data, return false at the worst case
            if (data == null)
                return false;

            // IMPORTANT NOTICE: Before the calling internal functions,
            // Last stored data must be removed on memory. Otherwise, we can not sent
            // Last stored data to master device. And additional, data removing will refresh
            // the size of data in memory. This is most important thing ...
            Transmit.Clear();

            // Second, calculate size of data and modulus
            ushort modulusofString = (ushort)(data.Length % DIVISOR_NUMBER);
            ushort sizeofString = (ushort)(data.Length / DIVISOR_NUMBER);
            char[] inputData = data.ToCharArray();

            // Add modulos of data, if possible
            if (modulusofString > 0)
                sizeofString++;

            for (ushort index = 0; index < sizeofString; index++)
            {
                string outputData = null;
                ushort upperBound = 0;

                // Calculate maximum upperbound of iterator
                if (modulusofString != 0 && index == sizeofString - 1)
                    upperBound = modulusofString;
                else
                    upperBound = DIVISOR_NUMBER;

                for (ushort iterator = 0; iterator < upperBound; iterator++)
                    outputData = outputData + inputData[(index * DIVISOR_NUMBER) + iterator];

                outputData = PROTOCOL_DELIMITERS[0] + outputData;
                outputData = outputData + PROTOCOL_DELIMITERS[1];

                // IMPORTANT NOTICE: At the here, We have two status for encoding data(s)
                // If you set the penultimate char as multiSTART, this means data is still available
                // For encoding. But if you set this var as multiEND, this means encoding is over
                // We are making this for receiver side. singleSTART means that data can encode
                // As one packet, do not need any more encoding
                if (sizeofString == 1)
                    outputData = outputData + IDLE_SINGLE_START;
                else if (index == sizeofString - 1)
                    outputData = outputData + IDLE_MULTI_END;
                else
                    outputData = outputData + IDLE_MULTI_START;

                outputData = outputData + PROTOCOL_DELIMITERS[2];
                Transmit.Enqueue(outputData);
            }

            // If everything goes well, we will arrive here and return true
            return true;
        }

        #endregion

        #region RGB

        public static void HSVToRGB(double hue, double saturation, double value, double red, double green, double blue)
        {
            // hue parameter checking/fixing
            if (hue < 0 || hue >= 360)
            {
                hue = 0;
            }
            // if Brightness is turned off, then everything is zero.
            if (value <= 0)
            {
                red = green = blue = 0;
            }

            // if saturation is turned off, then there is no color/hue. it's grayscale.
            else if (saturation <= 0)
            {
                red = green = blue = value;
            }
            else // if we got here, then there is a color to create.
            {
                double hf = hue / 60.0;
                int i = (int)System.Math.Floor(hf);
                double f = hf - i;
                double pv = value * (1 - saturation);
                double qv = value * (1 - saturation * f);
                double tv = value * (1 - saturation * (1 - f));

                switch (i)
                {

                    // Red Dominant
                    case 0:
                        red = value;
                        green = tv;
                        blue = pv;
                        break;

                    // Green Dominant
                    case 1:
                        red = qv;
                        green = value;
                        blue = pv;
                        break;
                    case 2:
                        red = pv;
                        green = value;
                        blue = tv;
                        break;

                    // Blue Dominant
                    case 3:
                        red = pv;
                        green = qv;
                        blue = value;
                        break;
                    case 4:
                        red = tv;
                        green = pv;
                        blue = value;
                        break;

                    // Red Red Dominant
                    case 5:
                        red = value;
                        green = pv;
                        blue = qv;
                        break;

                    // In case the math is out of bounds, this is a fix.
                    case 6:
                        red = value;
                        green = tv;
                        blue = pv;
                        break;
                    case -1:
                        red = value;
                        green = pv;
                        blue = qv;
                        break;

                    // If the color is not defined, go grayscale
                    default:
                        red = green = blue = value;
                        break;
                }
            }

            RGBRed.DutyCycle = ClampRGB(red);
            RGBGreen.DutyCycle = ClampRGB(green);
            RGBBlue.DutyCycle = ClampRGB(blue);
        }

        public static double ClampRGB(double index)
        {
            if (index < 0) return 0;
            if (index > 1) return 1;
            return index;
        }

        public static void ChangeRGBStatus(SColorbook source, SColorbook target, int divider, int sleep)
        {
            SColorbook effect = new SColorbook(
                (target.Red - source.Red) / divider,
                (target.Green - source.Green) / divider,
                (target.Blue - source.Blue) / divider,
                (target.Hue - source.Hue) / divider,
                (target.Saturation - source.Saturation) / divider,
                (target.Value - source.Value) / divider
                );

            double red = source.Red;
            double green = source.Green;
            double blue = source.Blue;
            double hue = source.Hue;
            double saturation = source.Saturation;
            double value = source.Value;

            HSVToRGB(hue, saturation, value, red, green, blue);
            for (int index = 0; index < divider; index++)
            {
                red = red + effect.Red;
                green = green + effect.Green;
                blue = blue + effect.Blue;
                hue = hue + effect.Hue;
                saturation = saturation + effect.Saturation;
                value = value + effect.Value;

                HSVToRGB(hue, saturation, value, red, green, blue);
                Thread.Sleep(sleep);
            }
        }

        public static void SetRGBStatus(ENotify status)
        {
            switch (status)
            {
                case ENotify.Offline:
                    switch (Notify)
                    {
                        case ENotify.Online:
                            ChangeRGBStatus(ColorWhite, ColorBlack, BLINK_CLOCKRATE, 1);
                            break;
                        case ENotify.Unconfirmed:
                            ChangeRGBStatus(ColorOrange, ColorBlack, BLINK_CLOCKRATE, 1);
                            break;
                        case ENotify.Confirmed:
                            ChangeRGBStatus(ColorBlue, ColorBlack, BLINK_CLOCKRATE, 1);
                            break;
                        default:
                            break;
                    }
                    break;

                case ENotify.Online:
                    for (int index = 0; index < 2; index++)
                    {
                        ChangeRGBStatus(ColorBlack, ColorWhite, BLINK_CLOCKRATE, 0);
                        Thread.Sleep(200);

                        ChangeRGBStatus(ColorWhite, ColorBlack, BLINK_CLOCKRATE, 1);
                        Thread.Sleep(500);
                    }
                    ChangeRGBStatus(ColorBlack, ColorWhite, BLINK_CLOCKRATE, 0);
                    break;

                case ENotify.Unconfirmed:
                    ChangeRGBStatus(ColorWhite, ColorOrange, BLINK_CLOCKRATE, 0);
                    break;

                case ENotify.Confirmed:
                    ChangeRGBStatus(ColorWhite, ColorBlue, BLINK_CLOCKRATE, 0);
                    break;

                default:
                    break;
            }

            // Wait for 2.5 second, otherwise it can be blocked by next process
            Thread.Sleep(2500);

            // Do not forget to set RGB status
            Notify = status;
        }

        #endregion

        #region Alphanumeric

        private static bool IsAlphanumeric(string data)
        {
            if (data == null)
                return false;

            // Decrease comparing options, looking good
            data = data.ToUpper();

            // Check function ID, type is alphanumeric
            for (ushort index = 0; index < data.Length; index++)
                if (!(IsAlpha(data) || IsNumeric(data)))
                    return false;

            return true;
        }

        private static bool IsNumeric(string data)
        {
            if (data == null)
                return false;

            // Check function ID, type is numeric
            for (ushort index = 0; index < data.Length; index++)
                if (data[index] < '0' || data[index] > '9')
                    return false;

            return true;
        }

        private static bool IsAlpha(string data)
        {
            if (data == null)
                return false;

            // Check function ID, type is alpha
            for (ushort index = 0; index < data.Length; index++)
                if (data[index] < 'A' || data[index] > 'Z')
                    return false;

            return true;
        }

        #endregion
    }
}
