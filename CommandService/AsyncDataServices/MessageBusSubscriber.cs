
using CommandService.EventProcessing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommandService.AsyncDataServices {
    public class MessageBusSubscriber : BackgroundService {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProcessor;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor) {
            _configuration = configuration;
            _eventProcessor = eventProcessor;
            InitializeRabbitMQConnection();
        }

        private void InitializeRabbitMQConnection() {
            var factory = new ConnectionFactory() {
                HostName = _configuration["RabbitMQHost"],
                Port = Int32.Parse(_configuration["RabbitMQPort"])
            };
            try {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
                _queueName = _channel.QueueDeclare().QueueName;
                _channel.QueueBind(queue: _queueName,
                    exchange: "trigger", routingKey: "");

                Console.WriteLine("--> Listening on the message bus...");

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            } catch (Exception ex) {
                Console.WriteLine($"Couldnt connect to the message bus: {ex.Message}");
            }
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) {
            Console.WriteLine("--> RabbitMQ Connection Shutdown");
        }

        public void Dispose() {
            Console.WriteLine("--> Message bus disposed...");
            if (_channel.IsOpen) {
                _channel.Close();
                _connection.Close();
            }
            base.Dispose();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken) {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ModuleHandle, ea) => {
                Console.WriteLine("--> Event received!");
                var body = ea.Body;
                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());
                _eventProcessor.ProcessEvent(notificationMessage);
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
            return Task.CompletedTask;
        }
    }
}
