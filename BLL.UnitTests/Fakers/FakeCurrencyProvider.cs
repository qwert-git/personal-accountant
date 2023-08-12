using BLL.Currencies;
using Models;
using Models.Common;

namespace BLL.UnitTests.Fakers;

internal class FakeCurrencyProvider : ICurrencyProvider
{
    private readonly IDictionary<string, Currency> _currencies = new Dictionary<string, Currency>
    {
        { "EUR", new Currency("EUR", 1, "€") },
        { "USD", new Currency("USD", 0.916m, "$") },
        { "GEL", new Currency("GEL", 0.3508m, "₾") }
    };
    
    public Task<Currency> GetAsync(string currencyName)
    {
        if(!_currencies.TryGetValue(currencyName, out var res))
            throw new ArgumentException($"Currency {currencyName} is not supported");
        
        return Task.FromResult(res);
    }
}