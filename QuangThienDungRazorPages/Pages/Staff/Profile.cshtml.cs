using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuangThienDung.Business.Services;
using QuangThienDung.DataAccess.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace QuangThienDungRazorPages.Pages.Staff
{
    [Authorize(Policy = "StaffOnly")]
    public class ProfileModel : PageModel
    {
        private readonly ISystemAccountService _accountService;
        private readonly INewsArticleService _newsService;

        public ProfileModel(ISystemAccountService accountService, INewsArticleService newsService)
        {
            _accountService = accountService;
            _newsService = newsService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public SystemAccount? CurrentAccount { get; set; }
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }
        
        // Statistics
        public int TotalNewsCreated { get; set; }
        public int ActiveNewsCreated { get; set; }
        public int InactiveNewsCreated { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(100)]
            [Display(Name = "Full Name")]
            public string AccountName { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            [StringLength(70)]
            [Display(Name = "Email Address")]
            public string AccountEmail { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Current Password")]
            public string CurrentPassword { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [StringLength(70, MinimumLength = 6)]
            [Display(Name = "New Password")]
            public string? NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm New Password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string? ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var currentUserId = short.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            try
            {
                CurrentAccount = await _accountService.GetAccountByIdAsync(currentUserId);
                if (CurrentAccount == null)
                {
                    return RedirectToPage("/Login");
                }

                // Populate form with current data
                Input.AccountName = CurrentAccount.AccountName ?? "";
                Input.AccountEmail = CurrentAccount.AccountEmail ?? "";

                // Load statistics
                await LoadStatisticsAsync(currentUserId);

                return Page();
            }
            catch (Exception)
            {
                ErrorMessage = "Failed to load profile information.";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var currentUserId = short.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            try
            {
                CurrentAccount = await _accountService.GetAccountByIdAsync(currentUserId);
                if (CurrentAccount == null)
                {
                    return RedirectToPage("/Login");
                }

                // Load statistics for display
                await LoadStatisticsAsync(currentUserId);

                if (!ModelState.IsValid)
                {
                    return Page();
                }

                // Verify current password
                var authenticatedUser = await _accountService.AuthenticateAsync(CurrentAccount.AccountEmail!, Input.CurrentPassword);
                if (authenticatedUser == null)
                {
                    ErrorMessage = "Current password is incorrect.";
                    return Page();
                }

                // Check if email is unique (excluding current user)
                if (Input.AccountEmail != CurrentAccount.AccountEmail)
                {
                    var isEmailUnique = await _accountService.IsEmailUniqueAsync(Input.AccountEmail, currentUserId);
                    if (!isEmailUnique)
                    {
                        ErrorMessage = "Email address is already in use by another account.";
                        return Page();
                    }
                }

                // Update account information
                CurrentAccount.AccountName = Input.AccountName;
                CurrentAccount.AccountEmail = Input.AccountEmail;

                // Update password if provided
                if (!string.IsNullOrEmpty(Input.NewPassword))
                {
                    CurrentAccount.AccountPassword = Input.NewPassword;
                }

                var success = await _accountService.UpdateAccountAsync(CurrentAccount);
                if (success)
                {
                    SuccessMessage = "Profile updated successfully.";
                    
                    // Clear password fields
                    Input.CurrentPassword = "";
                    Input.NewPassword = "";
                    Input.ConfirmPassword = "";
                }
                else
                {
                    ErrorMessage = "Failed to update profile. Please try again.";
                }

                return Page();
            }
            catch (Exception)
            {
                ErrorMessage = "An error occurred while updating your profile.";
                return Page();
            }
        }

        private async Task LoadStatisticsAsync(short userId)
        {
            try
            {
                var userNews = await _newsService.GetNewsByCreatorAsync(userId);
                TotalNewsCreated = userNews.Count();
                ActiveNewsCreated = userNews.Count(n => n.NewsStatus == true);
                InactiveNewsCreated = userNews.Count(n => n.NewsStatus != true);
            }
            catch (Exception)
            {
                TotalNewsCreated = ActiveNewsCreated = InactiveNewsCreated = 0;
            }
        }
    }
}
