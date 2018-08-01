namespace netduinoMaster
{
    public static class Core
    {
        #region Constant

        // IMPORTANT NOTICE: These all constant is related with your
        // MQTT server and WiFi protocol. In additional, at the here, we are
        // Using cloudMQTT server for communication
        public const string MQTT_USER = "XXXXXXXX";
        public const string MQTT_PASSWORD = "XXXXXXXX";
        public const string MQTT_SERVER = "XXXXXXXX";
        public const ushort MQTT_PORT = 00000;

        // IMPORTANT NOTICE: These all constant is depending on your protocol
        // As you can see, this protocol delimiter was declared in this scope
        // That's mean, all function will use this delimiter constant on
        // Communication between two or more devices
        public const string DEVICE_BRAND = "intelliPWR Incorporated";
        public const string DEVICE_MODEL = "MasterCore.XX";
        public const string DEVICE_VERSION = "VER 1.0.0";

        // // IMPORTANT NOTICE: We must redeclare our bus range because of
        // Subslave of slave device. At the here, we do not need to scan all
        // These device on the bus. In any case, slave device will scan their
        // Own slave device on the bus
        public const byte I2C_BUS_SDA = 5;
        public const byte I2C_BUS_SCL = 4;
        public const byte I2C_START_ADDRESS = 0x20;
        public const byte I2C_STOP_ADDRESS = 0x65;

        // IMPORTANT NOTICE: On I2C bus, You can send up to 32 bits on
        // Each transmission. Therefore, if there is more data than 32 bits
        // We should split it. Then we can send our data to master
        public const byte DIVISOR_NUMBER = 25;

        // IMPORTANT NOTICE: Based on buffer size of I2C bus and maximum
        // Range of your device. At the here, we declared it with 32 bit
        // Because of buffer size of Arduino but if you have a bigger buffer
        // Than 32 bit. you can upgrade and speed up your buffers
        public const byte BUFFER_SIZE = 32;

        // IMPORTANT NOTICE: If buffer size is not looking enough for you, 
        // You can extend or shrink your data with this variable. Due to lack 
        // Of resources on memory, we were setted it as 8 but if you have more 
        // Memory on your device, bigger value can be compatible
        public const byte MINIMIZED_BUFFER_SIZE = 16;

        // Outside and Inside protocol delimiters
        public const string PROTOCOL_DELIMITERS = "";
        public const string DATA_DELIMITER = "";

        // Start and end type of protocol delimiters
        public const byte IDLE_SINGLE_START = 0x15;
        public const byte IDLE_MULTI_START = 0x16;
        public const byte IDLE_MULTI_END = 0x17;

        #endregion

        #region Enumeration

        public enum EHandshake { Unknown, Ready };
        public enum ECommunication { Idle, Continue, End };
        public enum ENotify { Offline, Online, Unconfirmed, Confirmed };

        #endregion

        #region Structure

        public struct SVendor
        {
            string Brand;
            string Model;
            string Version;
        };

        public struct SFunction
        {
            string Name;
            bool Request;
            ushort Listen;
        };

        public struct SDevice
        {
            SVendor Vendor;
            SFunction[] Function;
            EHandshake Handshake;
            char Address;
        };

        public struct SMaster
        {
            SFunction[] Function;
            string Receive;
            string Transmit;
        }

        #endregion

        #region Variable

        public static SMaster Master;
        public static SDevice[] Device;
        public static ECommunication Communication;
        public static ENotify Notify;

        #endregion

        #region Extension > SDevice

        /// <summary>
        /// Removes all objects from the Queue.
        /// </summary>
        /// <param name="value"></param>
        public static void Clear(this SDevice[] value) {}

        /// <summary>
        /// Determines whether an element is in the Queue.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="device">The Object to locate in the Queue. The value can be null.</param>
        /// <returns>true if obj is found in the Queue; otherwise, false.</returns>
        public static bool Contain(this SDevice[] value, SDevice device) { }

        /// <summary>
        /// Adds an object to the end of the Queue.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="device">The object to add to the Queue. The value can be null.</param>
        public static void Enqueue(this SDevice[] value, SDevice device) { }

        /// <summary>
        /// Removes and returns the object at the beginning of the Queue.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The object that is removed from the beginning of the Queue.</returns>
        public static SDevice Dequeue(this SDevice[] value) { }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="device">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public static bool Equal(this SDevice[] value,SDevice device) { }

        /// <summary>
        /// Returns the object at the beginning of the Queue without removing it.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The object at the beginning of the Queue.</returns>
        public static SDevice Peek(this SDevice[] value) { }

        #endregion

        #region Extension > SFunction

        /// <summary>
        /// Removes all objects from the Queue.
        /// </summary>
        /// <param name="value"></param>
        public static void Clear(this SFunction[] value) { }

        /// <summary>
        /// Determines whether an element is in the Queue.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="function">The Object to locate in the Queue. The value can be null.</param>
        /// <returns>true if obj is found in the Queue; otherwise, false.</returns>
        public static bool Contain(this SFunction[] value, SFunction function) { }

        /// <summary>
        /// Adds an object to the end of the Queue.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="function">The object to add to the Queue. The value can be null.</param>
        public static void Enqueue(this SFunction[] value, SFunction function) { }

        /// <summary>
        /// Removes and returns the object at the beginning of the Queue.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The object that is removed from the beginning of the Queue.</returns>
        public static SFunction Dequeue(this SFunction[] value) { }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="function">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public static bool Equal(this SFunction[] value, SFunction function) { }

        /// <summary>
        /// Returns the object at the beginning of the Queue without removing it.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The object at the beginning of the Queue.</returns>
        public static SFunction Peek(this SFunction[] value) { }

        #endregion

    }
}
