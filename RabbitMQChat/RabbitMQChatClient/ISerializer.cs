namespace RabbitMQChatClient
{
    public interface ISerializer
    {
        T? Deserialize<T>(byte[] body);
        byte[] Serialize<T>(T message);
    }
}