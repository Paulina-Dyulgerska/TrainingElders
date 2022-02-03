using System.Threading.Tasks;

namespace ChatModels
{
    public interface IChatCommunicationChannel
    {
        Task SendMessage(ChatMessage message);

        Task SendMessage(Client receiver, ChatMessage message);
    }
}
