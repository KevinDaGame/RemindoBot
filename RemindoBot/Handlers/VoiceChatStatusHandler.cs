using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace RemindoBot.Handlers;

public class VoiceChatStatusHandler : IHandler
{
    private readonly DiscordSocketClient _client;
    private readonly ILogger<VoiceChatStatusHandler> _logger;

    public VoiceChatStatusHandler(DiscordSocketClient client, ILogger<VoiceChatStatusHandler> logger)
    {
        _client = client;
        _logger = logger;
    }

    public void RegisterHandlers()
    {
        _client.UserVoiceStateUpdated += HandleVoiceChannelEvent;
    }

    public Task HandleVoiceChannelEvent(SocketUser user, SocketVoiceState oldChannel, SocketVoiceState newChannel)
    {
        var guildId = ulong.Parse(Environment.GetEnvironmentVariable("DISCORD_GUILD_ID") ??
                                  throw new InvalidOperationException());
        var logChannelId = ulong.Parse(Environment.GetEnvironmentVariable("DISCORD_LOG_CHANNEL_ID") ??
                                       throw new InvalidOperationException());
        Embed? message = null;
        switch (oldChannel, newChannel)
        {
            case ({VoiceChannel: null}, {VoiceChannel: not null}):
                message = new EmbedBuilder()
                    .WithColor(Color.Green)
                    .WithAuthor(user)
                    .WithDescription($"{user.Mention} has joined {newChannel.VoiceChannel.Name}")
                    .Build();
                break;
            case ({VoiceChannel: not null}, {VoiceChannel: null}):
                message = new EmbedBuilder()
                    .WithColor(Color.Red)
                    .WithAuthor(user)
                    .WithDescription($"{user.Mention} has left {oldChannel.VoiceChannel.Name}")
                    .Build();
                break;
            case ({VoiceChannel: not null}, {VoiceChannel: not null}):
                if (oldChannel.VoiceChannel != newChannel.VoiceChannel)
                {
                    message = new EmbedBuilder()
                        .WithColor(Color.Blue)
                        .WithAuthor(user)
                        .WithDescription(
                            $"{user.Mention} has moved from {oldChannel.VoiceChannel.Name} to {newChannel.VoiceChannel.Name}")
                        .Build();
                }

                break;
        }

        if (message is not null)
        {
            _client.GetGuild(guildId)
                .GetTextChannel(logChannelId)
                .SendMessageAsync(embed: message);
        }

        return Task.CompletedTask;
    }
}