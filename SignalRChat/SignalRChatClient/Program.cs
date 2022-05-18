using Microsoft.AspNetCore.SignalR.Client;
using SignalRChatModels;
using System;
using System.Threading.Tasks;

namespace SignalRChatClient
{
    public class Program
    {
        public static async Task Main()
        {
            Console.WriteLine("Enter service IP (default: localhost): ");
            var ip = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(ip))
                ip = "localhost";

            Console.Write("Enter your username: ");
            var username = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("Username is required. Bye!");
                return;
            }

            var connection = new HubConnectionBuilder()
                        .WithUrl($"http://{ip}:5000/chathub")
                        .WithAutomaticReconnect()
                        .Build();

            //Func<object, Task> reconnect = async (error) =>
            //{
            //    await connection.StopAsync();
            //    await connection.StartAsync();
            //};

            try
            {
                //connection.Closed += reconnect;
                connection.On<ChatMessage.Dto>(
                    "ReceiveMessage",
                    chatMessage => Console.WriteLine($"{chatMessage.Author} ({chatMessage.CreatedOn}): {chatMessage.Content}"));

                await connection.StartAsync();
                connection.ServerTimeout = TimeSpan.FromSeconds(3 * 60);
                await connection.InvokeAsync("JoinAsync", new Client(username));

                Console.WriteLine("Type \"exit\" to quit the chat.");
                var line = Console.ReadLine();
                while (line != "exit")
                {
                    await connection.InvokeAsync("SendAsync", ChatMessage.Dto.From(new ChatMessage(username, line, DateTimeOffset.UtcNow)));
                    line = Console.ReadLine();
                }

                await connection.InvokeAsync("LeaveAsync", new Client(username));
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception)
            {
                Console.WriteLine("Chat service is not available.");
            }
            finally
            {
                await connection.DisposeAsync();
            }
        }
    }
}
