using System;
using System.Linq;
using WcfClient.ChatServiceProxy;

namespace WcfClient
{
    internal class ChatClientCallBack : IChatCallback
    {
        public void Receive(ChatMessage msg)
        {
            Console.WriteLine($"{msg.Sender} ({msg.Time}): {msg.Content}");
        }

        public void RefreshUsers(User[] users)
        {
            Console.WriteLine($"{nameof(RefreshUsers)}: Users online: {string.Join(", ", users.Select(x => x.Name))}");
        }

        public void UserJoin(User user)
        {
            Console.WriteLine($"{nameof(UserJoin)}: User {user.Name} joined the room.");
        }

        public void UserLeave(User user)
        {
            Console.WriteLine($"{nameof(UserLeave)}: User {user.Name} leaved the room.");
        }
    }

}
