namespace LogService.Domain.Entities;

public class Log
{
    public Log()
    {
        Id = Guid.NewGuid();
    }
    public Guid Id { get; set; }

    public string? Level { get; set; }

    public string Message { get; set; } = null!;

    public string ServiceName { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }
}