using Flashcards.Configuration;
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
        if (_environment.IsProduction())
        {
            return Enumerable.Empty<JsonBinsResponse>();
        }

        _logger.LogWarning($"test ApiKey");
        if (string.IsNullOrWhiteSpace(_jsonBinConfig.ApiKey))
        {
            _logger.LogError("ApiKey is missing");
        }
        else
        {
            _logger.LogWarning($"ApiKey: {_jsonBinConfig.ApiKey.Substring(0, 15)}");
        }

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