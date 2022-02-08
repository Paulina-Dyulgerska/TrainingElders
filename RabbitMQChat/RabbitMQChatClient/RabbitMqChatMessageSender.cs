using ChatModels;
using RabbitMQ.Client;

namespace RabbitMQChatClient
{
    public class RabbitMqChatMessageSender : IChatMessageSender
    {
        private readonly IModel channel;
        private readonly ISerializer serializer;

        public RabbitMqChatMessageSender(IModel channel, ISerializer serializer)
        {
            this.channel = channel ?? throw new ArgumentNullException(nameof(channel));
            this.serializer = serializer;
        }

        public Task SendMessageAsync(ChatMessage message)
        {
            PublishMessageAsync(string.Empty, Constants.MessageToAllExchangeType, message);
            return Task.CompletedTask;
        }

        public Task SendMessageAsync(Client receiver, ChatMessage message)
        {
            PublishMessageAsync(receiver.Username, Constants.DirectMessageExchangeType, message);
            return Task.CompletedTask;
        }

        private void PublishMessageAsync(string routingKey, string exchangeType, ChatMessage message)
        {
            var props = channel.CreateBasicProperties();
            props.ReplyTo = routingKey;

            var body = serializer.Serialize(ChatMessage.Dto.From(message));
            channel.BasicPublish(exchange: exchangeType,
                           routingKey: routingKey,
                           basicProperties: props,
                           body: body);
        }
    }
}
