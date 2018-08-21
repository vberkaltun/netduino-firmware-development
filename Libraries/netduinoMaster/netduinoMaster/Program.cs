using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Net.NetworkInformation;
using SecretLabs.NETMF.Hardware.Netduino;
using intelliPWR.MasterScanner;
using intelliPWR.Serializer;
using intelliPWR.NetduinoMQTT;

namespace netduinoMaster
{
    public class Program : Constant
    {
        #region Variable

        static PWM RGBRed = new PWM(PWMChannels.PWM_PIN_D11, 100, 0, true);
        static PWM RGBGreen = new PWM(PWMChannels.PWM_PIN_D10, 100, 0, true);
        static PWM RGBBlue = new PWM(PWMChannels.PWM_PIN_D9, 100, 0, true);
        static OutputPort SSR = new OutputPort(Pins.ONBOARD_LED, true);

        static SColorbook ColorBlack = new SColorbook(0, 0, 0, 0, 0, 0);
        static SColorbook ColorWhite = new SColorbook(255, 255, 255, 0, 0, 1);
        static SColorbook ColorBlue = new SColorbook(0, 204, 204, 180, 1, 0.8);
        static SColorbook ColorOrange = new SColorbook(255, 128, 0, 30, 1, 1);

        static MasterScanner Scanner = new MasterScanner(I2C_BUS_CLOCKRATE, I2C_BUS_TIMEOUT, I2C_BUS_RETRY);
        static Serializer Serializer = new Serializer();
        static NetduinoMQTT NetduinoMQTT = new NetduinoMQTT();

        static ECommunication CommunicationFlag = ECommunication.Idle;
        static ENotify NotifyFlag = ENotify.Offline;

        static SStringArray Master = new SStringArray();
        static SDeviceArray Slave = new SDeviceArray();
        static SNotifyArray Notify = new SNotifyArray();

        static SStringArray Transmit = new SStringArray();
        static string Receive = null;

        static I2CDevice.Configuration Configuration = new I2CDevice.Configuration(0, I2C_BUS_CLOCKRATE);
        static I2CDevice I2C = new I2CDevice(Configuration);

        static Thread ThreadMQTT;
        static Socket Socket;
        static Object LockRGB = new Object();

        static TimerCallback Callback = null;
        static Timer TimerPing = null;
        static Timer TimerScan = null;
        static Timer TimerRGB = null;

        #endregion

        #region Uncategorized

        public static void Main()
        {
            // Initialize device, do not remove
            Debug.Print("Waiting for Device Configuration...");
            while (!InitializeDevice()) ;
            Debug.Print("Waiting for Network Configuration...");
            while (!InitializeNetwork()) ;
            Debug.Print("Waiting for MQTT Configuration...");
            while (!InitializeMQTT()) ;

            // Start Pulse-Width Modulation of led operation
            RGBRed.Start();
            RGBGreen.Start();
            RGBBlue.Start();

            // IMPORTANT NOTICE: Due to our I2C scanner lib, When a new device
            // Connected or disconnected to master, our I2C scanner lib decides
            // Which one is to be triggered
            Scanner.OnConnected = OnConnected;
            Scanner.OnDisconnected = OnDisconnected;
            NetduinoMQTT.OnReceived = OnReceived;

            // Setup and start a new thread for the listener
            ThreadMQTT = new Thread(ListenMQTT);
            ThreadMQTT.Priority = ThreadPriority.Highest;
            ThreadMQTT.Start();

            // Attach functions to lib and after run main lib
            Callback = new TimerCallback(OnCallback);
            TimerPing = new Timer(Callback, ETimer.Ping, 0, 5000);
            TimerScan = new Timer(Callback, ETimer.Scan, 0, 250);
            TimerRGB = new Timer(Callback, ETimer.RGB, 0, 1000);

            // Calling the Thread.Sleep method causes the current thread to 
            // Immediately block for the number of milliseconds or the time interval
            // You pass to the method, and yields the remainder of its time 
            // Slice to another thread
            Thread.Sleep(Timeout.Infinite);
        }

        private static void UnknownEvent()
        {
            // Cross check, maybe it is returned as null
            Receive = Receive + " ";

            // Notify user
            Debug.Print("Error! Unexpected <" + Receive + ">[" + Receive.Length.ToString() + "] data received.");

            // IMPORTANT NOTICE: Before the calling internal functions,
            // Last stored data must be removed on memory. Otherwise, we can not sent
            // Last stored data to master device. And additional, data removing will refresh
            // the size of data in memory. This is most important thing ...
            Transmit.Clear();
            Receive = null;
        }

