using QuangThienDung.DataAccess.Models;

namespace QuangThienDung.Business.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<IEnumerable<Category>> GetActiveCategoriesAsync();
        Task<IEnumerable<Category>> SearchCategoriesAsync(string searchTerm);
        Task<Category?> GetCategoryByIdAsync(short id);
        Task<bool> CreateCategoryAsync(Category category);
        Task<bool> UpdateCategoryAsync(Category category);
        Task<bool> DeleteCategoryAsync(short id);
        Task<bool> ValidateCategoryAsync(Category category);
        Task<bool> CanDeleteCategoryAsync(short id);
    }
}
