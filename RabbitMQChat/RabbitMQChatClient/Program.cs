using ChatModels;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQChatClient;

string host = UI.Cli.GetHost();
string username = UI.Cli.GetUsername();

var serviceProvider = new ServiceCollection()
    .AddSingleton<IConnection>(sp => new ConnectionFactory() { HostName = host, Port = 5672, NetworkRecoveryInterval = TimeSpan.FromSeconds(10) }.CreateConnection())
    .AddTransient<RabbitMqModelFactory>()
    .AddTransient<ISerializer, Serializer>()
    .AddTransient<IChatMessageSender, RabbitMqChatMessageSender>(sp =>
    {
        var model = sp.GetRequiredService<RabbitMqModelFactory>().Build(username);
        var serializer = sp.GetRequiredService<ISerializer>();
        return new RabbitMqChatMessageSender(model, serializer);
    })
    .AddSingleton<IChatMessageReceiver, RabbitMqChatMessageReceiver>()
    .AddSingleton<ChatRoom>()
    .AddSingleton<ChatRoomApplicationService>()
    .AddSingleton<Chat>()
    .BuildServiceProvider();

using (serviceProvider)
{
    var chat = serviceProvider.GetRequiredService<Chat>();
    var client = new Client(username);
    await chat.StartAsync(client);
}