using System.Threading.Tasks;

namespace SignalRChatModels
{
    public interface IChatCommunicationChannel
    {
        Task SendMessage(ChatMessage message); // TODO: rename to async

        Task SendMessage(Client receiver, ChatMessage message); // TODO: rename
    }
}
