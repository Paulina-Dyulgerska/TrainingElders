using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApiChatModels;
using WebApiChatService.Services;

namespace WebApiChatService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ILogger<ChatController> logger;
        private readonly IChatService chatService;

        public ChatController(ILogger<ChatController> logger, IChatService chatService)
        {
            this.logger = logger;
            this.chatService = chatService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var chatMessages = chatService.GetAllMessages();
            foreach (var chatMessage in chatMessages)
            {
                logger.LogInformation($"{chatMessage.UserName} ({chatMessage.CreatedOn}): {chatMessage.Content}");
            }
            return this.Ok(chatMessages);
        }

        // chat/connect
        [HttpPost("connect")]
        public async Task<IActionResult> PostConnect(Client client)

        {
            var remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;
            string address = remoteIpAddress.ToString() == "::1" ? "localhost" : "";
            if (string.IsNullOrWhiteSpace(address))
            {
                // If we got an IPV6 address, then we need to ask the network for the IPV4 address. This usually only happens when the clinet/browser is on the same machine as the server.
                if (remoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    remoteIpAddress = System.Net.Dns.GetHostEntry(remoteIpAddress).AddressList.First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                }
                address = remoteIpAddress.ToString();
            }

            //var port = this.Request.HttpContext.Connection.RemotePort;
            //client.Url = new UriBuilder("http", address, port).ToString();
            client.Url = new UriBuilder("http", address, client.Port).ToString();

            await chatService.AddClientAsync(client);
            logger.LogInformation($"Client {client.UserName} subscribed!");
            return this.Ok($"Welcome to our chat {client.UserName}!");
        }

        [HttpPost]
        public async Task<IActionResult> Post(ChatMessage chatMessage)
        {
            await chatService.AddChatMessageAsync(chatMessage);
            return this.Ok();
        }

        // chat/disconnect
        [HttpPost("disconnect")]
        public async Task<IActionResult> PostDisconnect(Client client)
        {
            await chatService.RemoveClientAsync(client);
            logger.LogInformation($"Client {client.UserName} unsubscribed!");
            return this.Ok($"Bye {client.UserName}!");
        }
    }
}
