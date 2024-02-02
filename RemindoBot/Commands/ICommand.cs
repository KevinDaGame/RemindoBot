using Discord.Interactions.Builders;
using Discord.WebSocket;
using SlashCommandBuilder = Discord.SlashCommandBuilder;

namespace RemindoBot.Commands;

public interface ICommand
{
    string Name { get; }
    SlashCommandBuilder Command { get; }
    Task Handle(SocketSlashCommand command);
}