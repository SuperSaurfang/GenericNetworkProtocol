using System;
using System.Collections.Generic;
using System.Text;

namespace ComProtocol
{
    public abstract class PayloadBase
    {  
        /// <summary>
        /// Parse the byte array into the generic payload class
        /// </summary>
        /// <param name="bytes">the Bytes to parse</param>
        /// <returns>the parses payload</returns>
        public abstract PayloadBase Parse(byte[] bytes);

        /// <summary>
        /// Get the byte array of the payload
        /// </summary>
        /// <returns>the bytes of the payload</returns>
        public abstract byte[] GetBytes();
        

    }
}
