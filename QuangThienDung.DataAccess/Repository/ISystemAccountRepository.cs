using QuangThienDung.DataAccess.Models;

namespace QuangThienDung.DataAccess.Repository
{
    public interface ISystemAccountRepository : IRepository<SystemAccount>
    {
        Task<SystemAccount?> GetByEmailAsync(string email);
        Task<SystemAccount?> AuthenticateAsync(string email, string password);
        Task<IEnumerable<SystemAccount>> SearchAccountsAsync(string searchTerm);
        Task UpdateAsync(SystemAccount account);
    }
}
