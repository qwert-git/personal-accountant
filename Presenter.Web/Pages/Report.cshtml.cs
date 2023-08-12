using BLL;
using BLL.Config;
using BLL.Currencies;
using BLL.StatementReaders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;

namespace Presenter.Web.Pages;

public class Report : PageModel
{
    private readonly ReportGenerator _reportGenerator;
    private readonly ICurrencyProvider _currencyProvider;
    private readonly IStatementsReader _statementsReader;
    
    public StatementsReport ReportData { get; set; }
    public string BaseCurrency { get; set; }

    public Report(
        ReportGenerator reportGenerator, 
        CurrencyProviderConfig currencyProviderConfig,
        ICurrencyProvider currencyProvider,
        IStatementsReader statementsReader)
    {
        _reportGenerator = reportGenerator;
        _currencyProvider = currencyProvider;
        _statementsReader = statementsReader;
        BaseCurrency = currencyProviderConfig.BaseCurrency;
    }

    public async Task OnGetAsync(string pathToFile)
    {
        //ReportData = await _reportGenerator.GenerateAsync();
        
        //var fileContent = await System.IO.File.ReadAllTextAsync(pathToFile);
        var reader = new BogStatementReader(pathToFile, _currencyProvider);
        var statement = await reader.ReadAsync();
        ReportData = _reportGenerator.Generate(statement);
    }
}