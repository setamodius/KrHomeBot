using Akka.Actor;
using KrHomeBot.Message;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace KrHomeBot.Actor
{
    public class MqttActor : ReceiveActor
    {
        readonly IManagedMqttClient MqttClient;
        readonly IManagedMqttClientOptions Options;
        private readonly IUntypedActorContext ThisContext;

        public MqttActor()
        {
            ThisContext = Context;
            Options = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(10))
                .WithClientOptions(new MqttClientOptionsBuilder()
                .WithProtocolVersion(MQTTnet.Serializer.MqttProtocolVersion.V311)
                .WithClientId(Environment.GetEnvironmentVariable("MqttClientId"))
                .WithTcpServer(Environment.GetEnvironmentVariable("MqttServer"),1883)
                .WithCredentials(Environment.GetEnvironmentVariable("MqttUsername"), Environment.GetEnvironmentVariable("MqttPassword"))
                .WithCleanSession()
                .Build())
            .Build();

            MqttClient = new MqttFactory().CreateManagedMqttClient();
            MqttClient.ApplicationMessageReceived += MqttClient_ApplicationMessageReceived;
            MqttClient.Connected += MqttClient_Connected;
            
            Receive<MqttActorMessage.Start>(StartReceived, null);
            Receive<MqttActorMessage.Subscribe>(SubscribeReceived, null);
        }

        private void MqttClient_Connected(object sender, MqttClientConnectedEventArgs e)
        {
            Console.WriteLine(MqttClient.IsConnected);
        }

        private void MqttClient_ApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            ThisContext.Parent.Tell(new MqttActorMessage.PayloadReceived(e.ApplicationMessage.Topic, e.ApplicationMessage.ConvertPayloadToString()));            
        }

        private async void SubscribeReceived(MqttActorMessage.Subscribe msg)
        {
            await MqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic(msg.Topic).WithExactlyOnceQoS().Build());
        }

        private async void StartReceived(MqttActorMessage.Start obj)
        {
            await MqttClient.StartAsync(Options);
        }
    }
}
