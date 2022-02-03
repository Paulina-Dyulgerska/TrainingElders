using ChatModels;

namespace RabbitMQChatClient
{
    internal class RabbitMQService
    {

        private readonly ChatRoomApplicationService chatRoomApplicationService;

        public RabbitMQService(ChatRoomApplicationService chatRoomApplicationService)
        {
            this.chatRoomApplicationService = chatRoomApplicationService;
        }

        public async Task Join(Client client)
        {
            await chatRoomApplicationService.Join(client);
        }

        public async Task Leave(Client client)
        {
            await chatRoomApplicationService.Leave(client);
        }

        public async Task Send(ChatMessage.Dto message)
        {
            await chatRoomApplicationService.PublishMessage(message.ToModel());
        }
    }
}
