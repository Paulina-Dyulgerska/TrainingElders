using ChatModels;
using RabbitMQ.Client.Events;

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

        //private void OnNewMessageReceived(object? sender, BasicDeliverEventArgs ea)
        //{
        //    // Note: it is possible to access the channel via ((EventingBasicConsumer)sender).Model here
        //    if (sender != null)
        //        ((EventingBasicConsumer)sender).Model.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

        //    var message = MessageDeserializer.DeserializeMessage<ChatMessage.Dto>(sender, ea);
        //    Console.WriteLine($"{message?.Author} ({message?.CreatedOn}): {message?.Content}");
        //    if (message != null && ea.BasicProperties.ReplyTo != client.ConnectionId)
        //        chatRoom.AppentToHistory(message.ToModel());
        //}

        public async Task StartAsync(Client client)
        {
            //EventHandler<BasicDeliverEventArgs> OnNewClientConnected = async (sender, ea) =>
            //{
            //    var message = MessageDeserializer.DeserializeMessage<ChatMessage.Dto>(sender, ea);
            //    Console.WriteLine($"From archive queue => {message?.Author} ({message?.CreatedOn}): {message?.Content}")
            //    ;
            //    if (message != null && ea.BasicProperties.ReplyTo != client.ConnectionId)
            //        await chatRoomApplicationService.SendHistory(new Client(message?.Author, ea.BasicProperties.ReplyTo));
            //};

            try
            {
                //((RabbitMQCommunicationChannel)channel).OnNewClientConnect += OnNewClientConnected;
                //((RabbitMQCommunicationChannel)channel).OnNewMessageReceive += OnNewMessageReceived;
                chatMessageReceiver.RegisterMessageReseivedHandler((c, cm) =>
                {
                    Console.WriteLine($"{cm?.Author} ({cm?.CreatedOn}): {cm?.Content}");
                    return true;
                });

                //chatMessageReceiver.RegisterClientConnectedHandler(c =>
                //{
                //    Console.WriteLine($"{c.Username} joined");
                //    return Task.CompletedTask;
                //});

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
                    if (line == "hh")
                    {
                        var history = chatRoomApplicationService.GetHistory();
                        foreach (var cm in history)
                        {
                            Console.WriteLine($"(history): {cm?.Author} ({cm?.CreatedOn}): {cm?.Content}");
                        }


                        line = string.Empty;
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(line) == false)
                    {
                        await chatRoomApplicationService.PublishMessage(new ChatMessage(client.Username, line));
                    }

                    line = Console.ReadLine();
                }

                await chatRoomApplicationService.Leave(client);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //Console.WriteLine("Chat service is not available.");
            }
            finally
            {
                //((RabbitMQCommunicationChannel)channel).OnNewClientConnect -= OnNewClientConnected;
                //((RabbitMQCommunicationChannel)channel).OnNewMessageReceive -= OnNewMessageReceived;
            }
        }
    }
}
