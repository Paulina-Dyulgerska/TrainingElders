using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalRChatModels
{
    public class ChatRoom
    {
        private readonly HashSet<Client> clients = new HashSet<Client>();
        private readonly ConcurrentQueue<ChatMessage> messageHistory = new ConcurrentQueue<ChatMessage>();
        private IChatCommunicationChannel communicationChannel;

        public void SetCommunicationChannel(IChatCommunicationChannel communicationChannel)
        {
            this.communicationChannel = communicationChannel;
        }

        public async Task Join(Client client)
        {
            if (client is null) throw new ArgumentNullException(nameof(client));

            clients.Add(client);
            await SendMessage(client, new ChatMessage("Server", $"Welcome to our chat, {client.Username}!"));

            var welcomeMessage = new ChatMessage("Server", $"{client.Username} has joined our chat.");
            await PublishMessage(welcomeMessage);
        }

        public async Task Leave(Client client)
        {
            if (client is null) throw new ArgumentNullException(nameof(client));

            await SendMessage(client, new ChatMessage("Server", $"Bye, {client.Username}"));

            var byeMessage = new ChatMessage("Server", $"{client.Username} has left our chat.");
            await PublishMessage(byeMessage);
        }

        public async Task PublishMessage(ChatMessage message)
        {
            if (message is null) throw new ArgumentNullException(nameof(message));

            messageHistory.Enqueue(message);
            await SendMessage(message);
        }

        private async Task SendMessage(Client client, ChatMessage message)
        {
            await communicationChannel?.SendMessage(client, message);
        }

        private async Task SendMessage(ChatMessage message)
        {
            await communicationChannel?.SendMessage(message);
        }
    }
}
