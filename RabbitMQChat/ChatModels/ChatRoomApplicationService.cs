using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatModels
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

        public async Task Join(Client client)
        {
            var messages = chatRoom.Join(client);
            await SendMessages(messages);

            foreach (var msg in chatRoom.GetHistory().Where(x => x.IsForAll))
            {
                await chatCommunication.SendMessage(client, msg);
            }

            chatRoom.AppentToHistory(messages);
        }

        public async Task Leave(Client client)
        {
            if (client is null) throw new ArgumentNullException(nameof(client));

            var messages = chatRoom.Leave(client);

            await SendMessages(messages);

            chatRoom.AppentToHistory(messages);
        }

        public async Task PublishMessage(ChatMessage message)
        {
            if (message is null) throw new ArgumentNullException(nameof(message));

            await chatCommunication.SendMessage(message);

            chatRoom.AppentToHistory(message);
        }

        private async Task SendMessages(IEnumerable<ChatMessage> messages)
        {
            foreach (var msg in messages)
            {
                if (msg.IsForAll)
                    await chatCommunication.SendMessage(msg);
                else
                    await chatCommunication.SendMessage(msg.Receiver, msg);
            }
        }
    }
}
