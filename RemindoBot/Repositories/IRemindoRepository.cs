using RemindoBot.Models;

namespace RemindoBot.Repositories;

public interface IRemindoRepository
{
    Task<long> CreateReminder(Reminder reminder);

    Task<IEnumerable<Reminder>> GetReminders();
    Task SetReminderHandled(Reminder reminder);
    Reminder? GetReminder(long reminderId);
}