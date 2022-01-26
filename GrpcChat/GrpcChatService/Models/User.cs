using Grpc.Core;

namespace GrpcChatService.Models
{
    public class User
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public IAsyncStreamWriter<ChatMessage> responseStream { get; set; }
    }
}
