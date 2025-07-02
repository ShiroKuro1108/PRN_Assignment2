using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuangThienDung.Business.Services;
using QuangThienDung.DataAccess.Models;
using System.Text.Json;

namespace QuangThienDungRazorPages.Pages.Admin
{
    [Authorize(Policy = "AdminOnly")]
    public class AccountsModel : PageModel
    {
        private readonly ISystemAccountService _accountService;

        public AccountsModel(ISystemAccountService accountService)
        {
            _accountService = accountService;
        }

        public IList<SystemAccount> Accounts { get; set; } = new List<SystemAccount>();
        public string SearchTerm { get; set; } = string.Empty;

        public async Task OnGetAsync(string? searchTerm)
        {
            SearchTerm = searchTerm ?? string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    Accounts = (await _accountService.SearchAccountsAsync(SearchTerm)).ToList();
                }
                else
                {
                    Accounts = (await _accountService.GetAllAccountsAsync()).ToList();
                }
            }
            catch (Exception)
            {
                Accounts = new List<SystemAccount>();
            }
        }

        public async Task<IActionResult> OnPostCreateAsync([FromBody] CreateAccountRequest request)
        {
            try
            {
                var account = new SystemAccount
                {
                    AccountName = request.Name,
                    AccountEmail = request.Email,
                    AccountPassword = request.Password,
                    AccountRole = request.Role
                };

                var success = await _accountService.CreateAccountAsync(account);
                if (success)
                {
                    return new JsonResult(new { success = true, message = "Account created successfully" });
                }
                else
                {
                    return new JsonResult(new { success = false, message = "Failed to create account. Please check the data." });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        public async Task<IActionResult> OnPostUpdateAsync([FromBody] UpdateAccountRequest request)
        {
            try
            {
                var account = await _accountService.GetAccountByIdAsync(request.Id);
                if (account == null)
                {
                    return new JsonResult(new { success = false, message = "Account not found" });
                }

                account.AccountName = request.Name;
                account.AccountEmail = request.Email;
                account.AccountRole = request.Role;

                // Only update password if provided
                if (!string.IsNullOrEmpty(request.Password))
                {
                    account.AccountPassword = request.Password;
                }

                var success = await _accountService.UpdateAccountAsync(account);
                if (success)
                {
                    return new JsonResult(new { success = true, message = "Account updated successfully" });
                }
                else
                {
                    return new JsonResult(new { success = false, message = "Failed to update account" });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync([FromBody] DeleteAccountRequest request)
        {
            try
            {
                var success = await _accountService.DeleteAccountAsync(request.Id);
                if (success)
                {
                    return new JsonResult(new { success = true, message = "Account deleted successfully" });
                }
                else
                {
                    return new JsonResult(new { success = false, message = "Failed to delete account" });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        public async Task<IActionResult> OnGetAccountAsync(short id)
        {
            try
            {
                var account = await _accountService.GetAccountByIdAsync(id);
                if (account == null)
                {
                    return new JsonResult(new { success = false, message = "Account not found" });
                }

                return new JsonResult(new
                {
                    success = true,
                    account = new
                    {
                        id = account.AccountID,
                        name = account.AccountName,
                        email = account.AccountEmail,
                        role = account.AccountRole
                    }
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        public class CreateAccountRequest
        {
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public int Role { get; set; }
        }

        public class UpdateAccountRequest
        {
            public short Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string? Password { get; set; }
            public int Role { get; set; }
        }

        public class DeleteAccountRequest
        {
            public short Id { get; set; }
        }
    }
}
