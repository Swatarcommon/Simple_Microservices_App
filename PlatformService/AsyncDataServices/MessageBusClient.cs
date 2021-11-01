using Microsoft.Extensions.Configuration;
using PlatformService.DTOs;
using RabbitMQ.Client;
using System;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace PlatformService.AsyncDataServices {
    public class MessageBusClient : IMessageBusClient {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBusClient(IConfiguration configuration) {
            _configuration = configuration;
            var factory = new ConnectionFactory() {
                HostName = configuration["RabbitMQHost"],
                Port = Int32.Parse(configuration["RabbitMQPort"])
            };
            try {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                Console.WriteLine("--> Connected to the message bus");
            } catch (Exception ex) {
                Debug.WriteLine($"Couldnt connect to the message bus: {ex.Message}");
            }
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) {
            Debug.WriteLine("--> RabbitMQ Connection Shutdown");
        }

        public void PublishNewPlatform(PlatformPublishedDTO platformPublishedDTO) {
            var message = JsonSerializer.Serialize(platformPublishedDTO);
            if (_connection.IsOpen) {
                Debug.WriteLine("--> RabbitMQ connection open, sending message...");
                SendMessage(message);
            } else {
                Debug.WriteLine("--> RabbitMQ closed...");
            }
        }

        public void Dispose() {
            Debug.WriteLine("--> Message bus disposed...");
            if (_channel.IsOpen) {
                _channel.Close();
                _connection.Close();
            }
        }

        private void SendMessage(string message) {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "trigger", routingKey: "", basicProperties: null, body: body);
            Debug.WriteLine($"--> Message sended: {message}");
        }
    }
}
