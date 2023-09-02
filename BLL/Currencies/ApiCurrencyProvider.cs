using System.Collections.Concurrent;
using BLL.Config;
using Models.Common;
using Newtonsoft.Json;

namespace BLL.Currencies;

/// <summary>
/// Free Currency Api Provider (https://openexchangerates.org/)
/// </summary>
public class ApiCurrencyProvider : ICurrencyProvider
{
    // TODO: Thread safe lazy load here?
    private static ConcurrentDictionary<string, decimal>? _ratesCache;
    private static object _lockObj = new();
    private readonly OpenExchangeApiClient _client;
    private readonly CurrencyProviderConfig _config;

    public ApiCurrencyProvider(CurrencyProviderConfig config)
    {
        _config = config;
        _client = new OpenExchangeApiClient(config.ApiBaseUrl, config.ApiKey, config.SupportedCurrencies);
    }

    public Task<Currency> GetAsync(string currencyName)
    {
        LoadTheCache();

        var usdToBaseCurrencyRate = _ratesCache![_config.BaseCurrency];
        var usdToCurrency = _ratesCache[currencyName];
        var currencyToEur = usdToBaseCurrencyRate / usdToCurrency; 
        
        return Task.FromResult(new Currency(currencyName, currencyToEur, GetCurrencyPrefix(currencyName)));
    }

    public async Task<IReadOnlyCollection<Currency>> GetAllAsync()
    {
        LoadTheCache();

        return await Task.WhenAll(
            _ratesCache!
                .Select(c => GetAsync(c.Key))
            );
    }

    private void LoadTheCache()
    {
        if (_ratesCache is not null) return;
        lock (_lockObj)
        {
            if (_ratesCache is not null) return;
            
            var res = _client.GetAllAsync().Result;
            _ratesCache = new ConcurrentDictionary<string, decimal>(res.Rates);
        }
    }

    private static string GetCurrencyPrefix(string currencyName)
    {
        return currencyName switch
        {
            "USD" => "$",
            "GEL" => "₾",
            "EUR" => "€",
            _ => throw new ArgumentException($"Currency {currencyName} is not supported")
        };
    }

}

// TODO: Move it to Infrastructure layer?
public class OpenExchangeApiClient
{
    private readonly string _apiKey;
    private readonly string _symbols;
    private readonly HttpClient _httpClient;

    public OpenExchangeApiClient(string apiBaseUrl, string apiKey, IEnumerable<string> supportedCurrencies)
    {
        _apiKey = apiKey;
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(apiBaseUrl);

        _symbols = supportedCurrencies.Aggregate((c1,c2) => $"{c1},{c2}");
    }

    
    public async Task<CurrencyApiResult> GetAllAsync()
    {
        var res = await _httpClient.GetAsync($"api/latest.json?app_id={_apiKey}&base=USD&symbols={_symbols}");
            
        if(!res.IsSuccessStatusCode)
            throw new Exception($"Failed to get currency rates. Status code: {res.StatusCode}");

        var json = res.Content.ReadAsStringAsync();
        
        return JsonConvert.DeserializeObject<CurrencyApiResult>(json.Result)!;
    }
    
    public record CurrencyApiResult(Dictionary<string, decimal> Rates);
}