using System;
using System.Text;
using ComProtocol;

namespace GenericNetworlProtocol.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            //To begin a new protcol call CreateHeader method
            var protocol = Protocol<MessageType>.CreateHeader(MessageType.ComplexMessage, ComState.Connect);

            //create Payload call your Payload Constructor and set this to PayloadBase then call AddPayload
            //this is optional you can send payloadless packets
            PayloadBase myPayload = new MyPayload
            {
                Message = "Hello World!"
            };
            protocol.AddPayLoad(myPayload);

            //now call GetBytes to the data as a byte array, that you can send via tcp/ip or udp over the network
            var data = protocol.GetBytes();

            //If data in the stream available just create the following
            PayloadBase incommingPayload = new MyPayload();
            var incommingProtocol = Protocol<MessageType>.Parse(data, incommingPayload);
            //var incommingProtocol = Protocol<MessageType>.Parse(data);

            //you can e.g. switch over the type:
            //this can be dangerous cause there is no client/server validation, 
            //so this could come from another application, that implement this protocol
            switch (incommingProtocol.Header.Type)
            {
                case MessageType.SimpleMessage:
                    break;
                case MessageType.ComplexMessage:
                    var payload = (MyPayload)incommingPayload;
                    Console.WriteLine(payload.Message);
                    break;
                case MessageType.Mqtt:
                    break;
                case MessageType.Https:
                    break;
                default:
                    break;
            }
            
            Console.ReadKey();
        }
    }

    /// <summary>
    /// Your payload class have to inheritance from PayloadBase
    /// and overide both methods, cause i don't know how to do this.
    /// </summary>
    class MyPayload : PayloadBase
    {
        public string Message { get; set; }
        public override byte[] GetBytes()
        {
            return Encoding.UTF8.GetBytes(Message);
        }

        public override PayloadBase Parse(byte[] bytes)
        {
            Message = Encoding.UTF8.GetString(bytes);
            return this;
        }
    }

    /// <summary>
    /// Implement your own MessageType as an enum, 
    /// with this enum you can determine wich packet is incomming from the network
    /// </summary>
    enum MessageType 
    {
        SimpleMessage = 0b_0001,
        ComplexMessage,
        Mqtt,
        Https
    }
}
