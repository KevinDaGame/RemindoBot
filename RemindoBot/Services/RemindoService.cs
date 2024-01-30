using Discord;
using Discord.WebSocket;
using RemindoBot.Models;
using RemindoBot.Repositories;

namespace RemindoBot.Services;

public class RemindoService : IRemindoService
{
    private readonly IRemindoRepository _repository;
    private readonly DiscordSocketClient _client;
    private Dictionary<long, Timer> _timers = new Dictionary<long, Timer>();

    public RemindoService(IRemindoRepository repository, DiscordSocketClient client)
    {
        _repository = repository;
        _client = client;
    }

    public async Task CreateReminder(ReminderDTO reminder)
    {
        var reminderId = await _repository.CreateReminder(reminder);
        var timer = new Timer(_ => HandleReminder(reminderId), null, reminder.RemindTime - DateTime.Now, TimeSpan.Zero);
        _timers.Add(reminderId, timer);
    }

    public async Task HandleReminder(long reminderId)
    {
        var reminder = _repository.GetReminder(reminderId);
        switch (reminder)
        {
            case null:
                return;
            case {guildId: not null, channelId: not null}:
                try
                {
                    var guild = _client.GetGuild(reminder.guildId.Value);
                    var channel = guild.GetTextChannel(reminder.channelId.Value);
                    await channel.SendMessageAsync($"<@{reminder.userId}> {reminder.Message}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                break;
            default:
            {
                var user = await _client.GetUserAsync(reminder.userId);
                await user.SendMessageAsync(reminder.Message);
                break;
            }
        }


        await _repository.SetReminderHandled(reminder.Id);
        _timers.Remove(reminderId);
    }
}