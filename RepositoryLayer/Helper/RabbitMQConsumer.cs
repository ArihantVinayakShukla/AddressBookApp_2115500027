using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RepositoryLayer.Service;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Helper
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly string _hostName;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _queue;
        private readonly IServiceProvider _serviceProvider;

        public RabbitMQConsumer(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _hostName = configuration["RabbitMQ:HostName"];
            _userName = configuration["RabbitMQ:UserName"];
            _password = configuration["RabbitMQ:Password"];
            _queue = configuration["RabbitMQ:Queue"];
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _hostName,
                UserName = _userName,
                Password = _password
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: _queue, durable: false, exclusive: false, autoDelete: false, arguments: null);
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"[RabbitMQ] Received message: {message}");

                
                await ProcessMessageAsync(message);
            };

            channel.BasicConsume(queue: _queue, autoAck: true, consumer: consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken); 
            }
        }

        private async Task ProcessMessageAsync(string message)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                if (message.StartsWith("User registered:"))
                {
                    string[] parts = message.Split(":");
                    if (parts.Length >= 3)
                    {
                        string email = parts[1].Trim();
                        string role = parts[2].Trim();

                        Console.WriteLine($"[RabbitMQ] Sending welcome email to {email} with role {role}");
                        await emailService.SendWelcomeEmailAsync(email);
                    }
                }
                else if (message.StartsWith("New contact added:"))
                {
                    Console.WriteLine($"[RabbitMQ] Logging contact creation: {message}");
                }
            }
        }
    }
}
