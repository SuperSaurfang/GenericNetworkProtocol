using System;
using System.Collections.Generic;
using System.Linq;

namespace ComProtocol
{
    public class Protocol<T> where T : struct
    {
        public Header<T> Header { get; private set; }
        public PayloadBase Payload { get; private set; }

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
            if (data.Length < Header<T>.HEADER_LENGTH)
            {
                throw new ArgumentException($"Unable to parse data, data length is to short: {data.Length}");
            }
            var headerBytes = new List<byte>();
            var payloadBytes = new List<byte>();
            var index = 0;

            //parse header
            for (; index < data.Length; index++)
            {
                if (index == Header<T>.HEADER_LENGTH)
                {
                    break;
                }
                headerBytes.Add(data[index]);
            }
            Header<T> header;
            try
            {
                header = Header<T>.TryParse(headerBytes.ToArray());
            }
            catch (ProtocolHeaderParseException ex)
            {
                throw new ProtocolParseException("Unable to parse header", ex);
            }

            //simple length and size check to identify corrupted or incorrect packets
            ValidateLength(data, header);

            //check if packet has payload and parse it
            if (header.HasPayload && payload != null)
            {
                for (; index < data.Length; index++)
                {
                    payloadBytes.Add(data[index]);
                }

                try
                {
                    payload.Parse(payloadBytes.ToArray());
                }
                catch (Exception ex)
                {
                    throw new ProtocolParseException("Unable to parse payload", ex);
                }

                return new Protocol<T>(header, payload);
            }

            // Payloadless protocol
            return new Protocol<T>(header);
        }

        private static void ValidateLength(byte[] data, Header<T> header)
        {
            if (header.Length != data.Length)
            {
                throw new ProtocolLengthException(header.Length, data.Length);
            }
            var actualHeaderLength = data.Length - (header.Length - header.HeaderLength);
            if (header.HasPayload && header.HeaderLength != actualHeaderLength)
            {
                throw new ProtocolHeaderLengthException(header.HeaderLength, actualHeaderLength);
            }
        }

        public void AddPayLoad(PayloadBase payload)
        {
            var bytes = payload.GetBytes();
            Header.UpdateLength(bytes.Length);
            Header.SetHasPayload();
            Payload = payload;
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
