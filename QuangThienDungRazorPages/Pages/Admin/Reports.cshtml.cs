using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuangThienDung.Business.Services;
using QuangThienDung.DataAccess.Models;
using System.Text;

namespace QuangThienDungRazorPages.Pages.Admin
{
    [Authorize(Policy = "AdminOnly")]
    public class ReportsModel : PageModel
    {
        private readonly INewsArticleService _newsService;

        public ReportsModel(INewsArticleService newsService)
        {
            _newsService = newsService;
        }

        public IList<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        
        // Statistics
        public int TotalNews { get; set; }
        public int ActiveNews { get; set; }
        public int InactiveNews { get; set; }
        public int UniqueAuthors { get; set; }

        public async Task OnGetAsync(DateTime? startDate, DateTime? endDate)
        {
            StartDate = startDate;
            EndDate = endDate;

            try
            {
                IEnumerable<NewsArticle> allNews;

                if (StartDate.HasValue && EndDate.HasValue)
                {
                    // Ensure end date includes the full day
                    var adjustedEndDate = EndDate.Value.Date.AddDays(1).AddTicks(-1);
                    allNews = await _newsService.GetNewsByDateRangeAsync(StartDate.Value, adjustedEndDate);
                }
                else if (StartDate.HasValue)
                {
                    var adjustedEndDate = DateTime.Now.Date.AddDays(1).AddTicks(-1);
                    allNews = await _newsService.GetNewsByDateRangeAsync(StartDate.Value, adjustedEndDate);
                }
                else if (EndDate.HasValue)
                {
                    var adjustedEndDate = EndDate.Value.Date.AddDays(1).AddTicks(-1);
                    allNews = await _newsService.GetNewsByDateRangeAsync(DateTime.MinValue, adjustedEndDate);
                }
                else
                {
                    allNews = await _newsService.GetAllNewsAsync();
                }

                // Sort by created date descending as required
                NewsArticles = allNews.OrderByDescending(n => n.CreatedDate).ToList();

                // Calculate statistics
                TotalNews = NewsArticles.Count;
                ActiveNews = NewsArticles.Count(n => n.NewsStatus == true);
                InactiveNews = NewsArticles.Count(n => n.NewsStatus != true);
                UniqueAuthors = NewsArticles.Where(n => n.CreatedByID.HasValue)
                                          .Select(n => n.CreatedByID.Value)
                                          .Distinct()
                                          .Count();
            }
            catch (Exception)
            {
                NewsArticles = new List<NewsArticle>();
                TotalNews = ActiveNews = InactiveNews = UniqueAuthors = 0;
            }
        }

        public async Task<IActionResult> OnGetExportCsvAsync(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                // Get the same data as the main page
                await OnGetAsync(startDate, endDate);

                var csv = new StringBuilder();
                
                // Add header
                csv.AppendLine("ID,Title,Headline,Category,Author,Created Date,Status,Tags,Content");

                // Add data rows
                foreach (var news in NewsArticles)
                {
                    var tags = string.Join("; ", news.NewsTags.Select(nt => nt.Tag.TagName));
                    var content = news.NewsContent?.Replace("\"", "\"\"").Replace("\n", " ").Replace("\r", " ");
                    
                    csv.AppendLine($"\"{news.NewsArticleID}\"," +
                                  $"\"{news.NewsTitle?.Replace("\"", "\"\"")}\"," +
                                  $"\"{news.Headline?.Replace("\"", "\"\"")}\"," +
                                  $"\"{news.Category?.CategoryName?.Replace("\"", "\"\"")}\"," +
                                  $"\"{news.CreatedBy?.AccountName?.Replace("\"", "\"\"")}\"," +
                                  $"\"{news.CreatedDate?.ToString("yyyy-MM-dd HH:mm:ss")}\"," +
                                  $"\"{(news.NewsStatus == true ? "Active" : "Inactive")}\"," +
                                  $"\"{tags}\"," +
                                  $"\"{content}\"");
                }

                var fileName = $"NewsReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                var bytes = Encoding.UTF8.GetBytes(csv.ToString());

                return File(bytes, "text/csv", fileName);
            }
            catch (Exception)
            {
                TempData["Error"] = "Failed to export data to CSV.";
                return RedirectToPage();
            }
        }
    }
}
