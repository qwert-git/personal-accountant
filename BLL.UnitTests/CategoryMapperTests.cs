using BLL.Config;
using BLL.StatementProcessing;
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
        
        var mapper = new CategoryMapper(categoryMapperConfig);
        
        var result = mapper.GetCategory("PIATTO, Batumi, abashidze/vaja-fshavela 52/18");
        
        result.Should().Be("Restaurants");
    }
    
    [Fact]
    public void CategoryNotFound()
    {
        var emptyConfig = new CategoryMapperConfig();
        
        var mapper = new CategoryMapper(emptyConfig);
        
        var result = mapper.GetCategory("PIATTO, Batumi, abashidze/vaja-fshavela 52/18");
        
        result.Should().Be("Not found");
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void EmptyOrNullMerchant(string merchant)
    {
        var emptyConfig = new CategoryMapperConfig();
        
        var mapper = new CategoryMapper(emptyConfig);
        
        var result = mapper.GetCategory(merchant);
        
        result.Should().Be("Not found");
    }
}