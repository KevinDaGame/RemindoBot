namespace RemindoBot.Services;

public class UrbanDictionaryResponse
{
    public List<UrbanDictionaryDefinition> List { get; set; }
}

public class UrbanDictionaryDefinition
{
    public string Definition { get; set; }
    public string Example { get; set; }
    public string Permalink { get; set; }
    public string Word { get; set; }
    public int ThumbsUp { get; set; }
    public int ThumbsDown { get; set; }
    public string Author { get; set; }
    public string Written_on { get; set; }
}