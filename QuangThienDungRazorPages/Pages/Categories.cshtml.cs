using Microsoft.AspNetCore.Mvc.RazorPages;
using QuangThienDung.Business.Services;
using QuangThienDung.DataAccess.Models;

namespace QuangThienDungRazorPages.Pages
{
    public class CategoriesModel : PageModel
    {
        private readonly ICategoryService _categoryService;

        public CategoriesModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public IList<Category> Categories { get; set; } = new List<Category>();

        public async Task OnGetAsync()
        {
            try
            {
                Categories = (await _categoryService.GetAllCategoriesAsync()).ToList();
            }
            catch (Exception)
            {
                // Log the exception or handle it appropriately
                // For now, we'll just leave the list empty
                Categories = new List<Category>();
            }
        }
    }
}
