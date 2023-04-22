using Microsoft.AspNetCore.Mvc;

namespace Flashcards.Controllers.Frontpage;

[ApiController]
[Route("[controller]")]
public class FrontpageController : ControllerBase
{
    private readonly JsonBinConnector _jsonBinConnector;

    public FrontpageController(JsonBinConnector jsonBinConnector)
    {
        _jsonBinConnector = jsonBinConnector;
    }

    [HttpGet]
    public async Task<ActionResult<List<JsonBinsResponse>>> Get()
    {
        var bins = await _jsonBinConnector.GetFlashcardsBins();

        return Ok(bins);
    }

    [HttpGet]
    [Route("~/{binId}")]
    public async Task<ActionResult> GetBin([FromRoute] string binId)
    {
        var binCards = await _jsonBinConnector.GetFlashcardsBin(binId);

        return Ok(binCards);
    }

    [HttpGet]
    [Route("~/")]
    public ActionResult GetIndex()
    {
        return Ok(new { a = 3 });
    }
}