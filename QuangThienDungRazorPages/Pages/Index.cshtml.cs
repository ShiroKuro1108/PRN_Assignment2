using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuangThienDung.Business.Services;
using QuangThienDung.DataAccess.Models;

namespace QuangThienDungRazorPages.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly INewsArticleService _newsService;

        public IndexModel(ILogger<IndexModel> logger, INewsArticleService newsService)
        {
            _logger = logger;
            _newsService = newsService;
        }

        public IList<NewsArticle> ActiveNews { get; set; } = new List<NewsArticle>();
        public string UserRole { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return RedirectToPage("/Login");
            }

            UserRole = User.FindFirst("Role")?.Value ?? "";
            UserName = User.Identity?.Name ?? "";

            // Load active news for all users
            try
            {
                ActiveNews = (await _newsService.GetActiveNewsAsync()).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading active news");
                ActiveNews = new List<NewsArticle>();
            }

            return Page();
        }
    }
}
