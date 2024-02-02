namespace RemindoBot.Services;

public interface IUrbanDictionaryService
{
    public Task<List<UrbanDictionaryDefinition>> GetDefinitionsOfWord(string word);
    public Task<UrbanDictionaryDefinition?> GetDefinitionOfWord(string world);
}