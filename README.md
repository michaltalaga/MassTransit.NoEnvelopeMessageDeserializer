# MassTransit.NoEnvelopeMessageDeserializer
Hacky hack to allow MassTransit processing of messages not wrapped in envelopes. Tested with SQS but there is nothing SQS specific so should work with any transport.

## Usage
```csharp
Bus.Factory.CreateUsingAmazonSqs(x =>
{
    //...
    x.ReceiveEndpoint("MessagingApiEmail", e =>
    {
        e.UseNoEnvelopeMessageDeserializer(); // <---- add this line
        e.Consumer<YourConsumer>(provider);
    });
}));
```
