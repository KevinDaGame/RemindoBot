using Discord;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Pathoschild.NaturalTimeParser.Parser;
using RemindoBot.Models;
using RemindoBot.Repositories;
using RemindoBot.Services;

namespace RemindoBot;

public class Program
{
    private DiscordSocketClient _client;
    private IServiceProvider _services;

    public static Task Main(string[] args) => new Program().MainAsync();

    public async Task MainAsync()
    {
        _services = CreateServices();
        _client = _services.GetRequiredService<DiscordSocketClient>();
        _client.Log += Log;
        _client.Ready += Client_Ready;
        _client.SlashCommandExecuted += SlashCommandHandler;

        //read env file
        var token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }

    public async Task Client_Ready()
    {
        var guild = _client.GetGuild(Convert.ToUInt64(Environment.GetEnvironmentVariable("DISCORD_GUILD_ID")));

        var guildCommand = new SlashCommandBuilder()
            .WithName("remindme")
            .WithDescription("Create a reminder!")
            .AddOption("string", ApplicationCommandOptionType.String, "The reminder message", true)
            .AddOption("time", ApplicationCommandOptionType.String, "The time to remind you", true);

        try
        {
            await guild.CreateApplicationCommandAsync(guildCommand.Build());
        }
        catch (ApplicationCommandException exception)
        {
            var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
            Console.WriteLine(json);
        }
    }

    private async Task SlashCommandHandler(SocketSlashCommand command)
    {
        var message = command.Data.Options.First().Value.ToString();
        var time = command.Data.Options.Last().Value.ToString();
        DateTime dateTime;
        try
        {
            dateTime = DateTime.Now.Offset(time);
        }
        catch (Exception e)
        {
            await command.RespondAsync($"Unable to parse time: {time}");
            return;
        }

        var service = _services.GetRequiredService<IRemindoService>();
        
        await service.CreateReminder(new Reminder()
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

    static IServiceProvider CreateServices()
    {
        var config = new DiscordSocketConfig()
        {
            //...
        };
        var services = new ServiceCollection()
            .AddSingleton(config)
            .AddSingleton<DiscordSocketClient>()
            .AddTransient<IRemindoRepository, RemindoRepository>()
            .AddTransient<IRemindoService, RemindoService>()
            .BuildServiceProvider();
        return services;
    }

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}