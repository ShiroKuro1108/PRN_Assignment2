using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuangThienDung.Business.Services;
using QuangThienDung.DataAccess.Models;

namespace QuangThienDungRazorPages.Pages.Lecturer
{
    [Authorize(Policy = "LecturerOnly")]
    public class NewsModel : PageModel
    {
        private readonly INewsArticleService _newsService;
        private readonly ICategoryService _categoryService;

        public NewsModel(INewsArticleService newsService, ICategoryService categoryService)
        {
            _newsService = newsService;
            _categoryService = categoryService;
        }

        public IList<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
        public IList<Category> Categories { get; set; } = new List<Category>();
        
        public string SearchTerm { get; set; } = string.Empty;
        public string CategoryFilter { get; set; } = string.Empty;
        
        // Statistics
        public int TotalActiveNews { get; set; }
        public int FilteredNewsCount { get; set; }
        public int CategoriesCount { get; set; }
        public int RecentNewsCount { get; set; }

        public async Task OnGetAsync(string? searchTerm, string? categoryId)
        {
            SearchTerm = searchTerm ?? string.Empty;
            CategoryFilter = categoryId ?? string.Empty;

            try
            {
                // Load all active news
                var allActiveNews = await _newsService.GetActiveNewsAsync();
                TotalActiveNews = allActiveNews.Count();

                // Apply filters
                var filteredNews = allActiveNews.AsEnumerable();

                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    filteredNews = filteredNews.Where(n => 
                        (n.NewsTitle?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) == true) ||
                        (n.NewsContent?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) == true) ||
                        (n.Headline?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) == true));
                }

                if (!string.IsNullOrEmpty(CategoryFilter))
                {
                    var categoryIdValue = short.Parse(CategoryFilter);
                    filteredNews = filteredNews.Where(n => n.CategoryID == categoryIdValue);
                }

                // Sort by created date descending
                NewsArticles = filteredNews.OrderByDescending(n => n.CreatedDate).ToList();
                FilteredNewsCount = NewsArticles.Count;

                // Load categories for filter dropdown
                Categories = (await _categoryService.GetActiveCategoriesAsync()).ToList();
                CategoriesCount = Categories.Count;

                // Calculate recent news (this week)
                var thisWeek = DateTime.Now.Date.AddDays(-(int)DateTime.Now.DayOfWeek);
                RecentNewsCount = allActiveNews.Count(n => n.CreatedDate >= thisWeek);
            }
            catch (Exception)
            {
                NewsArticles = new List<NewsArticle>();
                Categories = new List<Category>();
                TotalActiveNews = FilteredNewsCount = CategoriesCount = RecentNewsCount = 0;
            }
        }

        public async Task<IActionResult> OnGetNewsDetailsAsync(string id)
        {
            try
            {
                var newsArticle = await _newsService.GetNewsByIdAsync(id);
                
                if (newsArticle == null || newsArticle.NewsStatus != true)
                {
                    return new JsonResult(new { success = false, message = "News article not found or not active" });
                }

                return new JsonResult(new
                {
                    success = true,
                    news = new
                    {
                        id = newsArticle.NewsArticleID,
                        title = newsArticle.NewsTitle,
                        headline = newsArticle.Headline,
                        content = newsArticle.NewsContent,
                        source = newsArticle.NewsSource,
                        category = newsArticle.Category?.CategoryName,
                        createdDate = newsArticle.CreatedDate?.ToString("MMMM dd, yyyy 'at' HH:mm"),
                        modifiedDate = newsArticle.ModifiedDate?.ToString("MMMM dd, yyyy 'at' HH:mm"),
                        tags = newsArticle.NewsTags.Select(nt => nt.Tag.TagName).ToList(),
                        createdBy = newsArticle.CreatedBy?.AccountName
                    }
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
    }
}