        private static bool SubscribeTopic(byte address, bool type)
        {
            // When we found given device in device list, generate MQTT vendor(s)
            for (ushort index = 0; index < Slave.Length; index++)
            {
                if (address == Slave[index].Address)
                {
                    string convertedAddress = "0x" + GenerateHexadecimal(address);
                    string[] topicList = { "isConnected", "brand", "model", "version" };
                    string[] messageList = { type ? "1" : "0", Slave[index].Vendor.Brand, Slave[index].Vendor.Model, Slave[index].Vendor.Version };

                    for (int iterator = 0; iterator < topicList.Length; iterator++)
                    {
                        string[] data = { convertedAddress, topicList[iterator] };
                        string result = '/' + Serializer.Encode('/', data);

                        if (type || iterator == 0)
                            NetduinoMQTT.PublishMQTT(Socket, result, messageList[iterator]);
                    }

                    // -----

                    for (ushort subindex = 0; subindex < Slave[index].Function.Length; subindex++)
                    {
                        if (!Slave[index].Function[subindex].Request)
                            continue;

                        string[] data = { convertedAddress, Slave[index].Function[subindex].Name };
                        string result = '/' + Serializer.Encode('/', data);

                        // Looking good, inline if-loop
                        if (type)
                            NetduinoMQTT.SubscribeMQTT(Socket, new string[] { result }, new int[] { 0 }, 1);
                        else
                            NetduinoMQTT.UnsubscribeMQTT(Socket, new string[] { result }, new int[] { 0 }, 1);
                    }

                    // Do not need to search all data, we are OK now
                    return true;
                }
            }

            // Worst case, when not find anything we will arrive there
            return false;
        }

        private static void ListenMQTT()
        {
            while (true)
            {
                try
                {
                    if (NetduinoMQTT == null)
                        throw new Exception();

                    // At the best case, start to listening port
                    NetduinoMQTT.Listen(Socket);
                }
                catch (Exception)
                {
                    // Notify end - user
                    Debug.Print("Error! MQTT Connection lost. Reconnecting <" + MQTT_PORT + "> port on <" + MQTT_SERVER + "> server...");

                    lock (Socket)
                    {
                        Debug.Print("Waiting for Network Configuration...");
                        while (!InitializeNetwork()) ;
                        Debug.Print("Waiting for MQTT Configuration...");
                        while (!InitializeMQTT()) ;
                    }
                }
            }
        }

