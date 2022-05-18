using Microsoft.AspNetCore.SignalR;
using SignalRChatModels;
using System;
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

        public async Task JoinAsync(Client client)
        {
            connectionStore.Add(client, Context.ConnectionId);
            await chatRoomApplicationService.JoinAsync(client);
        }

        public async Task LeaveAsync(Client client)
        {
            await chatRoomApplicationService.LeaveAsync(client);
            connectionStore.Remove(client);
        }

        public async Task SendAsync(ChatMessage.Dto message)
        {
            await chatRoomApplicationService.PublishMessageAsync(message.ToModel());
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var client = connectionStore.GetClient(Context.ConnectionId);
            chatRoomApplicationService.LeaveAsync(client).GetAwaiter().GetResult();
            return base.OnDisconnectedAsync(exception);
        }
    }
}
