# GenericNetworkProtocol

Proof of Concept for a network Protocol. This is not intended to use in production environment, cause this protocol doesn't implement any security features.

Packet Structure for Header:

Type | Size | Description
------------ | ------------- | -------------
Type | 1 Byte | Type of Message
ComState | 1 Byte | Connection State e.g. Connected or Disconnected
Length | 4 Byte | Length of Packet with Payload
HeaderLength | 4 Byte | Length of Header
HasPayload | 1 Byte | Indicate if a Payload attacted or not

The Length and HeaderLength will be validated when the data is parsed. If the validation failed an exception will be thrown, 
because the packet could be maybe corrupted or something else. Any other validation checks are at the moment not implemented, yet.

Implement the Protocol:
Include the Libary in your Project. Create an Enum for the Type:
```csharp
enum MessageType 
{
    SimpleMessage = 0b_0001,
    ComplexMessage,
    Mqtt,
    Https
}
```

Implement the abstract PayloadBase Class:
```csharp
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
```

Now you can you start using the Protocol Class. The class doesn't provide a constructor. To Create a new Protocol just call the following method:
```csharp
var protocol = Protocol<MessageType>.CreateHeader(MessageType.ComplexMessage, ComState.Connect);
```

Next step is to add a Payload to the protocol. For this you need the payload class:
```csharp
var myPayload = new MyPayload
{
    Message = "Hello World!"
};
protocol.AddPayLoad(myPayload);
```

The last thing do to is:
```csharp
var data = protocol.GetBytes();
```
The data can be passed into a stream e.g tcp or udp network stream or any other stream

If you get data from the network stream ou can parse the data with the following method:
```csharp
PayloadBase payload = new MyPayload();
var protocol = Protocol<MessageType>.Parse(data, payload);
```
