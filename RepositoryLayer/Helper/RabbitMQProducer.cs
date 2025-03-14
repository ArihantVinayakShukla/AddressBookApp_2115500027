using System;
using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace RepositoryLayer.Helper
{
    public class RabbitMQProducer
    {
        private readonly string _hostName;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _exchange;
        private readonly string _queue;

        public RabbitMQProducer(IConfiguration configuration)
        {
            _hostName = configuration["RabbitMQ:HostName"];
            _userName = configuration["RabbitMQ:UserName"];
            _password = configuration["RabbitMQ:Password"];
            _exchange = configuration["RabbitMQ:Exchange"];
            _queue = configuration["RabbitMQ:Queue"];
        }

        public void PublishMessage(string routingKey, string message)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _hostName,
                UserName = _userName,
                Password = _password
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: _exchange, type: ExchangeType.Direct, durable: true);

                channel.QueueDeclare(queue: _queue, durable: false, exclusive: false, autoDelete: false, arguments: null);

                channel.QueueBind(queue: _queue, exchange: _exchange, routingKey: routingKey);

                var body = Encoding.UTF8.GetBytes(message);
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true; 

                channel.BasicPublish(exchange: _exchange, routingKey: routingKey, basicProperties: properties, body: body);

                Console.WriteLine($"[RabbitMQ] Published message to queue '{_queue}': {message}");
            }
        }
    }
}
