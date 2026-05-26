using CodePulse.API.Data;
using CodePulse.API.Models.Domain;
using CodePulse.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CodePulse.API.Repositories.Implementation
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext dbContext;

        public CategoryRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Category> CreateCategory(Category category)
        {
            await dbContext.Categories.AddAsync(category);
            await dbContext.SaveChangesAsync();

            return category;
        }

        public async Task<IEnumerable<Category>> GetAllCategories(string? query = null, string? sortBy = null, string? sortDirection = null, int? pageNumber = 1, int? pageSize = 100)
        {
            var categories = dbContext.Categories.AsQueryable();

            //Filtering
            if (string.IsNullOrWhiteSpace(query) == false)
            {
                categories = categories.Where(x => x.Name.Contains(query));
            }

            //Sorting
            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if (string.Equals(sortBy, "Name", StringComparison.OrdinalIgnoreCase))
                {
                    var isAsc = string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase) ? true : false;

                    categories = isAsc
                        ? categories.OrderBy(x => x.Name)
                        : categories.OrderByDescending(x => x.Name);
                }

                if (string.Equals(sortBy, "UrlHandle", StringComparison.OrdinalIgnoreCase))
                {
                    var isAsc = string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase) ? true : false;

                    categories = isAsc
                        ? categories.OrderBy(x => x.UrlHandle)
                        : categories.OrderByDescending(x => x.UrlHandle);
                }
            }

            var skipResults = (pageNumber - 1) * pageSize;
            categories = categories
                .Skip(skipResults ?? 0)
                .Take(pageSize ?? 100);

            return await categories.ToListAsync();
        }

        public async Task<Category?> GetCategoryById(Guid id)
        {
            return await dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Category?> EditCategory(Category category)
        {
            var existingCategory = await dbContext.Categories.FirstOrDefaultAsync(c => c.Id == category.Id);
            if (existingCategory != null)
            {
                dbContext.Entry(existingCategory).CurrentValues.SetValues(category);
                await dbContext.SaveChangesAsync();
                return existingCategory;
            }
            return null;
        }

        public async Task<Category?> DeleteCategory(Guid id)
        {
            var existingCategory = await dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (existingCategory is null)
            {
                return null;
            }
            dbContext.Categories.Remove(existingCategory);
            await dbContext.SaveChangesAsync();
            return existingCategory;
        }

        public async Task<int> GetCategoryCount()
        {
            return await dbContext.Categories.CountAsync();
        }
    }
}
