using QuangThienDung.DataAccess.Models;

namespace QuangThienDung.Business.Services
{
    public interface ISystemAccountService
    {
        Task<IEnumerable<SystemAccount>> GetAllAccountsAsync();
        Task<IEnumerable<SystemAccount>> SearchAccountsAsync(string searchTerm);
        Task<SystemAccount?> GetAccountByIdAsync(short id);
        Task<SystemAccount?> AuthenticateAsync(string email, string password);
        Task<bool> CreateAccountAsync(SystemAccount account);
        Task<bool> UpdateAccountAsync(SystemAccount account);
        Task<bool> DeleteAccountAsync(short id);
        Task<bool> ValidateAccountAsync(SystemAccount account);
        Task<bool> IsEmailUniqueAsync(string email, short? excludeId = null);
    }
}
