using Models.BankStatement;
using Models.Common;

namespace BLL.UnitTests;

internal class TestStatementBuilder
{
    private readonly List<Transaction> _transactions = new();
    private Period? _period;

    public TestStatementBuilder WithTransaction(string purpose, decimal amount, Currency currency)
    {
        _transactions.Add(new Transaction(DateOnly.FromDateTime(DateTime.UtcNow), purpose, amount, currency));
        return this;
    }
        
    public TestStatementBuilder WithDefaultPeriod()
    {
        _period = GetDefaultPeriod();
        return this;
    }

    public Statement Build()
    {
        _period ??= GetDefaultPeriod();
        return new Statement(string.Empty, string.Empty, _period, _transactions);
    }
        
    private static Period GetDefaultPeriod() => new (default, default);
}