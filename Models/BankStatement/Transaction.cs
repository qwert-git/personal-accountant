using Models.Common;

namespace Models.BankStatement;

public record Transaction(DateOnly Date, string Purpose, decimal Amount, Currency Currency)
{
    public Transaction ConvertTo(Currency currency)
    {
        var inNewCurrency = 0m;
        if (Currency.RateToBaseCurrency == 1)
        {
            inNewCurrency = Amount / currency.RateToBaseCurrency;
        }
        else
        {
            var inBaseCurrency = Amount * Currency.RateToBaseCurrency;
            inNewCurrency = inBaseCurrency * currency.RateToBaseCurrency;    
        }

        return this with
        {
            Currency = currency,
            Amount = inNewCurrency
        };
    }
}