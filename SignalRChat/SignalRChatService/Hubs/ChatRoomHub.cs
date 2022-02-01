using Microsoft.AspNetCore.SignalR;
using SignalRChatModels;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace SignalRChatService.Hubs
{
    public class ChatRoomHub : Hub, IChatCommunicationChannel
    {
        private readonly ConcurrentDictionary<Client, string> clientToConnectionMap = new ConcurrentDictionary<Client, string>();
        private readonly ChatRoom chatRoom;

        public ChatRoomHub(ChatRoom chatRoom)
        {
            this.chatRoom = chatRoom;
            this.chatRoom.SetCommunicationChannel(this);
        }

        public async Task Join(Client client)
        {
            clientToConnectionMap.TryAdd(client, Context.ConnectionId);
            await chatRoom.Join(client);

        }

        public async Task Leave(Client client)
        {
            await chatRoom.Leave(client);
            clientToConnectionMap.TryRemove(client, out var _);
        }

        public async Task Send(ChatMessage message)
        {
            await chatRoom.PublishMessage(message);
        }
        async Task IChatCommunicationChannel.SendMessage(ChatMessage message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        async Task IChatCommunicationChannel.SendMessage(Client receiver, ChatMessage message)
        {
            var connection = clientToConnectionMap[receiver];
            await Clients.User(connection).SendAsync("ReceiveMessage", message);
        }
    }

    public class ChatRoomApplicationService
    {
        private ChatRoomHub chatRoomHub;

        public ChatRoomApplicationService(ChatRoomHub chatRoomHub)
        {
            this.chatRoomHub = chatRoomHub;
        }

        public async void JoinClient(Client client)
        {
            ///
            await chatRoomHub.Join(client);

            // Save
        }

        public async void LeaveClient(Client client)
        {
            await chatRoomHub.Leave(client);
        }

        public async void PublishMessage(ChatMessage message)
        {
            await chatRoomHub.Send(message);
        }
    }
}
