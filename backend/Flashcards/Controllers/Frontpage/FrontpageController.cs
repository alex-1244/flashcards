using Microsoft.AspNetCore.Mvc;

namespace Flashcards.Controllers.Frontpage;

[ApiController]
[Route("[controller]")]
public class FrontpageController : ControllerBase
{
    private readonly JsonBinConnector _jsonBinConnector;
    private readonly ILogger<FrontpageController> _logger;

    public FrontpageController(
        JsonBinConnector jsonBinConnector,
        ILogger<FrontpageController> logger)
    {
        _jsonBinConnector = jsonBinConnector;
        _logger = logger;
    }

    [HttpGet]
    [ResponseCache(VaryByHeader = "User-Agent", Duration = 300)]
    public async Task<ActionResult<List<JsonBinsResponse>>> Get()
    {
        var bins = await _jsonBinConnector.GetFlashcardsBins();

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

    [HttpGet]
    [Route("~/")]
    public ActionResult GetIndex()
    {
        _logger.LogWarning("Service alive");
        return Ok(new { a = 3 });
    }
}