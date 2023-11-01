using System.Configuration;
using System.Net;
using System.Text;
using RabbitMQ.Client;
string Env = Environment.MachineName == "MacBook-Air-Danil"
    ? Environment.MachineName
    : "default";  
Thread.Sleep(Convert.ToInt32(ConfigurationManager.AppSettings.Get(Env + "StartTime")));
var factory = new ConnectionFactory { HostName = ConfigurationManager.AppSettings.Get(Env + "rabbitmq") };
string lastDBVersion = "";
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();
{


    while (true)
    {
        Thread.Sleep(180000);
        string currentDBVersion = "0";
        try
        {
            currentDBVersion = GetRequest("https://test-rasp.guap.ru:9002/api/db/version");
        }
        catch{continue; }
        Console.WriteLine("Версия проверена");
        if (currentDBVersion != lastDBVersion)
        {
            channel.ExchangeDeclare(exchange: "SuaiScheduleApi", type: ExchangeType.Fanout);
            string message = "upda";
            var body = Encoding.UTF8.GetBytes(message);
            int i = 0;
            channel.BasicPublish(exchange: "SuaiScheduleApi",
                routingKey: "reCache",
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