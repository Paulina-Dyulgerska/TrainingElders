using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebApiChatModels;

namespace WebApiChatClient.Services
{
    public class ClientService : IClientService
    {
        public async Task PostToServerAsync(HttpClient httpClient, string requestUri, HttpContent httpContent)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage result = await httpClient.PostAsync(requestUri, httpContent);
            var resultContent = await result.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(resultContent) == false)
            {
                WriteString(resultContent.ToString());
            }
        }

        public void WriteMessage(ChatMessage chatMessage)
        {
            Console.WriteLine($"{chatMessage.UserName} ({chatMessage.CreatedOn}): {chatMessage.Content}");
        }

        public void WriteString(string data)
        {
            Console.WriteLine(data);
        }

        public string ReadString()
        {
            return Console.ReadLine();
        }
    }
}
