using Pathoschild.NaturalTimeParser.Parser;

namespace RemindoBot.Services;

class TimeParserService : ITimeParserService
{
    public DateTime ParseTime(string time)
    {
        //if it can be parsed as a datetime, return it
        if (DateTime.TryParse(time, out var datetime))
        {
            return datetime;
        }

        // if it can be parsed as a unix timestamp, return it
        try
        {
            datetime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(time)).DateTime;
            
            return datetime;
        }
        catch (Exception)
        {
            // ignored
        }

        // if it can be parsed as a natural time, return it
        try
        {
            datetime = DateTime.Now.Offset(time);
            return datetime;
        }
        catch (Exception)
        {
            // ignored
        }
        
        throw new Exception("Could not parse time");
        
    }
}