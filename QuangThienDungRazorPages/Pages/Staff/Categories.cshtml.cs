using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuangThienDung.Business.Services;
using QuangThienDung.DataAccess.Models;

namespace QuangThienDungRazorPages.Pages.Staff
{
    [Authorize(Policy = "StaffOnly")]
    public class CategoriesModel : PageModel
    {
        private readonly ICategoryService _categoryService;

        public CategoriesModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public IList<Category> Categories { get; set; } = new List<Category>();
        public string SearchTerm { get; set; } = string.Empty;

        public async Task OnGetAsync(string? searchTerm)
        {
            SearchTerm = searchTerm ?? string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    Categories = (await _categoryService.SearchCategoriesAsync(SearchTerm)).ToList();
                }
                else
                {
                    Categories = (await _categoryService.GetAllCategoriesAsync()).ToList();
                }
            }
            catch (Exception)
            {
                Categories = new List<Category>();
            }
        }

        public async Task<IActionResult> OnPostCreateAsync([FromBody] CreateCategoryRequest request)
        {
            try
            {
                var category = new Category
                {
                    CategoryName = request.Name,
                    CategoryDesciption = request.Description,
                    ParentCategoryID = request.ParentCategoryId,
                    IsActive = request.IsActive
                };

                var success = await _categoryService.CreateCategoryAsync(category);
                if (success)
                {
                    return new JsonResult(new { success = true, message = "Category created successfully" });
                }
                else
                {
                    return new JsonResult(new { success = false, message = "Failed to create category. Please check the data." });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        public async Task<IActionResult> OnPostUpdateAsync([FromBody] UpdateCategoryRequest request)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(request.Id);
                if (category == null)
                {
                    return new JsonResult(new { success = false, message = "Category not found" });
                }

                category.CategoryName = request.Name;
                category.CategoryDesciption = request.Description;
                category.ParentCategoryID = request.ParentCategoryId;
                category.IsActive = request.IsActive;

                var success = await _categoryService.UpdateCategoryAsync(category);
                if (success)
                {
                    return new JsonResult(new { success = true, message = "Category updated successfully" });
                }
                else
                {
                    return new JsonResult(new { success = false, message = "Failed to update category" });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync([FromBody] DeleteCategoryRequest request)
        {
            try
            {
                // Check if category can be deleted (no news articles)
                var canDelete = await _categoryService.CanDeleteCategoryAsync(request.Id);
                if (!canDelete)
                {
                    return new JsonResult(new { success = false, message = "Cannot delete category. It has associated news articles." });
                }

                var success = await _categoryService.DeleteCategoryAsync(request.Id);
                if (success)
                {
                    return new JsonResult(new { success = true, message = "Category deleted successfully" });
                }
                else
                {
                    return new JsonResult(new { success = false, message = "Failed to delete category" });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        public async Task<IActionResult> OnGetCategoryAsync(short id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                if (category == null)
                {
                    return new JsonResult(new { success = false, message = "Category not found" });
                }

                return new JsonResult(new
                {
                    success = true,
                    category = new
                    {
                        id = category.CategoryID,
                        name = category.CategoryName,
                        description = category.CategoryDesciption,
                        parentCategoryId = category.ParentCategoryID,
                        isActive = category.IsActive
                    }
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        public class CreateCategoryRequest
        {
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public short? ParentCategoryId { get; set; }
            public bool IsActive { get; set; }
        }

        public class UpdateCategoryRequest
        {
            public short Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public short? ParentCategoryId { get; set; }
            public bool IsActive { get; set; }
        }

        public class DeleteCategoryRequest
        {
            public short Id { get; set; }
        }
    }
}
