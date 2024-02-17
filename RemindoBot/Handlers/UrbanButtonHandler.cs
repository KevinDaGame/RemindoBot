using Discord;
using Discord.WebSocket;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using RemindoBot.Services;

namespace RemindoBot.Handlers;

public class UrbanButtonHandler : IHandler
{
    private readonly DiscordSocketClient _client;
    private readonly ILogger<VoiceChatStatusHandler> _logger;
    private readonly IUrbanDictionaryService _urbanDictionaryService;
    private readonly IActualDictionaryService _actualDictionaryService;

    public UrbanButtonHandler(DiscordSocketClient client, ILogger<VoiceChatStatusHandler> logger,
        IUrbanDictionaryService urbanDictionaryService,
        IActualDictionaryService actualDictionaryService)
    {
        _client = client;
        _logger = logger;
        _urbanDictionaryService = urbanDictionaryService;
        _actualDictionaryService = actualDictionaryService;
    }

    public void RegisterHandlers()
    {
        _client.ButtonExecuted += HandleUrbanButton;
    }

    public async Task HandleUrbanButton(SocketMessageComponent component)
    {
        if (component.Data.CustomId.StartsWith("urban-"))
        {
            string word = component.Data.CustomId.Split("-")[1];
            int number = int.Parse(component.Data.CustomId.Split("-")[^1]);

            if (number < 0)
            {
                await component.RespondAsync("Invalid definition number");
                return;
            }

            UrbanDictionaryDefinition definition;
            int definitionCount;
            try
            {
                var definitions = await _urbanDictionaryService.GetDefinitionsOfWord(word);
                if (number >= definitions.Count)
                {
                    await component.RespondAsync("no more definitions");
                    return;
                }

                definitionCount = definitions.Count;
                definition = definitions[number];
            }
            catch (FlurlHttpException)
            {
                await component.RespondAsync("Failed to fetch definition");
                return;
            }

            EmbedBuilder? embed = new EmbedBuilder()
                .WithTitle(definition.Word)
                .WithDescription(definition.Definition)
                .WithUrl(definition.Permalink)
                .WithFooter(
                    $"{definition.Author} - [{number + 1}/{definitionCount}] - {definition.Thumbs_up} 👍 {definition.Thumbs_down} 👎")
                .WithColor(Color.Blue)
                .WithTimestamp(DateTime.Parse(definition.Written_on));

            ComponentBuilder buttonsBuilder = new ComponentBuilder()
                .WithButton("View on Urban Dictionary", style: ButtonStyle.Link, url: definition.Permalink);

            if (number > 0)
            {
                buttonsBuilder.WithButton("Previous definition", customId: $"urban-{word}-{number - 1}");
            }

            if (number < definitionCount - 1)
            {
                buttonsBuilder.WithButton("Next definition", customId: $"urban-{word}-{number + 1}");
            }

            buttonsBuilder.WithButton("Actual definition", style: ButtonStyle.Success, customId: $"actual-{word}-0");

            await component.Message.ModifyAsync(properties =>
            {
                properties.Embed = embed.Build();
                properties.Components = buttonsBuilder.Build();
            });
            await component.DeferAsync();
        }

        if (component.Data.CustomId.StartsWith("actual-"))
        {
            string word = component.Data.CustomId.Split("-")[1];
            int number = int.Parse(component.Data.CustomId.Split("-")[^1]);
            if (number < 0)
            {
                await component.RespondAsync("Invalid definition number");
                return;
            }

            ActualDictionaryDefinition definition;
            int definitionCount;
            try
            {
                var definitions = await _actualDictionaryService.GetDefinitionsOfWord(word);
                if (number >= definitions.Count)
                {
                    await component.RespondAsync("no more definitions");
                    return;
                }

                definitionCount = definitions.Count;
                definition = definitions[number];
            }
            catch (FlurlHttpException e)
            {
                await component.RespondAsync("No definition found");
                _logger.LogError(e, "Failed to fetch definition");
                return;
            }

            EmbedBuilder? embed = new EmbedBuilder()
                .WithTitle(definition.Word)
                .WithDescription(definition.Definition)
                .WithFooter($"[{number + 1}/{definitionCount}]")
                .WithColor(Color.Blue);

            ComponentBuilder buttonsBuilder = new ComponentBuilder();
            if (number > 0)
            {
                buttonsBuilder.WithButton("Previous definition", customId: $"actual-{word}-{number - 1}");
            }

            if (number < definitionCount - 1)
            {
                buttonsBuilder.WithButton("Next definition", customId: $"actual-{word}-{number + 1}");
            }

            await component.Message.ModifyAsync(properties =>
            {
                properties.Embed = embed.Build();
                properties.Components = buttonsBuilder.Build();
            });
            await component.DeferAsync();
        }
    }
}