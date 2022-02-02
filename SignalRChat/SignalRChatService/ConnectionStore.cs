using SignalRChatModels;
using System;
using System.Collections.Concurrent;

namespace SignalRChatService
{
    public class ConnectionStore
    {
        private readonly ConcurrentDictionary<Client, string> connections = new ConcurrentDictionary<Client, string>();

        public void Add(Client client, string connection)
        {
            if (connections.ContainsKey(client))
            {
                connections[client] = connection;
                return;
            }

            connections.TryAdd(client, connection);
        }

        public void Remove(Client client)
        {
            if (connections.ContainsKey(client) == false)
                throw new ArgumentException($"'{client.Username}' is not connected");

            connections.TryRemove(client, out var _);
        }

        public string GetFor(Client client)
        {
            if (connections.ContainsKey(client))
                return connections[client];

            throw new ArgumentException($"'{client.Username}' is not connected");
        }
    }
}
