using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace RabbitMQChatClient
{
    public static class MessageDeserializer
    {
        public static T? DeserializeMessage<T>(object? sender, BasicDeliverEventArgs ea)
        {
            // Note: it is possible to access the channel via ((EventingBasicConsumer)sender).Model here
            if (sender != null)
                ((EventingBasicConsumer)sender).Model.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

            var body = ea.Body.ToArray();
            var serializedMessage = Encoding.UTF8.GetString(body);
            var message = JsonSerializer.Deserialize<T>(serializedMessage);

            return message;
        }
    }
}
