using ChatModels;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQChatClient;
using System.Text;
using System.Text.Json;

Console.WriteLine("Enter service IP (default: localhost): ");
var ip = Console.ReadLine();
if (string.IsNullOrWhiteSpace(ip))
    ip = "localhost";

Console.Write("Enter your username: ");
var username = Console.ReadLine();
if (string.IsNullOrWhiteSpace(username))
{
    Console.WriteLine("Username is required. Bye!");
    return;
}

var chatRoom = new ChatRoom();
var factory = new ConnectionFactory() { HostName = ip, Port = 5672 };
var connection = factory.CreateConnection();
var channel = new RabbitMQCommunicationChannel(connection);
var queueName = channel.Build(username);
var client = new Client(username, queueName);
var chatRoomApplicationService = new ChatRoomApplicationService(channel, chatRoom);

EventHandler<BasicDeliverEventArgs> OnNewClientConnected = async (sender, ea) =>
{
    if (sender != null)
        ((EventingBasicConsumer)sender).Model.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

    var body = ea.Body.ToArray();
    var serializedMessage = Encoding.UTF8.GetString(body);
    var message = JsonSerializer.Deserialize<ChatMessage.Dto>(serializedMessage);
    Console.WriteLine($"From archive queue => {message?.Author} ({message?.CreatedOn}): {message?.Content}");
    //channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false); // Note: it is possible to access the channel via ((EventingBasicConsumer)sender).Model here

    if (message != null && ea.BasicProperties.ReplyTo != client.ConnectionId)
        await chatRoomApplicationService.SendHistory(new Client(message?.Author, ea.BasicProperties.ReplyTo));
};

EventHandler<BasicDeliverEventArgs> OnNewMessageReceived = (sender, ea) =>
{
    if (sender != null)
        ((EventingBasicConsumer)sender).Model.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

    var body = ea.Body.ToArray();
    var serializedMessage = Encoding.UTF8.GetString(body);
    var message = JsonSerializer.Deserialize<ChatMessage.Dto>(serializedMessage);
    Console.WriteLine($"{message?.Author} ({message?.CreatedOn}): {message?.Content}");
    //channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false); // Note: it is possible to access the channel via ((EventingBasicConsumer)sender).Model here

    if (message != null && ea.BasicProperties.ReplyTo != client.ConnectionId)
        chatRoom.AppentToHistory(message.ToModel());
};

try
{
    using (connection)
    {
        channel.OnNewClientConnect += OnNewClientConnected;
        channel.OnNewMessageReceive += OnNewMessageReceived;
        //channel.OnModelShutdown += OnModeledShutdown;

        await chatRoomApplicationService.Join(client);

        var line = Console.ReadLine();
        while (line != "exit")
        {
            if (string.IsNullOrWhiteSpace(line) == false)
            {
                await chatRoomApplicationService.PublishMessage(new ChatMessage(username, line));
            }
            line = Console.ReadLine();
        }

        await chatRoomApplicationService.Leave(client);

        channel.OnNewMessageReceive -= OnNewMessageReceived;
        channel.OnNewClientConnect -= OnNewClientConnected;
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    //Console.WriteLine("Chat service is not available.");
    channel.OnNewMessageReceive -= OnNewMessageReceived;
    channel.OnNewClientConnect -= OnNewClientConnected;
    connection.Close();
}
