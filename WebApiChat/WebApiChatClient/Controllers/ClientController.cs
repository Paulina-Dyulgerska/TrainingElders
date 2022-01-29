using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApiChatClient.Services;
using WebApiChatModels;

namespace WebApiChatService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService clientService;
        private readonly IOptions<UrlOptions> options;

        public ClientController(IClientService clientService, IOptions<UrlOptions> options)
        {
            this.clientService = clientService;
            this.options = options;
        }

        [HttpPost]
        public IActionResult Post(ChatMessage chatMessage)
        {
            clientService.WriteMessage(chatMessage);
            return this.Ok();
        }
    }
}
