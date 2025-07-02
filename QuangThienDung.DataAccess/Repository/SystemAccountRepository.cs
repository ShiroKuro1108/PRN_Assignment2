using Microsoft.EntityFrameworkCore;
using QuangThienDung.DataAccess.Data;
using QuangThienDung.DataAccess.Models;

namespace QuangThienDung.DataAccess.Repository
{
    public class SystemAccountRepository : Repository<SystemAccount>, ISystemAccountRepository
    {
        private readonly FUNewsManagementContext _context;

        public SystemAccountRepository(FUNewsManagementContext context) : base(context)
        {
            _context = context;
        }

        public async Task<SystemAccount?> AuthenticateAsync(string email, string password)
        {
            return await _context.SystemAccounts
                .FirstOrDefaultAsync(a => a.AccountEmail == email && a.AccountPassword == password);
        }

        public async Task<SystemAccount?> GetByEmailAsync(string email)
        {
            return await _context.SystemAccounts
                .FirstOrDefaultAsync(a => a.AccountEmail == email);
        }

        public async Task<IEnumerable<SystemAccount>> SearchAccountsAsync(string searchTerm)
        {
            return await _context.SystemAccounts
                .Where(a => a.AccountName!.Contains(searchTerm) || 
                           a.AccountEmail!.Contains(searchTerm))
                .OrderBy(a => a.AccountName)
                .ToListAsync();
        }

        public async Task UpdateAsync(SystemAccount account)
        {
            _context.SystemAccounts.Update(account);
            await _context.SaveChangesAsync();
        }
    }
}
