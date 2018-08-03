using System;
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

        static MasterScanner Scanner = new MasterScanner();
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

        #endregion

        public static void Main()
        {
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

        private static bool Write(byte address, string data)
        {
            return false;
        }

        private static bool Read(byte address)
        {
            return false;
        }

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
                    Communication  = ECommunication.End;
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
            Receive = Receive+ newReceivedBuffer[0];
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
            ushort sizeofString = (ushort)(data.Length / DIVISOR_NUMBER) ;
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

        private static void UnknownEvent()
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

        private static string GenerateHexadecimal(byte data)
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
    }
}
