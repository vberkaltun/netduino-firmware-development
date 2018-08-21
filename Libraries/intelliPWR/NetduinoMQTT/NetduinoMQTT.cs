/*
 
Copyright 2011-2012 Dan Anderson. All rights reserved.
Redistribution and use in source and binary forms, with or without modification, 
are permitted provided that the following conditions are met:

   1. Redistributions of source code must retain the above copyright notice, 
      this list of conditions and the following disclaimer.

   2. Redistributions in binary form must reproduce the above copyright notice, 
      this list of conditions and the following disclaimer in the documentation 
      and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY DAN ANDERSON ''AS IS'' AND ANY EXPRESS OR IMPLIED
WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT 
SHALL DAN ANDERSON OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, 
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT 
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR 
PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR 
OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF
THE POSSIBILITY OF SUCH DAMAGE.

 */

using System;
using System.Text;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Socket = System.Net.Sockets.Socket;

namespace intelliPWR.NetduinoMQTT
{
    /// <summary>
    /// Delegate that defines event handler for received MQTT.
    /// </summary>
    public delegate void ReceivedEventHandler(string topic, string payload);

    public class NetduinoMQTT : Constant
    {
        // Checking for response of ping
        private bool pingresp = true;
        public ReceivedEventHandler OnReceived;

        // Setup our random number generator
        private Random Rand = new Random((int)(Utility.GetMachineTime().Ticks & 0xffffffff));

        #region Public

        // Connect to the MQTT Server
        public int ConnectMQTT(Socket socket, string clientID, int keepAlive = 20, bool cleanSession = true, string username = "", string password = "")
        {
            int index = 0;
            int tmp = 0;
            int remainingLength = 0;
            int fixedHeader = 0;
            int varHeader = 0;
            int payload = 0;
            int returnCode = 0;
            bool usingUsername = false;
            bool usingPassword = false;
            byte connectFlags = 0x00;
            byte[] buffer = null;
            byte[] inputBuffer = new byte[1];
            byte firstByte = 0x00;

            UTF8Encoding encoder = new UTF8Encoding();

            byte[] utf8ClientID = Encoding.UTF8.GetBytes(clientID);
            byte[] utf8Username = Encoding.UTF8.GetBytes(username);
            byte[] utf8Password = Encoding.UTF8.GetBytes(password);

            // Some Error Checking
            // ClientID improperly sized
            if ((utf8ClientID.Length > MAX_CLIENTID) || (utf8ClientID.Length < MIN_CLIENTID))
                return CLIENTID_LENGTH_ERROR;
            // KeepAlive out of bounds
            if ((keepAlive > MAX_KEEPALIVE) || (keepAlive < MIN_KEEPALIVE))
                return KEEPALIVE_LENGTH_ERROR;
            // Username too long
            if (utf8Username.Length > MAX_USERNAME)
                return USERNAME_LENGTH_ERROR;
            // Password too long
            if (utf8Password.Length > MAX_PASSWORD)
                return PASSWORD_LENGTH_ERROR;

            // Check features being used
            if (!username.Equals(""))
                usingUsername = true;
            if (!password.Equals(""))
                usingPassword = true;

            // Calculate the size of the var header
            varHeader += 2; // Protocol Name Length
            varHeader += 6; // Protocol Name
            varHeader++; // Protocol version
            varHeader++; // Connect Flags
            varHeader += 2; // Keep Alive

            // Calculate the size of the fixed header
            fixedHeader++; // byte 1

            // Calculate the payload
            payload = utf8ClientID.Length + 2;
            if (usingUsername)
            {
                payload += utf8Username.Length + 2;
            }
            if (usingPassword)
            {
                payload += utf8Password.Length + 2;
            }

            // Calculate the remaining size
            remainingLength = varHeader + payload;

            // Check that remaining length will fit into 4 encoded bytes
            if (remainingLength > MAXLENGTH)
                return MESSAGE_LENGTH_ERROR;

            tmp = remainingLength;

            // Add space for each byte we need in the fixed header to store the length
            while (tmp > 0)
            {
                fixedHeader++;
                tmp = tmp / 128;
            };
            // End of Fixed Header

            // Build buffer for message
            buffer = new byte[fixedHeader + varHeader + payload];

            // Fixed Header (2.1)
            buffer[index++] = MQTT_CONNECT_TYPE;

            // Encode the fixed header remaining length
            // Add remaining length
            index = DoRemainingLength(remainingLength, index, buffer);
            // End Fixed Header

            // Connect (3.1)
            // Protocol Name
            buffer[index++] = 0; // string (MQIsdp) Length MSB - always 6 so, zeroed
            buffer[index++] = 6; // Length LSB
            buffer[index++] = (byte)'M'; // M
            buffer[index++] = (byte)'Q'; // Q
            buffer[index++] = (byte)'I'; // I
            buffer[index++] = (byte)'s'; // s
            buffer[index++] = (byte)'d'; // d
            buffer[index++] = (byte)'p'; // p

            // Protocol Version
            buffer[index++] = MQTTPROTOCOLVERSION;

            // Connect Flags
            if (cleanSession)
                connectFlags |= (byte)CLEAN_SESSION_FLAG;
            if (usingUsername)
                connectFlags |= (byte)USING_USERNAME_FLAG;
            if (usingPassword)
                connectFlags |= (byte)USING_PASSWORD_FLAG;

            // Set the connect flags
            buffer[index++] = connectFlags;

            // Keep alive (defaulted to 20 seconds above)
            buffer[index++] = (byte)(keepAlive / 256); // Keep Alive MSB
            buffer[index++] = (byte)(keepAlive % 256); // Keep Alive LSB

            // ClientID
            buffer[index++] = (byte)(utf8ClientID.Length / 256); // Length MSB
            buffer[index++] = (byte)(utf8ClientID.Length % 256); // Length LSB
            for (var i = 0; i < utf8ClientID.Length; i++)
                buffer[index++] = utf8ClientID[i];

            // Username
            if (usingUsername)
            {
                buffer[index++] = (byte)(utf8Username.Length / 256); // Length MSB
                buffer[index++] = (byte)(utf8Username.Length % 256); // Length LSB

                for (var i = 0; i < utf8Username.Length; i++)
                    buffer[index++] = utf8Username[i];
            }

            // Password
            if (usingPassword)
            {
                buffer[index++] = (byte)(utf8Password.Length / 256); // Length MSB
                buffer[index++] = (byte)(utf8Password.Length % 256); // Length LSB

                for (var i = 0; i < utf8Password.Length; i++)
                    buffer[index++] = utf8Password[i];
            }

            // Send the message
            returnCode = socket.Send(buffer, index, 0);

            // The return code should equal our buffer length
            if (returnCode != buffer.Length)
                return CONNECTION_ERROR;

            // Get the acknowledgment message
            returnCode = socket.Receive(inputBuffer, 0);

            if (returnCode < 1)
                return CONNECTION_ERROR;

            firstByte = inputBuffer[0];

            // If this is the CONNACK - pass it to the CONNACK handler
            if (((int)firstByte & MQTT_CONNACK_TYPE) > 0)
            {
                returnCode = HandleCONNACK(socket, firstByte);
                if (returnCode > 0)
                    return ERROR;
            }
            return SUCCESS;
        }

