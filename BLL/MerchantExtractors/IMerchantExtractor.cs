namespace BLL.MerchantExtractors;

public interface IMerchantExtractor
{
    string? GetMerchant(string purpose);
}