        private static void OnCallback(object state)
        {
            switch ((ETimer)state)
            {
                case ETimer.Ping:
                    if (NetduinoMQTT == null)
                        break;

                    lock (NetduinoMQTT)
                    {
                        // Our keep alive is 15 seconds - we ping again every 10, So we should live forever
                        Debug.Print("Pinging <" + MQTT_PORT + "> port on <" + MQTT_SERVER + "> server...");

                        if (NetduinoMQTT.PingMQTT(Socket) == -1)
                        {
                            NetduinoMQTT.DisconnectMQTT(Socket);
                            NetduinoMQTT = null;
                        }
                    }
                    break;

                case ETimer.Scan:
                    if (I2C == null)
                        break;

                    lock (I2C)
                    {
                        Scanner.ScanSlaves(I2C);
                        FetchFunction();
                    }

                    break;

                case ETimer.RGB:
                    if (LockRGB == null)
                        break;

                    lock (LockRGB)
                    {
                        if (Notify.Length != 0)
                        {
                            // Notify end user, status is reserved
                            Debug.Print("Done! RGB LED status was changed from <" + NotifyFlag.ToString() + "> to <" + Notify.Peek().ToString() + "> status.");

                            SetRGBStatus(Notify.Dequeue());
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        #endregion

        #region Initialize

        private static bool InitializeDevice()
        {
            // Wait a little bit, That will make better the next step 
            Thread.Sleep(500);

            try
            {
                // Function list of master scanner, do not change
                Master.Enqueue("getVendors");
                Master.Enqueue("getFunctionList");

                // Display network config for debugging
                Debug.Print("Done! Device Configuration was setted to system successfully.");

                // Show device info, not necessary
                Debug.Print("\t DEVICE BRAND: " + DEVICE_BRAND);
                Debug.Print("\t DEVICE MODEL: " + DEVICE_MODEL);
                Debug.Print("\t DEVICE VERSION: " + DEVICE_VERSION);
                Debug.Print("\t ---");

                // Best case
                return true;
            }
            catch (Exception)
            {
                // Worst case
                Debug.Print("Error! Unexpected Device Error triggered.");
                return false;
            }

        }

        private static bool InitializeNetwork()
        {
            // Wait a little bit, That will make better the next step 
            Thread.Sleep(500);

            try
            {
                // Wait for Network address if DHCP
                NetworkInterface Network = NetworkInterface.GetAllNetworkInterfaces()[0];

                if (!Network.IsDhcpEnabled)
                    throw new Exception();

                if (NetworkInterface.GetAllNetworkInterfaces()[0].IPAddress == IPAddress.Any.ToString())
                    throw new Exception();

                // wait for DHCP-allocated IP address
                if (IPAddress.GetDefaultLocalAddress() == IPAddress.Any)
                    throw new Exception();

                // Display network config for debugging
                Debug.Print("Done! Network Configuration was setted to system successfully.");
                Debug.Print("\t NETWORK INTERFACE TYPE: " + Network.NetworkInterfaceType.ToString());
                Debug.Print("\t DHCP ENABLED: " + Network.IsDhcpEnabled.ToString());
                Debug.Print("\t DYNAMIC DNS ENABLED: " + Network.IsDynamicDnsEnabled.ToString());
                Debug.Print("\t IP ADDRESS: " + Network.IPAddress.ToString());
                Debug.Print("\t SUBNET MASK: " + Network.SubnetMask.ToString());
                Debug.Print("\t GATEWAY: " + Network.GatewayAddress.ToString());

                foreach (string dnsAddress in Network.DnsAddresses)
                    Debug.Print("\t DNS SERVER: " + dnsAddress.ToString());
                Debug.Print("\t ---");

                // Best case
                return true;
            }
            catch (Exception)
            {
                // Worst case
                Debug.Print("Error! Unexpected Network Error triggered.");
                return false;
            }
        }

        private static bool InitializeMQTT()
        {
            // Wait a little bit, That will make better the next step 
            Thread.Sleep(500);

            try
            {
                // Get broker's IP of MQTT address
                //IPHostEntry hostEntry = Dns.GetHostEntry(MQTT_SERVER);
                IPHostEntry hostEntry = Dns.GetHostEntry(MQTT_SERVER);

                // Create socket and connect to the broker's IP address and port
                Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Udp);
                Socket.Connect(new IPEndPoint(hostEntry.AddressList[0], MQTT_PORT));

                // Create MQTT and connect to the broker's port with username and password
                NetduinoMQTT = new NetduinoMQTT();

                if (NetduinoMQTT.ConnectMQTT(Socket, DEVICE_MODEL, MQTT_TIMEOUT, true, MQTT_USER, MQTT_PASSWORD) != 0)
                    throw new Exception();

                Debug.Print("Done! Socket Connection was established successfully.");

                // IMPORTANT NOTICE: First of all, we need to subscribe main device
                // We call it like XXXX/status and this broker is related with SSR
                // State of device. We will listen something about this and execute it
                string[] data = { DEVICE_MODEL, "status" };
                string result = '/' + Serializer.Encode(new char[1] { '/' }, data);
                NetduinoMQTT.SubscribeMQTT(Socket, new string[] { result }, new int[] { 0 }, 1);

                Debug.Print("Done! MQTT Configuration was established successfully <" + MQTT_PORT + "> port on <" + MQTT_SERVER + "> server.");

                // Best case
                return true;
            }
            catch (SocketException error)
            {
                // Worst case
                Debug.Print("Error! Unexpected Socket Error <" + error.ErrorCode + "> triggered.");
                return false;
            }
        }

        #endregion

        #region Trigger

        private static void OnConnected(byte[] array, byte count)
        {
            try
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

                // Notify end user, status is device online
                Notify.Enqueue(ENotify.Online);

                // -----

                // IMPORTANT NOTICE: Device registering is more priority than others
                // Step, When new device(s) were connected to master device, firstly
                // Register these device(s) to system, after continue what you do
                for (ushort index = 0; index < count; index++)
                {
                    // Register last added device to system
                    for (ushort subindex = 0; subindex < Master.Length; subindex++)
                    {

                        string[] data = { Master[subindex], "NULL" };
                        string result = Serializer.Encode(DATA_DELIMITER, data);

                        // Write internal function list to connected device, one-by-one
                        Write(array[index], result);

                        // We will do this till decoding will return false
                        while (true)
                        {

                            // More info was given at inside of this function
                            // Actually, that's not worst case
                            if (!Read(array[index]))
                                break;

                            // IMPORTANT NOTICE: At the here, We are making output control.
                            // The code that given at above changes global flag output(s). So,
                            // For next operations, We need to check this output(s) and in this
                            // Way detect our current status
                            if (CommunicationFlag != ECommunication.Continue)
                                break;
                        }

                        // If it is still IDLE, that's mean data is corrupted (Not END)
                        if (CommunicationFlag == ECommunication.Idle)
                        {
                            UnknownEvent();
                            break;
                        }

                        // IMPORTANT NOTICE: Final point, If we can arrive there, that's mean
                        // Bus communication is OK but inside data is unknown. At the next code,
                        // We will try to decode our inside data. If we can not do it, we will
                        // Add this unknown device as BLOCKED to system
                        if (!FillConfiguration(subindex, array[index]))
                            break;

                        // -----

                        // Subscribe all function of current slave to MQTT broker
                        SubscribeTopic(array[index], true);
                    }
                }
            }
            catch (Exception)
            {
                UnknownEvent();
            }

        }

        private static void OnDisconnected(byte[] array, byte count)
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

            // Unsubscribe all function of current slave to MQTT broker
            foreach (var item in array)
                SubscribeTopic(item, false);

            // Notify end user, status is device online
            Notify.Enqueue(ENotify.Offline);

            // -----

            // IMPORTANT NOTICE: At the here, firstly, we will make a search about
            // Disconnected device. When we find it, We will delete index of this
            // Device, and after we will add all temporarily popped device again
            // If disconnected count is equal to size of device data, delete all
            if (count >= Slave.Length)
                Slave.Clear();
            else {

                // IMPORTANT NOTICE: In this func, we are deleting information about
                // Disconnected device. For now, We can not delete it directly. Because
                // Of this, for solving this, firstly we will clone first item to there
                // Secondly we will delete this first item from queue
                for (int index = 0; index < count; index++)
                {
                    for (int iterator = 0; iterator < Slave.Length; iterator++)
                    {
                        if (array[index] == Slave[iterator].Address)
                        {
                            Slave.RemoveAt(iterator);
                            break;
                        }
                    }

                    // We found, stop the current session
                    if (Slave.Length == 0)
                        break;
                }
            }
        }

        private static void OnReceived(string topic, string payload)
        {
            lock (I2C)
            {
                WriteFunction(topic, payload);
            }
        }

        #endregion

        #region I2C

        private static void FetchFunction()
        {
            try
            {
                // IMPORTANT NOTICE: Device registering is more priority than others
                // Step, When new device(s) were connected to master device, firstly
                // Register these device(s) to system, after continue what you do
                for (ushort index = 0; index < Slave.Length; index++)
                {
                    // If current index is empty, go next
                    if (Slave[index].Function.Length == 0)
                        continue;

                    // If handshake is not ok, that's mean probably function is also not ok
                    if (Slave[index].Handshake != EHandshake.Ready)
                        continue;

                    // Listen functions on connected device(s)
                    for (ushort subindex = 0; subindex < Slave[index].Function.Length; subindex++)
                    {
                        if (!Slave[index].Function[subindex].Listen)
                            continue;

                        string[] data = new string[] { Slave[index].Function[subindex].Name, "NULL" };
                        string result = Serializer.Encode(DATA_DELIMITER, data);

                        // Write internal function list to connected device, one-by-one
                        Write(Slave[index].Address, result);

                        // We will do this till decoding will return false
                        while (true)
                        {
                            // More info was given at inside of this function
                            // Actually, that's not worst case
                            if (!Read(Slave[index].Address))
                                break;

                            // IMPORTANT NOTICE: At the here, We are making output control.
                            // The code that given at above changes global flag output(s). So,
                            // For next operations, We need to check this output(s) and in this
                            // Way detect our current status
                            if (CommunicationFlag != ECommunication.Continue)
                                break;
                        }

                        // If it is still IDLE, that's mean data is corrupted (Not END)
                        if (CommunicationFlag == ECommunication.Idle)
                        {
                            UnknownEvent();
                            break;
                        }

                        // -----

                        // Decode last given data from slave, after we will publish it
                        string[] subBuffer = Receive.Split(DATA_DELIMITER);

                        // Null operator check
                        if (subBuffer == null)
                            break;

                        // -----

                        // Looking good, inline if-loop
                        data = new string[] { "0x" + GenerateHexadecimal(Slave[index].Address), Slave[index].Function[subindex].Name };
                        result = '/' + Serializer.Encode('/', data);

                        // Publish last received buffer to MQTT broker
                        NetduinoMQTT.PublishMQTT(Socket, result, subBuffer[1]);
                    }
                }
            }
            catch (Exception)
            {
                UnknownEvent();
            }
        }

        private static void WriteFunction(string topic, string payload)
        {
            // Print out some debugging info
            Debug.Print("Done! Callback updated on <" + topic + ">[" + payload + "].");

            // Generate a delimiter data and use in with decoding function
            string[] data = Serializer.Decode(new char[] { '/', '/' }, topic);

            // Null operator check
            if (data != null)
            {
                // IMPORTANT NOTICE: If we can arrive there, that's mean the payload
                // Data is consisted of status data. Otherwise, we can say the payload
                // Data is not consisted of status data. Worst case, Go other state
                // Of if, that's mean also it is a function data(s)
                if (data[0] == DEVICE_MODEL)
                    SSR.Write(payload[0] == '1' ? true : false);
                else
                {
                    // Compare internal data(s) with registered function list
                    for (ushort index = 0; index < Slave.Length; index++)
                    {
                        // Store device name at the here, we can not use it directly
                        string convertedAddress = "0x" + GenerateHexadecimal(Slave[index].Address);

                        // Check that is it same or not
                        if (convertedAddress == null)
                            continue;

                        // Write internal data list to connected device, one-by-one
                        string result = Serializer.Encode(DATA_DELIMITER, new string[2] { data[1], payload });

                        Write(Slave[index].Address, result);

                        // Not need for stay searching
                        break;
                    }
                }
            }
        }

        private static bool Write(byte address, string data)
        {
            try
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
                    I2C.Config = Configuration;
                    byte[] handshake = Encoding.UTF8.GetBytes(Transmit.Dequeue());

                    // Generate a transaction for testing an connected device
                    I2CDevice.I2CTransaction[] transaction = { I2CDevice.CreateWriteTransaction(handshake) };
                    ushort retryCount = 0;

                    // When retry is overflowed, abort it to worst case
                    while (I2C.Execute(transaction, I2C_BUS_TIMEOUT) != handshake.Length)
                        if (retryCount++ > I2C_BUS_RETRY)
                            throw new Exception();
                }

                // If everything goes well, we will arrive here and return true
                return true;
            }
            catch (Exception)
            {
                UnknownEvent();
                return false;
            }
        }

