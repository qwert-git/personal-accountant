using BLL.Config;
using BLL.Currencies;
using BLL.Filters;
using BLL.MerchantExtractors;
using BLL.StatementProcessing;
using BLL.StatementReaders;
using Microsoft.Extensions.Logging;
using Models;
using Models.BankStatement;
using Models.Common;

namespace BLL;

public class ReportGenerator
{
    private readonly IStatementsReader _reader;
    private readonly IReadOnlyCollection<IMerchantExtractor> _merchantExtractors;
    private readonly ICategoryMapper _categoryMapper;
    private readonly Currency _baseCurrency;
    private readonly IReadOnlyCollection<ITransactionsFilter> _transactionsFilters;
    private readonly ILogger<ReportGenerator> _logger;

    public ReportGenerator(
        IStatementsReader reader, 
        IReadOnlyCollection<IMerchantExtractor> merchantExtractors,
        ICategoryMapper categoryMapper,
        ICurrencyProvider currencyProvider,
        CurrencyProviderConfig currencyProviderConfig, 
        IReadOnlyCollection<ITransactionsFilter> transactionsFilters,
        ILogger<ReportGenerator> logger)
    {
        _reader = reader;
        _merchantExtractors = merchantExtractors;
        _categoryMapper = categoryMapper;
        _transactionsFilters = transactionsFilters;
        _logger = logger;

        _baseCurrency = currencyProvider.GetAsync(currencyProviderConfig.BaseCurrency).Result;
    }

    [Obsolete("Use parameterized Generate method instead")]
    public async Task<StatementsReport> GenerateAsync()
    {
        _logger.LogInformation("Start reading the statements...");
        var report = await _reader.ReadAsync();
        
        return Generate(report);
    }
    
    public StatementsReport Generate(Statement report)
    {
        _logger.LogInformation("Start mapping categories...");
        var statementsByCategory = new Dictionary<string, List<Transaction>>();
        foreach (var s in report.Transactions)
        {
            if(_transactionsFilters.Any(f => !f.IsAllowed(s)))
                continue;

            var merchant = _merchantExtractors
                .Select(extractor => extractor.GetMerchant(s.Purpose))
                .FirstOrDefault(m => m is not null);
            
            var category = _categoryMapper.GetCategory(merchant);
            
            var statement = s.Currency != _baseCurrency ? s.ConvertTo(_baseCurrency) : s;
            if (!statementsByCategory.TryGetValue(category, out var statements))
            {
                statements = new List<Transaction> ();
            }

            statements.Add(statement);
            statementsByCategory[category] = statements;
        }

        return new StatementsReport
        {
            Name = report.StatementName,
            Period = report.Period,
            Categories = statementsByCategory.Select(pair =>
                new ReportCategory(Name: pair.Key, Total: pair.Value.Sum(s => s.Amount), Statements: pair.Value)
            ).ToList()
        };
    }
}