using System.Net;
using System.Text;
using RabbitMQ.Client;
Thread.Sleep(30000);

var factory = new ConnectionFactory { HostName = "rabbitmq" };
string lastDBVersion = "";
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();
{


    while (true)
    {
        Thread.Sleep(1800000);
        string currentDBVersion = "0";
        try
        {
            currentDBVersion = GetRequest("https://test-rasp.guap.ru:9002/api/db/version");
            
        }
        catch{}
        Console.WriteLine("Версия проверена");
        if (currentDBVersion != lastDBVersion)
        {
            channel.ExchangeDeclare(exchange: "SuaiScheduleApi", type: ExchangeType.Fanout);
            string message = "upda";
            var body = Encoding.UTF8.GetBytes(message);
            int i = 0;
            channel.BasicPublish(exchange: "SuaiScheduleApi",
                routingKey: "",
                basicProperties: null,
                body: body);
            i++;

            Console.WriteLine("Отправлен запрос на перекеширование в " + DateTime.Now);
            
        }

        lastDBVersion = currentDBVersion;

    }
}
string GetRequest(string URL)
{
    WebRequest reqGET = WebRequest.Create(URL);
    WebResponse resp = reqGET.GetResponse();
    Stream stream = resp.GetResponseStream();
    StreamReader sr = new StreamReader(stream);
    string s = sr.ReadToEnd();
    stream.Close();
    sr.Close();
    resp.Close();
    return s;
}