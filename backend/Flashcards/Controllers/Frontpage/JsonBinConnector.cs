using System.Text.Json.Nodes;
using Flashcards.Configuration;
using Flashcards.Models;
using Flurl.Http;
using Flurl.Http.Configuration;

namespace Flashcards.Controllers.Frontpage;

public class JsonBinConnector
{
    private readonly JsonBinConfig _jsonBinConfig;
    private readonly IFlurlClient _flurlClient;
    private ILogger<JsonBinConnector> _logger;
    private IWebHostEnvironment _environment;

    public JsonBinConnector(
        IFlurlClientFactory flurlClientFactory,
        JsonBinConfig jsonBinConfig,
        ILogger<JsonBinConnector> logger,
        IWebHostEnvironment environment)
    {
        _jsonBinConfig = jsonBinConfig;
        _logger = logger;
        _environment = environment;
        _flurlClient = flurlClientFactory.Get(jsonBinConfig.BaseUrl);
    }

    public async Task<IEnumerable<JsonBinsResponse>> GetFlashcardsBins()
    {
        var urlPart = $"/c/{_jsonBinConfig.FlashcardsCollectionId}/bins";
        var result = await _flurlClient
            .Request(urlPart)
            .WithHeader("X-Master-Key", _jsonBinConfig.ApiKey)
            .GetAsync();

        var bins = await result.GetJsonAsync<List<JsonBinCollection>>();

        return bins.Select(b => new JsonBinsResponse
        {
            Name = b.SnippetMeta.Name,
            BinId = b.Record
        });
    }

    public async Task<IEnumerable<FlashcardModel>> GetFlashcardsBin(string binId)
    {
        var urlPart = $"/b/{binId}";
        var result = await _flurlClient
            .Request(urlPart)
            .WithHeader("X-Master-Key", _jsonBinConfig.ApiKey)
            .WithHeader("X-Bin-Meta", "false")
            .GetAsync();

        var binCards = await result.GetJsonAsync<List<FlashcardModel>>();

        return binCards;
    }

    private class JsonBinCollection
    {
        public JsonBinCollectionMetadata? SnippetMeta { get; init; }
        public bool Private { get; init; }
        public string Record { get; init; }
        public DateTime CreatedAt { get; init; }
    }

    private class JsonBinCollectionMetadata
    {
        public string Name { get; init; }
    }
}

public class JsonBinsResponse
{
    public string Name { get; init; }

    public string BinId { get; init; }
}