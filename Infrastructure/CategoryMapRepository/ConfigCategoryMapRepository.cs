using BLL.CategoryMapRepository;
using BLL.Config;

namespace Infrastructure.CategoryMapRepository;

internal class ConfigCategoryMapRepository : ICategoryMapRepository
{
    private readonly CategoryMapperConfig _config;

    public ConfigCategoryMapRepository(CategoryMapperConfig config)
    {
        _config = config;
    }

    public IReadOnlyCollection<CategoryMap> GetAll()
    {
        return _config
            .SelectMany(pair => pair.Value.Select(value => new CategoryMap(value, pair.Key)))
            .ToList();
    }

    public Task<IReadOnlyCollection<CategoryMap>> GetAllAsync()
    {
        return Task.FromResult(GetAll());
    }
}