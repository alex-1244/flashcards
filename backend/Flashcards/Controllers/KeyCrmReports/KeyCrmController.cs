using System.Net;
using System.Text;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace Flashcards.Controllers.KeyCrmReports;

[ApiController]
[Route("keycrm")]
public class KeyCrmController : ControllerBase
{
    private readonly KeyCrmConnector _connector;
    private readonly IAmazonSimpleEmailService _awsSes;

    public KeyCrmController(KeyCrmConnector connector, IAmazonSimpleEmailService awsSes)
    {
        _connector = connector;
        _awsSes = awsSes;
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
    public async Task<ActionResult> GetProducts([FromQuery] int category)
    {
        var response = await GetProductDetails(category);

        return Ok(new
        {
            products = response.Products,
            startDate = response.StartDate,
            endDate = response.EndDate,
            partnerName = response.PartnerName
        });
    }

    [HttpPost("report-file")]
    public async Task<ActionResult> DownloadReport([FromQuery] int category)
    {
        var productDetails = await GetProductDetails(category);

        var fileName =
            $"ngd_{productDetails.StartDate.ToString("yyyy-MM-dd")}_{productDetails.EndDate.ToString("yyyy-MM-dd")}.csv";
        return File(await GetReport(productDetails), "application/csv", fileName);
    }

    [HttpPost("report")]
    public async Task<ActionResult> SendReport(SendReportRequest request)
    {
        var productDetails = await GetProductDetails(request.Category);

        var fileName =
            $"ngd_{productDetails.StartDate.ToString("yyyy-MM-dd")}_{productDetails.EndDate.ToString("yyyy-MM-dd")}.csv";
        var reportStream = await GetReport(productDetails);
        reportStream.Position = 0;

        //Source = "yiyi.store.od@gmail.com",

        var bodyBuilder = new BodyBuilder();

        bodyBuilder.HtmlBody = "Магазин 'ЇЇ', звіт за тиждень";
        bodyBuilder.TextBody = "Магазин 'ЇЇ', звіт за тиждень";

        bodyBuilder.Attachments.Add(fileName, reportStream);

        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(new MailboxAddress("yiyi store", "yiyi.store.od@gmail.com"));
        mimeMessage.To.Add(new MailboxAddress(productDetails.PartnerName, request.Email));

        mimeMessage.Subject = "Hey!";
        mimeMessage.Body = bodyBuilder.ToMessageBody();
        using (var messageStream = new MemoryStream())
        {
            await mimeMessage.WriteToAsync(messageStream);
            var sendRequest = new SendRawEmailRequest { RawMessage = new RawMessage(messageStream) };
            var response = await _awsSes.SendRawEmailAsync(sendRequest);

            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                return Ok();
            }

            return BadRequest(response.HttpStatusCode);
        }
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

        return outputStream;
    }

    private async Task<ProductDetailsResponse> GetProductDetails(int category)
    {
        var token = Request.Headers["Authentication"].FirstOrDefault();

        var today = DateTime.Today;
        var dayOfWeek = today.DayOfWeek;
        var lastSunday = today.AddDays(-1 * (int)dayOfWeek);
        var lastMonday = lastSunday.AddDays(-6);

        var products = await _connector.GetProducts(new ProductsRequest
        {
            CategoryId = category,
            From = lastMonday,
            To = lastSunday,
            Token = token
        });

        var response = new ProductDetailsResponse
        {
            Products = products,
            StartDate = lastMonday,
            EndDate = lastSunday,
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
}