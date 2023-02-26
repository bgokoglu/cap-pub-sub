using Contracts;
using DotNetCore.CAP;
using Microsoft.Extensions.Logging;

namespace Subscriber1;

public interface IConsumer1
{
    void ProcessMessage(CapMessage message);
}

public class Consumer1 : IConsumer1, ICapSubscribe
{
    private readonly ILogger<Consumer1> _logger;

    public Consumer1(ILogger<Consumer1> logger)
    {
        _logger = logger;
    }

    [CapSubscribe("test.show.time")]
    public void ProcessMessage(CapMessage message)
    {
        _logger.LogInformation("Received: {Id} - {Message}", message.Id, message.Message);
    }
}