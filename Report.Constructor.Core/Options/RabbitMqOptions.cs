namespace Report.Constructor.Core.Options;

public class RabbitMqOptions
{
    public string Host { get; set; } = default!;
    public string User { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string CommandsQueueName { get; set; } = default!;
    public string QueryQueueName { get; set; } = default!;
}