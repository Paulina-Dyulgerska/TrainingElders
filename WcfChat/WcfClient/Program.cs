using System;
using System.ServiceModel;
using WcfClient.ChatServiceProxy;

namespace WcfClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var chatClient = new ChatClient(new InstanceContext(new ChatClientCallBack()));
            Console.WriteLine("Enter service IP (default: localhost): ");
            var ip = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(ip))
                ip = "localhost";

            ChatClient chatClient = null;
            try
            {
                //var chatClient = new ChatClient(new InstanceContext(new ChatClientCallBack()), "NetTcpBinding_IChat", new EndpointAddress($"net.tcp://{ip}:50820/Chat/tcp"));
                // this is using the available tcp binding at the host: this one -> selfHost.AddServiceEndpoint(typeof(IChat), new NetTcpBinding(SecurityMode.None), "tcp");
                chatClient = new ChatClient(new InstanceContext(new ChatClientCallBack()), new NetTcpBinding(SecurityMode.None), new EndpointAddress($"net.tcp://{ip}:50820/Chat/tcp"));
                chatClient.Open();

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

                chatClient.Disconnect(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An exception occurred: {ex}");
            }
            finally
            {
                chatClient?.Close();
            }
        }
    }
}
