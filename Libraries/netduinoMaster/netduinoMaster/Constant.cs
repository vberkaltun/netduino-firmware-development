using SecretLabs.NETMF.Hardware.Netduino;
using static Microsoft.SPOT.Hardware.Cpu;

namespace netduinoMaster
{
    public class Constant
    {
        // IMPORTANT NOTICE: These all constant is related with your
        // MQTT server and WiFi protocol. In additional, at the here, we are
        // Using cloudMQTT server for communication
        protected const string MQTT_USER = "XXXXXXXX";
        protected const string MQTT_PASSWORD = "XXXXXXXX";
        protected const string MQTT_SERVER = "XXXXXXXX";
        protected const ushort MQTT_PORT = 00000;

        // IMPORTANT NOTICE: These all constant is depending on your protocol
        // As you can see, this protocol delimiter was declared in this scope
        // That's mean, all function will use this delimiter constant on
        // Communication between two or more devices
        protected const string DEVICE_BRAND = "intelliPWR Incorporated";
        protected const string DEVICE_MODEL = "MasterCore.XX";
        protected const string DEVICE_VERSION = "VER 1.0.0";

        // // IMPORTANT NOTICE: We must redeclare our bus range because of
        // Subslave of slave device. At the here, we do not need to scan all
        // These device on the bus. In any case, slave device will scan their
        // Own slave device on the bus
        protected const byte I2C_BUS_CLOCKRATE = 100;
        protected const byte I2C_BUS_TIMEOUT = 100;
        protected const byte I2C_BUS_ENDOFLINE = 255;
        protected const byte I2C_START_ADDRESS = 0x20;
        protected const byte I2C_STOP_ADDRESS = 0x65;

        // IMPORTANT NOTICE: On I2C bus, You can send up to 32 bits on
        // Each transmission. Therefore, if there is more data than 32 bits
        // We should split it. Then we can send our data to master
        protected const byte DIVISOR_NUMBER = 25;

        // IMPORTANT NOTICE: Based on buffer size of I2C bus and maximum
        // Range of your device. At the here, we declared it with 32 bit
        // Because of buffer size of Arduino but if you have a bigger buffer
        // Than 32 bit. you can upgrade and speed up your buffers
        protected const byte BUFFER_SIZE = 32;

        // Outside and Inside protocol delimiters
        protected const string PROTOCOL_DELIMITERS = "";
        protected const char DATA_DELIMITER = '';

        // Start and end type of protocol delimiters
        protected const char IDLE_SINGLE_START = '';
        protected const char IDLE_MULTI_START = '';
        protected const char IDLE_MULTI_END = '';

        // RGB led pins and solid state relay pin
        protected static readonly PWMChannel BLINK_R = PWMChannels.PWM_PIN_D9;
        protected static readonly PWMChannel BLINK_GB = PWMChannels.PWM_PIN_D10;
        protected static readonly PWMChannel SSR = PWMChannels.PWM_PIN_D11;
    }
}
