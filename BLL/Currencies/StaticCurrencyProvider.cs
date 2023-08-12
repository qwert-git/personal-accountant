using Models.Common;

namespace BLL.Currencies;

public class StaticCurrencyProvider : ICurrencyProvider
{
    private readonly IDictionary<string, Currency> _currencies = new Dictionary<string, Currency>
    {
        { "EUR", new Currency("EUR", 1, "€") },
        { "USD", new Currency("USD", 0.916m, "$") },
        { "GEL", new Currency("GEL", 0.351m, "₾") }
    };
    public Task<Currency> GetAsync(string currencyName)
    {
        return Task.FromResult(_currencies[currencyName]);
    }
}