using BLL.MerchantExtractors;
using BLL.StatementProcessing;
using FluentAssertions;

namespace BLL.UnitTests.MerchantExtractor;

public class MerchantExtractorTests
{
    [Theory]
    [InlineData("Payment - Amount: GEL40.70; Merchant: PIATTO, Batumi, abashidze/vaja-fshavela 52/18; MCC:5812; Date: 04/06/2023 15:22; Card No: ****1781; Payment transaction amount and currency: 40.70 GEL", 
        "PIATTO, Batumi, abashidze/vaja-fshavela 52/18")]
    [InlineData("Payment - Amount: GEL24.15; Merchant: McDonald's, Batumi, KHIMSHIASHVILI ST.; MCC:5814; Date: 29/05/2023 01:12; Card No: ****1781; Payment transaction amount and currency: 24.15 GEL",
        "McDonald's, Batumi, KHIMSHIASHVILI ST.")]
    public void MerchantFound(string purpose, string expected)
    {
        var extractor = new BogMerchantExtractor();
        
        var result = extractor.GetMerchant(purpose);
        
        result.Should().Be(expected);
    }
    
    [Theory]
    [InlineData("Set S Maintenance Fee.Acct #")]
    [InlineData("Automatic conversion, rate: 2.709")]
    public void NoMerchant(string purpose)
    {
        var extractor = new BogMerchantExtractor();
        
        var result = extractor.GetMerchant(purpose);
        
        result.Should().BeNull();
    }
}