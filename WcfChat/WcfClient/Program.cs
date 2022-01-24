using System;
using System.ServiceModel;
using WcfClient.ChatServiceProxy;

namespace WcfClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var chatClient = new ChatClient(new InstanceContext(new ChatClientCallBack()));
            Console.WriteLine("Enter your username: ");
            var username = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("Username is required. Bye!");
                return;
            }

            var user = new User { Name = username };
            var userConnected = chatClient.Connect(user);
            if (userConnected == false)
            {
                Console.WriteLine($"Cannot join Elders' chat room. Please contact room admin.");
                return;
            }
            Console.WriteLine($"Hello {user.Name} and welcome to Elders' chat room.");

            string message;
            do
            {
                message = Console.ReadLine();
                chatClient.Say(new ChatMessage() { Content = message, Sender = user.Name, Time = DateTime.Now });
            } while (message != "exit");
        }
    }
}