        private static bool Read(byte address)
        {
            try
            {
                // Reinitialize config data of an I2C device
                Configuration = new I2CDevice.Configuration(address, I2C_BUS_CLOCKRATE);
                I2C.Config = Configuration;
                byte[] newReceivedBuffer = new byte[BUFFER_SIZE];

                // Generate a transaction for testing an connected device
                I2CDevice.I2CTransaction[] transaction = { I2CDevice.CreateReadTransaction(newReceivedBuffer) };
                ushort retryCount = 0;

                // When retry is overflowed, abort it to worst case
                while (I2C.Execute(transaction, I2C_BUS_TIMEOUT) != newReceivedBuffer.Length)
                    if (retryCount++ > I2C_BUS_RETRY)
                        throw new Exception();

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

                // Decode last given data
                if (!Decode(output))
                    return false;

                // IMPORTANT NOTICE: Actually When we arrived this point, we arrived
                // Worst case point even though It was TRUE. If you came there, program will
                // Run till communication flag will be END or IDLE type. Otherwise, this
                // Point is related with CONTINUE status
                return true;
            }
            catch (Exception)
            {
                UnknownEvent();
                return false;
            }
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
                    CommunicationFlag = ECommunication.End;
                    break;

                case IDLE_MULTI_START:
                    CommunicationFlag = ECommunication.Continue;
                    break;

                default:
                    CommunicationFlag = ECommunication.Idle;

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

        #region Fill

        private static bool FillConfiguration(ushort status, byte address)
        {
            // Generate a delimiter data and use in with decoding function
            string[] newReceivedBuffer = Receive.Split(DATA_DELIMITER);

            // Null operator check
            if (newReceivedBuffer == null)
                return false;

            // IMPORTANT NOTICE: In this program, we have two data type, one of them
            // Is VENDOR data, other one is FUNCTION data. Because of this we have two
            // Statement in switch case
            switch (status)
            {
                case 0:
                    if (!FillVendor(address, newReceivedBuffer))
                        return false;

                    // Notify user
                    Debug.Print("Done! The vendors of [0x" + GenerateHexadecimal(address) + "] address were saved successfully to system.");
                    Debug.Print("\t BRAND: " + Slave[0].Vendor.Brand);
                    Debug.Print("\t MODEL: " + Slave[0].Vendor.Model);
                    Debug.Print("\t VERSION: " + Slave[0].Vendor.Version);
                    Debug.Print("\t ---");
                    break;

                case 1:
                    if (!FillFunction(address, newReceivedBuffer))
                    {
                        // Notify end user, status is device online
                        Notify.Enqueue(ENotify.Unconfirmed);

                        return false;
                    }

                    // Notify end user, status is device online
                    Notify.Enqueue(ENotify.Confirmed);

                    // Notify user
                    for (ushort index = 0; index < Slave[0].Function.Length; index++)
                        Debug.Print("\t FUNCTION: " + Slave[0].Function[index].Name);
                    Debug.Print("\t ---");

                    break;

                default:
                    return false;
            }

            // If everything goes well, we will arrive here and return true
            return true;
        }

        private static bool FillVendor(byte address, string[] data)
        {
            // Worst case, if vendor list size is not equal to default vendor list size
            if (data.Length != 3)
            {
                // Major code for device list
                Slave.Enqueue(new SDevice(new SVendor(), new SFunctionArray(), EHandshake.Unknown, address));
                UnknownEvent();

                return false;
            }

            // IMPORTANT NOTICE: When a new device is registered to master,
            // We are decoding all vendor Data at the here. When we are doing
            // All of these also we need temp variable
            Slave.Enqueue(new SDevice(new SVendor(data[0], data[1], data[2]), new SFunctionArray(), EHandshake.Ready, address));

            // If everything goes well, we will arrive here and return true
            return true;
        }

        private static bool FillFunction(byte address, string[] data)
        {
            // Worst case, function list size is equal to char - 1 size
            if (data.Length % 3 != 0)
                return false;

            for (ushort index = 0; index < data.Length; index += 3)
            {
                if (!IsAlphanumeric(data[index]))
                    continue;

                if (!IsNumeric(data[index + 1]))
                    continue;

                if (!IsNumeric(data[index + 2]))
                    continue;

                bool request = (data[index + 1][0] == '1' ? true : false);
                bool listen = (data[index + 2][0] == '1' ? true : false);

                Slave[0].Function.Enqueue(new SFunction(data[index], request, listen));
            }

            // If everything goes well, we will arrive here and return true
            return true;
        }

        #endregion

        #region RGB

        private static void HSVToRGB(double hue, double saturation, double value, double red, double green, double blue)
        {
            // hue parameter checking/fixing
            if (hue < 0 || hue >= 360)
                hue = 0;
            // if Brightness is turned off, then everything is zero.
            if (value <= 0)
                red = green = blue = 0;
            // if saturation is turned off, then there is no color/hue. it's grayscale.
            else if (saturation <= 0)
                red = green = blue = value;
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
                    case 6:
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
                    case -1:
                    case 5:
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

        private static double ClampRGB(double index)
        {
            if (index < 0) return 0;
            if (index > 1) return 1;
            return index;
        }

        private static void ChangeRGBStatus(SColorbook source, SColorbook target, int divider, int sleep)
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

        private static void SetRGBStatus(ENotify status)
        {
            switch (status)
            {
                case ENotify.Offline:
                    switch (NotifyFlag)
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
                    ChangeRGBStatus(ColorBlack, ColorWhite, BLINK_CLOCKRATE, 1);
                    break;

                case ENotify.Unconfirmed:
                    ChangeRGBStatus(ColorWhite, ColorOrange, BLINK_CLOCKRATE, 1);
                    break;

                case ENotify.Confirmed:
                    ChangeRGBStatus(ColorWhite, ColorBlue, BLINK_CLOCKRATE, 1);
                    break;

                default:
                    break;
            }

            // Do not forget to set RGB status
            NotifyFlag = status;
        }

        #endregion

        #region Bitwise

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

        #endregion
    }
}
