using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

Console.WriteLine("Start");

ConnectionFactory factory = new ConnectionFactory();
//factory.Uri = new(uriString: $"amqp://user:password@localhost:5672");
factory.Uri = new(uriString: $"amqp://localhost");
factory.Port = 5672;
factory.UserName = "user";
factory.Password = "password";
factory.ClientProvidedName = "RabbitMQ Receiver";


using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

string exchangeName = "rabbit-exchange";
string routingKey = "rabbit-routing-key";
string queueName = "RabbitQueue";

channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
channel.QueueDeclare(queueName,
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: default);

channel.QueueBind(queueName,
    exchangeName,
    routingKey,
    arguments: default);

channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);


var consumer = new EventingBasicConsumer(channel);
consumer.Received += (sender, args) =>
{
    Task.Delay(1000).Wait();
    var Body = args.Body.ToArray();
    string message = Encoding.UTF8.GetString(Body);
    Console.WriteLine($"[{DateTime.Now.ToString()}]  Received: {message}");
    channel.BasicAck(args.DeliveryTag, multiple: false);
};

string consumerTag = channel.BasicConsume(queueName, autoAck: false, consumer: consumer);

Console.WriteLine("press any key to continue");
Console.ReadLine();

channel.BasicCancel(consumerTag);

channel.Close();
connection.Close();

Console.WriteLine("End");