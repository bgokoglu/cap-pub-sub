using System.Text;
using System.Text.Json;
using Contracts;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WorkerService1;

public class RabbitMQWorker : BackgroundService
{
    private readonly ILogger<RabbitMQWorker> _logger;
    private IConnection _connection;
    private IModel _channel;
    private EventingBasicConsumer _consumer;

    public RabbitMQWorker(ILogger<RabbitMQWorker> logger)
    {
        _logger = logger;
        InitRabbitMQ();
    }

    private void InitRabbitMQ()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };

        // create connection  
        _connection = factory.CreateConnection();

        // create channel  
        _channel = _connection.CreateModel();

        // _channel.ExchangeDeclare("cap.default.router", ExchangeType.Topic);  
        // _channel.QueueDeclare("cap.queue.producer.v1", false, false, false, null);  
        // _channel.QueueBind("cap.queue.producer.v1", "cap.default.router", "test.show.time", null);  
        // _channel.BasicQos(0, 1, false);  

        _channel.QueueBind(queue: "cap.queue.producer.v1",
            exchange: "cap.default.router",
            routingKey: "test.show.time");
        
        _consumer = new EventingBasicConsumer(_channel);

        _consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var capMessage = JsonSerializer.Deserialize<CapMessage>(message);
            _logger.LogInformation("Received: {Id} - {Message}", capMessage.Id, capMessage.Message);
            //_channel.BasicAck(ea.DeliveryTag, false);
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            //stoppingToken.ThrowIfCancellationRequested();  
            _logger.LogInformation("RabbitMQWorker Waiting for messages at: {time}", DateTimeOffset.Now);

            _channel.BasicConsume(queue: "cap.queue.producer.v1",
                autoAck: true,
                consumer: _consumer);

            await Task.Delay(5000, stoppingToken);
        }
    }

    public override void Dispose()
    {
        _consumer = null;
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}