using RabbitMQ.Client;
using System.Text;

Console.WriteLine("Start", ConsoleColor.DarkGreen);


ConnectionFactory factory = new ConnectionFactory();
//factory.Uri = new(uriString: $"amqp://user:password@localhost:5672");
factory.Uri = new(uriString: $"amqp://localhost");
factory.Port = 5672;
factory.UserName = "user";
factory.Password = "password";
factory.ClientProvidedName = "RabbitMQ Sender";


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

for(int i = 0; i < 15; i++)
{
    Task.Delay(555).Wait();

    string message = $"Hello World! v{i}";
    var messageBytes = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(exchangeName, routingKey, basicProperties: default, messageBytes);

    Console.WriteLine($"[{DateTime.Now.ToString()}]  Sended {message}");
}

channel.Close();
connection.Close();

Console.WriteLine("Done", ConsoleColor.Magenta);
