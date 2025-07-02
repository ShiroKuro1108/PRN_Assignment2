using QuangThienDung.DataAccess.Models;

namespace QuangThienDung.DataAccess.Repository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<Category>> GetActiveCategoriesAsync();
        Task<bool> HasNewsArticlesAsync(short categoryId);
        Task<IEnumerable<Category>> SearchCategoriesAsync(string searchTerm);
        Task UpdateAsync(Category category);
    }
}