        // Publish a message to a broker (3.3)
        public int PublishMQTT(Socket socket, string topic, string message)
        {
            int index = 0;
            int tmp = 0;
            int fixedHeader = 0;
            int varHeader = 0;
            int payload = 0;
            int remainingLength = 0;
            int returnCode = 0;
            byte[] buffer = null;

            // Setup a UTF8 encoder
            UTF8Encoding encoder = new UTF8Encoding();

            // Encode the topic
            byte[] utf8Topic = Encoding.UTF8.GetBytes(topic);

            // Some error checking
            // Topic contains wildcards
            if ((topic.IndexOf('#') != -1) || (topic.IndexOf('+') != -1))
                return TOPIC_WILDCARD_ERROR;

            // Topic is too long or short
            if ((utf8Topic.Length > MAX_TOPIC_LENGTH) || (utf8Topic.Length < MIN_TOPIC_LENGTH))
                return TOPIC_LENGTH_ERROR;

            // Calculate the size of the var header
            varHeader += 2; // Topic Name Length (MSB, LSB)
            varHeader += utf8Topic.Length; // Length of the topic

            // Calculate the size of the fixed header
            fixedHeader++; // byte 1

            // Calculate the payload
            payload = message.Length;

            // Calculate the remaining size
            remainingLength = varHeader + payload;

            // Check that remaining length will fit into 4 encoded bytes
            if (remainingLength > MAXLENGTH)
                return MESSAGE_LENGTH_ERROR;

            // Add space for each byte we need in the fixed header to store the length
            tmp = remainingLength;
            while (tmp > 0)
            {
                fixedHeader++;
                tmp = tmp / 128;
            };
            // End of Fixed Header

            // Build buffer for message
            buffer = new byte[fixedHeader + varHeader + payload];

            // Start of Fixed header
            // Publish (3.3)
            buffer[index++] = MQTT_PUBLISH_TYPE;

            // Encode the fixed header remaining length
            // Add remaining length
            index = DoRemainingLength(remainingLength, index, buffer);
            // End Fixed Header

            // Start of Variable header
            // Length of topic name
            buffer[index++] = (byte)(utf8Topic.Length / 256); // Length MSB
            buffer[index++] = (byte)(utf8Topic.Length % 256); // Length LSB
            // Topic
            for (var i = 0; i < utf8Topic.Length; i++)
                buffer[index++] = utf8Topic[i];
            // End of variable header

            // Start of Payload
            // Message (Length is accounted for in the fixed header)
            for (var i = 0; i < message.Length; i++)
                buffer[index++] = (byte)message[i];
            // End of Payload

            returnCode = socket.Send(buffer, buffer.Length, 0);

            if (returnCode < buffer.Length)
                return CONNECTION_ERROR;

            return SUCCESS;
        }

