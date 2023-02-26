using WorkerService1;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        //services.AddHostedService<Worker>();
        //services.AddHostedService<RabbitMQWorker>();
        services.AddHostedService<RedisWorker>();
    })
    .Build();

host.Run();