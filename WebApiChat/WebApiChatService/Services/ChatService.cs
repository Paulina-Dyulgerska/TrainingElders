using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using WebApiChatModels;
using WebApiChatService.Data;

namespace WebApiChatService.Services
{
    public class ChatService : IChatService
    {
        private readonly ILogger<ChatService> logger;
        private readonly AppStore appStore;

        public ChatService(ILogger<ChatService> logger, AppStore appStore)
        {
            this.logger = logger;
            this.appStore = appStore;
        }

        public IEnumerable<ChatMessage> GetAllMessages()
        {
            return appStore.Messages;
        }

        public IEnumerable<Client> GetAllClients()
        {
            return appStore.Clients;
        }

        public async Task AddClientAsync(Client client)
        {
            await InformClients(new ChatMessage { UserName = "Server", Content = $"User {client.UserName} joined the chat.", CreatedOn = DateTime.Now });
            appStore.AddClient(client);
        }

        public async Task RemoveClientAsync(Client client)
        {
            appStore.RemoveClient(client);
            await InformClients(new ChatMessage { UserName = "Server", Content = $"User {client.UserName} left the chat.", CreatedOn = DateTime.Now });
        }

        public async Task AddChatMessageAsync(ChatMessage chatMessage)
        {
            appStore.AddChatMessage(chatMessage);
            await InformClients(chatMessage);
        }

        private async Task InformClients(ChatMessage chatMessage)
        {
            var clientsForRemove = new List<Client>();

            foreach (var client in appStore.Clients)
            {
                try
                {
                    using (var httpClient = new HttpClient() { BaseAddress = new Uri(client.Url) })
                    {
                        HttpContent httpContent = new StringContent(JsonSerializer.Serialize(chatMessage));
                        httpClient.DefaultRequestHeaders.Accept.Clear();
                        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                        HttpResponseMessage result = await httpClient.PostAsync("client", httpContent);
                        if (result.IsSuccessStatusCode)
                            logger.LogInformation($"Response from client {client.UserName} ({client.Url}): {result.StatusCode}");
                        else
                        {
                            logger.LogInformation($"Response from client {client.UserName} ({client.Url}): {result.StatusCode}");
                            clientsForRemove.Add(client);
                        }
                    }
                }
                catch (Exception)
                {
                    clientsForRemove.Add(client);
                }
            }

            foreach (var client in clientsForRemove)
            {
                appStore.RemoveClient(client);
                logger.LogInformation($"Client {client.UserName} ({client.Url}): removed!");
                await InformClients(new ChatMessage { UserName = "Server", Content = $"User {client.UserName} left the chat.", CreatedOn = DateTime.Now });
            }
        }
    }
}
