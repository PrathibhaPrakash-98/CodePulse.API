using CodePulse.API.Models.Domain;

namespace CodePulse.API.Repositories.Interface
{
    public interface ICategoryRepository
    {
        Task<Category> CreateCategory(Category category);

        Task<IEnumerable<Category>> GetAllCategories(string? query = null, string? sortBy = null, string? sortDirection = null, int? pageNumber = 1, int? pageSize = 100);

        Task<Category?> GetCategoryById(Guid id);

        Task<Category?> EditCategory(Category category);

        Task<Category?> DeleteCategory(Guid id);

        Task<int> GetCategoryCount();
    }
}
