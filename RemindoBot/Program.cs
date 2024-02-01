using DAL;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RemindoBot.Commands;
using RemindoBot.Repositories;
using RemindoBot.Services;

namespace RemindoBot;

public class Program
{
    private DiscordSocketClient _client;
    private IServiceProvider _services;
    private ILogger _logger;

    public static Task Main(string[] args)
    {
        return new Program().MainAsync();
    }

    private async Task MainAsync()
    {
        _services = CreateServices();

        var context = _services.GetRequiredService<RemindoDbContext>();
        await context.Database.MigrateAsync();

        _logger = _services.GetRequiredService<ILogger<Program>>();

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

    public Task Client_Ready()
    {
        var commandManager = _services.GetRequiredService<CommandManager>();
        commandManager.RegisterCommands();
        return Task.CompletedTask;
    }

    private async Task SlashCommandHandler(SocketSlashCommand command)
    {
        var commandManager = _services.GetRequiredService<CommandManager>();
        await commandManager.SlashCommandHandler(command);
    }

    private static IServiceProvider CreateServices()
    {
        var config = new DiscordSocketConfig();
        var connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING") ??
                               throw new Exception("MYSQL_CONNECTION_STRING not found");
        var services = new ServiceCollection()
            .AddSingleton(config)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<CommandManager>()
            .AddSingleton<ITimeParserService, TimeParserService>()
            .AddTransient<IRemindoRepository, RemindoRepository>()
            .AddTransient<IRemindoService, RemindoService>()
            .AddLogging(builder =>
            {
                builder.AddConsole();
            })
            .AddDbContext<RemindoDbContext>(optionsBuilder =>
                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)))
            .BuildServiceProvider();
        return services;
    }

    private Task Log(LogMessage msg)
    {
        _logger.LogInformation(msg.ToString());
        return Task.CompletedTask;
    }
}