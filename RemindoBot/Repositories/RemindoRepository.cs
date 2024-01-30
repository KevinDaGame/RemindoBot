using DAL;
using Microsoft.EntityFrameworkCore;
using RemindoBot.Models;

namespace RemindoBot.Repositories;

public class RemindoRepository : IRemindoRepository
{
    private readonly RemindoDbContext _context;

    public RemindoRepository(RemindoDbContext context)
    {
        _context = context;
    }

    public async Task<long> CreateReminder(ReminderDTO reminderDto)
    {
        var reminder = new Reminder
        {
            Message = reminderDto.Message,
            RemindTime = reminderDto.RemindTime,
            userId = reminderDto.userId,
            guildId = reminderDto.guildId,
            channelId = reminderDto.channelId
        };
        
        _context.Reminders.Add(reminder);
        await _context.SaveChangesAsync();
        return reminder.Id;
    }

    public async Task<IEnumerable<Reminder>> GetReminders()
    {
        return await _context.Reminders.ToListAsync();
    }

    public Task SetReminderHandled(long reminderId)
    {
        var reminder = GetReminder(reminderId);
        if (reminder == null)
        {
            return Task.CompletedTask;
        }
        
        _context.Reminders.Remove(reminder);
        return _context.SaveChangesAsync();
    }

    public Reminder? GetReminder(long reminderId)
    {
        return _context.Reminders.Find(reminderId);
    }
}