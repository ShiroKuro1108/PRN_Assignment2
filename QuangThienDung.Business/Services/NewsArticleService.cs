using QuangThienDung.DataAccess.Models;
using QuangThienDung.DataAccess.Repository;

namespace QuangThienDung.Business.Services
{
    public class NewsArticleService : INewsArticleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NewsArticleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CreateNewsAsync(NewsArticle newsArticle, List<int> tagIds)
        {
            try
            {
                if (!await ValidateNewsArticleAsync(newsArticle))
                    return false;

                newsArticle.NewsArticleID = GenerateNewsId();
                newsArticle.CreatedDate = DateTime.Now;
                newsArticle.ModifiedDate = DateTime.Now;

                await _unitOfWork.NewsArticle.AddAsync(newsArticle);

                // Add tags
                foreach (var tagId in tagIds)
                {
                    var newsTag = new NewsTag
                    {
                        NewsArticleID = newsArticle.NewsArticleID,
                        TagID = tagId
                    };
                    newsArticle.NewsTags.Add(newsTag);
                }

                await _unitOfWork.SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteNewsAsync(string id)
        {
            try
            {
                var newsArticle = await _unitOfWork.NewsArticle.GetAsync(n => n.NewsArticleID == id);
                if (newsArticle == null)
                    return false;

                _unitOfWork.NewsArticle.Remove(newsArticle);
                await _unitOfWork.SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GenerateNewsId()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        public async Task<IEnumerable<NewsArticle>> GetActiveNewsAsync()
        {
            return await _unitOfWork.NewsArticle.GetActiveNewsAsync();
        }

        public async Task<IEnumerable<NewsArticle>> GetAllNewsAsync()
        {
            return await _unitOfWork.NewsArticle.GetAllAsync(includeProperties: "Category,CreatedBy,UpdatedBy,NewsTags.Tag");
        }

        public async Task<IEnumerable<NewsArticle>> GetNewsByCreatorAsync(short creatorId)
        {
            return await _unitOfWork.NewsArticle.GetNewsByCreatorAsync(creatorId);
        }

        public async Task<IEnumerable<NewsArticle>> GetNewsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _unitOfWork.NewsArticle.GetNewsByDateRangeAsync(startDate, endDate);
        }

        public async Task<NewsArticle?> GetNewsByIdAsync(string id)
        {
            return await _unitOfWork.NewsArticle.GetAsync(n => n.NewsArticleID == id, 
                "Category,CreatedBy,UpdatedBy,NewsTags.Tag");
        }

        public async Task<IEnumerable<NewsArticle>> SearchNewsAsync(string searchTerm)
        {
            return await _unitOfWork.NewsArticle.SearchNewsAsync(searchTerm);
        }

        public async Task<bool> UpdateNewsAsync(NewsArticle newsArticle, List<int> tagIds)
        {
            try
            {
                if (!await ValidateNewsArticleAsync(newsArticle))
                    return false;

                var existingNews = await _unitOfWork.NewsArticle.GetAsync(n => n.NewsArticleID == newsArticle.NewsArticleID, 
                    "NewsTags");
                
                if (existingNews == null)
                    return false;

                // Update properties
                existingNews.NewsTitle = newsArticle.NewsTitle;
                existingNews.Headline = newsArticle.Headline;
                existingNews.NewsContent = newsArticle.NewsContent;
                existingNews.NewsSource = newsArticle.NewsSource;
                existingNews.CategoryID = newsArticle.CategoryID;
                existingNews.NewsStatus = newsArticle.NewsStatus;
                existingNews.UpdatedByID = newsArticle.UpdatedByID;
                existingNews.ModifiedDate = DateTime.Now;

                // Update tags - remove existing and add new ones
                existingNews.NewsTags.Clear();
                foreach (var tagId in tagIds)
                {
                    existingNews.NewsTags.Add(new NewsTag
                    {
                        NewsArticleID = existingNews.NewsArticleID,
                        TagID = tagId
                    });
                }

                _unitOfWork.NewsArticle.Update(existingNews);
                await _unitOfWork.SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ValidateNewsArticleAsync(NewsArticle newsArticle)
        {
            if (string.IsNullOrWhiteSpace(newsArticle.Headline))
                return false;

            if (newsArticle.CategoryID.HasValue)
            {
                var categoryExists = await _unitOfWork.Category.AnyAsync(c => c.CategoryID == newsArticle.CategoryID);
                if (!categoryExists)
                    return false;
            }

            if (newsArticle.CreatedByID.HasValue)
            {
                var creatorExists = await _unitOfWork.SystemAccount.AnyAsync(a => a.AccountID == newsArticle.CreatedByID);
                if (!creatorExists)
                    return false;
            }

            return true;
        }
    }
}
