using BLL.Config;

namespace BLL.StatementProcessing;

public class CategoryMapper : ICategoryMapper
{
    private const string NotFoundCategory = "Not found";
    private readonly List<CategoryMap> _mappings;

    public CategoryMapper(CategoryMapperConfig categoryMapperConfig)
    {
        _mappings = categoryMapperConfig
            .SelectMany(pair => pair.Value.Select(value => new CategoryMap(value, pair.Key)))
            .ToList();
    }

    public string GetCategory(string? merchant)
    {
        if (string.IsNullOrWhiteSpace(merchant))
            return NotFoundCategory;

        var res = _mappings
            .FirstOrDefault(map => IsMerchantInCategory(merchant, map.CategoryMarker));
        return res?.CategoryName ?? NotFoundCategory;
    }

    private static bool IsMerchantInCategory(string merchant, string categoryMark)
    {
        return merchant.Contains(categoryMark, StringComparison.OrdinalIgnoreCase);
    }
    
    private record CategoryMap(string CategoryMarker, string CategoryName);
}