using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using WebApiChatClient.Services;
using WebApiChatModels;

namespace WebApiChatClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //var host = CreateHostBuilder(args).Build();
            var host = WebHost.CreateDefaultBuilder()
                                .UseStartup<Startup>()
                                .UseUrls("http://*:0") // This 0 enables binding to random port
                                .Build();

            await host.StartAsync();

            var address = host.ServerFeatures.Get<IServerAddressesFeature>().Addresses.First();
            var port = int.Parse(address.Split(':').Last());

            Console.WriteLine("Enter your username: ");
            var username = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(username))
                return;
            var client = new Client { Id = Guid.NewGuid().ToString(), UserName = username, Port = port };
            HttpContent clientHttpContent = new StringContent(JsonSerializer.Serialize(client));

            var clientService = new ClientService();
            using (var chatServer = new HttpClient() { BaseAddress = new Uri("http://localhost:5000") })
            {
                try
                {
                    await clientService.PostToServerAsync(chatServer, "chat/connect", clientHttpContent);

                    var line = clientService.ReadString();
                    while (line != "exit")
                    {
                        var chatMessage = new ChatMessage()
                        {
                            UserName = username,
                            Content = line,
                            CreatedOn = DateTime.Now,
                        };
                        var messageHttpContent = new StringContent(JsonSerializer.Serialize(chatMessage));
                        await clientService.PostToServerAsync(chatServer, "chat", messageHttpContent);
                        line = clientService.ReadString();
                    }

                    //await host.WaitForShutdownAsync();
                }
                catch (Exception)
                {
                }
                finally
                {
                    await clientService.PostToServerAsync(chatServer, "chat/disconnect", clientHttpContent);
                }
            }

            await host.StopAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
