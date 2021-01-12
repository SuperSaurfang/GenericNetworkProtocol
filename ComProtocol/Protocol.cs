using System;
using System.Collections.Generic;
using System.Linq;

namespace ComProtocol
{
    public class Protocol<T> where T : struct
    {
        public Header<T> Header { get; }
        public PayloadBase Payload { get; }

        private Protocol(Header<T> header, PayloadBase payload = null) 
        {
            Header = header;
            if(payload != null) 
            {
                Payload = payload;
            }
        }

        /// <summary>
        /// Start create a new protocol with this method a new Protocol has only a header
        /// but not a payload call AddPayload method to a Payload
        /// </summary>
        /// <param name="type">The message type</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static Protocol<T> CreateHeader(T type, ComState state)
        {
            var header = new Header<T>(type, state);
            return new Protocol<T>(header);
        }

        /// <summary>
        /// Parse an incomming packet into a protocol object
        /// </summary>
        /// <param name="data">the incomming byte array</param>
        /// <returns>the protocol</returns>
        public static Protocol<T> Parse(byte[] data, PayloadBase payload = null)
        {
            var headerBytes = new List<byte>();
            var payloadBytes = new List<byte>();
            var index = 0;

            //parse header
            for (; index < data.Length; index++)
            {
                if (index == 11)
                {
                    break;
                }
                headerBytes.Add(data[index]);
            }
            var header = Header<T>.Parse(headerBytes.ToArray());

            //simple length and size check to identify corrupted or incorrect packets
            if(header.Length != data.Length)
            {
                throw new ProtocolLengthExecption(header.Length, data.Length);
            }
            var actualHeaderLength = data.Length - (header.Length - header.HeaderLength);
            if(header.HasPayload && header.HeaderLength != actualHeaderLength)
            {
                throw new ProtocolHeaderLengthException(header.HeaderLength, actualHeaderLength);
            }

            //check if packet has payload
            if (header.HasPayload && payload != null)
            {
                for (; index < data.Length; index++)
                {
                    payloadBytes.Add(data[index]);
                }
                payload.Parse(payloadBytes.ToArray());

                return new Protocol<T>(header, payload);
            }

            // Payloadless protocol
            return new Protocol<T>(header);
        }

        private void UpdateLength(ref Protocol<T> protocol, int length)
        {
            var newLength = protocol.Header.Length + length;
            if (protocol.Payload != null)
            {
                var header = new Header<T>(protocol.Header.Type, protocol.Header.State, newLength, protocol.Header.HeaderLength, true);
                var payload = protocol.Payload.Parse(protocol.Payload.GetBytes());
                protocol = new Protocol<T>(header, payload);
            }
            else
            {
                var header = new Header<T>(protocol.Header.Type, protocol.Header.State, newLength, protocol.Header.HeaderLength);
                protocol = new Protocol<T>(header);
            }
        }

        public void AddPayLoad(ref Protocol<T> protocol, PayloadBase payload)
        {
            var bytes = payload.GetBytes();
            UpdateLength(ref protocol, bytes.Length);
            var header = new Header<T>(protocol.Header.Type, protocol.Header.State, protocol.Header.Length, protocol.Header.HeaderLength, true);
            var newPayload = payload.Parse(bytes);
            protocol = new Protocol<T>(header, newPayload);
        }

        /// <summary>
        /// Get the Byte of the protocol
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            List<byte> data = new List<byte>();
            var headerBytes = Header.GetBytes();
            data.AddRange(headerBytes);

            if (Payload != null) 
            {
                var payloadBytes = Payload.GetBytes();
                data.AddRange(payloadBytes);
            }
            return data.ToArray();
        }
 
    }
}
