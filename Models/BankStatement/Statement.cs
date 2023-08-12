using Models.Common;

namespace Models.BankStatement;

public record Statement(
    string StatementName,
    string AccountHolder,
    Period Period,
    IReadOnlyCollection<Transaction> Transactions);