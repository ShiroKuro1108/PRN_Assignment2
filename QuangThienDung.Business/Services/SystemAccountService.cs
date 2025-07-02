using QuangThienDung.DataAccess.Models;
using QuangThienDung.DataAccess.Repository;

namespace QuangThienDung.Business.Services
{
    public class SystemAccountService : ISystemAccountService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SystemAccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SystemAccount?> AuthenticateAsync(string email, string password)
        {
            return await _unitOfWork.SystemAccount.AuthenticateAsync(email, password);
        }

        public async Task<bool> CreateAccountAsync(SystemAccount account)
        {
            try
            {
                if (!await ValidateAccountAsync(account))
                    return false;

                await _unitOfWork.SystemAccount.AddAsync(account);
                await _unitOfWork.SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAccountAsync(short id)
        {
            try
            {
                var account = await _unitOfWork.SystemAccount.GetAsync(a => a.AccountID == id);
                if (account == null)
                    return false;

                _unitOfWork.SystemAccount.Remove(account);
                await _unitOfWork.SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<SystemAccount?> GetAccountByIdAsync(short id)
        {
            return await _unitOfWork.SystemAccount.GetAsync(a => a.AccountID == id);
        }

        public async Task<IEnumerable<SystemAccount>> GetAllAccountsAsync()
        {
            return await _unitOfWork.SystemAccount.GetAllAsync();
        }

        public async Task<bool> IsEmailUniqueAsync(string email, short? excludeId = null)
        {
            var existingAccount = await _unitOfWork.SystemAccount.GetByEmailAsync(email);
            if (existingAccount == null)
                return true;

            return excludeId.HasValue && existingAccount.AccountID == excludeId.Value;
        }

        public async Task<IEnumerable<SystemAccount>> SearchAccountsAsync(string searchTerm)
        {
            return await _unitOfWork.SystemAccount.SearchAccountsAsync(searchTerm);
        }

        public async Task<bool> UpdateAccountAsync(SystemAccount account)
        {
            try
            {
                if (!await ValidateAccountAsync(account))
                    return false;

                _unitOfWork.SystemAccount.Update(account);
                await _unitOfWork.SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ValidateAccountAsync(SystemAccount account)
        {
            if (string.IsNullOrWhiteSpace(account.AccountName))
                return false;

            if (string.IsNullOrWhiteSpace(account.AccountEmail))
                return false;

            if (string.IsNullOrWhiteSpace(account.AccountPassword))
                return false;

            // Check email format
            if (!IsValidEmail(account.AccountEmail))
                return false;

            // Check email uniqueness
            if (!await IsEmailUniqueAsync(account.AccountEmail, account.AccountID))
                return false;

            // Validate role
            if (!account.AccountRole.HasValue || 
                (account.AccountRole != AccountRoles.Staff && account.AccountRole != AccountRoles.Lecturer))
                return false;

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
