using System;
using System.ServiceModel;
using WcfChatClient.NetFramework.WcfChat;

namespace WcfChatClient.NetFramework
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //            ServiceHost selfHost = new ServiceHost();
            var chatClient = new ChatClient(new InstanceContext(new MyClass()));
            Console.WriteLine("Enter username: ");
            var username = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("Bye!");
                return;
            }

            var client = new Client { Name = username };
            chatClient.Connect(client);

            string message;
            do
            {
                message = Console.ReadLine();
                chatClient.Say(new Message() { Content = message, Sender = client.Name, Time = DateTime.Now });
            } while (message != "exit");
        }
    }

    internal class MyClass : IChatCallback
    {
        public void IsWritingCallback(Client client)
        {
            Console.WriteLine(nameof(IsWritingCallback));
        }

        public void Receive(Message msg)
        {
            Console.WriteLine($"{msg.Sender} ({msg.Time}): {msg.Content}");
        }

        public void ReceiveWhisper(Message msg, Client receiver)
        {
            Console.WriteLine(nameof(ReceiveWhisper));
        }

        public void RefreshClients(Client[] clients)
        {
            Console.WriteLine(nameof(RefreshClients));
        }

        public void UserJoin(Client client)
        {
            Console.WriteLine(nameof(UserJoin));
        }

        public void UserLeave(Client client)
        {
            Console.WriteLine(nameof(UserLeave));
        }
    }
}
