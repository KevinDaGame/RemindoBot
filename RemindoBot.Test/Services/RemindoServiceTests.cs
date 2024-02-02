using DAL;
using Discord;
using Discord.WebSocket;
using Moq;
using RemindoBot.Models;
using RemindoBot.Repositories;
using RemindoBot.Services;

namespace RemindoBot.Test.Services;

[TestClass]
public class RemindoServiceTests
{
    [TestMethod]
    public async Task CreateReminder_Calls_CreateReminder_On_Repository()
    {
        // Arrange
        var repositoryMock = new Mock<IRemindoRepository>(MockBehavior.Strict);
        var clientMock = new Mock<DiscordSocketClient>();
        
        repositoryMock.Setup(x => x.CreateReminder(It.IsAny<ReminderDTO>())).ReturnsAsync(1L);
        
        var service = new RemindoService(repositoryMock.Object, clientMock.Object);
        var reminder = new ReminderDTO
        {
            Message = "Test",
            RemindTime = DateTime.Now.AddSeconds(10), // 10 seconds from now
            userId = 1
        };

        // Act
        await service.CreateReminder(reminder);

        // Assert
        repositoryMock.Verify(x => x.CreateReminder(reminder), Times.Once);
    }
       
}