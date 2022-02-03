using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

//var queueName = "chat";
var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    //channel.QueueDeclare(queue: queueName,
    //                            durable: true,
    //                            exclusive: false,
    //                            autoDelete: false,
    //                            arguments: null);

    var queueName = channel.QueueDeclare().QueueName;

    channel.QueueBind(queue: queueName,
                      exchange: "messageExchange",
                      routingKey: "");

    Console.WriteLine(" [*] Waiting for messages.");

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (model, ea) =>
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false); // Note: it is possible to access the channel via ((EventingBasicConsumer)sender).Model here

        Console.WriteLine(" [x] Received {0}", message);
    };
    channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadLine();
}
