using ChatModels;
using RabbitMQ.Client;
using RabbitMQChatClient;

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

var client = new Client(username);
//var queueName = "chat";

var factory = new ConnectionFactory() { HostName = ip, Port = 5672 };
var connection = factory.CreateConnection();
var channel = new RabbitMQCommunicationChannel(connection);
var rabbitMQService = new RabbitMQService(new ChatRoomApplicationService(channel, new ChatRoom()));
try
{
    using (connection)
    {
        await rabbitMQService.Join(client);

        var line = Console.ReadLine();
        while (line != "exit")
        {
            if (string.IsNullOrWhiteSpace(line) == false)
            {
                await rabbitMQService.Send(ChatMessage.Dto.From(new ChatMessage(username, line)));
            }
            line = Console.ReadLine();
        }

        await rabbitMQService.Leave(client);
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    //Console.WriteLine("Chat service is not available.");
    connection.Close();
}
