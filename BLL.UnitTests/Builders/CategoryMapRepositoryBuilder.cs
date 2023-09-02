using BLL.CategoryMapRepository;
using BLL.Config;
using Moq;

namespace BLL.UnitTests.Builders;

internal static class CategoryMapRepositoryBuilder
{
    public static ICategoryMapRepository WithConfig(CategoryMapperConfig categoryMapperConfig)
    {
        return Mock.Of<ICategoryMapRepository>(repo =>
            repo.GetAllAsync() == Task.FromResult<IReadOnlyCollection<CategoryMap>>(categoryMapperConfig
                .SelectMany(pair => pair.Value.Select(value => new CategoryMap(value, pair.Key)))
                .ToList()));
    }
    
    public static ICategoryMapRepository WithEmptyConfig()
    {
        return WithConfig(new CategoryMapperConfig());
    }
}