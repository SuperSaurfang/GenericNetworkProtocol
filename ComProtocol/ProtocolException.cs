using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ComProtocol
{
    class ProtocolException : Exception 
    {
        public ProtocolException()
        {
        }

        public ProtocolException(string message) 
            : base(message)
        {
        }

        public ProtocolException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ProtocolException(int exceptedLength, int actualLength)
        {
            ExceptedLength = exceptedLength;
            ActualLength = actualLength;
        }

        public int ExceptedLength { get; }
        public int ActualLength { get; }
    }
    class ProtocolLengthException : ProtocolException
    {
        public ProtocolLengthException(int exceptedLength, int actualLength) 
            : base(exceptedLength, actualLength)
        {
        }

        public override string Message => $"Protocol length difference detected, packet maybe corrupted. excepted length: {ExceptedLength}, actual length: {ActualLength}";
    }
    class ProtocolHeaderLengthException : ProtocolException
    {
        public ProtocolHeaderLengthException(int exceptedLength, int actualLength) 
            : base(exceptedLength, actualLength)
        {
        }

        public override string Message => $"Protocol header length difference detected, packet maybe corrupted. excepted length: {ExceptedLength}, actual length: {ActualLength}";
    }
    class ProtocolHeaderParseException : Exception
    {
        public ProtocolHeaderParseException()
        {
        }

        public ProtocolHeaderParseException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }

    class ProtocolParseException : Exception
    {
        public ProtocolParseException()
        {
        }

        public ProtocolParseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    class UndefinedEnumValueException : Exception
    {
        public UndefinedEnumValueException(int value)
        {
            Value = value;
        }

        public int Value { get; }

        public override string Message => $"Value not defined in Enum: {Value}";
    }
}

