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

    public Task<UrbanDictionaryDefinition?> GetDefinitionOfWord(string world, out int definitionCount)
    {
        List<UrbanDictionaryDefinition> result = GetDefinitionsOfWord(world).Result;
        definitionCount = result.Count;
        return Task.FromResult(result.Count == 0 ? null : result.FirstOrDefault());
    }
}

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
    public int Thumbs_up { get; set; }
    public int Thumbs_down { get; set; }
    public string Author { get; set; }
    public string Written_on { get; set; }
}