        // Disconnect from broker (3.14)
        public int DisconnectMQTT(Socket socket)
        {
            byte[] buffer = null;
            int returnCode = 0;
            buffer = new byte[2];
            buffer[0] = MQTT_DISCONNECT_TYPE;
            buffer[1] = 0x00;
            returnCode = socket.Send(buffer, buffer.Length, 0);

            if (returnCode < buffer.Length)
                return CONNECTION_ERROR;

            return SUCCESS; ;
        }

        // Subscribe to a topic 
        public int SubscribeMQTT(Socket socket, string[] topic, int[] QoS, int topics)
        {
            int index = 0;
            int index2 = 0;
            int messageIndex = 0;
            int messageID = 0;
            int tmp = 0;
            int fixedHeader = 0;
            int varHeader = 0;
            int payloadLength = 0;
            int remainingLength = 0;
            int returnCode = 0;
            byte[] buffer = null;
            byte[][] utf8Topics = null;

            UTF8Encoding encoder = new UTF8Encoding();

            utf8Topics = new byte[topics][];

            while (index < topics)
            {
                utf8Topics[index] = new byte[Encoding.UTF8.GetBytes(topic[index]).Length];
                utf8Topics[index] = Encoding.UTF8.GetBytes(topic[index]);
                if ((utf8Topics[index].Length > MAX_TOPIC_LENGTH) || (utf8Topics[index].Length < MIN_TOPIC_LENGTH))
                    return TOPIC_LENGTH_ERROR;
                else
                {
                    payloadLength += 2; // Size (LSB + MSB)
                    payloadLength += utf8Topics[index].Length;  // Length of topic
                    payloadLength++; // QoS Requested
                    index++;
                }
            }

            // Calculate the size of the fixed header
            fixedHeader++; // byte 1

            // Calculate the size of the var header
            varHeader += 2; // Message ID is 2 bytes

            // Calculate the remaining size
            remainingLength = varHeader + payloadLength;

            // Check that remaining encoded length will fit into 4 encoded bytes
            if (remainingLength > MAXLENGTH)
                return MESSAGE_LENGTH_ERROR;

            // Add space for each byte we need in the fixed header to store the length
            tmp = remainingLength;
            while (tmp > 0)
            {
                fixedHeader++;
                tmp = tmp / 128;
            };

            // Build buffer for message
            buffer = new byte[fixedHeader + varHeader + payloadLength];

            // Start of Fixed header
            // Publish (3.3)
            buffer[messageIndex++] = MQTT_SUBSCRIBE_TYPE;

            // Add remaining length
            messageIndex = DoRemainingLength(remainingLength, messageIndex, buffer);
            // End Fixed Header

            // Start of Variable header
            // Message ID
            messageID = Rand.Next(MAX_MESSAGEID);
            buffer[messageIndex++] = (byte)(messageID / 256); // Length MSB
            buffer[messageIndex++] = (byte)(messageID % 256); // Length LSB
            // End of variable header

            // Start of Payload
            index = 0;
            while (index < topics)
            {
                // Length of Topic
                buffer[messageIndex++] = (byte)(utf8Topics[index].Length / 256); // Length MSB 
                buffer[messageIndex++] = (byte)(utf8Topics[index].Length % 256); // Length LSB 

                index2 = 0;
                while (index2 < utf8Topics[index].Length)
                {
                    buffer[messageIndex++] = utf8Topics[index][index2];
                    index2++;
                }
                buffer[messageIndex++] = (byte)(QoS[index]);
                index++;
            }
            // End of Payload

            returnCode = socket.Send(buffer, buffer.Length, 0);

            if (returnCode < buffer.Length)
                return CONNECTION_ERROR;

            return SUCCESS;
        }

