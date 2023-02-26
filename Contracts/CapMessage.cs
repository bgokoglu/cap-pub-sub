namespace Contracts;

public class CapMessage
{
    public static string Topic => "test.show.time";
    
    public Guid Id { get; set; }
    public string Message { get; set; }
}