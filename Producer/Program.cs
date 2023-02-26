using System.Text.Encodings.Web;
using System.Text.Unicode;
using DotNetCore.CAP.Messages;
using Microsoft.EntityFrameworkCore;
using Producer;
using Savorboard.CAP.InMemoryMessageQueue;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IConsumer, Consumer>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.EnableSensitiveDataLogging();
    //options.UseSqlServer(builder.Configuration.GetConnectionString("SalesDb"));
    options.UseSqlServer("Server=localhost;Database=CapAppDB;User Id=sa;Password=Passw@rd;TrustServerCertificate=true");
});

builder.Services.AddCap(capOptions =>
{
    //capOptions.UseInMemoryStorage();
    //capOptions.UseInMemoryMessageQueue();
    capOptions.UseRedis("localhost:6379");
    capOptions.UseEntityFramework<AppDbContext>();
    //capOptions.UseRabbitMQ("localhost");
    // capOptions.UseRabbitMQ(y =>
    // {
    //     y.UserName = "guest";
    //     y.Password = "guest";
    //     y.HostName = "localhost";
    //     ////If BasicQosOptions are created then the basic channel will use the qos settings, otherwise will ignore BasicQos 
    //     ////In the case below will enforce a prefetchCount of max 3 messages unacknowledged to be consumed
    //     //y.BasicQosOptions = new DotNetCore.CAP.RabbitMQOptions.BasicQos(3);
    // });
    // capOptions.FailedRetryCount = 5;
    // capOptions.UseDispatchingPerGroup = true;
    // capOptions.FailedThresholdCallback = failed =>
    // {
    //     var logger = failed.ServiceProvider.GetRequiredService<ILogger<Program>>();
    //     logger.LogError(
    //         $@"A message of type {failed.MessageType} failed after executing {capOptions.FailedRetryCount} several times, 
    //                     requiring manual troubleshooting. Message name: {failed.Message.GetName()}");
    // };
    // capOptions.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
    capOptions.UseDashboard(options => { });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();