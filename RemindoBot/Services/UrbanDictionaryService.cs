using Flurl;
using Flurl.Http;

namespace RemindoBot.Services;

public class UrbanDictionaryService : IUrbanDictionaryService
{
    string baseURI = "https://api.urbandictionary.com/v0/define";

    public async Task<List<UrbanDictionaryDefinition>> GetDefinitionsOfWord(string word)
    {
        var result = await baseURI.AppendQueryParam("term", word).GetJsonAsync<UrbanDictionaryResponse>();
        return result.List;
    }

    public async Task<UrbanDictionaryDefinition?> GetDefinitionOfWord(string world)
    {
        List<UrbanDictionaryDefinition> result = await GetDefinitionsOfWord(world);
        
        return result.Count == 0 ? null : result.FirstOrDefault();
    }
}