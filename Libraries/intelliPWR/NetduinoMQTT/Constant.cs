namespace intelliPWR.NetduinoMQTT
{
    public class Constant
    {
        // IMPORTANT NOTICE: All these constants have been scaled down to the 
        // Hardware maximum values are commented out. You can adjust, but keep 
        // In mind the limits of the hardware. So, All update at here is your 
        // Own risk about your hardware
        protected const int MQTTPROTOCOLVERSION = 3;
        protected const int MAXLENGTH = 10240;
        protected const int MAX_CLIENTID = 23;
        protected const int MIN_CLIENTID = 1;
        protected const int MAX_KEEPALIVE = 65535;
        protected const int MIN_KEEPALIVE = 0;
        protected const int MAX_USERNAME = 12;
        protected const int MAX_PASSWORD = 12;
        protected const int MAX_TOPIC_LENGTH = 256;
        protected const int MIN_TOPIC_LENGTH = 1;
        protected const int MAX_MESSAGEID = 65535;

        // Error codes of MQTT Clients
        protected const int CLIENTID_LENGTH_ERROR = 1;
        protected const int KEEPALIVE_LENGTH_ERROR = 1;
        protected const int MESSAGE_LENGTH_ERROR = 1;
        protected const int TOPIC_LENGTH_ERROR = 1;
        protected const int TOPIC_WILDCARD_ERROR = 1;
        protected const int USERNAME_LENGTH_ERROR = 1;
        protected const int PASSWORD_LENGTH_ERROR = 1;
        protected const int CONNECTION_ERROR = 1;
        protected const int ERROR = 1;
        protected const int SUCCESS = 0;
        protected const int CONNECTION_OK = 0;
        protected const int CONNACK_LENGTH = 4;
        protected const int PINGRESP_LENGTH = 2;

        // Connection codes of MQTT Clients
        protected const byte MQTT_CONN_OK = 0x00;
        protected const byte MQTT_CONN_BAD_PROTOCOL_VERSION = 0x01;
        protected const byte MQTT_CONN_BAD_IDENTIFIER = 0x02;
        protected const byte MQTT_CONN_SERVER_UNAVAILABLE = 0x03;
        protected const byte MQTT_CONN_BAD_AUTH = 0x04;
        protected const byte MQTT_CONN_NOT_AUTH = 0x05;

        // Message types of MQTT Clients
        protected const byte MQTT_CONNECT_TYPE = 0x10;
        protected const byte MQTT_CONNACK_TYPE = 0x20;
        protected const byte MQTT_PUBLISH_TYPE = 0x30;
        protected const byte MQTT_PING_REQ_TYPE = 0xc0;
        protected const byte MQTT_PING_RESP_TYPE = 0xd0;
        protected const byte MQTT_DISCONNECT_TYPE = 0xe0;
        protected const byte MQTT_SUBSCRIBE_TYPE = 0x82;
        protected const byte MQTT_UNSUBSCRIBE_TYPE = 0xa2;

        // Flags of MQTT Clients
        protected const int CLEAN_SESSION_FLAG = 0x02;
        protected const int USING_USERNAME_FLAG = 0x80;
        protected const int USING_PASSWORD_FLAG = 0x40;
        protected const int CONTINUATION_BIT = 0x80;
    }
}
