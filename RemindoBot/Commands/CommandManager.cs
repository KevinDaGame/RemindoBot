using System.Reflection;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace RemindoBot.Commands;

public class CommandManager
{
    private readonly DiscordSocketClient _client;
    private readonly IServiceProvider _services;
    private readonly ILogger<CommandManager> _logger;
    private Dictionary<string, Type> _commands = new();

    public CommandManager(DiscordSocketClient client, IServiceProvider services, ILogger<CommandManager> logger)
    {
        _client = client;
        _services = services;
        _logger = logger;
    }

    public void RegisterCommands()
    {
        _commands.Clear();
        IEnumerable<Type> commandTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.GetInterfaces().Contains(typeof(ICommand)));

        foreach (Type commandType in commandTypes)
        {
            var command = (ICommand) ActivatorUtilities.CreateInstance(_services, commandType);

            try
            {
                _client.CreateGlobalApplicationCommandAsync(command.Command.Build());
                _commands.Add(command.Command.Name, command.GetType());
                _logger.LogInformation($"Registered command {command.Command.Name}");
            }
            catch (ApplicationCommandException exception)
            {
                string json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                _logger.LogError($"Unable to register command {command.Command.Name}: {json}");
            }
        }
    }

    public async Task SlashCommandHandler(SocketSlashCommand command)
    {
        if (_commands.TryGetValue(command.Data.Name, out var handler))
        {
            var commandHandler = (ICommand) ActivatorUtilities.CreateInstance(_services, handler);
            await commandHandler.Handle(command);
        }
    }
}