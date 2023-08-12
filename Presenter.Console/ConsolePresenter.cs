using BLL.Config;
using Models;

namespace Presenter.Console;

internal class ConsolePresenter
{
    private readonly CurrencyProviderConfig _currencyProviderConfig;
    private readonly AppOptions _options;

    public ConsolePresenter(CurrencyProviderConfig currencyProviderConfig, AppOptions options)
    {
        _currencyProviderConfig = currencyProviderConfig;
        _options = options;
    }

    public void Present(StatementsReport report)
    {
        System.Console.WriteLine("It's time to count money!");
        System.Console.WriteLine("Generating report...");

        System.Console.WriteLine();
        System.Console.WriteLine("`````````````````````````````````````");
        System.Console.WriteLine($"Report: {report.Name}");
        System.Console.WriteLine($"Report for {report.Period.Start} - {report.Period.End}");
        System.Console.WriteLine();

        var totalForPeriod = 0m;
        foreach (var category in report.Categories.OrderBy(c => c.Total))
        {
            System.Console.WriteLine($"{category.Name} - {category.Total:N1} {_currencyProviderConfig.BaseCurrency}");
            totalForPeriod += category.Total;
            if (_options.ShowTransactions || _options.ExpandCategory == category.Name)
            {
                ShowCategory(category);
            }

            System.Console.WriteLine();
        }

        System.Console.WriteLine("======================================");
        System.Console.WriteLine($"Grand Total: {totalForPeriod:N1} {_currencyProviderConfig.BaseCurrency}");

        System.Console.WriteLine();
        System.Console.WriteLine("`````````````````````````````````````");
        System.Console.WriteLine("I'm done! Laundering finished!");
    }

    private static void ShowCategory(ReportCategory reportCategory)
    {
        foreach (var statement in reportCategory.Statements)
        {
            // TODO: Move ToString to Amount class
            System.Console.WriteLine(
                $"{statement.Date} - {statement.Amount:N1} {statement.Currency.Name} - {statement.Purpose}");
        }
    }
}