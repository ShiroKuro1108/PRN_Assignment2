using QuangThienDung.DataAccess.Models;
using QuangThienDung.DataAccess.Repository;

namespace QuangThienDung.Business.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CanDeleteCategoryAsync(short id)
        {
            return !await _unitOfWork.Category.HasNewsArticlesAsync(id);
        }

        public async Task<bool> CreateCategoryAsync(Category category)
        {
            try
            {
                if (!await ValidateCategoryAsync(category))
                    return false;

                await _unitOfWork.Category.AddAsync(category);
                await _unitOfWork.SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteCategoryAsync(short id)
        {
            try
            {
                if (!await CanDeleteCategoryAsync(id))
                    return false;

                var category = await _unitOfWork.Category.GetAsync(c => c.CategoryID == id);
                if (category == null)
                    return false;

                _unitOfWork.Category.Remove(category);
                await _unitOfWork.SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
        {
            return await _unitOfWork.Category.GetActiveCategoriesAsync();
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _unitOfWork.Category.GetAllAsync(includeProperties: "ParentCategory");
        }

        public async Task<Category?> GetCategoryByIdAsync(short id)
        {
            return await _unitOfWork.Category.GetAsync(c => c.CategoryID == id, "ParentCategory,SubCategories");
        }

        public async Task<IEnumerable<Category>> SearchCategoriesAsync(string searchTerm)
        {
            return await _unitOfWork.Category.SearchCategoriesAsync(searchTerm);
        }

        public async Task<bool> UpdateCategoryAsync(Category category)
        {
            try
            {
                if (!await ValidateCategoryAsync(category))
                    return false;

                _unitOfWork.Category.Update(category);
                await _unitOfWork.SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ValidateCategoryAsync(Category category)
        {
            if (string.IsNullOrWhiteSpace(category.CategoryName))
                return false;

            if (string.IsNullOrWhiteSpace(category.CategoryDesciption))
                return false;

            // Check if parent category exists (if specified)
            if (category.ParentCategoryID.HasValue)
            {
                var parentExists = await _unitOfWork.Category.AnyAsync(c => c.CategoryID == category.ParentCategoryID);
                if (!parentExists)
                    return false;

                // Prevent circular reference
                if (category.ParentCategoryID == category.CategoryID)
                    return false;
            }

            return true;
        }
    }
}
