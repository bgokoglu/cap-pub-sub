using Contracts;
using DotNetCore.CAP;

namespace Producer;

public interface IConsumer
{
    void ProcessMessage(CapMessage message);
}

public class Consumer : IConsumer, ICapSubscribe
{
    private readonly ILogger<Consumer> _logger;

    public Consumer(ILogger<Consumer> logger)
    {
        _logger = logger;
    }

    [CapSubscribe("test.show.time")]
    public void ProcessMessage(CapMessage message)
    {
       _logger.LogInformation("Received: {Id} - {Message}", message.Id, message.Message);
    }
}