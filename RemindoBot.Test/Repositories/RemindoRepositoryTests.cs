using DAL;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RemindoBot.Models;
using RemindoBot.Repositories;

namespace RemindoBot.Test.Repositories;

[TestClass]
public class RemindoRepositoryTests
{
    private SqliteConnection _connection = null!;
    private DbContextOptions<RemindoDbContext> _contextOptions = null!;

    [TestInitialize]
    public void BeforeEach()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
        _contextOptions = new DbContextOptionsBuilder<RemindoDbContext>()
            .UseSqlite(_connection)
            .Options;
        using var context = new RemindoDbContext(_contextOptions);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }

    [TestMethod]
    public async Task CreateReminder_Creates_Reminder_Returns_Id()
    {
        // Arrange
        var reminderDto = new ReminderDTO
        {
            Message = "Test",
            RemindTime = DateTime.Now,
            userId = 12345,
            guildId = 12356,
            channelId = 1234567
        };
        await using (var context = new RemindoDbContext(_contextOptions))
        {
            var repository = new RemindoRepository(context);

            // Act
            var result = repository.CreateReminder(reminderDto).Result;

            // Assert
            Assert.AreEqual(1L, result);
        }

        await using (var context = new RemindoDbContext(_contextOptions))
        {
            const ulong expectedUserId = 12345;
            const ulong expectedGuildId = 12356;
            const ulong expectedChannelId = 1234567;
            var reminders = await context.Reminders.ToListAsync();
            Assert.AreEqual(1, reminders.Count);
            Assert.AreEqual(1L, reminders[0].Id);
            Assert.AreEqual("Test", reminders[0].Message);
            Assert.AreEqual(expectedUserId, reminders[0].userId);
            Assert.AreEqual(expectedGuildId, reminders[0].guildId);
            Assert.AreEqual(expectedChannelId, reminders[0].channelId);
        }
    }

    [TestMethod]
    public async Task GetReminders_Returns_Reminders()
    {
        // Arrange
        await using (var context = new RemindoDbContext(_contextOptions))
        {
            context.Reminders.Add(new Reminder
            {
                Message = "Test",
                RemindTime = DateTime.Now,
                userId = 12345,
                guildId = 12356,
                channelId = 1234567
            });
            context.Reminders.Add(new Reminder
            {
                Message = "Test2",
                RemindTime = DateTime.Now,
                userId = 12345,
                guildId = 12356,
                channelId = 1234567
            });
            await context.SaveChangesAsync();
        }

        // Act
        await using (var context = new RemindoDbContext(_contextOptions))
        {
            var repository = new RemindoRepository(context);
            var result = repository.GetReminders().Result;

            // Assert
            Assert.AreEqual(2, result.Count());
        }
    }
    
    [TestMethod]
    public async Task SetReminderHandled_Removes_Reminder()
    {
        // Arrange
        await using (var context = new RemindoDbContext(_contextOptions))
        {
            context.Reminders.Add(new Reminder
            {
                Message = "Test",
                RemindTime = DateTime.Now,
                userId = 12345,
                guildId = 12356,
                channelId = 1234567
            });
            await context.SaveChangesAsync();
        }

        // Act
        await using (var context = new RemindoDbContext(_contextOptions))
        {
            var repository = new RemindoRepository(context);
            await repository.SetReminderHandled(1L);
        }

        // Assert
        await using (var context = new RemindoDbContext(_contextOptions))
        {
            var reminders = await context.Reminders.ToListAsync();
            Assert.AreEqual(0, reminders.Count);
        }
    }
    
    [TestMethod]
    public async Task GetReminder_Returns_Reminder()
    {
        // Arrange
        await using (var context = new RemindoDbContext(_contextOptions))
        {
            context.Reminders.Add(new Reminder
            {
                Message = "Test",
                RemindTime = DateTime.Now,
                userId = 12345,
                guildId = 12356,
                channelId = 1234567
            });
            await context.SaveChangesAsync();
        }

        // Act
        await using (var context = new RemindoDbContext(_contextOptions))
        {
            var repository = new RemindoRepository(context);
            var result = repository.GetReminder(1L);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1L, result!.Id);
        }
    }
}