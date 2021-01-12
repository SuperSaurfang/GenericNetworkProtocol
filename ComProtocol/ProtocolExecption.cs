using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ComProtocol
{
    class ProtocolExecption : Exception 
    {
        public ProtocolExecption()
        {
        }

        public ProtocolExecption(string message) 
            : base(message)
        {
        }

        public ProtocolExecption(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ProtocolExecption(int exceptedLength, int actualLength)
        {
            ExceptedLength = exceptedLength;
            ActualLength = actualLength;
        }

        public int ExceptedLength { get; }
        public int ActualLength { get; }
    }
    class ProtocolLengthExecption : ProtocolExecption
    {
        public ProtocolLengthExecption(int exceptedLength, int actualLength) 
            : base(exceptedLength, actualLength)
        {
        }

        public override string Message => $"Protocol length difference detected, packet maybe corrupted. excepted length: {ExceptedLength}, actual length: {ActualLength}";
    }

    class ProtocolHeaderLengthException : ProtocolExecption
    {
        public ProtocolHeaderLengthException(int exceptedLength, int actualLength) 
            : base(exceptedLength, actualLength)
        {
        }

        public override string Message => $"Protocol header length difference detected, packet maybe corrupted. excepted length: {ExceptedLength}, actual length: {ActualLength}";
    }
}
