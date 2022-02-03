using RabbitMQ.Client;

namespace RabbitMQChatClient
{
    public static class RabbitMQCommunicationChannel
    {
        public static IModel Create(string hostName, int port)
        {
            var factory = new ConnectionFactory() { HostName = hostName, Port = port };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            return channel;
        }
    }
}
