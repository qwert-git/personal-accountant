using System.Globalization;
using BLL.Currencies;
using Models;
using Models.BankStatement;
using Models.Common;
using OfficeOpenXml;

namespace BLL.StatementReaders;

/// <summary>
/// Reader for Bank of Georgia statements
/// </summary>
public class BogStatementReader : IStatementsReader
{
    private static readonly Dictionary<int, string> CurrencyNameByCellOffsetMap = new()
    {
        { AmountGelColumnOffset, "GEL" },
        { AmountUsdColumnOffset, "USD" },
        { AmountEurColumnOffset, "EUR" }
    };

    #region Offset constants

    private const int AmountGelColumnOffset = 2;
    private const int AmountUsdColumnOffset = 3;
    private const int AmountEurColumnOffset = 4;
    private const int PurposeColumnOffset = 1;
    private const int DateColumnOffset = 0;
    private const int HolderNameRowOffset = 2;
    private const int HolderNameColumnOffset = 1;
    private const int StartPeriodRowOffset = 7;
    private const int StartPeriodColumnOffset = 2;
    private const int EndPeriodRowOffset = 8;
    private const int EndPeriodColumnOffset = 2;

    #endregion

    private readonly string _pathToFile;
    private readonly ICurrencyProvider _currencyProvider;

    public BogStatementReader(string pathToFile, ICurrencyProvider currencyProvider)
    {
        if (!File.Exists(pathToFile))
            throw new ArgumentException("File has not found", nameof(pathToFile));

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        _pathToFile = pathToFile;
        _currencyProvider = currencyProvider;
    }
    
    public BogStatementReader(ICurrencyProvider currencyProvider)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        _currencyProvider = currencyProvider;
    }

    public Task<Statement> ReadAsync()
    {
        return ReadAsync(_pathToFile);
    }
    
    public async Task<Statement> ReadAsync(string pathToFile)
    {
        var fi = new FileInfo(pathToFile);
        using var package = new ExcelPackage(fi);

        var generalInfo = CollectStatementSummary(package.Workbook.Worksheets["Summary"]);
        var statements = await CollectStatementsAsync(package.Workbook.Worksheets["Statement"]);

        return new Statement(
            StatementName: fi.Name,
            AccountHolder: generalInfo.HolderName,
            Period: generalInfo.StatementPeriod,
            Transactions: statements);
    }

    private static StatementSummary CollectStatementSummary(ExcelWorksheet summary)
    {
        var tableRange = WholeTableRange(summary);

        return new StatementSummary(
            HolderName: tableRange.GetCellValue<string>(HolderNameRowOffset, HolderNameColumnOffset),
            StatementPeriod: new Period(
                tableRange.GetCellValue<DateOnly>(StartPeriodRowOffset, StartPeriodColumnOffset),
                tableRange.GetCellValue<DateOnly>(EndPeriodRowOffset, EndPeriodColumnOffset)
            )
        );
    }

    private async Task<IReadOnlyCollection<Transaction>> CollectStatementsAsync(ExcelWorksheet transactionsWorksheet)
    {
        var transactionsTable = WholeTableRange(transactionsWorksheet);

        var result = new List<Transaction>();

        for (var i = 1; i < transactionsTable.Rows; i++)
        {
            var transactionAmount = ParseTransactionAmount(transactionsTable, i);
            var currency = await _currencyProvider.GetAsync(transactionAmount.Currency);

            var purpose = transactionsTable.GetCellValue<string>(i, PurposeColumnOffset);
            result.Add(new Transaction(
                Date: DateOnly.Parse(transactionsTable.GetCellValue<string>(i, DateColumnOffset)),
                Purpose: purpose,
                Amount: transactionAmount.Value,
                Currency: currency
            ));
        }

        return result;
    }
    
    private static ExcelRange WholeTableRange(ExcelWorksheet worksheet)
    {
        return worksheet.Cells[1, 1, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column];
    }

    private static TransactionAmount ParseTransactionAmount(ExcelRange tableRange, int row)
    {
        if (TryGetTransactionAmount(tableRange, row, AmountGelColumnOffset, out var res))
            return res;

        if (TryGetTransactionAmount(tableRange, row, AmountUsdColumnOffset, out res))
            return res;

        if (TryGetTransactionAmount(tableRange, row, AmountEurColumnOffset, out res))
            return res;

        throw new ApplicationException("Cannot parse amount. Currency is not supported");
    }

    private static bool TryGetTransactionAmount(
        ExcelRange tableRange, 
        int row, 
        int cellOffset, 
        out TransactionAmount transactionAmount)
    {
        transactionAmount = default!;

        var amount = ParseWithInvariantCulture(tableRange.GetCellValue<string>(row, cellOffset));
        if (amount == null)
            return false;

        transactionAmount = new TransactionAmount(amount.Value, CurrencyNameByCellOffsetMap[cellOffset]);

        return true;
    }

    private static decimal? ParseWithInvariantCulture(string value)
    {
        return decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var amount)
            ? amount
            : null;
    }

    private record StatementSummary(string HolderName, Period StatementPeriod);

    private record TransactionAmount(decimal Value, string Currency);
}