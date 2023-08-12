namespace BLL.MerchantExtractors;

public class BogMerchantExtractor : IMerchantExtractor
{
    public string? GetMerchant(string purpose)
    {
        // Purpose="Location;Merchant:MERCHANT_NAME;Transaction amount:AMOUNT;"
        var merchant = purpose.Split(';').FirstOrDefault(x => x.Contains("Merchant"));

        return merchant?.Split(':')[1].Trim();
    }
}