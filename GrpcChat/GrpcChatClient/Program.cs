using Grpc.Core;
using Grpc.Net.Client;
using GrpcClient;
using System;
using System.Threading.Tasks;

namespace GrpcChatClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Enter service IP (default: localhost): ");
            var ip = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(ip))
                ip = "localhost";

            Console.WriteLine("Enter your username: ");
            var username = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("Username is required. Bye!");
                return;
            }

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Name = username,
            };

            var channel = GrpcChannel.ForAddress($"https://{ip}:5001", new GrpcChannelOptions { Credentials = ChannelCredentials.SecureSsl }); // TODO 5000 port for http?
            var client = new Chat.ChatClient(channel);
            var connectUserReply = await client.ConnectAsync(new ConnectUserRequest
            {
                User = user,
            });

            using (var streaming = client.SendMessageToChatService(new Metadata { new Metadata.Entry("Username", user.Name) }))
            {
                var response = Task.Run(async () =>
                {
                    while (await streaming.ResponseStream.MoveNext())
                    {
                        Console.WriteLine($"{streaming.ResponseStream.Current.UserName}: {streaming.ResponseStream.Current.Message}");
                    }
                });

                await streaming.RequestStream.WriteAsync(new ChatMessage
                {
                    UserId = user.Id,
                    UserName = user.Name,
                    Message = "",
                });
                Console.WriteLine($"Joined the chat as {user.Name}");

                var line = Console.ReadLine();
                while (!string.Equals(line, "exit", StringComparison.OrdinalIgnoreCase))
                {
                    await streaming.RequestStream.WriteAsync(new ChatMessage
                    {
                        UserId = user.Id,
                        UserName = user.Name,
                        Message = line,
                    });
                    line = Console.ReadLine();
                }
                await streaming.RequestStream.CompleteAsync();
            }
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
