using Microsoft.AspNetCore.SignalR;
using SignalRChatModels;
using SignalRChatService.Hubs;
using System.Threading.Tasks;

namespace SignalRChatService
{
    public class SignalRCommunicationChannel : IChatCommunicationChannel
    {
        private readonly IHubContext<ChatRoomHub> hubContext; // injects the hub
        private readonly ConnectionStore connectionStore;

        public SignalRCommunicationChannel(IHubContext<ChatRoomHub> hubContext, ConnectionStore connectionStore)
        {
            this.hubContext = hubContext;
            this.connectionStore = connectionStore;
        }

        public Task SendMessageAsync(ChatMessage message)
        {
            return hubContext.Clients.All.SendAsync("ReceiveMessage", ChatMessage.Dto.From(message));
        }

        public async Task SendMessageAsync(Client receiver, ChatMessage message)
        {
            var connectionId = connectionStore.GetFor(receiver);
            await hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", ChatMessage.Dto.From(message));
        }
    }
}
