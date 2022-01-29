using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiChatModels;

namespace WebApiChatService.Services
{
    public interface IChatService
    {
        Task AddClientAsync(Client client);

        Task RemoveClientAsync(Client client);

        Task AddChatMessageAsync(ChatMessage chatMessage);

        IEnumerable<Client> GetAllClients();

        IEnumerable<ChatMessage> GetAllMessages();
    }
}
