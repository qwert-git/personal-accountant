using BLL.Config;
using BLL.MerchantExtractors;
using BLL.StatementProcessing;
using FluentAssertions;

namespace BLL.UnitTests.MerchantExtractor;

public class CustomMerchantExtractorTests
{
    [Theory]
    [InlineData("Withdraw from ATM", "Withdraw", "Withdraw")]
    [InlineData("Outgoing Transfer", "Transfer", "Money Transfer")]
    [InlineData("SOCAR Georgia Gas", "Gas", "Utilities")]
    public void CustomMerchantMarkerExists(string transactionPurpose, string merchantMarker, string expectedMerchant)
    {
        var config = new CustomMerchantExtractorConfig
        {
            new(merchantMarker, expectedMerchant)
        };
        
        var customExtractor = new CustomMerchantExtractor(config);

        var res = customExtractor.GetMerchant(transactionPurpose);

        res.Should().Be(expectedMerchant);
    }
    
    [Theory]
    [InlineData("Withdraw from ATM", "Withdraw Money")]
    public void Test(string transactionPurpose, string merchantMarker)
    {
        var config = new CustomMerchantExtractorConfig
        {
            new(merchantMarker, merchantMarker)
        };
        
        var customExtractor = new CustomMerchantExtractor(config);

        var res = customExtractor.GetMerchant(transactionPurpose);

        res.Should().BeNull();
    }
}