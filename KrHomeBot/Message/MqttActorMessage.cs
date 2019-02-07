using System;
using System.Collections.Generic;
using System.Text;

namespace KrHomeBot.Message
{
    class MqttActorMessage
    {
        internal class Start
        {
            internal Start()
            {
            }
        }

        internal class Stop
        {
            internal Stop()
            {
            }
        }

        internal class Subscribe
        {
            public Subscribe(string topic)
            {
                Topic = topic ?? throw new ArgumentNullException(nameof(topic));
            }

            public string Topic { get; }
        }

        internal class PayloadReceived
        {
            public PayloadReceived(string topic, string payload)
            {
                Topic = topic ?? throw new ArgumentNullException(nameof(topic));
                Payload = payload ?? throw new ArgumentNullException(nameof(payload));
            }

            public string Topic { get; }
            public string Payload { get; }
        }
    }
}
