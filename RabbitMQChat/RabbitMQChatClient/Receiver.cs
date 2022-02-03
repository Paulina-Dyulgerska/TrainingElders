using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitMQChatClient
{
    public class Receiver
    {
        public string ReceiveMessage(IConnection connection, string queueName)
        {
            try
            {
                using (IModel channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    var consumer = new EventingBasicConsumer(channel);
                    BasicGetResult result = channel.BasicGet(queue: queueName, autoAck: true);
                    if (result != null)
                        return Encoding.UTF8.GetString(result.Body.ToArray());
                    else
                        return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
