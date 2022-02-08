using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace ChatModels
{
    public class ChatRoom
    {
        private readonly HashSet<Client> clients = new HashSet<Client>();
        private readonly ConcurrentQueue<ChatMessage> messageHistory = new ConcurrentQueue<ChatMessage>();

        public IEnumerable<ChatMessage> Join(Client client)
        {
            if (client is null) throw new ArgumentNullException(nameof(client));

            if (clients.Contains(client))
                return Enumerable.Empty<ChatMessage>();
            //    throw new ArgumentException($"There is already an user with username '{client.Username}'");

            clients.Add(client);
            var messages = new List<ChatMessage>
            {
                new ChatMessage("Server", $"Welcome to our chat, {client.Username}!").To(client),
                new ChatMessage("Server", $"{client.Username} has joined our chat.")
            };

            return messages;
        }

        public IEnumerable<ChatMessage> Leave(Client client)
        {
            if (client is null) throw new ArgumentNullException(nameof(client));

            clients.Remove(client);
            var messages = new List<ChatMessage>
            {
                new ChatMessage("Server", $"Bye, {client.Username}").To(client),
                new ChatMessage("Server", $"{client.Username} has left our chat.")
            };

            return messages;
        }

        public void AppentToHistory(IEnumerable<ChatMessage> messages)
        {
            if (messages is null) throw new ArgumentNullException(nameof(messages));

            foreach (var msg in messages)
            {
                messageHistory.Enqueue(msg);
            }
        }

        public void AppentToHistory(ChatMessage messages)
        {
            if (messages is null) throw new ArgumentNullException(nameof(messages));

            messageHistory.Enqueue(messages);
        }

        public IEnumerable<ChatMessage> GetHistory()
        {
            return messageHistory.AsEnumerable();
        }

        public bool HasClient(Client client)
        {
            return this.clients.Contains(client);
        }

        public bool HasClient(string username)
        {
            return clients.Any(x => x.Username == username);
        }
    }
}
