// See https://aka.ms/new-console-template for more information

using RabbitMQ.Client;
using System.Text;

var queueName = "chat";
var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.ExchangeDeclare(exchange: "messageExchange", ExchangeType.Fanout);

    //channel.QueueDeclare(queue: queueName,
    //                            durable: true,
    //                            exclusive: false,
    //                            autoDelete: false,
    //                            arguments: null);

    string message = "Hello from my message sender!";
    var body = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(exchange: "messageExchange",
                         //routingKey: queueName,
                         routingKey: "",
                         basicProperties: null,
                         body: body);
    Console.WriteLine($" [x] Sent {message}");
}

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();