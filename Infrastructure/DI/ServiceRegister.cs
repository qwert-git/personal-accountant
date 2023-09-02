using BLL.CategoryMapRepository;
using BLL.Config;
using Infrastructure.CategoryMapRepository;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DI;

public static class ServiceRegister
{
    public static IServiceCollection AddCategoryMapRepository(this IServiceCollection services, AppConfig config)
    {
        return services.AddSingleton<ICategoryMapRepository>(_ => new ConfigCategoryMapRepository(config.CategoryMapping));   
    }
}