        // Unsubscribe to a topic
        public int UnsubscribeMQTT(Socket socket, string[] topic, int[] QoS, int topics)
        {
            int index = 0;
            int index2 = 0;
            int messageIndex = 0;
            int messageID = 0;
            int tmp = 0;
            int fixedHeader = 0;
            int varHeader = 0;
            int payloadLength = 0;
            int remainingLength = 0;
            int returnCode = 0;
            byte[] buffer = null;
            byte[][] utf8Topics = null;

            UTF8Encoding encoder = new UTF8Encoding();

            utf8Topics = new byte[topics][];

            while (index < topics)
            {
                utf8Topics[index] = new byte[Encoding.UTF8.GetBytes(topic[index]).Length];
                utf8Topics[index] = Encoding.UTF8.GetBytes(topic[index]);
                if ((utf8Topics[index].Length > MAX_TOPIC_LENGTH) || (utf8Topics[index].Length < MIN_TOPIC_LENGTH))
                    return TOPIC_LENGTH_ERROR;
                else
                {
                    payloadLength += 2; // Size (LSB + MSB)
                    payloadLength += utf8Topics[index].Length;  // Length of topic
                    index++;
                }
            }

            // Calculate the size of the fixed header
            fixedHeader++; // byte 1

            // Calculate the size of the var header
            varHeader += 2; // Message ID is 2 bytes

            // Calculate the remaining size
            remainingLength = varHeader + payloadLength;

            // Check that remaining encoded length will fit into 4 encoded bytes
            if (remainingLength > MAXLENGTH)
                return MESSAGE_LENGTH_ERROR;

            // Add space for each byte we need in the fixed header to store the length
            tmp = remainingLength;
            while (tmp > 0)
            {
                fixedHeader++;
                tmp = tmp / 128;
            };

            // Build buffer for message
            buffer = new byte[fixedHeader + varHeader + payloadLength];

            // Start of Fixed header
            // Publish (3.3)
            buffer[messageIndex++] = MQTT_UNSUBSCRIBE_TYPE;

            // Add remaining length - writes to buffer, so need to get the new index back
            messageIndex = DoRemainingLength(remainingLength, messageIndex, buffer);
            // End Fixed Header

            // Start of Variable header
            // Message ID
            messageID = Rand.Next(MAX_MESSAGEID);
            buffer[messageIndex++] = (byte)(messageID / 256); // Length MSB
            buffer[messageIndex++] = (byte)(messageID % 256); // Length LSB
            // End of variable header

            // Start of Payload
            index = 0;
            while (index < topics)
            {
                // Length of Topic
                buffer[messageIndex++] = (byte)(utf8Topics[index].Length / 256); // Length MSB 
                buffer[messageIndex++] = (byte)(utf8Topics[index].Length % 256); // Length LSB 

                index2 = 0;
                while (index2 < utf8Topics[index].Length)
                {
                    buffer[messageIndex++] = utf8Topics[index][index2];
                    index2++;
                }
                index++;
            }
            // End of Payload

            returnCode = socket.Send(buffer, buffer.Length, 0);

            if (returnCode < buffer.Length)
                return CONNECTION_ERROR;

            return SUCCESS;
        }

        // Respond to a PINGRESP
        private int SendPINGRESP(Socket socket)
        {
            int index = 0;
            int returnCode = 0;
            byte[] buffer = new byte[2];

            buffer[index++] = MQTT_PING_RESP_TYPE;
            buffer[index++] = 0x00;

            // Send the ping
            returnCode = socket.Send(buffer, index, 0);
            // The return code should equal our buffer length
            if (returnCode != buffer.Length)
                return CONNECTION_ERROR;
            return SUCCESS;
        }

        // Ping the MQTT broker - used to extend keep alive
        public int PingMQTT(Socket socket)
        {
            if (pingresp)
                pingresp = false;
            else
                return -1;

            int index = 0;
            int returnCode = 0;
            byte[] buffer = new byte[2];

            buffer[index++] = MQTT_PING_REQ_TYPE;
            buffer[index++] = 0x00;

            // Send the ping
            returnCode = socket.Send(buffer, index, 0);
            // The return code should equal our buffer length
            if (returnCode != buffer.Length)
                return CONNECTION_ERROR;
            return SUCCESS;
        }

