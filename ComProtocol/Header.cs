using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ComProtocol
{
    public class Header<M> where M : struct
    {
        public const int HEADER_LENGTH = 11;
        private const int INT_SIZE = 4;
        private const int TYPE_POSITION = 0;
        private const int STATE_POSITION = 1;
        private const int LENGTH_POSITION = 2;
        private const int HEADER_LENGTH_POSITION = 6;
        private const int HAS_PAYLOAD_POSITION = 10;
        public M Type { get; private set; }
        public ComState State { get; private set; }
        public int Length { get; private set; }
        public int HeaderLength { get; private set; }
        public bool HasPayload { get; private set; }

        internal Header(M type, ComState state, bool hasPayLoad = false)
            :this(type, state, 0, 0, hasPayLoad)
        {
            var bytes = GetBytes();
            Length = bytes.Length;
            HeaderLength = HEADER_LENGTH;
        }

        internal Header(M type, ComState state, int length, int headerLength, bool hasPayload = false)
        {
            Type = type;
            State = state;
            Length = length;
            HeaderLength = HEADER_LENGTH;
            HasPayload = hasPayload;
        }

        /// <summary>
        /// Parse the bytes of the header and returns the header object
        /// </summary>
        /// <param name="data">bytes of header</param>
        /// <returns>header object</returns>
        /// <exception cref="ProtocolHeaderParseException">If header cannot be parsed</exception>
        internal static Header<M> TryParse(byte[] data)
        {
            try
            {
                int typeValue = data[TYPE_POSITION];
                Enum.TryParse(typeValue.ToString(), out M type);

                int stateValue = data[STATE_POSITION];
                Enum.TryParse(stateValue.ToString(), out ComState state);

                byte[] lenghtArray = new byte[INT_SIZE];
                Array.Copy(data, LENGTH_POSITION, lenghtArray, 0, INT_SIZE);
                int length = BitConverter.ToInt32(lenghtArray, 0);

                byte[] headerLengthArrray = new byte[INT_SIZE];
                Array.Copy(data, HEADER_LENGTH_POSITION, headerLengthArrray, 0, INT_SIZE);
                int size = BitConverter.ToInt32(headerLengthArrray, 0);

                int hasPayloadValue = data[HAS_PAYLOAD_POSITION];
                var hasPayload = Convert.ToBoolean(hasPayloadValue);

                return new Header<M>(type, state, length, size, hasPayload);
            }
            catch (Exception ex)
            {
                throw new ProtocolHeaderParseException("Failed to parsed header", ex);
            }
            
        }

        /// <summary>
        /// Get the byte of the header
        /// </summary>
        /// <returns>the bytes</returns>
        internal byte[] GetBytes()
        {
            List<byte> data = new List<byte>();

            var type = Convert.ToByte(Type);
            data.Add(type);
            var state = Convert.ToByte(State);
            data.Add(state);

            //lenght could be higher then 255 so let's get the length via bit converter
            //because the we use int32 for length the get 4 bytes for the length
            var length = BitConverter.GetBytes(Length);
            data.AddRange(length);

            //same as length
            var headerLength = BitConverter.GetBytes(HeaderLength);
            data.AddRange(headerLength);

            data.Add(Convert.ToByte(HasPayload));
            
            return data.ToArray();
        }

        internal void UpdateLength(int length)
        {
            Length += length;
        }

        internal void SetHasPayload() 
        {
            HasPayload = true;
        }
    }
}
