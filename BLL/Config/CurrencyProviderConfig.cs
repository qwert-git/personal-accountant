namespace BLL.Config;

public record CurrencyProviderConfig
{
    public string ApiBaseUrl { get; set; }
    
    public string ApiKey { get; set; }
    
    public string BaseCurrency { get; set; }
    
    public IReadOnlyCollection<string> SupportedCurrencies { get; set; }
}