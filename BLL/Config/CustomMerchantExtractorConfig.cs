namespace BLL.Config;

public record CustomMerchantMap(string MerchantMarker, string Merchant);

public class CustomMerchantExtractorConfig : List<CustomMerchantMap> { }