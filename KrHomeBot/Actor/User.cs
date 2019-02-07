using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Args;

namespace KrHomeBot.Actor
{
    public class User : ReceiveActor
    {

        public User()
        {
            Receive<Telegram.Bot.Types.Message>(MessageReceived, null);
            Context.Parent.Tell(new Message.BotActorMessage.SendTextMessage("510112041", "Merhaba dünya!!!"));
        }

        private void MessageReceived(Telegram.Bot.Types.Message obj)
        {
            Context.Parent.Tell(new Message.BotActorMessage.SendTextMessage(obj.Chat.Id, "Merhaba " + obj.From.FirstName + " Senin ID " + obj.Chat.Id));
        }
    }
}
