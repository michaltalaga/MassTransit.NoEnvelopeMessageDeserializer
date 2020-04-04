using GreenPipes;
using MassTransit.Context;
using MassTransit.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Text;

namespace MassTransit.NoEnvelopeMessageDeserializer
{
    public class NoEnvelopeMessageDeserializer : IMessageDeserializer
    {
        public ContentType ContentType => JsonMessageSerializer.JsonContentType;

        public ConsumeContext Deserialize(ReceiveContext receiveContext)
        {
            var deserializer = JsonMessageSerializer.Deserializer;
            var messageEncoding = GetMessageEncoding(receiveContext);
            using (var body = receiveContext.GetBodyStream())
            {
                using (var reader = new StreamReader(body, messageEncoding, false, 1024, true))
                {
                    using (var jsonReader = new JsonTextReader(reader))
                    {
                        var msg = deserializer.Deserialize<JToken>(jsonReader);
                        var envelope = new MsgEnvelope
                        {
                            Message = msg,
                            MessageType = new string[0],
                        };
                        return new GenericJsonConsumeContext(JsonMessageSerializer.Deserializer, receiveContext, envelope);
                    }
                }
            }
        }
        static Encoding GetMessageEncoding(ReceiveContext receiveContext)
        {
            var contentEncoding = receiveContext.TransportHeaders.Get("Content-Encoding", default(string));

            return string.IsNullOrWhiteSpace(contentEncoding) ? Encoding.UTF8 : Encoding.GetEncoding(contentEncoding);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("json");
            scope.Add("contentType", JsonMessageSerializer.JsonContentType.MediaType);
        }
        class MsgEnvelope : MessageEnvelope
        {
            public string MessageId { get; set; }

            public string RequestId { get; set; }

            public string CorrelationId { get; set; }

            public string ConversationId { get; set; }

            public string InitiatorId { get; set; }

            public string SourceAddress { get; set; }

            public string DestinationAddress { get; set; }

            public string ResponseAddress { get; set; }

            public string FaultAddress { get; set; }

            public string[] MessageType { get; set; }

            public object Message { get; set; }

            public DateTime? ExpirationTime { get; set; }

            public DateTime? SentTime { get; set; }

            public IDictionary<string, object> Headers { get; set; }

            public HostInfo Host { get; set; }
        }

        class GenericJsonConsumeContext : JsonConsumeContext
        {
            private readonly JsonSerializer deserializer;
            private JToken messageToken;

            public GenericJsonConsumeContext(JsonSerializer deserializer, ReceiveContext receiveContext, MessageEnvelope envelope) : base(deserializer, receiveContext, envelope)
            {
                this.deserializer = deserializer;
                messageToken = GetMessageToken(envelope.Message);
            }
            public override bool TryGetMessage<T>(out ConsumeContext<T> message)
            {
                using (JsonReader jsonReader = messageToken.CreateReader())
                {

                    var obj = deserializer.Deserialize(jsonReader, typeof(T));
                    message = new MessageConsumeContext<T>(this, (T)obj);

                    return true;
                }
            }
            static JToken GetMessageToken(object message)
            {
                var messageToken = message as JToken;
                if (messageToken == null || messageToken.Type == JTokenType.Null)
                    return new JObject();

                return messageToken;
            }
        }
    }
}
