namespace QuangThienDung.DataAccess.Repository
{
    public interface IUnitOfWork
    {
        INewsArticleRepository NewsArticle { get; }
        ICategoryRepository Category { get; }
        ISystemAccountRepository SystemAccount { get; }
        ITagRepository Tag { get; }
        Task SaveAsync();
    }
}
