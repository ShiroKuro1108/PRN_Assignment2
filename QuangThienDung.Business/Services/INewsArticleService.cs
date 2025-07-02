using QuangThienDung.DataAccess.Models;

namespace QuangThienDung.Business.Services
{
    public interface INewsArticleService
    {
        Task<IEnumerable<NewsArticle>> GetAllNewsAsync();
        Task<IEnumerable<NewsArticle>> GetActiveNewsAsync();
        Task<IEnumerable<NewsArticle>> GetNewsByCreatorAsync(short creatorId);
        Task<IEnumerable<NewsArticle>> GetNewsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<NewsArticle>> SearchNewsAsync(string searchTerm);
        Task<NewsArticle?> GetNewsByIdAsync(string id);
        Task<bool> CreateNewsAsync(NewsArticle newsArticle, List<int> tagIds);
        Task<bool> UpdateNewsAsync(NewsArticle newsArticle, List<int> tagIds);
        Task<bool> DeleteNewsAsync(string id);
        Task<bool> ValidateNewsArticleAsync(NewsArticle newsArticle);
        string GenerateNewsId();
    }
}
