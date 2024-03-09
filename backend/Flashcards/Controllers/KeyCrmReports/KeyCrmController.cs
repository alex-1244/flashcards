using System.Globalization;
using System.Text;
using Flashcards.Controllers.KeyCrmReports.Services;
using Microsoft.AspNetCore.Mvc;

namespace Flashcards.Controllers.KeyCrmReports;

[ApiController]
[Route("keycrm")]
public class KeyCrmController : ControllerBase
{
    private readonly KeyCrmConnector _connector;
    private readonly ReportEmailService _reportEmailService;
    private readonly ILogger<KeyCrmController> _logger;

    public KeyCrmController(
        KeyCrmConnector connector,
        ReportEmailService reportEmailService,
        ILogger<KeyCrmController> logger)
    {
        _connector = connector;
        _reportEmailService = reportEmailService;
        _logger = logger;
    }

    [HttpPost("token")]
    public async Task<ActionResult> GetToken([FromBody] TokenRequest request)
    {
        var token = await _connector.GetToken(request.Username, request.Password);

        return Ok(new
        {
            token
        });
    }

    [HttpGet("token")]
    public async Task<ActionResult> GetToken()
    {
        var token = await _connector.GetToken();

        return Ok(new
        {
            token
        });
    }

    [HttpGet("partners")]
    [ResponseCache(VaryByHeader = "User-Agent", Duration = 300)]
    public async Task<ActionResult> GetPartners()
    {
        var token = Request.Headers["Authentication"].FirstOrDefault();

        var partners = await _connector.GetPartners(token);

        return Ok(new
        {
            partners
        });
    }

    [HttpGet("products")]
    public async Task<ActionResult> GetProducts([FromQuery] int category, [FromQuery] string startDate, [FromQuery] string endDate)
    {
        var sDate = DateTime.ParseExact(startDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
        var eDate = DateTime.ParseExact(endDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

        var response = await GetProductDetails(category, sDate, eDate);

        return Ok(new
        {
            products = response.Products,
            startDate = response.StartDate,
            endDate = response.EndDate,
            partnerName = response.PartnerName
        });
    }

    [HttpGet("report-file")]
    public async Task<ActionResult> DownloadReport([FromQuery] int category, [FromQuery] string startDate, [FromQuery] string endDate)
    {
        var sDate = DateTime.ParseExact(startDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
        var eDate = DateTime.ParseExact(endDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

        var productDetails = await GetProductDetails(category, sDate, eDate);

        var fileName =
            $"ngd_{productDetails.StartDate.ToString("yyyy-MM-dd")}_{productDetails.EndDate.ToString("yyyy-MM-dd")}.csv";
        return File(await GetReport(productDetails), "application/csv", fileName);
    }

    [HttpPost("report")]
    public async Task<ActionResult> SendReport(SendReportRequest request)
    {
        var sDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
        var eDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

        var productDetails = await GetProductDetails(request.Category, sDate, eDate);

        var fileName =
            $"ngd_{productDetails.StartDate.ToString("yyyy-MM-dd")}_{productDetails.EndDate.ToString("yyyy-MM-dd")}.csv";
        var reportStream = await GetReport(productDetails);
        reportStream.Position = 0;

        //Source = "yiyi.store.od@gmail.com",
        try
        {
            _reportEmailService.SendReport(request.Email, fileName, reportStream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while sending report e-mail");
            return BadRequest();
        }

        return Ok();
    }

    private async Task<Stream> GetReport(ProductDetailsResponse details)
    {
        var outputStream = new MemoryStream();
        var sWriter = new StreamWriter(outputStream, Encoding.UTF8);
        sWriter.Write("Назва товару;Середня ціна;Сума закупівлі;Суума продажів;Кількість товарів");

        foreach (var record in details.Products)
            sWriter.Write(
                $"{Environment.NewLine}{record.Title};{record.avg_price};{record.total_purchase};{record.total_price};{record.SalesCount}");
        await sWriter.FlushAsync();

        outputStream.Position = 0;
        return outputStream;
    }

    private async Task<ProductDetailsResponse> GetProductDetails(int category, DateTime startDate, DateTime endDate)
    {
        var token = Request.Headers["Authentication"].FirstOrDefault();

        var today = DateTime.Today;
        var dayOfWeek = today.DayOfWeek;
        endDate = endDate == DateTime.MinValue
            ? today.AddDays(-1 * (int)dayOfWeek)
            : endDate;
        startDate = startDate == DateTime.MinValue
            ? startDate.AddDays(-6)
            : startDate;

        var products = await _connector.GetProducts(new ProductsRequest
        {
            CategoryId = category,
            From = startDate,
            To = endDate,
            Token = token
        });

        var response = new ProductDetailsResponse
        {
            Products = products,
            StartDate = startDate,
            EndDate = endDate,
            PartnerName = (await _connector.GetPartners(token)).FirstOrDefault(x => x.Id == category)?.Name
                          ?? string.Empty
        };

        return response;
    }
}

public class TokenRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class ProductDetailsResponse
{
    public List<Product> Products { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string PartnerName { get; set; }
}

public class SendReportRequest
{
    public int Category { get; set; }
    public string Email { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
}