using System;
using System.Collections.Generic;

namespace ChatModels
{
    public class Client : IEquatable<Client>
    {
        public Client(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException($"'{nameof(username)}' cannot be null or whitespace.", nameof(username));

            Username = username;
        }

        public string Username { get; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Client);
        }

        public bool Equals(Client other)
        {
            return other != null && Username == other.Username;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Username);
        }

        public static bool operator ==(Client left, Client right) => EqualityComparer<Client>.Default.Equals(left, right);

        public static bool operator !=(Client left, Client right) => !(left == right);
    }

}
