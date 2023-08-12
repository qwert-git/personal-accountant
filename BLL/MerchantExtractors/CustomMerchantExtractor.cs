using BLL.Config;

namespace BLL.MerchantExtractors;

/// <summary>
/// Extract merchant for transaction that don't have merchant in purpose.
/// </summary>
public class CustomMerchantExtractor : IMerchantExtractor
{
    private readonly CustomMerchantExtractorConfig _config;

    public CustomMerchantExtractor(CustomMerchantExtractorConfig config)
    {
        _config = config;
    }

    public string? GetMerchant(string purpose)
    {
        var res = _config.FirstOrDefault(pair => purpose.Contains(pair.MerchantMarker));
        return res?.Merchant;
    }
}