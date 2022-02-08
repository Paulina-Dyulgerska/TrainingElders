using ChatModels;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQChatClient
{
    public class RabbitMqChatMessageReceiver : IChatMessageReceiver
    {
        private readonly List<Func<Client, ChatMessage, bool>> messageReceivedHandlers = new List<Func<Client, ChatMessage, bool>>();
        private readonly List<Func<Client, Task>> clientConnectedHandlers = new List<Func<Client, Task>>();
        private readonly RabbitMqModelFactory modelFactory;
        private readonly ISerializer serializer;
        private Client? client;

        public RabbitMqChatMessageReceiver(RabbitMqModelFactory modelFactory, ISerializer serializer)
        {
            this.modelFactory = modelFactory;
            this.serializer = serializer;
        }

        public void Start(Client client)
        {
            if (client is null) throw new ArgumentNullException(nameof(client));

            this.client = client;
            var channel = modelFactory.Build(client.Username);
            channel.ModelShutdown += OnModelShutedDown;
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += OnMessageReceived;
            channel.BasicConsume(queue: client.Username, autoAck: false, consumer: consumer);

            var archiveConsumer = new EventingBasicConsumer(channel);
            archiveConsumer.Received += OnNewClientConnected;

            var props = channel.CreateBasicProperties();
            props.ReplyTo = client.Username;

            var body = serializer.Serialize(ChatMessage.Dto.From(new ChatMessage(client.Username, $"{client.Username} has joined the party!")));
            channel.BasicPublish(exchange: Constants.DirectMessageExchangeType,
                           routingKey: Constants.NewUserQueueName,
                           basicProperties: props,
                           body: body);

            channel.BasicConsume(queue: Constants.NewUserQueueName, autoAck: false, consumer: archiveConsumer);
        }

        public void Stop() { }

        public void RegisterMessageReseivedHandler(Func<Client, ChatMessage, bool> func)
        {
            messageReceivedHandlers.Add(func);
        }

        public void RegisterClientConnectedHandler(Func<Client, Task> func)
        {
            clientConnectedHandlers.Add(func);
        }

        private void OnMessageReceived(object? sender, BasicDeliverEventArgs ea)
        {
            if (client is null)
                return;

            if (sender == null)
                return;

            var consumer = sender as EventingBasicConsumer;
            if (consumer == null)
                return;

            var message = serializer.Deserialize<ChatMessage.Dto>(ea.Body.ToArray());
            if (message == null)
                return;

            var model = message.ToModel();
            foreach (var handler in messageReceivedHandlers)
            {
                if (handler(client, model) == false)
                    return;
            }

            consumer.Model.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        }

        private void OnNewClientConnected(object? sender, BasicDeliverEventArgs ea)
        {
            if (clientConnectedHandlers.Count == 0)
                return;

            var message = serializer.Deserialize<ChatMessage.Dto>(ea.Body.ToArray());
            if (message == null)
                return;

            var consumer = sender as EventingBasicConsumer;
            if (consumer == null)
                return;

            foreach (var item in clientConnectedHandlers)
            {
                item(new Client(message.Author)).ConfigureAwait(false).GetAwaiter().GetResult();
                Console.WriteLine($"{client.Username} sending history");
            }

            consumer.Model.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        }

        private void OnModelShutedDown(object? sender, ShutdownEventArgs ea)
        {
        }
    }
}
