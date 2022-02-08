using ChatModels;

namespace RabbitMQChatClient
{
    public class Chat
    {
        private readonly ChatRoomApplicationService chatRoomApplicationService;
        private readonly IChatMessageReceiver chatMessageReceiver;

        public Chat(IChatMessageReceiver chatMessageReceiver, ChatRoomApplicationService chatRoomApplicationService)
        {
            this.chatMessageReceiver = chatMessageReceiver;
            this.chatRoomApplicationService = chatRoomApplicationService;
        }

        public async Task StartAsync(Client client)
        {
            try
            {
                chatMessageReceiver.RegisterMessageReseivedHandler((c, cm) =>
                {
                    Console.WriteLine($"{cm?.Author} ({cm?.CreatedOn}): {cm?.Content}");
                    return true;
                });

                if (chatRoomApplicationService.UserIsJoined(client.Username))
                {
                    Console.WriteLine("bye");
                    return;
                }

                chatMessageReceiver.Start(client);

                await chatRoomApplicationService.Join(client);

                var line = Console.ReadLine();
                while (line != "exit")
                {
                    //// For histroty debug:
                    //if (line == "hh")
                    //{
                    //    var history = chatRoomApplicationService.GetHistory();
                    //    foreach (var cm in history)
                    //    {
                    //        Console.WriteLine($"(history): {cm?.Author} ({cm?.CreatedOn}): {cm?.Content} {cm?.IsForAll} {cm?.Receiver?.Username}");
                    //    }
                    //    line = string.Empty;
                    //    continue;
                    //}

                    if (string.IsNullOrWhiteSpace(line) == false)
                    {
                        await chatRoomApplicationService.PublishMessage(new ChatMessage(client.Username, line));
                    }

                    line = Console.ReadLine();
                }

                await chatRoomApplicationService.Leave(client);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
