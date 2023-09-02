using BLL.Config;
using BLL.StatementProcessing;
using BLL.UnitTests.Builders;
using FluentAssertions;

namespace BLL.UnitTests;

public class CategoryMapperTests
{
    [Fact]
    public void CategoryFound()
    {
        var categoryMapperConfig = new CategoryMapperConfig
        {
            { 
                "Restaurants", 
                new [] 
                {
                    "PIATTO",
                    "McDonald's"
                }
            }
        };

        var categoryMapRepository = CategoryMapRepositoryBuilder.WithConfig(categoryMapperConfig);
        var mapper = new CategoryMapper(categoryMapRepository);
        
        var result = mapper.GetCategory("PIATTO, Batumi, abashidze/vaja-fshavela 52/18");
        
        result.Should().Be("Restaurants");
    }

    [Fact]
    public void CategoryNotFound()
    {
        var categoryMapRepository = CategoryMapRepositoryBuilder.WithEmptyConfig();
        var mapper = new CategoryMapper(categoryMapRepository);
        
        var result = mapper.GetCategory("PIATTO, Batumi, abashidze/vaja-fshavela 52/18");
        
        result.Should().Be("Not found");
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void EmptyOrNullMerchant(string merchant)
    {
        var categoryMapRepository = CategoryMapRepositoryBuilder.WithEmptyConfig();
        var mapper = new CategoryMapper(categoryMapRepository);
        
        var result = mapper.GetCategory(merchant);
        
        result.Should().Be("Not found");
    }
}