using BLL.CategoryMapRepository;

namespace BLL.StatementProcessing;

public class CategoryMapper : ICategoryMapper
{
    private const string NotFoundCategory = "Not found";
    private readonly IReadOnlyCollection<CategoryMap> _mappings;

    public CategoryMapper(ICategoryMapRepository categoryMapRepository)
    {
        _mappings = categoryMapRepository.GetAll();
    }

    public string GetCategory(string? merchant)
    {
        if (string.IsNullOrWhiteSpace(merchant))
            return NotFoundCategory;

        var res = _mappings
            .FirstOrDefault(map => IsMerchantInCategory(merchant, map.MerchantMarker));
        return res?.CategoryName ?? NotFoundCategory;
    }

    private static bool IsMerchantInCategory(string merchant, string categoryMark)
    {
        return merchant.Contains(categoryMark, StringComparison.OrdinalIgnoreCase);
    }
}