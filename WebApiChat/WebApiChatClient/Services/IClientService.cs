using System.Net.Http;
using System.Threading.Tasks;
using WebApiChatModels;

namespace WebApiChatClient.Services
{
    public interface IClientService
    {
        Task PostToServerAsync(HttpClient httpClient, string requestUri, HttpContent httpContent);

        void WriteMessage(ChatMessage chatMessage);

        void WriteString(string data);

        string ReadString();
    }
}
