using System.Text;
using System.Text.Json;

namespace RabbitMQChatClient
{
    public class Serializer : ISerializer
    {
        public byte[] Serialize<T>(T message)
        {
            var jsonMessage = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(jsonMessage);
            return body;
        }

        public T? Deserialize<T>(byte[] body)
        {
            var serializedMessage = Encoding.UTF8.GetString(body);
            var message = JsonSerializer.Deserialize<T>(serializedMessage);
            return message;
        }
    }
}
