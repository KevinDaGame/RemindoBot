using Discord;
using Discord.WebSocket;
using Flurl.Http;
using RemindoBot.Services;
using SlashCommandBuilder = Discord.SlashCommandBuilder;

namespace RemindoBot.Commands;

public class UrbanDictionaryCommand : ICommand
{
    private readonly IUrbanDictionaryService _urbanDictionaryService;

    public UrbanDictionaryCommand(IUrbanDictionaryService urbanDictionaryService)
    {
        _urbanDictionaryService = urbanDictionaryService;
    }

    public string Name => "urban";

    public SlashCommandBuilder Command => new SlashCommandBuilder()
        .WithName(Name)
        .WithDescription("Search for a word on Urban Dictionary")
        .AddOption(new SlashCommandOptionBuilder()
            .WithName("word")
            .WithDescription("The word to search for")
            .WithRequired(true)
            .WithType(ApplicationCommandOptionType.String));

    public async Task Handle(SocketSlashCommand command)
    {
        string word = command.Data.Options.First().Value.ToString() ??
                      throw new ArgumentNullException("word", "Word cannot be null");
        UrbanDictionaryDefinition? definition;
        int definitionCount;
        try
        {
            definition = await _urbanDictionaryService.GetDefinitionOfWord(word, out definitionCount);
        }
        catch (FlurlHttpException)
        {
            await command.RespondAsync("Failed to fetch definition");
            return;
        }

        if (definition == null)
        {
            await command.RespondAsync("No definition found");
            return;
        }

        EmbedBuilder? embed = new EmbedBuilder()
            .WithTitle(definition.Word)
            .WithDescription(definition.Definition)
            .WithUrl(definition.Permalink)
            .WithFooter(
                $"{definition.Author} - [1/{definitionCount}] - {definition.Thumbs_up} 👍 {definition.Thumbs_down} 👎")
            .WithColor(Color.Blue)
            .WithTimestamp(DateTime.Parse(definition.Written_on));

        MessageComponent buttons = new ComponentBuilder()
            .WithButton("View on Urban Dictionary", style: ButtonStyle.Link, url: definition.Permalink)
            .WithButton("Next definition", customId: $"urban-{word}-1")
            .WithButton("Actual definition", style: ButtonStyle.Success, customId: $"actual-{word}-0")
            .Build();

        await command.RespondAsync(embed: embed.Build(), components: buttons);
    }
}