using System.Collections.Concurrent;
using Flashcards.Configuration;
using Flurl.Http;
using Flurl.Http.Configuration;
using Microsoft.Extensions.Caching.Memory;

namespace Flashcards.Controllers.KeyCrmReports;

public class KeyCrmConnector
{
    private readonly ILogger<KeyCrmConnector> _logger;
    private readonly KeyCrmConfig _crmConfig;
    private readonly IFlurlClient _flurlClient;
    private readonly IMemoryCache _cache;

    public KeyCrmConnector(
        IFlurlClientFactory flurlClientFactory,
        ILogger<KeyCrmConnector> logger,
        KeyCrmConfig crmConfig,
        IMemoryCache cache)
    {
        _logger = logger;
        _crmConfig = crmConfig;
        _cache = cache;
        _flurlClient = flurlClientFactory.Get(crmConfig.BaseUrl);
    }

    public async Task<string> GetToken(string? username = null, string? password = null)
    {
        if (_cache.TryGetValue("keycrm_token", out var cachedToken))
        {
            return (string)cachedToken!;
        }

        try
        {
            var loginEndpoint = "/auth/login";

            var result = await _flurlClient
                .Request(loginEndpoint)
                .PostJsonAsync(new
                {
                    username = username ?? _crmConfig.Username,
                    password = password ?? _crmConfig.Password
                });

            var token = await result.GetJsonAsync<KeyCrmLoginResponse>();

            _cache.Set("keycrm_token", token.access_token, TimeSpan.FromMinutes(5));
            _cache.Set("keycrm_refresh_token", token.refresh_token, TimeSpan.FromMinutes(5));

            return token.access_token;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "error while getting KeyCrm token");
            throw;
        }
    }

    public async Task<List<Partner>> GetPartners(string? token)
    {
        if (_cache.TryGetValue("keycrm_partners", out var partnersCached))
        {
            return (List<Partner>)partnersCached!;
        }

        token ??= await GetToken();

        try
        {
            var loginEndpoint = "/catalog/categories/tree?flat=true";

            var result = await _flurlClient
                .Request(loginEndpoint)
                .WithHeader("Authorization", $"Bearer {token}")
                .GetAsync();

            var partners = await result.GetJsonAsync<List<Partner>>();

            _cache.Set("keycrm_partners", partners, TimeSpan.FromMinutes(5));

            return partners;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "error while getting KeyCrm token");
            throw;
        }
    }

    public async Task<List<Product>> GetProducts(ProductsRequest request)
    {
        try
        {
            var productsEndpoint = "/statistics/catalog/products";

            var token = request.Token ?? await GetToken();
            var result = await _flurlClient
                .Request(productsEndpoint)
                .SetQueryParam("per_page", 100)
                .SetQueryParam("page", 1)
                .SetQueryParam("filters[sources][]", "")
                .SetQueryParam("filters[category_id][0]", request.CategoryId)
                .SetQueryParam("filters[created_at][0]", request.From.ToString("yyyy-MM-dd"))
                .SetQueryParam("filters[created_at][1]", request.To.ToString("yyyy-MM-dd"))
                .SetQueryParam("filters[order]", "all")
                .WithHeader("Authorization", $"Bearer {token}")
                .GetAsync();

            var products = await result.GetJsonAsync<List<Product>>();



            foreach (var product in products)
            {
                product.SalesCount = int.Parse(product.sales_count.Split(".")[0]);
            }

            return products;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "error while getting KeyCrm token");
            throw;
        }
    }
}

public class Product
{
    public int Id { get; set; }
    public string Title { get; set; }
    public double avg_price { get; set; }
    public double total_price { get; set; }
    public double total_purchase { get; set; }
    public double total_margin_amount { get; set; }
    public string sales_count { get; set; }
    public int SalesCount { get; set; }
}

public class ProductsRequest
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public int CategoryId { get; set; }
    public string? Token { get; set; }
}

public class Partner
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class KeyCrmLoginResponse
{
    public string access_token { get; set; }

    public string refresh_token { get; set; }

    public int expires_in { get; set; }
}