using RemindoBot.Models;

namespace RemindoBot.Services;

public interface IRemindoService
{
    Task CreateReminder(Reminder reminder);
}