        // Listen for data on the socket - call appropriate handlers based on first byte
        public void Listen(Socket socket)
        {
            int returnCode = 0;
            byte first = 0x00;
            byte[] buffer = new byte[1];
            returnCode = socket.Receive(buffer, 0);
            if (returnCode > 0)
            {
                first = buffer[0];
                switch (first >> 4)
                {
                    case 0:  // RESERVED
                        Debug.Print("Done! First reserved message received.");
                        returnCode = ERROR;
                        break;
                    case 1:  // Connect (Broker Only)
                        Debug.Print("Done! CONNECT message received.");
                        returnCode = ERROR;
                        break;
                    case 2:  // CONNACK
                        Debug.Print("Done! CONNACK message received.");
                        returnCode = HandleCONNACK(socket, first);
                        break;
                    case 3:  // PUBLISH
                        Debug.Print("Done! PUBLISH message received.");
                        returnCode = HandlePUBLISH(socket, first);
                        break;
                    case 4:  // PUBACK (QoS > 0 - did it anyway)
                        Debug.Print("Done! PUBACK message received.");
                        returnCode = HandlePUBACK(socket, first);
                        break;
                    case 5:  // PUBREC (QoS 2)
                        Debug.Print("Done! PUBREC message received.");
                        returnCode = ERROR;
                        break;
                    case 6:  // PUBREL (QoS 2)
                        Debug.Print("Done! PUBREL message received.");
                        returnCode = ERROR;
                        break;
                    case 7:  // PUBCOMP (QoS 2)
                        Debug.Print("Done! PUBCOMP message received.");
                        returnCode = ERROR;
                        break;
                    case 8:  // SUBSCRIBE (Broker only)
                        Debug.Print("Done! SUBSCRIBE message received.");
                        returnCode = ERROR;
                        break;
                    case 9:  // SUBACK 
                        Debug.Print("Done! SUBACK message received.");
                        returnCode = HandleSUBACK(socket, first);
                        break;
                    case 10:  // UNSUBSCRIBE (Broker Only)
                        Debug.Print("Done! UNSUBSCRIBE message received.");
                        returnCode = ERROR;
                        break;
                    case 11:  // UNSUBACK
                        Debug.Print("Done! UNSUBACK message received.");
                        returnCode = HandleUNSUBACK(socket, first);
                        break;
                    case 12:  // PINGREQ (Technically a Broker Deal - but we're doing it anyway)
                        Debug.Print("Done! PINGREQ message received.");
                        returnCode = HandlePINGREQ(socket, first);
                        break;
                    case 13:  // PINGRESP
                        Debug.Print("Done! PINGRESP message received.");
                        pingresp = true;
                        returnCode = HandlePINGRESP(socket, first);
                        break;
                    case 14:  // DISCONNECT (Broker Only)
                        Debug.Print("Done! DISCONNECT message received.");
                        returnCode = ERROR;
                        break;
                    case 15:  // RESERVED
                        Debug.Print("Done! Last reserved Message received.");
                        returnCode = ERROR;
                        break;
                    default:  // Default action
                        Debug.Print("Done! Unknown message received.");  // Should never get here
                        returnCode = ERROR;
                        break;
                }
                if (returnCode != SUCCESS)
                    Debug.Print("Error! An error occurred in message processing on <" + socket.ToString() + "> port.");
            }
        }

        #endregion

        #region Private

        // Append the remaining length field for the fixed header
        private int DoRemainingLength(int remainingLength, int index, byte[] buffer)
        {
            int digit = 0;
            do
            {
                digit = remainingLength % 128;
                remainingLength /= 128;
                if (remainingLength > 0)
                {
                    digit = digit | CONTINUATION_BIT;
                }
                buffer[index++] = (byte)digit;
            } while (remainingLength > 0);
            return index;
        }

        // Extract the remaining length field from the fixed header
        private int UndoRemainingLength(Socket socket)
        {
            int multiplier = 1;
            int count = 0;
            int digit = 0;
            int remainingLength = 0;
            byte[] nextByte = new byte[1];
            do
            {
                if (socket.Receive(nextByte, 0) == 1)
                {
                    digit = (byte)nextByte[0];
                    remainingLength += ((digit & 0x7F) * multiplier);
                    multiplier *= 128;
                }
                count++;
            } while (((digit & 0x80) != 0) && count < 4);
            return remainingLength;
        }

