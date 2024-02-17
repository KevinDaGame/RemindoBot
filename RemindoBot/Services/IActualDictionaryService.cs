namespace RemindoBot.Services;

public interface IActualDictionaryService
{
    public Task<List<ActualDictionaryDefinition>> GetDefinitionsOfWord(string word);
    public Task<ActualDictionaryDefinition?> GetDefinitionOfWord(string world, out int definitionCount);
}

public class ActualDictionaryDefinition
{
    public string Word { get; set; }
    public string Definition { get; set; }
    public string Example { get; set; }
}