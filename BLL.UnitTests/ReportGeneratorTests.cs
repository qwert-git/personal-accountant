using BLL.Config;
using BLL.Filters;
using BLL.MerchantExtractors;
using BLL.StatementProcessing;
using BLL.StatementReaders;
using BLL.UnitTests.Builders;
using BLL.UnitTests.Fakers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Models.BankStatement;
using Moq;

namespace BLL.UnitTests;

public class ReportGeneratorTests
{
    private readonly ReportGenerator _sut;

    public ReportGeneratorTests()
    {
        var mockReader = new Mock<IStatementsReader>();
        mockReader
            .Setup(r => r.ReadAsync())
            .ReturnsAsync(GetDefaultTestStatement());

        var categoryMapRepository = CategoryMapRepositoryBuilder.WithConfig(new CategoryMapperConfig
        {
            { "Restaurants", new [] { "PIATTO", "McDonalnd" }}
        });
        var categoryMapper = new CategoryMapper(categoryMapRepository);

        var filters = Enumerable.Empty<ITransactionsFilter>().ToList();
        var currencyConfig = new CurrencyProviderConfig
        {
            BaseCurrency = "EUR"
        };
        
        _sut = new ReportGenerator(
            mockReader.Object,
            new []{ new BogMerchantExtractor() },
            categoryMapper,
            new FakeCurrencyProvider(),
            currencyConfig, 
            filters,
            Mock.Of<ILogger<ReportGenerator>>());
    }

    [Fact]
    public async Task ReportGroupedByCategory()
    {
        var report = await _sut.GenerateAsync();

        report.Categories.Should().HaveCount(2);
        report.Categories.First().Name.Should().Be("Restaurants");
        report.Categories.First().Statements.Should().HaveCount(2);
    }
    
    [Fact]
    public async Task TotalConvertsToBaseCurrency()
    {
        var report = await _sut.GenerateAsync();

        report.Categories.Sum(c => c.Total).Should().BeApproximately(23.2m, 0.05m);
    }

    private static Statement GetDefaultTestStatement()
    {
        var currencyProvider = new FakeCurrencyProvider();
            
        var usd = currencyProvider.GetAsync("USD").Result;
        var eur = currencyProvider.GetAsync("EUR").Result;
        var gel = currencyProvider.GetAsync("GEL").Result;
        
        var statementBuilder = new TestStatementBuilder()
            .WithDefaultPeriod()
            .WithTransaction("Test purpose 1; Merchant: PIATTO; Price: 10.13 USD", 10.13m, usd)
            .WithTransaction("Test purpose 2; Merchant: McDonalnd; Price: 5.5 GEL", 5.5m, gel)
            .WithTransaction("Money Transfer; Amount: 12 EUR", 12m, eur);

        return statementBuilder.Build();
    }
}