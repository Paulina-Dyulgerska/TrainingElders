using RabbitMQ.Client;
using System.Text;

namespace RabbitMQChatClient
{
    public class Sender
    {
        public bool SendMessage(IConnection connection, string queueName, string message)
        {
            try
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "messageExchange", ExchangeType.Direct);
                    channel.QueueDeclare(queue: queueName,
                                                durable: false,
                                                exclusive: false,
                                                autoDelete: false,
                                                arguments: null);

                    channel.QueueBind(queue: queueName,
                                        exchange: "messageExchange",
                                        routingKey: queueName,
                                        arguments: null);
                    var msg = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish("messageExchange", queueName, null, msg);
                }
            }
            catch (Exception)
            {
            }
            return true;
        }
    }
}

