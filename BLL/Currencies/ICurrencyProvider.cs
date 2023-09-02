using Models.Common;

namespace BLL.Currencies;

public interface ICurrencyProvider
{
    Task<Currency> GetAsync(string currencyName);
    
    Task<IReadOnlyCollection<Currency>> GetAllAsync();
}