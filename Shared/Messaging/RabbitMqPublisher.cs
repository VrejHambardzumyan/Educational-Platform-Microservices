using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Shared.Messaging
{
    public class RabbitMqPublisher : IEventPublisher, IAsyncDisposable
    {
        private readonly ILogger<RabbitMqPublisher> _logger;
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private const string ExchangeName = "edu-platform.events";

        private RabbitMqPublisher(ILogger<RabbitMqPublisher> logger, IConnection connection, IChannel channel)
        {
            _logger = logger;
            _connection = connection;
            _channel = channel;
        }

        public static async Task<RabbitMqPublisher> CreateAsync(IConfiguration configuration, ILogger<RabbitMqPublisher> logger)
        {
            var factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:Host"] ?? "localhost",
                Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
                UserName = configuration["RabbitMQ:UserName"] ?? "guest",
                Password = configuration["RabbitMQ:Password"] ?? "guest"
            };

            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(
                exchange: ExchangeName,
                type: ExchangeType.Topic,
                durable: true);

            return new RabbitMqPublisher(logger, connection, channel);
        }

        public async Task PublishAsync<T>(string routingKey, T message, CancellationToken cancellationToken = default)
        {
            try
            {
                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);

                var properties = new BasicProperties
                {
                    ContentType = "application/json",
                    DeliveryMode = DeliveryModes.Persistent
                };

                await _channel.BasicPublishAsync(
                    exchange: ExchangeName,
                    routingKey: routingKey,
                    mandatory: false,
                    basicProperties: properties,
                    body: body,
                    cancellationToken: cancellationToken);

                _logger.LogInformation("Published event {RoutingKey}", routingKey);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to publish event {RoutingKey} — continuing without notification", routingKey);
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _channel.CloseAsync();
            await _connection.CloseAsync();
            _channel.Dispose();
            _connection.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
