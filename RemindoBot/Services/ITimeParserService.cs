namespace RemindoBot.Services;

public interface ITimeParserService
{
    public DateTime ParseTime(string time);
}