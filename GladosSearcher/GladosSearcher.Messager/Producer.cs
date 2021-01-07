using GladosSearcher.Domain;
using GladosSearcher.Messager.Domain;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace GladosSearcher.Messager
{
    public class Producer
    {
        private readonly ConnectionFactory connectionFactory;

        public Producer()
        {
            connectionFactory = new ConnectionFactory()
            {
                Uri = new Uri("amqps://jkkhsszz:Gj4F7_5qNVZjhO-EbsOiIV3GZqWvZEkN@jellyfish.rmq.cloudamqp.com/jkkhsszz")
            };
        }

        const string queuename = "save_jurisprudence";

        public void Publish(List<CourtJurisprudenceModel> jurisprudences)
        {
            using (var connection = connectionFactory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queuename,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                    foreach(var jurisprudence in jurisprudences)
                    {
                        var message = JsonConvert.SerializeObject(jurisprudence);
                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "",
                                             routingKey: queuename,
                                             basicProperties: null,
                                             body: body);
                        Console.WriteLine(" [x] Sent {0}", message);
                    }
                }
            }
        }
    }
}
