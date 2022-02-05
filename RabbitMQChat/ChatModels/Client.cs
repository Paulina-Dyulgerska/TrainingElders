using System;
using System.Collections.Generic;

namespace ChatModels
{
    public class Client : IEquatable<Client>
    {
        public Client(string username, string connectionId)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException($"'{nameof(username)}' cannot be null or whitespace.", nameof(username));
            if (string.IsNullOrWhiteSpace(connectionId))
            {
                throw new ArgumentException($"'{nameof(connectionId)}' cannot be null or whitespace.", nameof(connectionId));
            }

            Username = username;
            ConnectionId = connectionId;
        }

        public string Username { get; }

        public string ConnectionId { get; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Client);
        }

        public bool Equals(Client other)
        {
            return other != null && Username == other.Username && ConnectionId == other.ConnectionId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Username, ConnectionId);
        }

        public static bool operator ==(Client left, Client right) => EqualityComparer<Client>.Default.Equals(left, right);

        public static bool operator !=(Client left, Client right) => !(left == right);
    }

}
