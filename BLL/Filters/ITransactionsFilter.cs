using Models.BankStatement;

namespace BLL.Filters;

public interface ITransactionsFilter
{
    bool IsAllowed(Transaction transactions);
}