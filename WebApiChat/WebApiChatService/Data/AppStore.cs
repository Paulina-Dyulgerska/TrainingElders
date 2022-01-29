using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiChatModels;

namespace WebApiChatService.Data
{
    public class AppStore
    {
        private readonly List<Client> clients;
        private readonly List<ChatMessage> chatMessages;

        public AppStore()
        {
            this.clients = new List<Client>();
            this.chatMessages = new List<ChatMessage>()
            {
                new ChatMessage
                {
                     UserName = "Server",
                     Content ="Hello from Web Api Chat Server",
                     CreatedOn = DateTime.Now,
                }
            };
        }

        public IEnumerable<Client> Clients => clients.AsReadOnly();

        public IEnumerable<ChatMessage> Messages => chatMessages.AsReadOnly();

        public void AddClient(Client client)
        {
            if (clients.Any(x => x.Id == client.Id))
                return;
            clients.Add(client);
        }

        public bool RemoveClient(Client client)
        {
            var entity = clients.Where(x => x.Id == client.Id).FirstOrDefault();
            return clients.Remove(entity);
        }

        public void AddChatMessage(ChatMessage chatMessage)
        {
            chatMessages.Add(chatMessage);
        }

        public bool RemoveChatMessage(ChatMessage chatMessage)
        {
            return chatMessages.Remove(chatMessage);
        }
    }
}
