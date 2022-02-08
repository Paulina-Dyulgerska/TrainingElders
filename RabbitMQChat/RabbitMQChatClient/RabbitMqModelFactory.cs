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

            var queueName = channel.QueueDeclare(username, false, false, true, null).QueueName;
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
}
