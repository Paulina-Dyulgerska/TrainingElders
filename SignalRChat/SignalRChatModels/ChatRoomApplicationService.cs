using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRChatModels
{
    public class ChatRoomApplicationService
    {
        private readonly IChatCommunicationChannel chatCommunication;
        private readonly ChatRoom chatRoom;

        public ChatRoomApplicationService(IChatCommunicationChannel chatCommunication, ChatRoom chatRoom)
        {
            this.chatCommunication = chatCommunication;
            this.chatRoom = chatRoom;
        }

        public async Task JoinAsync(Client client)
        {
            var messages = chatRoom.Join(client);
            await SendMessagesAsync(messages);

            foreach (var msg in chatRoom.GetHistory().Where(x => x.IsForAll))
            {
                await chatCommunication.SendMessageAsync(client, msg);
            }

            chatRoom.AppentToHistory(messages);
        }

        public async Task LeaveAsync(Client client)
        {
            if (client is null) throw new ArgumentNullException(nameof(client));

            var messages = chatRoom.Leave(client);

            await SendMessagesAsync(messages);

            chatRoom.AppentToHistory(messages);
        }

        public async Task PublishMessageAsync(ChatMessage message)
        {
            if (message is null) throw new ArgumentNullException(nameof(message));

            await chatCommunication.SendMessageAsync(message);

            chatRoom.AppentToHistory(message);
        }

        private async Task SendMessagesAsync(IEnumerable<ChatMessage> messages)
        {
            foreach (var msg in messages)
            {
                if (msg.IsForAll)
                    await chatCommunication.SendMessageAsync(msg);
                else
                    await chatCommunication.SendMessageAsync(msg.Receiver, msg);
            }
        }
    }
}
