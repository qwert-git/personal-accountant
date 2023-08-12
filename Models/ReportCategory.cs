using Models.BankStatement;

namespace Models;

public record ReportCategory(string Name, decimal Total, IReadOnlyCollection<Transaction> Statements);

// TODO: I need to combine these two records into one
public record CategoryTotal(string Name, decimal Total);
public record ReportTransaction(string CategoryName, decimal Amount, string Merchant, DateOnly Date);