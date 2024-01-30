using RemindoBot.Models;

namespace RemindoBot.Repositories;

public class RemindoRepository : IRemindoRepository
{
    static List<Reminder> _reminders = new List<Reminder>();

    public async Task<long> CreateReminder(Reminder reminder)
    {
        _reminders.Add(reminder);
        return Convert.ToInt64(_reminders.Count - 1);
    }

    public Task<IEnumerable<Reminder>> GetReminders()
    {
        return Task.FromResult(_reminders.AsEnumerable());
    }

    public Task SetReminderHandled(Reminder reminder)
    {
        _reminders.Remove(reminder);
        return Task.CompletedTask;
    }

    public Reminder? GetReminder(long reminderId)
    {
        return _reminders[Convert.ToInt32(reminderId)];
    }
}