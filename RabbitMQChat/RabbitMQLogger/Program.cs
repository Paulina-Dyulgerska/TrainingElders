using ChatModels;
using RabbitMQ.Client;
using RabbitMQChatLogger;

var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
var connection = factory.CreateConnection();
var channel = new RabbitMQLogger(connection);
var chatRoomApplicationService = new ChatRoomApplicationService(channel, new ChatRoom());
var username = "logger";
//var client = new Client(username);
try
{
    using (connection)
    {
        var line = Console.ReadLine();
        while (line != "q")
        {
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    connection.Close();
}