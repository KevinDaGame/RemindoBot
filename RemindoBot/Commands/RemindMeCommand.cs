using Discord;
using Discord.WebSocket;
using RemindoBot.Models;
using RemindoBot.Services;

namespace RemindoBot.Commands;

public class RemindMeCommand : ICommand
{
    private readonly ITimeParserService _timeParserService;
    private readonly IRemindoService _remindoService;

    public RemindMeCommand(ITimeParserService timeParserService, IRemindoService remindoService)
    {
        _timeParserService = timeParserService;
        _remindoService = remindoService;
    }
    public string Name => "remindme";

    public SlashCommandBuilder Command => new SlashCommandBuilder()
        .WithName(Name)
        .WithDescription("Create a reminder!")
        .AddOption("string", ApplicationCommandOptionType.String, "The reminder message", true)
        .AddOption("time", ApplicationCommandOptionType.String, "The time to remind you", true);

    public async Task Handle(SocketSlashCommand command)
    {
        var message = command.Data.Options.First().Value.ToString()!;
        var time = command.Data.Options.Last().Value.ToString()!;
        DateTime dateTime;
        try
        {
            dateTime = _timeParserService.ParseTime(time);
        }
        catch (Exception)
        {
            await command.RespondAsync($"Unable to parse time: {time}");
            return;
        }

        if (dateTime < DateTime.Now)
        {
            await command.RespondAsync($"{time} is invalid. It is in the past.");
            return;
        }
        

        await _remindoService.CreateReminder(new ReminderDTO
        {
            RemindTime = dateTime,
            Message = message,
            userId = command.User.Id,
            guildId = command.GuildId,
            channelId = command.ChannelId
        });

        await command.RespondAsync(
            $"You executed {command.Data.Name} successfully! You will be reminded at {dateTime} with the message: {message}");
    }
}