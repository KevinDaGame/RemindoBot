namespace DAL;

public class Reminder
{
    public long Id { get; set; }
    public required DateTime RemindTime { get; set; }
    public string Message { get; set; } = string.Empty;
    public ulong userId { get; set; }
    public ulong? guildId { get; set; }
    public ulong? channelId { get; set; }
}