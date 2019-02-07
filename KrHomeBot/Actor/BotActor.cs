using Akka.Actor;
using KrHomeBot.Message;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace KrHomeBot.Actor
{
    public class BotActor:ReceiveActor
    {
        private static readonly TelegramBotClient Bot = new TelegramBotClient(Environment.GetEnvironmentVariable("TelegramApi"));
        private readonly IUntypedActorContext ThisContext;
        private readonly IActorRef MqttActor;
        private readonly string DeviceStatus = "tele/livingroom/LWT";
        private readonly string LightStatus = "stat/livingroom/POWER";

        public BotActor()
        {
            ThisContext = Context;
            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnMessageEdited += BotOnMessageReceived;
            Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            Bot.OnInlineQuery += BotOnInlineQueryReceived;
            Bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
            Bot.OnReceiveError += BotOnReceiveError;

            MqttActor = ThisContext.ActorOf(Props.Create(() => new MqttActor()), "MqttActor");
            MqttActor.Tell(new MqttActorMessage.Start());
            MqttActor.Tell(new MqttActorMessage.Subscribe(DeviceStatus));
            MqttActor.Tell(new MqttActorMessage.Subscribe(LightStatus));
            Receive<BotActorMessage.Start>(StartReceived, null);
            Receive<BotActorMessage.Stop>(StopReceived, null);
            Receive<BotActorMessage.SendTextMessage>(SendTextMessageReceived, null);
            Receive<MqttActorMessage.PayloadReceived>(MqttPayloadReceived, null);
        }

        private void MqttPayloadReceived(MqttActorMessage.PayloadReceived obj)
        {
            if (obj.Topic == DeviceStatus)
            {
                if (obj.Payload == "Online")
                {
                    Bot.SendTextMessageAsync("510112041", "Oturma odası aydınlatma cihazı bağlandı.");
                }
                else
                {
                    Bot.SendTextMessageAsync("510112041", "Oturma odası aydınlatma cihazı bağlantı kesildi.");
                }
            }

            if (obj.Topic == LightStatus)
            {
                if (obj.Payload == "On")
                {
                    Bot.SendTextMessageAsync("510112041", "Oturma odası aydınlatma açıldı.");
                }
                else
                {
                    Bot.SendTextMessageAsync("510112041", "Oturma odası aydınlatma kapandı.");
                }
            }
        }

        private void SendTextMessageReceived(BotActorMessage.SendTextMessage message)
        {
            Bot.SendTextMessageAsync(message.Id, message.Text);
        }

        private void StopReceived(BotActorMessage.Stop obj)
        {
            Bot.StopReceiving();
        }

        private void StartReceived(BotActorMessage.Start obj)
        {
            var me = Bot.GetMeAsync().Result;
            Bot.StartReceiving(Array.Empty<UpdateType>());
            Console.WriteLine($"Start listening for @{me.Username}");
            //Bot.SendTextMessageAsync("510112041", "merhaba dünya!!!");
        }

        private void BotOnReceiveError(object sender, ReceiveErrorEventArgs e)
        {
            
        }

        private void BotOnChosenInlineResultReceived(object sender, ChosenInlineResultEventArgs e)
        {
            
        }

        private void BotOnInlineQueryReceived(object sender, InlineQueryEventArgs e)
        {
            
        }

        private void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs e)
        {
           
        }

        private void BotOnMessageReceived(object sender, MessageEventArgs e)
        {
            long id = e.Message.Chat.Id;
            var childName = "User" + id;

            var child = ThisContext.Child(childName);

            if (child == ActorRefs.Nobody)
            {
                var props = Props.Create(() => new User());
                child = ThisContext.ActorOf(props, childName);
            }

            child.Tell(e.Message);

        }
    }
}
