using MassTransit.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace MassTransit.NoEnvelopeMessageDeserializer
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
