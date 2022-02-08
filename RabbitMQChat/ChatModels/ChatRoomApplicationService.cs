using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatModels
{
    public class ChatRoomApplicationService
    {
        private readonly IChatMessageSender chatMessageSender;
        private readonly ChatRoom chatRoom;

        public ChatRoomApplicationService(IChatMessageSender chatMessageSender, IChatMessageReceiver chatMessageReceiver, ChatRoom chatRoom)
        {
            this.chatMessageSender = chatMessageSender;
            this.chatRoom = chatRoom;
            chatMessageReceiver.RegisterMessageReseivedHandler((c, msg) =>
            {
                if (msg.Author == c.Username)
                    return true;

                chatRoom.AppentToHistory(msg);
                return true;
            });

            chatMessageReceiver.RegisterClientConnectedHandler(async client =>
            {
                await SendHistory(client);
            });
        }

        public async Task Join(Client client)
        {
            var welcomeMessages = chatRoom.Join(client);
            await SendMessages(welcomeMessages);

            await SendHistory(client);

            chatRoom.AppentToHistory(welcomeMessages);
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

            await chatMessageSender.SendMessageAsync(message);

            chatRoom.AppentToHistory(message);
        }

        public async Task SendHistory(Client client)
        {
            foreach (var msg in chatRoom.GetHistory().Where(x => x.IsForAll).OrderBy(x => x.CreatedOn))
            {
                await chatMessageSender.SendMessageAsync(client, msg);
            }
        }

        public IEnumerable<ChatMessage> GetHistory() => chatRoom.GetHistory();

        public bool UserIsJoined(string username) => chatRoom.HasClient(username);

        private async Task SendMessages(IEnumerable<ChatMessage> messages)
        {
            foreach (var msg in messages)
            {
                if (msg.IsForAll)
                    await chatMessageSender.SendMessageAsync(msg);
                else
                    await chatMessageSender.SendMessageAsync(msg.Receiver, msg);
            }
        }
    }
}
