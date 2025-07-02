using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using QuangThienDung.Business.Services;
using QuangThienDung.DataAccess.Models;
using QuangThienDungRazorPages.Hubs;
using System.Security.Claims;

namespace QuangThienDungRazorPages.Pages.Staff
{
    [Authorize(Policy = "StaffOnly")]
    public class NewsModel : PageModel
    {
        private readonly INewsArticleService _newsService;
        private readonly ICategoryService _categoryService;
        private readonly ITagService _tagService;
        private readonly IHubContext<NewsHub> _hubContext;

        public NewsModel(INewsArticleService newsService, ICategoryService categoryService, 
                        ITagService tagService, IHubContext<NewsHub> hubContext)
        {
            _newsService = newsService;
            _categoryService = categoryService;
            _tagService = tagService;
            _hubContext = hubContext;
        }

        public IList<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
        public IList<Category> Categories { get; set; } = new List<Category>();
        public IList<Tag> Tags { get; set; } = new List<Tag>();
        public string SearchTerm { get; set; } = string.Empty;

        public async Task OnGetAsync(string? searchTerm)
        {
            SearchTerm = searchTerm ?? string.Empty;

            try
            {
                // Load news articles
                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    NewsArticles = (await _newsService.SearchNewsAsync(SearchTerm)).ToList();
                }
                else
                {
                    NewsArticles = (await _newsService.GetAllNewsAsync()).ToList();
                }

                // Load categories and tags for dropdowns
                Categories = (await _categoryService.GetActiveCategoriesAsync()).ToList();
                Tags = (await _tagService.GetAllTagsAsync()).ToList();
            }
            catch (Exception)
            {
                NewsArticles = new List<NewsArticle>();
                Categories = new List<Category>();
                Tags = new List<Tag>();
            }
        }

        public async Task<IActionResult> OnPostCreateAsync([FromBody] CreateNewsRequest request)
        {
            try
            {
                var currentUserId = short.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                var newsArticle = new NewsArticle
                {
                    NewsTitle = request.Title,
                    Headline = request.Headline,
                    NewsContent = request.Content,
                    NewsSource = request.Source,
                    CategoryID = request.CategoryId,
                    NewsStatus = request.IsActive,
                    CreatedByID = currentUserId,
                    UpdatedByID = currentUserId
                };

                var success = await _newsService.CreateNewsAsync(newsArticle, request.TagIds);
                if (success)
                {
                    // Send real-time notification
                    await _hubContext.Clients.All.SendAsync("NewsCreated", newsArticle.NewsArticleID, 
                        newsArticle.NewsTitle, User.Identity?.Name);

                    return new JsonResult(new { success = true, message = "News article created successfully" });
                }
                else
                {
                    return new JsonResult(new { success = false, message = "Failed to create news article" });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        public async Task<IActionResult> OnPostUpdateAsync([FromBody] UpdateNewsRequest request)
        {
            try
            {
                var currentUserId = short.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                var newsArticle = await _newsService.GetNewsByIdAsync(request.Id);
                if (newsArticle == null)
                {
                    return new JsonResult(new { success = false, message = "News article not found" });
                }

                newsArticle.NewsTitle = request.Title;
                newsArticle.Headline = request.Headline;
                newsArticle.NewsContent = request.Content;
                newsArticle.NewsSource = request.Source;
                newsArticle.CategoryID = request.CategoryId;
                newsArticle.NewsStatus = request.IsActive;
                newsArticle.UpdatedByID = currentUserId;

                var success = await _newsService.UpdateNewsAsync(newsArticle, request.TagIds);
                if (success)
                {
                    // Send real-time notification
                    await _hubContext.Clients.All.SendAsync("NewsUpdated", newsArticle.NewsArticleID, 
                        newsArticle.NewsTitle, User.Identity?.Name);

                    return new JsonResult(new { success = true, message = "News article updated successfully" });
                }
                else
                {
                    return new JsonResult(new { success = false, message = "Failed to update news article" });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync([FromBody] DeleteNewsRequest request)
        {
            try
            {
                var newsArticle = await _newsService.GetNewsByIdAsync(request.Id);
                if (newsArticle == null)
                {
                    return new JsonResult(new { success = false, message = "News article not found" });
                }

                var title = newsArticle.NewsTitle;
                var success = await _newsService.DeleteNewsAsync(request.Id);
                if (success)
                {
                    // Send real-time notification
                    await _hubContext.Clients.All.SendAsync("NewsDeleted", request.Id, title, User.Identity?.Name);

                    return new JsonResult(new { success = true, message = "News article deleted successfully" });
                }
                else
                {
                    return new JsonResult(new { success = false, message = "Failed to delete news article" });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        public async Task<IActionResult> OnGetNewsAsync(string id)
        {
            try
            {
                var newsArticle = await _newsService.GetNewsByIdAsync(id);
                if (newsArticle == null)
                {
                    return new JsonResult(new { success = false, message = "News article not found" });
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
                        categoryId = newsArticle.CategoryID,
                        isActive = newsArticle.NewsStatus,
                        tagIds = newsArticle.NewsTags.Select(nt => nt.TagID).ToList()
                    }
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        public class CreateNewsRequest
        {
            public string? Title { get; set; }
            public string Headline { get; set; } = string.Empty;
            public string? Content { get; set; }
            public string? Source { get; set; }
            public short? CategoryId { get; set; }
            public bool IsActive { get; set; }
            public List<int> TagIds { get; set; } = new();
        }

        public class UpdateNewsRequest
        {
            public string Id { get; set; } = string.Empty;
            public string? Title { get; set; }
            public string Headline { get; set; } = string.Empty;
            public string? Content { get; set; }
            public string? Source { get; set; }
            public short? CategoryId { get; set; }
            public bool IsActive { get; set; }
            public List<int> TagIds { get; set; } = new();
        }

        public class DeleteNewsRequest
        {
            public string Id { get; set; } = string.Empty;
        }
    }
}
