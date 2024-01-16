using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Flashcards.Controllers.Frontpage.Models;
using Microsoft.AspNetCore.Mvc;

namespace Flashcards.Controllers.Frontpage;

[ApiController]
[Route("frontpage")]
public class FrontpageController : ControllerBase
{
    private readonly Guid _defaultUserId = Guid.Empty;

    private readonly JsonBinConnector _jsonBinConnector;
    private readonly ILogger<FrontpageController> _logger;
    private readonly IDynamoDBContext _dbContext;

    public FrontpageController(
        JsonBinConnector jsonBinConnector,
        IDynamoDBContext dbContext,
        ILogger<FrontpageController> logger)
    {
        _jsonBinConnector = jsonBinConnector;
        _logger = logger;
        _dbContext = dbContext;
    }

    [HttpGet]
    [ResponseCache(VaryByHeader = "User-Agent", Duration = 300)]
    public async Task<ActionResult<List<JsonBinsResponse>>> Get()
    {
        var bins = await _jsonBinConnector.GetFlashcardsBins();

        //var awsBins = await _dbContext.QueryAsync<Bucket>(, QueryOperator.Equal, _defaultUserId);

        return Ok(bins);
    }

    [HttpGet]
    [Route("~/{binId}")]
    [ResponseCache(VaryByHeader = "User-Agent", Duration = 300)]
    public async Task<ActionResult> GetBin([FromRoute] string binId)
    {
        var binCards = await _jsonBinConnector.GetFlashcardsBin(binId);

        return Ok(binCards);
    }

    [HttpPost]
    [Route("card")]
    public async Task<ActionResult> CreateCard()
    {
        return Ok();

        var card = new Flashcard
        {
            CardId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Definition = "Def",
            Word = "Word",
            BucketName = "BucketName",
            ImageUrl = "Img",
            BucketId = Guid.NewGuid(),
            Children = new List<TestChild>()
            {
                new TestChild
                {
                    Key = "C1K",
                    Value = "C1V"
                },
                new TestChild
                {
                    Key = "C2K",
                    Value = "C2V"
                }
            }
        };

        await _dbContext.SaveAsync(card);

        return Ok();
    }
}