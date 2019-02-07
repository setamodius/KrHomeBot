using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;

namespace KrHomeBot.Message
{
    internal class BotActorMessage
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

        internal class SendTextMessage
        {
            public SendTextMessage(ChatId id, string text)
            {
                Id = id ?? throw new ArgumentNullException(nameof(id));
                Text = text ?? throw new ArgumentNullException(nameof(text));
            }

            public ChatId Id { get; }
            public string Text { get; }

           
        }
    }
}
