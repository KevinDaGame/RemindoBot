using Flurl;
using Flurl.Http;

namespace RemindoBot.Services;

public class ActualDictionaryService : IActualDictionaryService
{
    string baseURI = "https://api.dictionaryapi.dev/api/v2/entries/en/";

    public async Task<List<ActualDictionaryDefinition>> GetDefinitionsOfWord(string word)
    {
        var result = await baseURI.AppendPathSegment(word).GetJsonAsync<ActualDictionaryResponse[]>();
        return result.First().Meanings.Select(m => new ActualDictionaryDefinition
        {
            Word = result.First().Word,
            Definition = m.Definitions.First().Definition
        }).ToList();
    }

    public Task<ActualDictionaryDefinition?> GetDefinitionOfWord(string world, out int definitionCount)
    {
        List<ActualDictionaryDefinition> result = GetDefinitionsOfWord(world).Result;
        definitionCount = result.Count;
        return Task.FromResult(result.Count == 0 ? null : result.FirstOrDefault());
    }
}

public class ActualDictionaryResponse
{
    public string Word { get; set; }
    public List<ActualDictionaryMeaning> Meanings { get; set; }
}

public class ActualDictionaryMeaning
{
    public List<ActualDictionaryDefinitionR> Definitions { get; set; }
}

public class ActualDictionaryDefinitionR
{
    public string Definition { get; set; }
}