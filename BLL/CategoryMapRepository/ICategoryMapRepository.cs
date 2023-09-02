namespace BLL.CategoryMapRepository;

public interface ICategoryMapRepository
{
    IReadOnlyCollection<CategoryMap> GetAll();

    Task<IReadOnlyCollection<CategoryMap>> GetAllAsync();
}

public record CategoryMap(string MerchantMarker, string CategoryName);