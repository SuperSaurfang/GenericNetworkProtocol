using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ComProtocol
{
    public class Header<M> where M : struct
    {
        public M Type { get; }
        public ComState State { get; }
        public int Length { get; }
        public int HeaderLength { get; }
        public bool HasPayload { get; }

        internal Header(M type, ComState state, bool hasPayLoad = false)
            :this(type, state, 0, 0, hasPayLoad)
        {
            var bytes = GetBytes();
            Length = bytes.Length;
            HeaderLength = Length;
        }

        internal Header(M type, ComState state, int length, int headerLength, bool hasPayload = false)
        {
            Type = type;
            State = state;
            Length = length;
            HeaderLength = headerLength;
            HasPayload = hasPayload;
        }

        /// <summary>
        /// Parse the bytes of the header and returns the header object
        /// </summary>
        /// <param name="data">bytes of header</param>
        /// <returns>header object</returns>
        internal static Header<M> Parse(byte[] data)
        {
            int typeValue = data[0];
            Enum.TryParse(typeValue.ToString(), out M type);

            int stateValue = data[1];
            Enum.TryParse(stateValue.ToString(), out ComState state);

            byte[] lenghtArray = new byte[4];
            Array.Copy(data, 2, lenghtArray, 0, 4);
            int length = BitConverter.ToInt32(lenghtArray, 0);

            byte[] sizeArrray = new byte[4];
            Array.Copy(data, 6, sizeArrray, 0, 4);
            int size = BitConverter.ToInt32(sizeArrray, 0);

            int hasPayloadValue = data[10];
            var hasPayload = Convert.ToBoolean(hasPayloadValue);

            return new Header<M>(type, state, length, size, hasPayload);
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
            var size = BitConverter.GetBytes(HeaderLength);
            data.AddRange(size);

            data.Add(Convert.ToByte(HasPayload));
            
            return data.ToArray();
        }

    }
}
