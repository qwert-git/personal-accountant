using Models.BankStatement;

namespace BLL.Filters;

public class PurposeStopWordFilter : ITransactionsFilter
{
    private readonly string _stopWord;

    public PurposeStopWordFilter(string stopWord)
    {
        _stopWord = stopWord;
    }

    public bool IsAllowed(Transaction transactions)
    {
        return !transactions.Purpose.Contains(_stopWord);
    }
}

public class NegativeAmountFilter : ITransactionsFilter
{
    public bool IsAllowed(Transaction transactions)
    {
        return transactions.Amount < 0;
    }
}