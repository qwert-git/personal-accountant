using BLL.Currencies;
using FluentAssertions;
using Models.BankStatement;
using Models.Common;

namespace BLL.UnitTests;


public class TransactionTests
{
    private readonly Currency _eur = new("EUR", 1, "€");
    private readonly Currency _usd = new("USD", 0.916m, "$");
    private readonly Currency _gel = new("GEL", 0.3508m, "₾");
    
    [Fact]
    public void UsdToEur()
    {
        var inUsd = CreateTestTransaction(amount: 100, _usd);

        var inEur = inUsd.ConvertTo(_eur);

        inEur.Amount.Should().BeApproximately(91.6m, 0.01m);
    }
    
    
    [Fact]
    public void GelToEur()
    {
        var inGel = CreateTestTransaction(amount: 100, _gel);

        var inEur = inGel.ConvertTo(_eur);

        inEur.Amount.Should().BeApproximately(35.08m, 0.01m);
    }

    [Fact]
    public void EurToUsd()
    {
        var inEur = CreateTestTransaction(100, _eur);

        var inUsd = inEur.ConvertTo(_usd);
        
        inUsd.Amount.Should().BeApproximately(109.16m, 0.05m);
    }
    
    private static Transaction CreateTestTransaction(decimal amount, Currency currency) => new(default, "Test transaction", amount, currency);
}