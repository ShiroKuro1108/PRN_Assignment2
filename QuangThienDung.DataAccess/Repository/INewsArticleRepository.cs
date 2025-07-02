using QuangThienDung.DataAccess.Models;

namespace QuangThienDung.DataAccess.Repository
{
    public interface INewsArticleRepository : IRepository<NewsArticle>
    {
        Task<IEnumerable<NewsArticle>> GetActiveNewsAsync();
        Task<IEnumerable<NewsArticle>> GetNewsByCreatorAsync(short creatorId);
        Task<IEnumerable<NewsArticle>> GetNewsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<NewsArticle>> SearchNewsAsync(string searchTerm);
        Task UpdateAsync(NewsArticle newsArticle);
    }
}
