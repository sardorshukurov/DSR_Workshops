using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NetSchool.Services.RabbitMq;

public class RabbitMq : IRabbitMq, IDisposable
{
    const int ConnectRetriesCount = 10;

    private readonly object _connectionLock = new();
    private readonly RabbitMqSettings _settings;
    private IModel _channel;

    private IConnection _connection;

    public RabbitMq(RabbitMqSettings settings)
    {
        _settings = settings;
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }

    private IModel GetChannel()
    {
        return _channel;
    }

    private async Task RegisterListener(string queueName, EventHandler<BasicDeliverEventArgs> onReceive)
    {
        Connect();

        AddQueue(queueName);

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += onReceive;

        _channel.BasicConsume(queueName, false, consumer);
    }

    private async Task Publish<T>(string queueName, T data)
    {
        Connect();

        AddQueue(queueName);

        var json = JsonSerializer.Serialize<object>(data, new JsonSerializerOptions() { });

        var message = Encoding.UTF8.GetBytes(json);

        _channel.BasicPublish(string.Empty, queueName, null, message);
    }

    private void Connect()
    {
        lock (_connectionLock)
        {
            if (_connection?.IsOpen ?? false)
                return;

            var factory = new ConnectionFactory
            {
                Uri = new Uri(_settings.Uri),
                UserName = _settings.UserName,
                Password = _settings.Password,

                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
            };

            var retriesCount = 0;
            while (retriesCount < ConnectRetriesCount)
                try
                {
                    if (_connection == null)
                    {
                        _connection = factory.CreateConnection();
                    }

                    if (_channel == null)
                    {
                        _channel = _connection.CreateModel();
                        _channel.BasicQos(0, 1, false);
                    }

                    break;
                }
                catch (BrokerUnreachableException)
                {
                    Task.Delay(500).Wait();

                    retriesCount++;
                }
        }
    }

    private void AddQueue(string queueName)
    {
        Connect();
        _channel.QueueDeclare(queueName, true, false, false, null);
    }

    public async Task Subscribe<T>(string queueName, OnDataReceiveEvent<T> onReceive)
    {
        if (onReceive == null)
            return;

        await RegisterListener(queueName, async (_, eventArgs) =>
        {
            var channel = GetChannel();
            try
            {
                var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

                var obj = JsonSerializer.Deserialize<T>(message ?? "");

                await onReceive(obj);
                channel.BasicAck(eventArgs.DeliveryTag, false);
            }
            catch (Exception e)
            {
                channel.BasicNack(eventArgs.DeliveryTag, false, false);
            }
        });
    }

    public async Task PushAsync<T>(string queueName, T data)
    {
        await Publish(queueName, data);
    }
}