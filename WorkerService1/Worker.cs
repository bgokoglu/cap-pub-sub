using System.Text;
using System.Text.Json;
using Contracts;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;

namespace WorkerService1;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("GenericWorker Waiting for messages at: {time}", DateTimeOffset.Now);

            //ReceiveFromRabbitMQ();
            //await ReceiveFromRedis();
            
            await Task.Delay(5000, stoppingToken);
        }
    }

    private void ReceiveFromRabbitMQ()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        
        channel.QueueBind(queue: "cap.queue.producer.v1",
            exchange: "cap.default.router",
            routingKey: "test.show.time");

        // channel.QueueDeclare(queue: "cap.queue.producer.v1",
        //     durable: false,
        //     exclusive: false,
        //     autoDelete: false,
        //     arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var capMessage = JsonSerializer.Deserialize<CapMessage>(message);
            _logger.LogInformation("Received: {Id} - {Message}", capMessage.Id, capMessage.Message);
        };
            
        channel.BasicConsume(queue: "cap.queue.producer.v1",
            autoAck: true,
            consumer: consumer);
    }

    private async Task ReceiveFromRedis()
    {
        var connString = "localhost:6379";
        var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(connString);
        var channel = "cap.queue.producer.v1";

        var subscriber = connectionMultiplexer.GetSubscriber();

        await subscriber.SubscribeAsync(channel, (c, m) =>
        {
            var capMessage = JsonSerializer.Deserialize<CapMessage>(m);
            _logger.LogInformation("Received: {Id} - {Message}", capMessage.Id, capMessage.Message);
        });
    }
}