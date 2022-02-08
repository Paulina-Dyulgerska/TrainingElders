using System.Threading.Tasks;

namespace ChatModels
{
    public interface IChatMessageSender
    {
        Task SendMessageAsync(ChatMessage message);

        Task SendMessageAsync(Client receiver, ChatMessage message);
    }
}
