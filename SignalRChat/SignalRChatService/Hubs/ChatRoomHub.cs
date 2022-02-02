using Microsoft.AspNetCore.SignalR;
using SignalRChatModels;
using System.Threading.Tasks;

namespace SignalRChatService.Hubs
{
    public class ChatRoomHub : Hub
    {
        private readonly ChatRoomApplicationService chatRoomApplicationService;
        private readonly ConnectionStore connectionStore;

        public ChatRoomHub(ChatRoomApplicationService chatRoomApplicationService, ConnectionStore connectionStore)
        {
            this.chatRoomApplicationService = chatRoomApplicationService;
            this.connectionStore = connectionStore;
        }

        public async Task Join(Client client)
        {
            connectionStore.Add(client, Context.ConnectionId);
            await chatRoomApplicationService.Join(client);
        }

        public async Task Leave(Client client)
        {
            await chatRoomApplicationService.Leave(client);
            connectionStore.Remove(client);
        }

        public async Task Send(ChatMessage.Dto message)
        {
            await chatRoomApplicationService.PublishMessage(message.ToModel());
        }
    }
}
