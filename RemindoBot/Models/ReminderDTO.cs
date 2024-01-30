namespace RemindoBot.Models;

public class ReminderDTO
{
    public DateTime RemindTime { get; set; }
    public string Message { get; set; }
    public ulong userId { get; set; }
    public ulong? guildId { get; set; }
    public ulong? channelId { get; set; }
}