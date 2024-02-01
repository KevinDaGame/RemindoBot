using System.Reflection;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace RemindoBot.Handlers;

public class HandlerManager
{
    private readonly DiscordSocketClient _client;
    private readonly IServiceProvider _services;
    private readonly ILogger<HandlerManager> _logger;
    private List<IHandler> _handlers = new();

    public HandlerManager(DiscordSocketClient client, IServiceProvider services, ILogger<HandlerManager> logger)
    {
        _client = client;
        _services = services;
        _logger = logger;
    }

    public void RegisterHandlers()
    {
        var handlerTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.GetInterfaces().Contains(typeof(IHandler)));

        foreach (var handlerType in handlerTypes)
        {
            var handler = (IHandler) ActivatorUtilities.CreateInstance(_services, handlerType);
            handler.RegisterHandlers();
            _handlers.Add(handler);
            _logger.LogInformation($"Registered handler {handlerType.Name}");
        }
    }
}