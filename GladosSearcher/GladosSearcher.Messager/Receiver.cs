using GladosSearcher.Messager.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace GladosSearcher.Messager
{
    public class Receiver
    {
        private readonly ConnectionFactory connectionFactory;

        public Receiver() 
        { 
            connectionFactory = new ConnectionFactory()
            {
                Uri = new Uri("amqps://jkkhsszz:Gj4F7_5qNVZjhO-EbsOiIV3GZqWvZEkN@jellyfish.rmq.cloudamqp.com/jkkhsszz")
            };
        }

        public void Execute(string queueName, Action<ScheduleJurimetryModel> exec) 
        {
            using (var connection = connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (sender, eventArgs) =>
                {
                    var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

                    Console.WriteLine(Environment.NewLine + "[New message received] " + message);

                    var format = "dd/MM/yyyy"; // your datetime format
                    var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };

                    var responseJson = JsonConvert.DeserializeObject<ScheduleJurimetryModel>(message, dateTimeConverter);
                    
                    exec.Invoke(responseJson);
                };

                channel.BasicConsume(queue: queueName,
                     autoAck: true,
                     consumer: consumer);

                while (true) 
                {
                    Console.WriteLine("Waiting messages to proccess");
                    Thread.Sleep(10000);
                }
            }
        }
    }
}
