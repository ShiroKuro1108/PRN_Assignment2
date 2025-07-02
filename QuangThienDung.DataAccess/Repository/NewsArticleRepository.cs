using Microsoft.EntityFrameworkCore;
using QuangThienDung.DataAccess.Data;
using QuangThienDung.DataAccess.Models;

namespace QuangThienDung.DataAccess.Repository
{
    public class NewsArticleRepository : Repository<NewsArticle>, INewsArticleRepository
    {
        private readonly FUNewsManagementContext _context;

        public NewsArticleRepository(FUNewsManagementContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NewsArticle>> GetActiveNewsAsync()
        {
            return await _context.NewsArticles
                .Where(n => n.NewsStatus == NewsStatus.Active)
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<NewsArticle>> GetNewsByCreatorAsync(short creatorId)
        {
            return await _context.NewsArticles
                .Where(n => n.CreatedByID == creatorId)
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<NewsArticle>> GetNewsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.NewsArticles
                .Where(n => n.CreatedDate >= startDate && n.CreatedDate <= endDate)
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<NewsArticle>> SearchNewsAsync(string searchTerm)
        {
            return await _context.NewsArticles
                .Where(n => n.NewsTitle!.Contains(searchTerm) || 
                           n.Headline.Contains(searchTerm) || 
                           n.NewsContent!.Contains(searchTerm))
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task UpdateAsync(NewsArticle newsArticle)
        {
            _context.NewsArticles.Update(newsArticle);
            await _context.SaveChangesAsync();
        }
    }
}
