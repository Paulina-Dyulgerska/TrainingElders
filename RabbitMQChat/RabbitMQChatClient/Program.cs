using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQChatClient;
using System.Text;

Console.WriteLine("Welcome to our chat!");

//var queueName = "chat";

var channel = RabbitMQCommunicationChannel.Create("localhost", 5672);

using (channel)
{
    //channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

    //// receive message
    //var consumer = new EventingBasicConsumer(channel);
    //consumer.Received += (model, ea) =>
    //{
    //    var body = ea.Body.ToArray();
    //    var message = Encoding.UTF8.GetString(body);
    //    Console.WriteLine($" [x] Received {message}");
    //};
    //channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

    // receive message
    var queueName = channel.QueueDeclare().QueueName;
    channel.QueueBind(queue: queueName,
                      exchange: "messageExchange",
                      routingKey: "");
    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (model, ea) =>
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false); // Note: it is possible to access the channel via ((EventingBasicConsumer)sender).Model here
        Console.WriteLine($" [x] Received {message}");
    };
    channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

    // send message
    var message = Console.ReadLine();
    while (message != "exit")
    {
        if (string.IsNullOrWhiteSpace(message))
            continue;

        //var body = Encoding.UTF8.GetBytes(message);
        //channel.BasicPublish(exchange: "",
        //                     routingKey: queueName,
        //                     basicProperties: null,
        //                     body: body);
        //Console.WriteLine($" [x] Sent {message}");

        channel.ExchangeDeclare(exchange: "messageExchange", ExchangeType.Fanout);
        var body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchange: "messageExchange",
                             //routingKey: queueName,
                             routingKey: "",
                             basicProperties: null,
                             body: body);

        Console.WriteLine($" [x] Sent {message}");
        message = Console.ReadLine();
    }

    //channel.QueueDelete(queueName);
    //channel.ExchangeDelete(exchange: "messageExchange");
}


//using (var connection = factory.CreateConnection())
//{
//    var receiver = new Receiver();
//    var message = receiver.ReceiveMessage(connection, queueName);
//    Console.WriteLine($" [x] Received {message}");

//    // send message
//    var sender = new Sender();
//    var line = Console.ReadLine();
//    while (line != "exit")
//    {
//        if (string.IsNullOrWhiteSpace(line))
//            continue;

//        var sendResult = sender.SendMessage(connection, queueName, line);
//        if (sendResult)
//        {
//            Console.WriteLine($" [x] Sent {line}");
//        }
//        else
//        {
//            Console.WriteLine($" [x] Cannot sent {line}");
//        }
//        line = Console.ReadLine();
//    }
//}