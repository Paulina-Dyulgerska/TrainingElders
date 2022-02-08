using ChatModels;
using RabbitMQ.Client;

namespace RabbitMQChatClient
{
    public class RabbitMqModelFactory
    {
        private readonly IConnection connection;

        public RabbitMqModelFactory(IConnection connection)
        {
            this.connection = connection;
        }

        public IModel Build(string username)
        {
            var channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange: Constants.MessageToAllExchangeType, ExchangeType.Fanout);
            channel.ExchangeDeclare(exchange: Constants.DirectMessageExchangeType, ExchangeType.Direct);

            var queueName = channel.QueueDeclare(username).QueueName;
            channel.QueueDeclare(queue: Constants.NewUserQueueName, durable: false, exclusive: false, autoDelete: false, arguments: new Dictionary<string, object> { { "name", username } });


            channel.QueueBind(queue: Constants.NewUserQueueName,
                              exchange: Constants.DirectMessageExchangeType,
                              routingKey: Constants.NewUserQueueName);
            channel.QueueBind(queue: queueName,
                              exchange: Constants.MessageToAllExchangeType,
                              routingKey: string.Empty);
            channel.QueueBind(queue: queueName,
                              exchange: Constants.DirectMessageExchangeType,
                              routingKey: queueName); //could be other key string, like username if the last is unique

            return channel;
        }
    }

    //public class RabbitMQModel
    //{
    //    private readonly IModel channel;
    //    private readonly ISerializer serializer;
    //    private IBasicProperties? props;
    //    private string? queueName;

    //    public RabbitMQModel(IModel channel, ISerializer serializer)
    //    {
    //        this.channel = channel;
    //        this.serializer = serializer;
    //    }

    //    public IModel Configure(string username)
    //    {
    //        channel.ExchangeDeclare(exchange: Constants.MessageToAllExchangeType, ExchangeType.Fanout);
    //        channel.ExchangeDeclare(exchange: Constants.DirectMessageExchangeType, ExchangeType.Direct);

    //        queueName = channel.QueueDeclare(username).QueueName;
    //        channel.QueueDeclare(queue: Constants.NewUserQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);


    //        channel.QueueBind(queue: Constants.NewUserQueueName,
    //                          exchange: Constants.DirectMessageExchangeType,
    //                          routingKey: Constants.NewUserQueueName);
    //        channel.QueueBind(queue: queueName,
    //                          exchange: Constants.MessageToAllExchangeType,
    //                          routingKey: string.Empty);
    //        channel.QueueBind(queue: queueName,
    //                          exchange: Constants.DirectMessageExchangeType,
    //                          routingKey: queueName); //could be other key string, like username if the last is unique

    //        props = channel.CreateBasicProperties();
    //        props.ReplyTo = queueName;
    //        props.CorrelationId = username;

    //        return channel;
    //    }

    //    public Task PublishMessageAsync(string routingKey, string exchangeType, ChatMessage message)
    //    {
    //        var body = serializer.Serialize(message);
    //        channel.BasicPublish(exchange: exchangeType,
    //                       routingKey: routingKey,
    //                       basicProperties: props,
    //                       body: body);

    //        return Task.CompletedTask;
    //    }

    //    public string GetQueueName() => queueName ?? string.Empty;
    //}
}
