using QuangThienDung.DataAccess.Data;

namespace QuangThienDung.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FUNewsManagementContext _context;
        public INewsArticleRepository NewsArticle { get; private set; }
        public ICategoryRepository Category { get; private set; }
        public ISystemAccountRepository SystemAccount { get; private set; }
        public ITagRepository Tag { get; private set; }

        public UnitOfWork(FUNewsManagementContext context)
        {
            _context = context;
            NewsArticle = new NewsArticleRepository(_context);
            Category = new CategoryRepository(_context);
            SystemAccount = new SystemAccountRepository(_context);
            Tag = new TagRepository(_context);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
