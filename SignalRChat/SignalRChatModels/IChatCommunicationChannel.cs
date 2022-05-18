using System.Threading.Tasks;

namespace SignalRChatModels
{
    public interface IChatCommunicationChannel
    {
        Task SendMessageAsync(ChatMessage message);

        Task SendMessageAsync(Client receiver, ChatMessage message);
    }
}
