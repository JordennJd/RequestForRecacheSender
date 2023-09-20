using System.Text;
using RabbitMQ.Client;
// Console.WriteLine(DateTime.Now.AddDays(1).Date.ToString("d"));
var factory = new ConnectionFactory { HostName = "10.244.0.173" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();
{

    channel.ExchangeDeclare(exchange: "SuaiScheduleApi", type: ExchangeType.Fanout);
    string message = "Tvou doch ebut a ty chto?";
    var body = Encoding.UTF8.GetBytes(message);
    int i = 0;
    channel.BasicPublish(exchange: "SuaiScheduleApi",
        routingKey: "",
        basicProperties: null,
        body: body);
    i++;

    Console.WriteLine($" [x] Sent {message}");
    Console.WriteLine(" Press [enter] to exit.");
}