using MassTransit.Serialization;

namespace MassTransit
{
    public static class ReceiveEndpointConfiguratorExtensions
    {
        public static void UseNoEnvelopeMessageDeserializer(this IReceiveEndpointConfigurator receiveEndpointConfigurator)
        {
            receiveEndpointConfigurator.ClearMessageDeserializers();
            receiveEndpointConfigurator.AddMessageDeserializer(JsonMessageSerializer.JsonContentType, () => new NoEnvelopeMessageDeserializer());
        }
    }
}