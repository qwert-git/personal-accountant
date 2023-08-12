namespace BLL.Config;

public class AppConfig
{
    public bool IsDevelopment { get; set; }

    public CategoryMapperConfig CategoryMapping { get; set; }
    
    public CurrencyProviderConfig CurrencyProvider { get; set; }
    
    public CustomMerchantExtractorConfig CustomMerchantMapping { get; set; }
}