using ChatModels;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace RabbitMQChatClient
{
    public class RabbitMQCommunicationChannel : IChatCommunicationChannel
    {
        private const string newUserQueueName = "newUser";
        private const string messageToAllExchangeType = "messageExchange";
        private const string directMessageExchangeType = "direct";

        private readonly IModel channel;
        private IBasicProperties? props;
        private string? queueName;

        public event EventHandler<BasicDeliverEventArgs>? OnNewClientConnect;
        public event EventHandler<BasicDeliverEventArgs>? OnNewMessageReceive;
        public event EventHandler<ShutdownEventArgs>? OnModelShutdown;

        public RabbitMQCommunicationChannel(IConnection connection)
        {
            channel = connection.CreateModel();
        }

        public string Build(string username)
        {
            channel.ExchangeDeclare(exchange: messageToAllExchangeType, ExchangeType.Fanout);
            channel.ExchangeDeclare(exchange: directMessageExchangeType, ExchangeType.Direct);

            queueName = channel.QueueDeclare().QueueName;
            channel.QueueDeclare(queue: newUserQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            props = channel.CreateBasicProperties();
            props.ReplyTo = queueName;
            props.CorrelationId = username;

            channel.QueueBind(queue: newUserQueueName,
                              exchange: directMessageExchangeType,
                              routingKey: newUserQueueName);
            channel.QueueBind(queue: queueName,
                              exchange: messageToAllExchangeType,
                              routingKey: string.Empty);
            channel.QueueBind(queue: queueName,
                              exchange: directMessageExchangeType,
                              routingKey: queueName); //could be other key string, like username if the last is unique

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += OnMessageReceived;
            channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
            var archiveConsumer = new EventingBasicConsumer(channel);
            archiveConsumer.Received += OnNewClientConnected;
            channel.BasicConsume(queue: newUserQueueName, autoAck: false, consumer: archiveConsumer);

            channel.ModelShutdown += OnModelShuteddown;

            PublishMessage(newUserQueueName, directMessageExchangeType, new ChatMessage(queueName, $"I am here ({props.CorrelationId})")).GetAwaiter().GetResult();

            return queueName;
        }

        public Task SendMessage(ChatMessage message)
        {
            return PublishMessage(string.Empty, messageToAllExchangeType, message);
        }

        public Task SendMessage(Client receiver, ChatMessage message)
        {
            return PublishMessage(receiver.ConnectionId, directMessageExchangeType, message);
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
            OnNewMessageReceive?.Invoke(sender, ea);
        }

        private void OnNewClientConnected(object? sender, BasicDeliverEventArgs ea)
        {
            OnNewClientConnect?.Invoke(sender, ea);
        }

        private void OnModelShuteddown(object? sender, ShutdownEventArgs ea)
        {
            OnModelShutdown?.Invoke(sender, ea);
        }

        //public void Close()
        //{
        //    consumer.Received -= OnMessageReceived; // Do not do this, because the result is "RabbitMQ.Client.Exceptions.AlreadyClosedException"
        //}
    }
}
