using BLL.StatementReaders;
using BLL.UnitTests.Fakers;
using FluentAssertions;
using Models;
using Models.BankStatement;
using Models.Common;

namespace BLL.UnitTests;

public class ExcelStatementsReaderTests
{
    private readonly BogStatementReader _sut;
    private readonly Currency _gel;

    public ExcelStatementsReaderTests()
    {
        var currencyProvider = new FakeCurrencyProvider();
        _gel = currencyProvider.GetAsync("GEL").Result;
        _sut = new BogStatementReader("./Samples/bog.xlsx", currencyProvider);
    }

    [Fact]
    public void FileHasNotFound()
    {
        Assert.Throws<ArgumentException>(() => new BogStatementReader("./Samples/not_exists_file.pdf", new FakeCurrencyProvider()));
    }

    [Fact]
    public async Task FillTheAccountHolder()
    {
        var report = await _sut.ReadAsync();

        report.AccountHolder.Should().Be("AHANUR AKUNDOV");
    }
    
    [Fact]
    public async Task CollectStatements()
    {
        var report = await _sut.ReadAsync();

        report.Transactions.Should().NotBeNullOrEmpty().And
            .Subject.Count().Should().Be(390);

        var firstExpected = new Transaction(new DateOnly(2023, 06, 05), "Payment - Amount: GEL40.70; Merchant: PIATTO, Batumi, abashidze/vaja-fshavela 52/18; MCC:5812; Date: 04/06/2023 15:22; Card No: ****1781; Payment transaction amount and currency: 40.70 GEL", -40.7m, _gel);
        report.Transactions.First().Should().Be(firstExpected);
        
        var lastExpected = new Transaction(new DateOnly(2023, 03, 06), "Automatic conversion, rate: 2.713", 5.5m, _gel);
        report.Transactions.Last().Should().Be(lastExpected);
    }
}