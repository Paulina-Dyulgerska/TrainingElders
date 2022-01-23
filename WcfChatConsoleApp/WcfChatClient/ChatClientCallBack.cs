using System;
using System.Linq;
using WcfChatClient.ChatService;

namespace WcfChatClient
{
    internal class ChatClientCallBack : IChatCallback
    {
        public void IsWritingCallback(User user)
        {
            Console.WriteLine(nameof(IsWritingCallback));
        }

        public void Receive(ChatMessage msg)
        {
            Console.WriteLine($"{msg.Sender} ({msg.Time}): {msg.Content}");
        }

        public void ReceiveWhisper(ChatMessage msg, User receiver)
        {
            Console.WriteLine(nameof(ReceiveWhisper));
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
