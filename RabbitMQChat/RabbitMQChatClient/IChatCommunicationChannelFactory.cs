using ChatModels;

namespace RabbitMQChatClient
{
    public interface IChatCommunicationChannelFactory
    {
        IChatMessageSender Build(string username);
    }
}
