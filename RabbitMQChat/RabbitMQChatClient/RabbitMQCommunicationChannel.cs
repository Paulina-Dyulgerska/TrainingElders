using ChatModels;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace RabbitMQChatClient
{
    public class RabbitMQCommunicationChannel : IChatCommunicationChannel
    {
        private readonly IModel channel;
        private readonly EventingBasicConsumer consumer;
        private readonly IBasicProperties props;
        private readonly ConcurrentQueue<ChatMessage> respQueue = new ConcurrentQueue<ChatMessage>();
        private string queueName;
        private const string messageToAllExchangeType = "messageExchange";
        private const string directMessageExchangeType = "direct";

        public RabbitMQCommunicationChannel(IConnection connection)
        {
            channel = connection.CreateModel();
            queueName = channel.QueueDeclare().QueueName;
            props = channel.CreateBasicProperties();
            props.ReplyTo = queueName;
            channel.ExchangeDeclare(exchange: messageToAllExchangeType, ExchangeType.Fanout);
            channel.ExchangeDeclare(exchange: directMessageExchangeType, ExchangeType.Direct);
            channel.QueueBind(queue: queueName,
                              exchange: messageToAllExchangeType,
                              routingKey: string.Empty);
            channel.QueueBind(queue: queueName,
                              exchange: directMessageExchangeType,
                              routingKey: queueName); //could be other key string, like username if the last is unique
            consumer = new EventingBasicConsumer(channel);
            consumer.Received += OnMessageReceived;
            channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        }

        public Task SendMessage(ChatMessage message)
        {
            return PublishMessage(string.Empty, messageToAllExchangeType, message);
        }

        public Task SendMessage(Client receiver, ChatMessage message)
        {
            return PublishMessage(props.ReplyTo, directMessageExchangeType, message);
        }

        private Task PublishMessage(string routingKey, string exchangeType, ChatMessage message)
        {
            Task sendMessageTask = Task.Run(() =>
            {
                var jsonMessage = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(jsonMessage);
                channel.BasicPublish(exchange: exchangeType,
                                     routingKey: routingKey,
                                     basicProperties: props,
                                     body: body);
            });

            return sendMessageTask;
        }

        private void OnMessageReceived(object? sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var serializedMessage = Encoding.UTF8.GetString(body);
            var message = JsonSerializer.Deserialize<ChatMessage.Dto>(serializedMessage);
            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false); // Note: it is possible to access the channel via ((EventingBasicConsumer)sender).Model here
            Console.WriteLine($"{message?.Author} ({message?.CreatedOn}): {message?.Content}");

            if (message is not null)
                respQueue.Enqueue(message.ToModel());
        }
    }
}
