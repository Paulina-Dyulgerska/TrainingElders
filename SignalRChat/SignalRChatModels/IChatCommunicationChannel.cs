using System.Threading.Tasks;

namespace SignalRChatModels
{
    public interface IChatCommunicationChannel
    {
        Task SendMessage(ChatMessage message);

        Task SendMessage(Client receiver, ChatMessage message);
    }
}
