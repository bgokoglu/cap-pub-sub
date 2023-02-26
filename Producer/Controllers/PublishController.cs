using Contracts;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;

namespace Producer.Controllers;

[ApiController]
[Route("[controller]")]
public class PublishController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public PublishController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [Route("~/[controller]/send")]
    [HttpGet]
    public IActionResult SendMessage([FromServices] ICapPublisher capBus)
    {
        capBus.Publish("test.show.time", new CapMessage
        {
            Id = Guid.NewGuid(),
            Message = DateTime.Now.ToString()
        });
        return Ok();
    }
    
    [Route("~/[controller]/sendwithtransaction")]
    [HttpGet]
    public async Task<IActionResult> SendMessageWithTransaction([FromServices] ICapPublisher capBus)
    {
        await using (await _dbContext.Database.BeginTransactionAsync(capBus, autoCommit: true))
        {
            var obj = new CapMessage
            {
                Id = Guid.NewGuid(),
                Message = DateTime.Now.ToString()
            };
            
            _dbContext.CapMessages.Add(obj);
            await _dbContext.SaveChangesAsync();
            await capBus.PublishAsync("test.show.time", obj);
        }
        return Ok();
    }
}