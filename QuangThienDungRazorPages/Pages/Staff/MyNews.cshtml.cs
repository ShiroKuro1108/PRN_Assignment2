using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuangThienDung.Business.Services;
using QuangThienDung.DataAccess.Models;
using System.Security.Claims;

namespace QuangThienDungRazorPages.Pages.Staff
{
    [Authorize(Policy = "StaffOnly")]
    public class MyNewsModel : PageModel
    {
        private readonly INewsArticleService _newsService;
        private readonly ICategoryService _categoryService;

        public MyNewsModel(INewsArticleService newsService, ICategoryService categoryService)
        {
            _newsService = newsService;
            _categoryService = categoryService;
        }

        public IList<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
        public IList<Category> Categories { get; set; } = new List<Category>();
        
        // Filter properties
        public string SearchTerm { get; set; } = string.Empty;
        public string StatusFilter { get; set; } = string.Empty;
        public string CategoryFilter { get; set; } = string.Empty;
        
        // Statistics
        public int TotalNews { get; set; }
        public int ActiveNews { get; set; }
        public int InactiveNews { get; set; }
        public int RecentNews { get; set; }

        public async Task OnGetAsync(string? searchTerm, string? status, string? category)
        {
            SearchTerm = searchTerm ?? string.Empty;
            StatusFilter = status ?? string.Empty;
            CategoryFilter = category ?? string.Empty;

            var currentUserId = short.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            try
            {
                // Load all news by current user
                var allUserNews = await _newsService.GetNewsByCreatorAsync(currentUserId);
                
                // Apply filters
                var filteredNews = allUserNews.AsEnumerable();

                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    filteredNews = filteredNews.Where(n => 
                        (n.NewsTitle?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) == true) ||
                        (n.NewsContent?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) == true) ||
                        (n.Headline?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) == true));
                }

                if (!string.IsNullOrEmpty(StatusFilter))
                {
                    var isActive = bool.Parse(StatusFilter);
                    filteredNews = filteredNews.Where(n => n.NewsStatus == isActive);
                }

                if (!string.IsNullOrEmpty(CategoryFilter))
                {
                    var categoryId = short.Parse(CategoryFilter);
                    filteredNews = filteredNews.Where(n => n.CategoryID == categoryId);
                }

                // Sort by created date descending
                NewsArticles = filteredNews.OrderByDescending(n => n.CreatedDate).ToList();

                // Load categories for filter dropdown
                Categories = (await _categoryService.GetActiveCategoriesAsync()).ToList();

                // Calculate statistics
                CalculateStatistics(allUserNews);
            }
            catch (Exception)
            {
                NewsArticles = new List<NewsArticle>();
                Categories = new List<Category>();
                TotalNews = ActiveNews = InactiveNews = RecentNews = 0;
            }
        }

        public async Task<IActionResult> OnGetNewsDetailsAsync(string id)
        {
            try
            {
                var currentUserId = short.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var newsArticle = await _newsService.GetNewsByIdAsync(id);
                
                if (newsArticle == null || newsArticle.CreatedByID != currentUserId)
                {
                    return new JsonResult(new { success = false, message = "News article not found or access denied" });
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
                        status = newsArticle.NewsStatus == true ? "Active" : "Inactive",
                        createdDate = newsArticle.CreatedDate?.ToString("MMMM dd, yyyy 'at' HH:mm"),
                        modifiedDate = newsArticle.ModifiedDate?.ToString("MMMM dd, yyyy 'at' HH:mm"),
                        tags = newsArticle.NewsTags.Select(nt => nt.Tag.TagName).ToList(),
                        createdBy = newsArticle.CreatedBy?.AccountName,
                        updatedBy = newsArticle.UpdatedBy?.AccountName
                    }
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        private void CalculateStatistics(IEnumerable<NewsArticle> allNews)
        {
            TotalNews = allNews.Count();
            ActiveNews = allNews.Count(n => n.NewsStatus == true);
            InactiveNews = allNews.Count(n => n.NewsStatus != true);
            
            // Recent news (this month)
            var thisMonth = DateTime.Now.Date.AddDays(-DateTime.Now.Day + 1); // First day of current month
            RecentNews = allNews.Count(n => n.CreatedDate >= thisMonth);
        }
    }
}
