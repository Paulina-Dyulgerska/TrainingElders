using ChatModels;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace RabbitMQChatLogger
{
    public class RabbitMQLogger : IChatCommunicationChannel
    {
        private readonly IModel channel;
        private readonly EventingBasicConsumer consumer;
        private readonly ConcurrentQueue<ChatMessage> messageHistory = new ConcurrentQueue<ChatMessage>();
        private readonly HashSet<Client> clients = new HashSet<Client>();
        private string queueName = "RabbitMQChatArchive";
        private const string directMessageExchangeType = "direct";

        public RabbitMQLogger(IConnection connection)
        {
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: true, autoDelete: false, arguments: null);
            channel.ExchangeDeclare(exchange: directMessageExchangeType, ExchangeType.Direct);
            channel.QueueBind(queue: queueName,
                              exchange: directMessageExchangeType,
                              routingKey: queueName); //could be other key string, like username if the last is unique
            consumer = new EventingBasicConsumer(channel);
            consumer.Received += OnMessageReceived;
            channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        }

        public Task SendMessage(Client receiver, ChatMessage message)
        {
            return PublishMessage(receiver.Username, directMessageExchangeType, message);
        }

        private Task PublishMessage(string routingKey, string exchangeType, ChatMessage message)
        {
            Task sendMessageTask = Task.Run(() =>
            {
                var jsonMessage = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(jsonMessage);
                channel.BasicPublish(exchange: exchangeType,
                                     routingKey: routingKey,
                                     basicProperties: null,
                                     body: body);
            });

            return sendMessageTask;
        }

        private void OnMessageReceived(object? sender, BasicDeliverEventArgs ea)
        {
            var senderQueueId = ea.BasicProperties.ReplyTo;
            if (senderQueueId != null)
            {
                var client = new Client(senderQueueId);
                if (clients.Contains(client) == false)
                {
                    clients.Add(client);
                    var messages = messageHistory.AsEnumerable();
                    foreach (var msg in messages)
                    {
                        SendMessage(client, msg);
                    }
                }
            }

            var body = ea.Body.ToArray();
            var serializedMessage = Encoding.UTF8.GetString(body);
            var message = JsonSerializer.Deserialize<ChatMessage.Dto>(serializedMessage);
            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false); // Note: it is possible to access the channel via ((EventingBasicConsumer)sender).Model here
            Console.WriteLine($"{message?.Author} ({message?.CreatedOn}): {message?.Content}");

            if (message is not null)
                messageHistory.Enqueue(message.ToModel());
        }

        public Task SendMessage(ChatMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