        #endregion

        #region Handle

        // Messages from the broker come back to us as publish messages
        private int HandleSUBACK(Socket socket, byte firstByte)
        {
            int remainingLength = 0;
            int messageID = 0;
            int index = 0;
            int QoSIndex = 0;
            int[] QoS = null;
            byte[] buffer = null;
            remainingLength = UndoRemainingLength(socket);
            buffer = new byte[remainingLength];
            if ((socket.Receive(buffer, 0) != remainingLength) || remainingLength < 3)
                return ERROR;
            messageID += buffer[index++] * 256;
            messageID += buffer[index++];
            do
            {
                QoS = new int[remainingLength - 2];
                QoS[QoSIndex++] = buffer[index++];
            } while (index < remainingLength);
            return SUCCESS;
        }

        // Messages from the broker come back to us as publish messages
        private int HandlePUBLISH(Socket socket, byte firstByte)
        {
            int remainingLength = 0;
            int messageID = 0;
            int topicLength = 0;
            int topicIndex = 0;
            int payloadIndex = 0;
            int index = 0;
            byte[] buffer = null;
            byte[] topic = null;
            byte[] payload = null;
            int QoS = 0x00;
            string topicstring = null;
            string payloadstring = null;

            remainingLength = UndoRemainingLength(socket);
            buffer = new byte[remainingLength];
            if ((socket.Receive(buffer, 0) != remainingLength) || remainingLength < 5)
                return ERROR;
            topicLength += buffer[index++] * 256;
            topicLength += buffer[index++];
            topic = new byte[topicLength];
            while (topicIndex < topicLength)
                topic[topicIndex++] = buffer[index++];
            QoS = firstByte & 0x06;
            if (QoS > 0)
            {
                messageID += buffer[index++] * 256;
                messageID += buffer[index++];
            }
            topicstring = new string(Encoding.UTF8.GetChars(topic));
            payload = new byte[remainingLength - index];
            while (index < remainingLength)
                payload[payloadIndex++] = buffer[index++];

            // This doesn't work if the payload isn't UTF8
            payloadstring = new string(Encoding.UTF8.GetChars(payload));

            // don't bother if user hasn't registered a callback
            if (OnReceived != null)
                OnReceived(topicstring, payloadstring);

            return SUCCESS;
        }

        // Messages from the broker come back to us as publish messages
        private int HandleUNSUBACK(Socket socket, byte firstByte)
        {
            int returnCode = 0;
            int messageID = 0;
            byte[] buffer = new byte[3];
            returnCode = socket.Receive(buffer, 0);
            if ((buffer[0] != 2) || (returnCode != 3))
                return ERROR;
            messageID += buffer[1] * 256;
            messageID += buffer[2];
            return SUCCESS;
        }

        // Ping response - this should be a total of 2 bytes - that's pretty much all I'm looking for
        private int HandlePINGRESP(Socket socket, byte firstByte)
        {
            int returnCode = 0;
            byte[] buffer = new byte[1];
            returnCode = socket.Receive(buffer, 0);
            if ((buffer[0] != 0) || (returnCode != 1))
                return ERROR;
            return SUCCESS;
        }

        // Ping Request 
        private int HandlePINGREQ(Socket socket, byte firstByte)
        {
            int returnCode = 0;
            byte[] buffer = new byte[1];
            returnCode = socket.Receive(buffer, 0);
            if ((returnCode != 1) || (buffer[0] != 0))
                return ERROR;
            returnCode = SendPINGRESP(socket);
            if (returnCode != 0)
                return ERROR;
            return SUCCESS;
        }

        // Connect acknowledgment - returns 3 more bytes, byte 3 should be 0 for success
        private int HandleCONNACK(Socket socket, byte firstByte)
        {
            int returnCode = 0;
            byte[] buffer = new byte[3];
            returnCode = socket.Receive(buffer, 0);
            if ((buffer[0] != 2) || (buffer[2] > 0) || (returnCode != 3))
                return ERROR;
            return SUCCESS;
        }

        // We're not doing QoS 1 yet, so this is just here for flushing 
        private int HandlePUBACK(Socket socket, byte firstByte)
        {
            int returnCode = 0;
            int messageID = 0;
            byte[] buffer = new byte[3];
            returnCode = socket.Receive(buffer, 0);
            if ((buffer[0] != 2) || (returnCode != 3))
                return ERROR;
            messageID += buffer[1] * 256;
            messageID += buffer[2];
            return SUCCESS;
        }

        #endregion
    }
}