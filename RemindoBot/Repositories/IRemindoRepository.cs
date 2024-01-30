using DAL;
using RemindoBot.Models;

namespace RemindoBot.Repositories;

public interface IRemindoRepository
{
    Task<long> CreateReminder(ReminderDTO reminder);

    Task<IEnumerable<Reminder>> GetReminders();
    Task SetReminderHandled(long reminderId);
    Reminder? GetReminder(long reminderId);
}