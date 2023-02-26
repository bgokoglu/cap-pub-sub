using System.Text;
using System.Text.Json;
using Contracts;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;

namespace WorkerService1;

public class RedisWorker : BackgroundService
{
    private readonly ILogger<RedisWorker> _logger;
    private ISubscriber _subscriber;
    private string _channel;

    public RedisWorker(ILogger<RedisWorker> logger)
    {
        _logger = logger;
        InitRedis();
    }

    private void InitRedis()
    {
        const string connString = "localhost:6379,abortConnect=false";
        var connectionMultiplexer = ConnectionMultiplexer.Connect(connString);
        _channel = "cap.queue.producer.v1";
        _subscriber = connectionMultiplexer.GetSubscriber();
        
        _subscriber.Subscribe(_channel, (c, m) =>
        {
            var capMessage = JsonSerializer.Deserialize<CapMessage>(m);
            _logger.LogInformation("Received: {Id} - {Message}", capMessage.Id, capMessage.Message);
        });
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            //stoppingToken.ThrowIfCancellationRequested();  
            _logger.LogInformation("RedisWorker waiting for messages at: {time}", DateTimeOffset.Now);

            await Task.Delay(5000, stoppingToken);
        }
    }

    public override void Dispose()
    {
        _subscriber = null;
        base.Dispose();
    }
}